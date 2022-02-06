using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Contexts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Fluent;
using NLog.Web;

namespace CityInfo.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // This is to catch setup errors.
            // Can't get the logger here from the DI container. Explictly create the NLog here.
            var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

            try
            {
                logger.Debug("init main");
                var host = CreateHostBuilder(args).Build();
                
                using (var scope = host.Services.CreateScope())
                {
                    try
                    {
                        var context = scope.ServiceProvider.GetService<CityInfoContext>();

                        // for demo purpose, delete the database and then start the up to do migrate
                        context.Database.EnsureDeleted();
                        context.Database.Migrate();

                    }
                    catch(Exception ex)
                    {
                        logger.Error(ex, "An error occur while migrating the database.");
                        throw;
                    }
                }

                host.Run();
            }
            catch (Exception exception)
            {
                //NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                // Configure NLog. Ref: https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-3
                // Note: this can be applied to logs during bootstrapping. This is why the code is not added to Startup.cs instead.
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();  // NLog: Setup NLog for Dependency injection

        //public static void Main(string[] args)
        //{
        //    CreateHostBuilder(args).Build().Run();
        //}

        //public static IHostBuilder CreateHostBuilder(string[] args) =>
        //    Host.CreateDefaultBuilder(args)
        //        .ConfigureWebHostDefaults(webBuilder =>
        //        {
        //            webBuilder.UseStartup<Startup>();
        //        });
    }
}
