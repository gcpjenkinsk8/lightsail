using DARReferenceData.DatabaseHandlers;
using DARReferenceData.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Script.Services;

namespace DAR_ReferenceDataUI.Controllers
{
    public class HelperController : Controller
    {
        // GET: Helper
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult LookupExchange(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            Exchange e = new Exchange();
            var result = e.Get(input) as ExchangeViewModel;

            if (result == null)
            {
                return Json(jss.Serialize(""), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(jss.Serialize(result), JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult LookupExchangePublic(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            Exchange e = new Exchange();
            var result = e.GetExchangePublic(input) as ExchangeViewModel;

            if (result == null)
            {
                return Json(jss.Serialize(""), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(jss.Serialize(result), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult LookupToken(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            TokenTable e = new TokenTable();
            var result = e.GetTokenDetails(input) as TokenTableViewModel;
            
            if (result == null)
            {
                return Json(jss.Serialize(""), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(jss.Serialize(result), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult LookupToken_Name(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            TokenTable e = new TokenTable();
            var result = e.GetTokenDetailsName(input) as TokenTableViewModel;

            if (result == null)
            {
                return Json(jss.Serialize(""), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(jss.Serialize(result), JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult LookupAsset(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            Asset a = new Asset();
            var result = a.Get(input) as AssetViewModel;

            if (result == null)
            {
                return Json(jss.Serialize(""), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(jss.Serialize(result), JsonRequestBehavior.AllowGet);
            }
        }


        
        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult LookupSource(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            Source a = new Source();
            var result = a.Get(input) as SourceViewModel;

            if (result == null)
            {
                return Json(jss.Serialize(""), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(jss.Serialize(result), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult LookupPrice(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            Price a = new Price();
            var result = a.GetLastHourlyPrice(input) as PriceViewModel;

            if (result == null)
            {
                return Json(jss.Serialize(""), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(jss.Serialize(result), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult LookupPriceInput(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            Price a = new Price();
            var result = a.GetPriceInput(input);

            if (result == null)
            {
                return Json(jss.Serialize(""), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(jss.Serialize(result), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public FileResult DownlaodPriceInput(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            Price a = new Price();
            var result = a.GetPriceInput(input);

            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Name,Pair,Ticker,AvgUSDPrice,TradeCount,USDVolume");
                foreach(var item in result)
                {
                    sb.AppendLine($"{item.Name},{item.Pair},{item.Ticker},{item.AvgUSDPrice},{item.TradeCount},{item.USDVolume}");
                }

                // Return FileResult
                byte[] byteArray = Encoding.UTF8.GetBytes(sb.ToString());
                return File(new MemoryStream(byteArray, 0, byteArray.Length), "application/octet-stream", $"{input}_price_input.csv");
            }
            catch (Exception ex)
            {

            }
            return null;
        }

    }
}