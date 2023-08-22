using DARReferenceData.DatabaseHandlers;
using DARReferenceData.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using log4net;
using log4net.Repository.Hierarchy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Script.Services;

namespace DAR_ReferenceDataUI.Controllers
{
    public class ExchangePairsController : DARController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);

        private ExchangePairsMap dhSource = new ExchangePairsMap();

        public ActionResult ExchangePairsIndex()
        {
            try
            {
                var r = ValidateUser();
                if (r == null)
                {
                    ViewData["DARMnemonic"] = Configuration.GetDARMnemonicFamily();
                    ViewData["ExchangeVettingStatus"] = Configuration.GetVettingStatus();
                    ViewData["DARMnemonics"] = Configuration.GetDARDMnemonics();
                    ViewData["PricingTier"] = Configuration.GetPricingTier();
                    return View();
                }
                else
                {
                    return r;
                }
            }
            
            catch (Exception ex)
            {
                return RedirectToInsufficientAcess(ex.Message);
            }

        }

        public ActionResult Editing_Exchange_Pair_Read([DataSourceRequest] DataSourceRequest request)
        {
            IList<ExchangePairsV2ViewModel> result = new List<ExchangePairsV2ViewModel>();
            try
            {
                result = dhSource.Get().Cast<ExchangePairsV2ViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }

        public ActionResult Editing_Exchange_Status_Read([DataSourceRequest] DataSourceRequest request)
        {
            IList<ExchangeStatusViewModel> result = new List<ExchangeStatusViewModel>();
            try
            {
                result = dhSource.GetExchangeStatus().Cast<ExchangeStatusViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }

        public ActionResult Editing_Exclude_from_Pricing_Read([DataSourceRequest] DataSourceRequest request)
        {
            IList<ExcludeFromPricingViewModel> result = new List<ExcludeFromPricingViewModel>();
            try
            {
                result = dhSource.GetExcludeFromPricing().Cast<ExcludeFromPricingViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }

        public ActionResult Editing_Serv_List_V2_Read([DataSourceRequest] DataSourceRequest request)
        {
            IList<ServListV2ViewModel> result = new List<ServListV2ViewModel>();
            try
            {
                result = dhSource.GetServListV2().Cast<ServListV2ViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Exchange_Pair_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ExchangePairsV2ViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<ExchangePairsV2ViewModel>();

            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        dhSource.Add(product);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Failed to add {product.GetDescription()} Error: {ex.Message}");
                    }
                    results.Add(product);
                }
            }
            if (sb.Length != 0)
            {
                ModelState.AddModelError(string.Empty, sb.ToString());
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }


        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult Add_exchange_pair_v2(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            try
            {
                input = input.Replace("+010000-01-01T04:59:59.999Z", "9999-12-31 00:00:00.000000");

                ExchangePairsV2ViewModel inputObj = jss.Deserialize<ExchangePairsV2ViewModel>(input);


                ExchangePairsMap epm = new ExchangePairsMap();
                long result = epm.Add(inputObj);
                if(result == 0)
                    return Json(jss.Serialize("Pair added!"), JsonRequestBehavior.AllowGet);
                else
                    return Json(jss.Serialize("Failed to add exchange pair!"), JsonRequestBehavior.AllowGet);


            }
            catch (Exception ex)
            {
                return Json(jss.Serialize(ex.Message), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult remove_exchange_pairs_v2(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();

            ExchangePairsV2ViewModel inputObj = jss.Deserialize<ExchangePairsV2ViewModel>(input);

            ExchangePairsMap epm = new ExchangePairsMap();
            try
            {
                epm.remove_exchange_pairs_v2(inputObj);
            }
            catch (Exception ex)
            {
                return Json(jss.Serialize(ex.Message), JsonRequestBehavior.AllowGet);
            }
            
            return Json(jss.Serialize("Removed!"), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult Roll_exchange_pairs_v2(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();

            ExchangePairsV2ViewModel inputObj = jss.Deserialize<ExchangePairsV2ViewModel>(input);

            ExchangePairsMap epm = new ExchangePairsMap();
            try
            {
                epm.Roll_exchange_pairs_v2(inputObj);
            }
            catch (Exception ex)
            {
                return Json(jss.Serialize(ex.Message), JsonRequestBehavior.AllowGet);
            }
            return Json(jss.Serialize("Rolled going forward."), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult Adjust_exchange_pairs_v2(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();

            ExchangePairsV2ViewModel inputObj = jss.Deserialize<ExchangePairsV2ViewModel>(input);

            //Price a = new Price();
            //var result = a.GetLastHourlyPrice(input) as PriceViewModel;
            ExchangePairsMap ep = new ExchangePairsMap();
            

            try
            {
                ep.Adjust_exchange_pairs_v2(inputObj); ;
            }
            catch (Exception ex)
            {
                return Json(jss.Serialize(ex.Message), JsonRequestBehavior.AllowGet);
            }
            return Json(jss.Serialize("Adjusted record."), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Exchange_Pair_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ExchangePairsV2ViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Update fuction is not supported.");


            if (sb.Length != 0)
            {
                ModelState.AddModelError(string.Empty, sb.ToString());
            }
            return null;
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Exchange_Pair_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ExchangePairsV2ViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhSource.DeleteExchangePairsV2(product);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Failed to delete {product.GetDescription()} Error: {ex.Message}");
                    }
                }
            }

            if (sb.Length != 0)
            {
                ModelState.AddModelError(string.Empty, sb.ToString());
            }

            return Json(products.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Exchange_Status_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ExchangeStatusViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<ExchangeStatusViewModel>();

            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        var output = dhSource.CreateExchangeStatus(product);
                        if (output)
                        {
                            sb.AppendLine($"Record added successfully");
                        }
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Failed to add {product.GetDescription()} Error: {ex.Message}");
                    }
                    results.Add(product);
                }
            }
            if (sb.Length != 0)
            {
                ModelState.AddModelError(string.Empty, sb.ToString());
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }


        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult Add(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            input = input.Replace("+010000-01-01T04:59:59.999Z", "9999-12-31 00:00:00.000000");

            ExchangeStatusViewModel inputObj = jss.Deserialize<ExchangeStatusViewModel>(input);

            ExchangePairsMap epm = new ExchangePairsMap();
            var result = epm.CreateExchangeStatus(inputObj);

            if (result)
            {
                return Json(jss.Serialize("Status Added"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(jss.Serialize(result), JsonRequestBehavior.AllowGet);
            }
        }

        

        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult remove_exchange_status(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();

            ExchangeStatusViewModel inputObj = jss.Deserialize<ExchangeStatusViewModel>(input);

            ExchangePairsMap epm = new ExchangePairsMap();
            try
            {
                epm.remove_exchange_status(inputObj);
            }
            catch (Exception ex)
            {
                return Json(jss.Serialize(ex.Message), JsonRequestBehavior.AllowGet);
            }
            return Json(jss.Serialize("Removed!"), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult Roll(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            
            ExchangeStatusViewModel inputObj = jss.Deserialize<ExchangeStatusViewModel>(input);

            ExchangePairsMap epm = new ExchangePairsMap();
            try
            {
                epm.RollExchangeStatus(inputObj);
            }
            catch (Exception ex)
            {
                return Json(jss.Serialize(ex.Message), JsonRequestBehavior.AllowGet);
            }
            return Json(jss.Serialize("Rolled going forward."), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult Adjust(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();

            ExchangeStatusViewModel inputObj = jss.Deserialize<ExchangeStatusViewModel>(input);

            
            ExchangePairsMap ep = new ExchangePairsMap();

            try
            {
                ep.AdjustExchangeStatus(inputObj);
            }
            catch (Exception ex)
            {
                return Json(jss.Serialize(ex.Message), JsonRequestBehavior.AllowGet);
            }
            return Json(jss.Serialize("Record Adjusted"), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Exchange_Status_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ExchangeStatusViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Update fuction is not supported.");
            //if (products != null && ModelState.IsValid)
            //{
            //    foreach (var product in products)
            //    {
            //        try
            //        {
            //            dhSource.UpdateExchangeStatus(product);
            //        }
            //        catch (Exception ex)
            //        {
            //            sb.AppendLine($"Failed to {product.GetDescription()} Error: {ex.Message}");
            //        }
            //    }
            //}

            if (sb.Length != 0)
            {
                ModelState.AddModelError(string.Empty, sb.ToString());
            }
            return null;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Exchange_Status_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ExchangeStatusViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhSource.DeleteExchangeStatus(product);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Failed to delete {product.GetDescription()} Error: {ex.Message}");
                    }
                }
            }

            if (sb.Length != 0)
            {
                ModelState.AddModelError(string.Empty, sb.ToString());
            }

            return Json(products.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Exclude_from_Pricing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ExcludeFromPricingViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<ExcludeFromPricingViewModel>();

            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        dhSource.CreateExcludefromPricing(product);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Failed to add {product.GetDescription()} Error: {ex.Message}");
                    }
                    results.Add(product);
                }
            }
            if (sb.Length != 0)
            {
                ModelState.AddModelError(string.Empty, sb.ToString());
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult Add_exclude_from_pricing(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            input = input.Replace("+010000-01-01T04:59:59.999Z", "9999-12-31 00:00:00.000000");

            ExcludeFromPricingViewModel inputObj = jss.Deserialize<ExcludeFromPricingViewModel>(input);

            ExchangePairsMap epm = new ExchangePairsMap();

            try
            {
                epm.CreateExcludefromPricing(inputObj);
            }
            catch (Exception ex)
            {
                return Json(jss.Serialize(ex.Message), JsonRequestBehavior.AllowGet);
            }
            return Json(jss.Serialize("Record Added Successfully"), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult Roll_exclude_from_pricing(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();

            ExcludeFromPricingViewModel inputObj = jss.Deserialize<ExcludeFromPricingViewModel>(input);

            ExchangePairsMap epm = new ExchangePairsMap();
            try
            {
                epm.Roll_exclude_from_pricing(inputObj);
            }
            catch (Exception ex)
            {
                return Json(jss.Serialize(ex.Message), JsonRequestBehavior.AllowGet);
            }
            return Json(jss.Serialize("Rolled going forward."), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult remove_exclude_from_pricing(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();

            ExcludeFromPricingViewModel inputObj = jss.Deserialize<ExcludeFromPricingViewModel>(input);

            //Price a = new Price();
            //var result = a.GetLastHourlyPrice(input) as PriceViewModel;
            ExchangePairsMap ep = new ExchangePairsMap();
            ep.remove_exclude_from_pricing(inputObj);

            string result = "";
            if (result == null)
            {
                return Json(jss.Serialize("Record removed Successfully"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(jss.Serialize(result), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult Adjust_exclude_from_pricing(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();

            ExcludeFromPricingViewModel inputObj = jss.Deserialize<ExcludeFromPricingViewModel>(input);

            //Price a = new Price();
            //var result = a.GetLastHourlyPrice(input) as PriceViewModel;
            ExchangePairsMap ep = new ExchangePairsMap();
            

            try
            {
                ep.Adjust_exclude_from_pricing(inputObj);
            }
            catch (Exception ex)
            {
                return Json(jss.Serialize(ex.Message), JsonRequestBehavior.AllowGet);
            }
            return Json(jss.Serialize("Record Adjusted Successfully."), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Exclude_from_Pricing_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ExcludeFromPricingViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Update fuction is not supported.");


            if (sb.Length != 0)
            {
                ModelState.AddModelError(string.Empty, sb.ToString());
            }
            return null;
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Exclude_from_Pricing_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ExcludeFromPricingViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhSource.DeleteExcludefromPricing(product);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Failed to delete {product.GetDescription()} Error: {ex.Message}");
                    }
                }
            }

            if (sb.Length != 0)
            {
                ModelState.AddModelError(string.Empty, sb.ToString());
            }

            return Json(products.ToDataSourceResult(request, ModelState));
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Serv_List_V2_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ServListV2ViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<ServListV2ViewModel>();

            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        dhSource.CreateServListV2(product);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Failed to add {product.GetDescription()} Error: {ex.Message}");
                    }
                    results.Add(product);
                }
            }
            if (sb.Length != 0)
            {
                ModelState.AddModelError(string.Empty, sb.ToString());
            }

            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult Add_servlist_v2(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            input = input.Replace("+010000-01-01T04:59:59.999Z", "9999-12-31 00:00:00.000000");

            ServListV2ViewModel inputObj = jss.Deserialize<ServListV2ViewModel>(input);

            ExchangePairsMap epm = new ExchangePairsMap();
            var result = epm.CreateServListV2(inputObj);

            if (result)
            {
                return Json(jss.Serialize("Record Added successfully"), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(jss.Serialize(result), JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult remove_servlist_v2(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            ServListV2ViewModel inputObj = jss.Deserialize<ServListV2ViewModel>(input);

            ExchangePairsMap epm = new ExchangePairsMap();
            try
            {
                epm.remove_servlist_v2(inputObj);

            }
            catch (Exception ex)
            {
                return Json(jss.Serialize(ex.Message), JsonRequestBehavior.AllowGet);
            }
            return Json(jss.Serialize("Removed!"), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult Roll_servlist_v2(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            ServListV2ViewModel inputObj = jss.Deserialize<ServListV2ViewModel>(input);

            ExchangePairsMap epm = new ExchangePairsMap();
            try
            {
                epm.Roll_servlist_v2(inputObj);

            }
            catch (Exception  ex)
            {
                return Json(jss.Serialize(ex.Message), JsonRequestBehavior.AllowGet);
            }
            return Json(jss.Serialize("Rolled going forward."), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult CleanUnmappedTrades(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            ExchangePairsMap epm = new ExchangePairsMap();
            if(epm.CleanUnmappedTrades(input))
            {
                return Json(jss.Serialize(""), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(jss.Serialize("Failed to start cleanup!"), JsonRequestBehavior.AllowGet);
            }
        }



        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult Adjust_servlist_v2(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();

            ServListV2ViewModel inputObj = jss.Deserialize<ServListV2ViewModel>(input);

            //Price a = new Price();
            //var result = a.GetLastHourlyPrice(input) as PriceViewModel;
            ExchangePairsMap ep = new ExchangePairsMap();
            

            try
            {
                ep.Adjust_servlist_v2(inputObj);

            }
            catch (Exception ex)
            {
                return Json(jss.Serialize(ex.Message), JsonRequestBehavior.AllowGet);
            }
            return Json(jss.Serialize("Adjusted record."), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Serv_List_V2_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ServListV2ViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Update fuction is not supported.");
           

            if (sb.Length != 0)
            {
                ModelState.AddModelError(string.Empty, sb.ToString());
            }
            return null;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Serv_List_V2_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ServListV2ViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhSource.DeleteServlistV2(product);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Failed to delete {product.GetDescription()} Error: {ex.Message}");
                    }
                }
            }

            if (sb.Length != 0)
            {
                ModelState.AddModelError(string.Empty, sb.ToString());
            }

            return Json(products.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Editing_Non_Pricing_Serv_List_Read([DataSourceRequest] DataSourceRequest request)
        {
            IList<NonPricingServListViewModel> result = new List<NonPricingServListViewModel>();
            try
            {
                result = dhSource.GetNonPricingServList().Cast<NonPricingServListViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }

        public ActionResult Editing_Include_Pricing_Currency_Read([DataSourceRequest] DataSourceRequest request)
        {
            IList<IncludePricingCurrencyViewModel> result = new List<IncludePricingCurrencyViewModel>();
            try
            {
                result = dhSource.GetIncludePricingCurrency().Cast<IncludePricingCurrencyViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }
        public ActionResult Save(IEnumerable<HttpPostedFileBase> files)
        {
            StringBuilder errors = new StringBuilder();
            List<string> uploadedFiles = new List<string>();
            string filePath = @"C:\temp\UploadedFiles";
            // The Name of the Upload component is "files"
            string error;
            if (files != null)
            {
                foreach (var f in files)
                {
                    try
                    {
                        string p = Path.Combine(filePath, f.FileName);

                        f.SaveAs(p);
                        uploadedFiles.Add(f.FileName);
                        try
                        {
                            dhSource.LoadDataFromExcelFile(p, out error);
                            if (!string.IsNullOrEmpty(error))
                                errors.AppendLine(error);

                        }
                        catch (Exception ex)
                        {
                            //TODO return this error to frontend
                            Logger.Fatal(ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.AppendLine(ex.Message);
                    }


                }
            }

            if (errors.Length != 0)
                return Content(errors.ToString());

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult Remove(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                    // TODO: Verify user permissions

                    if (System.IO.File.Exists(physicalPath))
                    {
                        // The files are not actually removed in this demo
                        // System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult exclude_pairs_from_servlist_Save(IEnumerable<HttpPostedFileBase> filesexcludepairs)
        {
            StringBuilder errors = new StringBuilder();
            List<string> uploadedFiles = new List<string>();
            string filePath = @"C:\temp\UploadedFiles";
            // The Name of the Upload component is "files"
            string error;
            if (filesexcludepairs != null)
            {
                foreach (var f in filesexcludepairs)
                {
                    try
                    {
                        string p = Path.Combine(filePath, f.FileName);

                        f.SaveAs(p);
                        uploadedFiles.Add(f.FileName);
                        try
                        {
                            dhSource.exclude_pairs_from_servlist_LoadDataFromExcelFile(p, out error);
                            if (!string.IsNullOrEmpty(error))
                                errors.AppendLine(error);

                        }
                        catch (Exception ex)
                        {
                            //TODO return this error to frontend
                            Logger.Fatal(ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.AppendLine(ex.Message);
                    }


                }
            }

            if (errors.Length != 0)
                return Content(errors.ToString());

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult exclude_pairs_from_servlist_Remove(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                    // TODO: Verify user permissions

                    if (System.IO.File.Exists(physicalPath))
                    {
                        // The files are not actually removed in this demo
                        // System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult servlistv2_Save(IEnumerable<HttpPostedFileBase> filesservlistV2)
        {
            StringBuilder errors = new StringBuilder();
            List<string> uploadedFiles = new List<string>();
            string filePath = @"C:\temp\UploadedFiles";
            // The Name of the Upload component is "files"
            string error;
            if (filesservlistV2 != null)
            {
                foreach (var f in filesservlistV2)
                {
                    try
                    {
                        string p = Path.Combine(filePath, f.FileName);

                        f.SaveAs(p);
                        uploadedFiles.Add(f.FileName);
                        try
                        {
                            dhSource.servlistv2_LoadDataFromExcelFile(p, out error);
                            if (!string.IsNullOrEmpty(error))
                                errors.AppendLine(error);

                        }
                        catch (Exception ex)
                        {
                            //TODO return this error to frontend
                            Logger.Fatal(ex);
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.AppendLine(ex.Message);
                    }


                }
            }

            if (errors.Length != 0)
                return Content(errors.ToString());

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult servlistv2_Remove(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                    // TODO: Verify user permissions

                    if (System.IO.File.Exists(physicalPath))
                    {
                        // The files are not actually removed in this demo
                        // System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }
    }
}
 