using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Contexts;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CityInfo.API
{
    public class Startup
    {
        private IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers()
               // Required to use JsonPatchDocument for PATCH.
               .AddNewtonsoftJson()
               // Uncomment this add XML output format to support response body in xml format.
               .AddMvcOptions(options =>
                {
                    options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                });
            // Uncomment this to disable the default cammel case property formatting.
            //.AddJsonOptions(options =>
            //{
            //    options.JsonSerializerOptions.PropertyNamingPolicy = null;
            //});

            // Configured in appsettings.Development.json for Development environment.
            // Configured in Properties/launchSettings.json for Production. This file defines environment variables when lauch IIS.
            var connectionString = _configuration["connectionStrings:cityInfoDBConnectionString"];
            services.AddDbContext<CityInfoContext>(o => o.UseSqlServer(connectionString));

#if DEBUG
            services.AddSingleton<IMailService, LocalMailService>();
#else
            services.AddSingleton<IMailService, CloudMailService>();
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            // app.UseStatusCodePages();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
