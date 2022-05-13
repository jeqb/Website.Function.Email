using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace Website.Function.EmailService
{
    public class EmailFunction
    {
        [FunctionName("EmailFunction")]
        public void Run([QueueTrigger("email-service-queue", Connection = "ConnectionString")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");
        }
    }
}
