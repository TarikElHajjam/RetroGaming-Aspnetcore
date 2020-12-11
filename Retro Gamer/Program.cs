using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Retro_Gamer
{
    public class Program
    {
        /// <summary>
        /// Credit for this project,
        /// Built by Tarik El hajjam, Email: TarikElHajjam@outlook.com
        /// Github repository: https://github.com/TarikElHajjam/RetroGaming-Aspnetcore
        /// </summary>
 
        public static void Main(string[] args)
        {

            //Uncomment this when building an environment on Beanstalk AWS to create an environment 
            //
            //Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
