using DARReferenceData.DatabaseHandlers;
using DARReferenceData.ViewModels;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using Kendo.Mvc.Extensions;
using System.Text;
using System.IO;

namespace DAR_ReferenceDataUI.Controllers
{
    public class ExchangeController : DARController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        Exchange dhExchange = new Exchange();
        ProcessDar dhProcess = new ProcessDar();
        VettingStatus dhVettingStatus = new VettingStatus();
        // GET: Exchange

        public ActionResult ExchangeIndex()
        {
            try
            {
                // ViewData["Process"] = dhProcess.Get();
                ViewData["Exchange"] = dhExchange.Get();
                //ViewData["VettingStatus"] = dhVettingStatus.Get();
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

        public ActionResult ExchangeUpload()
        {
            return View();
        }


        #region Exchange
        public ActionResult Exchange_Read([DataSourceRequest] DataSourceRequest request)
        {
            IList<ExchangeViewModel> result = new List<ExchangeViewModel>();
            try
            {
                result = dhExchange.Get().Cast<ExchangeViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Exchange_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ExchangeViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<ExchangeViewModel>();

            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        dhExchange.Add(product);
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
        public ActionResult Exchange_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ExchangeViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhExchange.Update(product);
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
        public ActionResult Exchange_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ExchangeViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhExchange.Delete(product);
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
        #endregion

    


        #region Exchange Upload
        public ActionResult Events_Save(IEnumerable<HttpPostedFileBase> files)
        {
            StringBuilder errors = new StringBuilder();
            try
            {
                // The Name of the Upload component is "files"
                string filePath = @"C:\temp\UploadedFiles";

                //Path.Combine(Server.MapPath("~/UploadedFiles"));
                DirectoryInfo di = new DirectoryInfo(filePath);
                if (!di.Exists)
                    di.Create();

                Exchange a = new Exchange();
                string e;
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        // Some browsers send file names with full path.
                        // We are only interested in the file name.
                        try
                        {

                            var fileName = Path.GetFileName(file.FileName);
                            var physicalPath = $@"{filePath}\{fileName}";
                            file.SaveAs(physicalPath);
                            if (!a.LoadDataFromExcelFile(physicalPath, out e))
                            {
                                errors.AppendLine(e);
                            }

                        }
                        catch (Exception ex)
                        {
                            errors.AppendLine(ex.Message);
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                errors.AppendLine(ex.Message);
            }

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
        #endregion



    }
}