using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrainInterfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Orleans;

namespace apicolors
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSingleton(CreateClusterClient);
        }

        private IClusterClient CreateClusterClient(IServiceProvider serviceProvider)
        {
            var client = new ClientBuilder()
                .UseLocalhostClustering(serviceId: "colors-api")
                .ConfigureApplicationParts(parts => parts.AddApplicationPart(typeof(IColorGrain).Assembly).WithReferences())
                .ConfigureLogging(_ => _.AddConsole())
                .Build();

            StartClientWithRetries(client).Wait();
            return client;
        }

        private static async Task StartClientWithRetries(IClusterClient client)
        {
            for (var i = 0; i < 5; i++)
            {
                try
                {
                    await client.Connect();
                    return;
                }
                catch (Exception)
                {
                    // ignored
                }

                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
