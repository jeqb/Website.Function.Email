using Azure.Data.Tables;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Website.Function.Email.EmailClient;

[assembly: FunctionsStartup(typeof(Website.Function.Email.Startup))]
namespace Website.Function.Email
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();

            // TODO: consider using the Options Pattern
            builder.Services.AddSingleton<IConfiguration>(builder.GetContext().Configuration);

            builder.Services.AddSingleton<IEmailClient, SmtpEmailClient>((serviceProvider) =>
            {
                string smtpServer = serviceProvider.GetService<IConfiguration>()["SmtpServer"];
                int.TryParse(serviceProvider.GetService<IConfiguration>()["SmtpPort"], out int smtpPort);
                string userName = serviceProvider.GetService<IConfiguration>()["SmtpUserName"];
                string password = serviceProvider.GetService<IConfiguration>()["SmtpPassword"];

                return new SmtpEmailClient(smtpServer, smtpPort, userName, password);
            });

            builder.Services.AddSingleton<TableClient>((serviceProvider) =>
            {
                string storageUri = serviceProvider.GetService<IConfiguration>()["StorageUri"];
                string tableName = serviceProvider.GetService<IConfiguration>()["StorageTableName"];
                string storageAccountName = serviceProvider.GetService<IConfiguration>()["StorageAccountName"];
                string storageAccountKey = serviceProvider.GetService<IConfiguration>()["StorageAccountKey"];

                return new TableClient(new Uri(storageUri), tableName,
                    new TableSharedKeyCredential(storageAccountName, storageAccountKey)
                    );
            });
        }
    }
}
