using EntityFX.IotSimulator.Common;
using EntityFX.IotSimulator.Engine;
using EntityFX.IotSimulator.Engine.TelemetryGenerator;
using EntityFX.IotSimulator.Engine.TelemetrySender;
using EntityFX.IotSimulator.Engine.TelemetrySerializer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace EntityFX.IotSimulator.WebSocketServer
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
            services.AddRazorPages();

            var settings = Configuration.Get<SimulatorSettings>();

            services.AddScoped<SimulatorSettings>(scope =>  settings);
            services.AddScoped<IBuilderFactory, BuilderFactory>(scope =>
            {
                var generatorAsmType = BuilderHelper.GetDefaultAssemblyAndTypeName<TelemetryGeneratorBuilder>(settings.TelemetryGenerator?.Plugin);
                var serializerAsmType = BuilderHelper.GetDefaultAssemblyAndTypeName<TelemetrySerializerBuilder>(settings.TelemetryGenerator?.Plugin);

                var dictionarySettings = new Dictionary<string, object>()
                {
                    ["settings"] = settings,
                    ["generatorAsmType"] = generatorAsmType,
                    ["serializerAsmType"] = serializerAsmType,
                };

                var logger = scope.GetService<ILogger>();
                return new BuilderFactory(logger, Configuration, dictionarySettings);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseWebSockets();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(c => c.MapControllers());
        }
    }
}
