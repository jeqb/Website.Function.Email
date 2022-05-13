using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Website.Function.EmailService.EmailClient;

[assembly: FunctionsStartup(typeof(Website.Function.EmailService.Startup))]
namespace Website.Function.EmailService
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging();

            // TODO: consider using the Options Pattern
            builder.Services.AddSingleton<IConfiguration>(builder.GetContext().Configuration);

            builder.Services.AddScoped<IEmailClient, SmtpEmailClient>((serviceProvider) =>
            {
                string smtpServer = serviceProvider.GetService<IConfiguration>()["SmtpServer"];
                int.TryParse(serviceProvider.GetService<IConfiguration>()["smtpPort"], out int smtpPort);
                string userName = serviceProvider.GetService<IConfiguration>()["smtpUserName"];
                string password = serviceProvider.GetService<IConfiguration>()["smtpPassword"];

                return new SmtpEmailClient(smtpServer, smtpPort, userName, password);
            });
        }
    }
}
