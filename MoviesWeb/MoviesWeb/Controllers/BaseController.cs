using MoviesWeb.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MoviesWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodexIntraWeb.Controllers
{
    public abstract class BaseController<T> : Controller where T : BaseController<T>
    {
        private CxSettings _cxSettings;

        //protected ILogger<T> Logger => logger ?? (logger = HttpContext.RequestServices.GetService<ILogger<T>>());
        protected CxSettings CxSettings => _cxSettings ??= HttpContext.RequestServices.GetService<IOptions<CxSettings>>().Value;

        public override async void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Session.GetString("IntraApiToken") == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(
                new
                {
                    controller = "Account",
                    action = "Login"
                }));
            }
            else
            {
                ViewBag.ApiUrl = CxSettings.MovieAPI;
                ViewBag.ApiToken = HttpContext.Session.GetString("IntraApiToken");
                ViewBag.CurrentUser = User.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value;
                ViewBag.CurrentUserFullName = User.Claims.FirstOrDefault(c => c.Type == "IME_PREZIME").Value;

                ViewBag.PravoBackup = User.Claims.FirstOrDefault(c => c.Type == "BACKUP").Value;
                ViewBag.PravoProject = User.Claims.FirstOrDefault(c => c.Type == "PROJECT").Value;
                ViewBag.PravoRegistar = User.Claims.FirstOrDefault(c => c.Type == "REGISTAR").Value;

                ViewBag.TP = User.Claims.FirstOrDefault(c => c.Type == "TP").Value;
                ViewBag.P = User.Claims.FirstOrDefault(c => c.Type == "P").Value;
                ViewBag.RabotnaEdinica = User.Claims.FirstOrDefault(c => c.Type == "RE").Value;
            }
        }
    }
}