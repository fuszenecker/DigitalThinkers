using System;
using System.IO;
using System.Reflection;
using CorrelationId;
using CorrelationId.DependencyInjection;
using ServiceTemplate.Domain.Interfaces;
using ServiceTemplate.Domain.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Prometheus;
using Serilog;

namespace ServiceTemplate
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
            services
                .AddControllers();

            services.AddApiVersioning(config =>
            {
                config.ReportApiVersions = true;
                config.AssumeDefaultVersionWhenUnspecified = true;
                config.DefaultApiVersion = new ApiVersion(1, 0);
                config.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            services.AddVersionedApiExplorer(setup =>
            {
                setup.GroupNameFormat = "'v'VVV";
                setup.DefaultApiVersion = new ApiVersion(1, 0);
                setup.SubstituteApiVersionInUrl = true;
            });

            services
                .AddSwaggerGen(c => {
                    // FIXME: works, but not the best solution:
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ServiceTemplate", Version = "v1" });
                    c.SwaggerDoc("v2", new OpenApiInfo { Title = "ServiceTemplate", Version = "v2" });

                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                });

            services
                .AddHttpClient(Options.DefaultName)
                .UseHttpClientMetrics();

            services
                .AddHealthChecks()
                .ForwardToPrometheus();

            services
                .AddDefaultCorrelationId();

            // Serilog to access the Correlation ID:
            services
                .AddHttpContextAccessor();

            // For now, it is in-memory:
            services.AddSingleton<IMonetaryService, InMemoryMonetaryService>();

            // For presistent nonetary service, uncomment the two lines below,
            // and comment out the in-memoty one:
            // services.AddScoped<INotesRepository, NotesRepository>();
            // services.AddScoped<IMonetaryService, PersistentMonetaryService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment() || true)
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();

                app.UseSwaggerUI(options =>
                {
                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                    }
                });
            }

            app.UseCorrelationId();
            app.UseSerilogRequestLogging();
            loggerFactory.AddSerilog();

            if (!env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseCors(options => {
                options.AllowAnyHeader();
                options.AllowAnyMethod();
                options.AllowAnyOrigin();
            });

            app.UseRouting();
            app.UseHttpMetrics();
            app.UseAuthorization();

            // app.UseStaticFiles(new StaticFileOptions
            // {
            //     FileProvider = new PhysicalFileProvider(
            //         Path.Combine(env.ContentRootPath, "../ui/build")),
            //     RequestPath = "/ui"
            // });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/health");
                endpoints.MapMetrics();
            });
        }
    }
}
