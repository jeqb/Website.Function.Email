using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Website.Function.Email.EmailClient;
using Website.Function.Email.TableStorage;

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

            builder.Services.AddSingleton<IStorageTableClient, StorageTableClient>((serviceProvider) =>
            {
                string storageUri = serviceProvider.GetService<IConfiguration>()["StorageUri"];
                string storageAccountName = serviceProvider.GetService<IConfiguration>()["StorageAccountName"];
                string storageAccountKey = serviceProvider.GetService<IConfiguration>()["StorageAccountKey"];

                return new StorageTableClient(storageUri, storageAccountName, storageAccountKey);
            });
        }
    }
}
