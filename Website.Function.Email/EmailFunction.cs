using Azure.Data.Tables;
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

        private readonly TableClient _tableClient;

        public EmailFunction(IEmailClient emailClient, TableClient tableClient)
        {
            _emailClient = emailClient;

            _tableClient = tableClient;
        }

        [FunctionName("EmailFunction")]
        public async Task Run([QueueTrigger("%QueueName%", Connection = "ConnectionString")] string myQueueItem,
            ILogger log)
        {
            log.LogInformation($"EmailFunction started with payload: {myQueueItem}");

            try
            {
                EmailRequestModel request = JsonSerializer.Deserialize<EmailRequestModel>(myQueueItem,
                    new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true,
                    });

                await _emailClient.SendEmailAsync(request.ToEmail, request.Subject, request.HtmlBody);

                log.LogInformation("Email Successfully sent to: {ToEmail}", request.ToEmail);
            }
            catch (Exception ex)
            {
                string eMessage = ex.Message;

                log.LogError("An Exception occured while sending an email with message: {eMessage}", eMessage);

                TableEntity exceptionDetails = new();
                exceptionDetails.PartitionKey = "ExceptionDetails";
                exceptionDetails.RowKey = Guid.NewGuid().ToString();
                exceptionDetails.Add("ExceptionMesssage", eMessage);
                exceptionDetails.Add("QueueMessage", myQueueItem);

                await _tableClient.AddEntityAsync(exceptionDetails);
            }
        }
    }
}
