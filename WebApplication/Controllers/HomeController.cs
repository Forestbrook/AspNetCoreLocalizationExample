using AspNetCoreLocalizationExample.WebApplication.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Diagnostics;
using System.Linq;

namespace AspNetCoreLocalizationExample.WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly RequestLocalizationOptions _localizationOptions;

        public HomeController(RequestLocalizationOptions localizationOptions)
        {
            _localizationOptions = localizationOptions;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SetLanguage(string language, string returnUrl)
        {
            var defaultFormattingCulture = _localizationOptions.DefaultRequestCulture.Culture.Name;
            var formattingCulture = defaultFormattingCulture;
            if (HttpContext.Request.Cookies.TryGetValue(CookieRequestCultureProvider.DefaultCookieName, out var value))
            {
                formattingCulture = CookieRequestCultureProvider.ParseCookieValue(value).Cultures.FirstOrDefault().Value;
                if (string.IsNullOrEmpty(formattingCulture))
                    formattingCulture = defaultFormattingCulture;
            }

            value = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(formattingCulture, language));
            var expiration = new CookieOptions { Expires = DateTime.UtcNow.AddYears(4) };
            HttpContext.Response.Cookies.Delete(CookieRequestCultureProvider.DefaultCookieName);
            HttpContext.Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, value, expiration);
            return LocalRedirect(returnUrl);
        }
    }
}
