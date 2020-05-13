using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace H2020.IPMDecisions.IDP.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        /*
        Notice that one might have to pay special attention to the Hosting Lifetime Startup Messages, 
        if removing all other LoggingProviders (Like Console) and only using NLog. As it can cause 
        hosting environment (Visual Studio / Docker / Azure Container) to not see application as started.
        */
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    /*Allow console logging to assist system testing, because without, if there are problems with NLog.config 
                    then no error messages are visible.*/
                    logging.AddConsole();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();              
    }
}

