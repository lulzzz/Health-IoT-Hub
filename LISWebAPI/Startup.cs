using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using LISWebAPI.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

namespace LISWebAPI
{
    public class Startup
    {
        private static string _Authority;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _Authority = Configuration["AzureAd:Instance"] + Configuration["AzureAd:TenantId"];

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(o =>
                {
                  o.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                  o.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;
                });

            if (HostingEnvironment.IsDevelopment())
            {
                services.AddDbContext<DatabaseDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DBConnectionString"),
                x => x.MigrationsAssembly("LISWebAPI.DevelopmentMigrations")
                ));
            }
            if(HostingEnvironment.IsProduction())
            {
                services.AddDbContext<DatabaseDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DBConnectionString"),
               x => x.MigrationsAssembly("LISWebAPI.ReleaseMigrations")
               ));
            }
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddAzureAdBearer(options => Configuration.Bind("AzureAd", options));

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("datastore", new Info
                {
                    Version = "v1",
                    Title = "LIS Database Storage API",
                    Description = "A Web API on the Health IoT Hub to manage LIS data in a SQL database",
                    TermsOfService = "None",
                });
                c.SwaggerDoc("processor", new Info
                {
                    Version = "v1",
                    Title = "LIS Message Processor API",
                    Description = "A Web API on the Health IoT Hub to process and decode HL7, ASTM and POCT messages",
                    TermsOfService = "None",
                });
                c.SwaggerDoc("reports", new Info
                {
                    Version = "v1",
                    Title = "LIS Clinical Reports",
                    Description = "A Web API on the Health IoT Hub to create clinical reports from lab results",
                    TermsOfService = "None",
                });
                c.SwaggerDoc("fhir", new Info
                {
                    Version = "v1",
                    Title = "LIS FHIR Web API",
                    Description = "A FHIR based Web API to access LIS data in the central SQL database",
                    TermsOfService = "None",
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Type = "oauth2",
                    Flow = "implicit",
                    AuthorizationUrl = $"{_Authority}/oauth2/authorize",
                    TokenUrl = $"{_Authority}/oauth2/token",
                    Scopes = new Dictionary<string, string>()
                    {
                          { "user_impersonation", "Access LIS Web API" }
                    }
                });
                c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "oauth2", new[] { "user_impersonation"} }
                });
            });
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

            app.UseAuthentication();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/datastore/swagger.json", "LIS Database Storage API v1");
                c.DocumentTitle = "LIS Database Storage API Docs";
                c.InjectStylesheet("/css/swagger-custom.css");
                c.RoutePrefix = "datastoreapidocs";
                c.OAuthClientId($"{Configuration["AzureAdClient:ClientId"]}");
                c.OAuthAdditionalQueryStringParams(new { resource = Configuration["AzureAd:ClientId"] });
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/processor/swagger.json", "LIS Message Processor API v1");
                c.DocumentTitle = "LIS Message Processor API Docs";
                c.InjectStylesheet("/css/swagger-custom.css");
                c.RoutePrefix = "processorapidocs";
                c.OAuthClientId($"{Configuration["AzureAdClient:ClientId"]}");
                c.OAuthAdditionalQueryStringParams(new { resource = Configuration["AzureAd:ClientId"] });
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/reports/swagger.json", "LIS Clinical Reports v1");
                c.DocumentTitle = "LIS Clinical Reports API Docs";
                c.InjectStylesheet("/css/swagger-custom.css");
                c.RoutePrefix = "reportsapidocs";
                c.OAuthClientId($"{Configuration["AzureAdClient:ClientId"]}");
                c.OAuthAdditionalQueryStringParams(new { resource = Configuration["AzureAd:ClientId"] });
            });

            app.UseMvc();

            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService <DatabaseDBContext> ();
                context.Database.Migrate();
            }
        }
    }
}
