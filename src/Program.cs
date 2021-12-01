/*
xxori - Patrick
Program entry point, I just used the default from the aspnet api template for creating the service
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Exchange.WebServices.Data;
using Models;

namespace exchangeapi
{
    public class Program
    {
        private readonly ILogger _logger;
        public Program(ILogger<Program> logger) {
            _logger = logger;
        }
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        // just werks 
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging => 
                {
                    logging.ClearProviders();  

                    // add built-in providers manually, as needed 
                    logging.AddConsole();   
                    //logging.AddDebug();  
                    //logging.AddEventSourceLogger();
                });
    }
}
