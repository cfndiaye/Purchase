using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Purchase
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Read configuration builder
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            //Initialize logger
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();
            try
            {
                Log.Information("Application starting");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "The Application failed to start");
            }
            finally
            {
                Log.CloseAndFlush();
            }

            
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSerilog() //Use Serilog instead of default .Net Logger
            /*.ConfigureLogging(logBuilder => {
                logBuilder.ClearProviders(); // removes all providers from LoggerFactory
                logBuilder.AddConsole();
                logBuilder.AddTraceSource("Information, ActivityTracing"); // Add Trace listener provider

            })*/
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
                
    }
}
