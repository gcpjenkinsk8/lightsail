using DARReferenceData.DatabaseHandlers;
using DARReferenceData.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using AspNet.Identity.MySQL;
using Microsoft.Owin.Security;
using System.Web.UI.WebControls;
using Microsoft.AspNet.Identity.Owin;
using DARReferenceData.DatabaseHandlersTest;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
using System.Web.Script.Services;

namespace DAR_ReferenceDataUI.Controllers
{
    public class HomeController : DARController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);

        private LogData lDH = new LogData();
        private Asset aDH = new Asset();

        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            ViewData["AssetCount"] = $"{Asset.GetCount():#,##}";
            ViewData["CirculatingSupplyCount"] = $"{OutstandingSupply.GetCount():#,##}";
            ViewData["ExchangeCount"] = $"{Exchange.GetCount():#,##}";
            ViewData["ActiveDerivativeCount"] = $"{Derivatives.GetActiveCount():#,##}";
            ViewData["ExpiredDerivativeCount"] = $"{Derivatives.GetExpiredCount():#,##}";
            ViewData["PairCount"] = $"{Pair.GetCount():#,##}";
            ViewData["FutureCryptoEventCount"] = $"{CryptoEvent.GetFutureEventCount():#,##}";
            ViewData["PastCryptoEventCount"] = $"{CryptoEvent.GetPastEventCount():#,##}";

            ViewData["APICallCount"] = "[{data:[90000, 60000, 40000, 30000, 10000]}]";

            return View();
        }

        public ActionResult Login()
        {
            ViewBag.Message = "DAR Login";

            return View();
        }

        public ActionResult InsufficientAccess(string message)
        {
            ViewBag.Message = message;

            return View();
        }

        public ActionResult Register()
        {
            var r = IsAllowedToAddUser();
            if (r == null)
            {
                ViewBag.Message = "Register User";
                return View();
            }
            else
                return r;
        }

        [HttpPost]
        public ActionResult Authenticate(UserViewModel model)
        {
            MySQLDatabase db = new MySQLDatabase("DefaultConnectionStringName");

            var userStore = new UserStore<IdentityUser>(db);
            var userManager = new UserManager<IdentityUser>(userStore);
            var user = userManager.Find(model.UserName, model.Password);

            if (user != null)
            {
                var authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
                var userIdentity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = false }, userIdentity);

                DARReferenceData.DARApplicationInfo.CurrentUserMessage = $"Welcome {User.Identity.Name}!";
                if (model.Password.ToUpper().StartsWith(model.UserName.ToUpper()))
                {
                    return RedirectToAction("ChangePassword", "Home");
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                return RedirectToAction("LogIn", "Home");
            }
        }

        [HttpPost]
        public ActionResult RegisterUser([DataSourceRequest] DataSourceRequest request, UserViewModel model)
        {
            var r = IsAllowedToAddUser();
            if (r == null)
            {
                MySQLDatabase db = new MySQLDatabase("DefaultConnectionStringName");

                var userStore = new UserStore<IdentityUser>(db);
                var manager = new UserManager<IdentityUser>(userStore);
                var user = new IdentityUser() { UserName = model.UserName };

                IdentityResult result = manager.Create(user, model.Password);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", result.Errors.FirstOrDefault());
                    return Json(new[] { model }.ToDataSourceResult(request, ModelState));
                }
            }
            else
                return r;
        }

        public ActionResult ChangePassword()
        {
            ViewBag.Message = "Change password!";

            return View();
        }

        [HttpPost]
        public ActionResult UpdatePassword(UserViewModel model)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("LogIn", "Home");
            }

            MySQLDatabase db = new MySQLDatabase("DefaultConnectionStringName");
            var userStore = new UserStore<IdentityUser>(db);
            var userManager = new UserManager<IdentityUser>(userStore);
            var user = userManager.Find(User.Identity.Name, model.Password);

            if (user != null)
            {
                userManager.RemovePassword(User.Identity.GetUserId());
                userManager.AddPassword(User.Identity.GetUserId(), model.NewPassword);
                return SignOut();
            }

            return RedirectToAction("LogIn", "Home");
        }

        public ActionResult SignOut()
        {
            var authenticationManager = System.Web.HttpContext.Current.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            return RedirectToAction("LogIn", "Home");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


        public ActionResult LogViewer()
        {
            ViewBag.Message = "Log ";

            return View();
        }

        public ActionResult ReadLog([DataSourceRequest] DataSourceRequest request, string logType, string goback)
        {
            IList<LogViewModel> result = new List<LogViewModel>();
            try
            {
                result = lDH.GetLogByType(logType, goback).Cast<LogViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }

        

        public ActionResult RunTest()
        {
            ExchangeTest et = new ExchangeTest();
            ViewBag.Message = et.RunTest();

            //AssetTest at = new AssetTest();
            //ViewBag.Message =  at.RunTest();

            return View();
        }



        #region Dashboard
        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult GetNewDerivativeContractByDay()
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            List<ChartModelViewsChart> result = (new Chart()).GetNewDerivativeContractByDay().ToList();
            return Json(jss.Serialize(result), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult GetApiClientCallCount()
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            //int[] result = new int[5] {2000,60000,40000,30000,10000 };
            int[] result = (new Chart()).GetApiCallCount().Select(x=>int.Parse(x.value)).ToArray();


            return Json(jss.Serialize(result), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult GetApiClients()
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            // string[] result = new string[5] { "Stover", "GoldenTree", "Armanino", "Tanweer", "Mike" };
            string[] result = (new Chart()).GetApiCallCount().Select(x => x.category).ToArray();
            return Json(jss.Serialize(result), JsonRequestBehavior.AllowGet);
        }



        #endregion

    }
}