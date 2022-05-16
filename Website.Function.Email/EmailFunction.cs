using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Website.Function.Email.EmailClient;
using Website.Function.Email.Models;
using Website.Function.Email.TableStorage;
using Website.Function.Email.TableStorage.Entities;

namespace Website.Function.Email
{
    public class EmailFunction
    {
        private readonly IEmailClient _emailClient;

        private readonly IStorageTableClient _storageTableClient;

        public EmailFunction(IEmailClient emailClient, IStorageTableClient storageTableClient)
        {
            _emailClient = emailClient;

            _storageTableClient = storageTableClient;
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

                await _emailClient.SendEmailAsync(request.ToEmail, request.Subject, request.HtmlBody);

                log.LogInformation("Email Successfully sent to: {ToEmail}", request.ToEmail);
            }
            catch (Exception ex)
            {
                string eMessage = ex.Message;

                log.LogError("An Exception occured while sending an email with message: {eMessage}", eMessage);

                ExceptionDetails exceptionDetails = new()
                {
                    PartitionKey = request.ToEmail ?? "",
                    RowKey = myQueueItem,
                    ExceptionMessage = eMessage
                };

                await _storageTableClient.InsertEntityAsync(exceptionDetails);
            }
        }
    }
}
