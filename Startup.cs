using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using System.Data;
using System.Data.SqlClient;
using System;

namespace OpenTelemetryDbApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            var otlpEndpoint = "http://52.146.40.174:4317";
            var serviceName = "DOTNET31";
            // Configuração de Tracing com OpenTelemetry
            services.AddOpenTelemetryTracing(builder =>
            {
                builder
                    //.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("dotnet-3.1"))
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddSqlClientInstrumentation()
                    // // Exporta os traces para o console
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                    })
                    .AddConsoleExporter();

            });

            // Configuração de Métricas com OpenTelemetry
            services.AddOpenTelemetryMetrics(builder =>
            {
                builder
                    //.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("dotnet-3.1"))
                    .AddAspNetCoreInstrumentation()
                    //.AddConsoleExporter();
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(otlpEndpoint);
                        options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
                    })
                    .AddConsoleExporter();
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
