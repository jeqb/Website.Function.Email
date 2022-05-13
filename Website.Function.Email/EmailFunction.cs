using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Website.Function.Email.EmailClient;
using Website.Function.Email.Models;

namespace Website.Function.Email
{
    public class EmailFunction
    {
        private readonly IEmailClient _emailClient;

        public EmailFunction(IEmailClient emailClient)
        {
            _emailClient = emailClient;
        }

        [FunctionName("EmailFunction")]
        public async Task Run([QueueTrigger("%QueueName%", Connection = "ConnectionString")] string myQueueItem,
            ILogger log)
        {
            log.LogInformation($"EmailFunction started with payload: {myQueueItem}");

            EmailRequestModel request = new();

            try
            {
                request = JsonSerializer.Deserialize<EmailRequestModel>(myQueueItem,
                new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true,
                });
            }
            catch (Exception ex)
            {
                string eMessage = ex.Message;

                log.LogError("Could not serialize jsonPayload into model with message: {eMessage}", eMessage);

                return;
            }

            try
            {
                await _emailClient.SendEmailAsync(request.ToEmail, request.Subject, request.HtmlBody);

                log.LogInformation("Email Successfully sent to: {ToEmail}", request.ToEmail);
            }
            catch (Exception ex)
            {
                string eMessage = ex.Message;

                log.LogError("An Exception occured while sending an email with message: {eMessage}", eMessage);

                log.LogDebug("ToEmail: {ToEmail}", request.ToEmail);
            }
        }
    }
}