using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AspNetCoreLocalizationExample.ResourceLibrary;
using Microsoft.AspNetCore.Mvc.Razor;

namespace AspNetCoreLocalizationExample.WebApplication
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
            services.AddLocalization(options => options.ResourcesPath = "Resources");
            services.AddSingleton(CreateRequestLocalizationOptions());
            services.AddControllersWithViews()
                .AddViewLocalization(LanguageViewLocationExpanderFormat.SubFolder)
                .AddDataAnnotationsLocalization(options => { options.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResource)); });
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseRequestLocalization(app.ApplicationServices.GetService<RequestLocalizationOptions>());
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static RequestLocalizationOptions CreateRequestLocalizationOptions()
        {
            var supportedLanguages = new[] { new CultureInfo("nl-NL"), new CultureInfo("en") };
            var supportedFormattingCultures = new[] { new CultureInfo("nl-NL"), new CultureInfo("en-US") };
            var result = new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("nl-NL", "nl-NL"),
                SupportedCultures = supportedFormattingCultures,
                SupportedUICultures = supportedLanguages
            };

            // Now if your default browser language is English, your WebApplication will startup in English,
            // even though you set the DefaultRequestCulture to Dutch (Netherlands).
            // In most situations this is correct, but if you DO want to start in the language specified in DefaultRequestCulture
            // you can add these lines:
            var acceptLanguageProvider = result.RequestCultureProviders.FirstOrDefault(p => p is AcceptLanguageHeaderRequestCultureProvider);
            if (acceptLanguageProvider != null)
                result.RequestCultureProviders.Remove(acceptLanguageProvider);

            return result;
        }
    }
}
