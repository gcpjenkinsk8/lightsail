using DAR_ReferenceDataUI.Models;
using DARReferenceData;
using DARReferenceData.DatabaseHandlers;
using DARReferenceData.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using log4net;
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
using System.Web.Security;

namespace DAR_ReferenceDataUI.Controllers
{
    public class StagedCryptoEventsController : DARController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);


        CryptoEvent ce = new CryptoEvent();
        // GET: StagedCryptoEvents
        public ActionResult Index()
        {
            try
            {
                

                //TODO: Remove following line before going live
                return View();
                var r = ValidateUser();
                if (r == null)
                {
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

        public ActionResult ReviewedEvents()
        {try
            {
                var r = ValidateUser();
                if (r == null)
                {
                    return View();
                }
                else
                {
                    return r;
                }
            }
            catch (Exception ex) { return RedirectToInsufficientAcess(ex.Message); }
        }




        #region Published
        public ActionResult FinalizedCrypoEvents_Read([DataSourceRequest] DataSourceRequest request, string startDate, string endDate)
        {
            IList<StagedCryptoEventViewModel> result = new List<StagedCryptoEventViewModel>();
            try
            {
                result = ce.GetFinalizedCryptoEvents(startDate,endDate).Cast<StagedCryptoEventViewModel>().ToList(); 
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinalizedCrypoEvents_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<StagedCryptoEventViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<StagedCryptoEventViewModel>();

            if (products != null && ModelState.IsValid)
            {
                string error;
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        if(!ce.AddCryptoEventFinal(product,out error))
                        {
                            sb.AppendLine(error);
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinalizedCrypoEvents_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<StagedCryptoEventViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    try
                    {
                        ce.UpdateCryptoEventFinal(product);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Failed to update {product.GetDescription()} Error: {ex.Message}");
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
        public ActionResult FinalizedCrypoEvents_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<StagedCryptoEventViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        ce.DeleteCryptoEventFinal(product);
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

        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult Editing_Update_published(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();

            StagedCryptoEventViewModel inputObj = jss.Deserialize<StagedCryptoEventViewModel>(input);


            try
            {
                ce.UpdateCryptoEventFinal(inputObj);
            }
            catch (Exception ex)
            {
                return Json(jss.Serialize(ex.Message), JsonRequestBehavior.AllowGet);
            }
            return Json(jss.Serialize("Record Updated Successfully."), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult remove_published_events(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();

            StagedCryptoEventViewModel inputObj = jss.Deserialize<StagedCryptoEventViewModel>(input);

            try
            {
                ce.DeleteCryptoEventFinal(inputObj);
            }
            catch (Exception ex)
            {
                return Json(jss.Serialize(ex.Message), JsonRequestBehavior.AllowGet);
            }

            return Json(jss.Serialize("Removed!"), JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region Staging
        public ActionResult Editing_Read([DataSourceRequest] DataSourceRequest request)
        {
            IList<StagedCryptoEventViewModel> result = new List<StagedCryptoEventViewModel>();
            try
            {
                result = ce.Get().Cast<StagedCryptoEventViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<StagedCryptoEventViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<StagedCryptoEventViewModel>();

            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        ce.AddStagedCryptoEvent(product);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Failed to add {product.GetDescription()} Error: Event description and Source URL are required fields !!");
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
        public JsonResult Editing_Update_1(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();

            StagedCryptoEventViewModel inputObj = jss.Deserialize<StagedCryptoEventViewModel>(input);


            try
            {
                ce.Update(inputObj);
            }
            catch (Exception ex)
            {
                return Json(jss.Serialize(ex.Message), JsonRequestBehavior.AllowGet);
            }
            return Json(jss.Serialize("Record Updated Successfully."), JsonRequestBehavior.AllowGet);
        }
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Editing_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<StagedCryptoEventViewModel> products)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    if (products != null)// && ModelState.IsValid)
        //    {
        //        foreach (var product in products)
        //        {
        //            try
        //            {
        //                ce.Update(product);
        //            }
        //            catch (Exception ex)
        //            {
        //                sb.AppendLine($"Failed to update {product.GetDescription()} Error: {ex.Message}");
        //            }
        //        }
        //    }

        //    if (sb.Length != 0)
        //    {
        //        ModelState.AddModelError(string.Empty, sb.ToString());
        //    }
        //    return Json(products.ToDataSourceResult(request, ModelState));
        //}

        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult remove_staged_events(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();

            StagedCryptoEventViewModel inputObj = jss.Deserialize<StagedCryptoEventViewModel>(input);

            try
            {
                ce.Delete(inputObj);
            }
            catch (Exception ex)
            {
                return Json(jss.Serialize(ex.Message), JsonRequestBehavior.AllowGet);
            }

            return Json(jss.Serialize("Removed!"), JsonRequestBehavior.AllowGet);
        }

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Editing_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<StagedCryptoEventViewModel> products)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    if (products.Any())
        //    {
        //        foreach (var product in products)
        //        {
        //            try
        //            {
        //                ce.Delete(product);
        //            }
        //            catch (Exception ex)
        //            {
        //                sb.AppendLine($"Failed to delete {product.GetDescription()} Error: {ex.Message}");
        //            }
        //        }
        //    }

        //    if(sb.Length != 0)
        //    {
        //        ModelState.AddModelError(string.Empty, sb.ToString());
        //    }

        //    return Json(products.ToDataSourceResult(request, ModelState));
        //}

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Publish_Data()
        {
            Logger.Info("Publishing events...");
            Response.StatusCode = (int)HttpStatusCode.OK;
            if(ce.PublishStagedCryptoEvents())
                return Content("Crypto events published", MediaTypeNames.Text.Plain);
            else
                return Content("Failed to publish one or more crypto events", MediaTypeNames.Text.Plain);
        }


        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult Publish_Selected_Items(string input)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            string[] events_to_publish = input.Split('\u002C');
            CryptoEvent ce = new CryptoEvent();
          
            return Json(jss.Serialize(ce.PublishSelectedEvents(events_to_publish)), JsonRequestBehavior.AllowGet);
        }


        #endregion



        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }



        public ActionResult Events_Remove(string[] fileNames)
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
        public ActionResult Events_Save(IEnumerable<HttpPostedFileBase> files)
        {
            StringBuilder errors = new StringBuilder();
            //try
            //{
            //    // The Name of the Upload component is "files"
            //    string filePath = @"C:\temp\UploadedFiles";

            //    //Path.Combine(Server.MapPath("~/UploadedFiles"));
            //    DirectoryInfo di = new DirectoryInfo(filePath);
            //    if (!di.Exists)
            //        di.Create();

            //    Asset a = new Asset();
            //    string e;
            //    if (files != null)
            //    {
            //        foreach (var file in files)
            //        {
            //            // Some browsers send file names with full path.
            //            // We are only interested in the file name.
            //            try
            //            {
            //                var fileName = Path.GetFileName(file.FileName);
            //                var physicalPath = $@"{filePath}\{fileName}";
            //                file.SaveAs(physicalPath);
            //                if (!a.LoadDataFromExcelFile(physicalPath, out e))
            //                {
            //                    errors.AppendLine(e);
            //                }
            //            }
            //            catch (Exception ex)
            //            {
            //                errors.AppendLine(ex.Message);
            //            }
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    errors.AppendLine(ex.Message);
            //}

            if (errors.Length != 0)
            {
                // Return an empty string to signify success
                return Content(errors.ToString());
            }
            else
            {
                // Return an empty string to signify success
                return Content("");
            }
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
                            ce.LoadDataFromExcelFile(p, out error);
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

            if(errors.Length != 0)
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
    }
}
