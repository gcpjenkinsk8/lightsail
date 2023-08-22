using DAR_ReferenceDataUI.Helpers;
using DARReferenceData.DatabaseHandlers;
using DARReferenceData.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DAR_ReferenceDataUI.Controllers
{
    public class ServingListController : DARController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);

        Pair dhPair = new Pair();
        ProcessDar dhProcess = new ProcessDar();
        ServingList dhServingList = new ServingList();
        // GET: ServingList
        public ActionResult ServingListIndex()
        {
            try
            {
                var r = ValidateUser();

                if (r == null)
                {
                    ViewData["TimeUnits"] = Configuration.GetTimeUnits();
                    return View();
                }
                else
                {
                    return r;
                }
            }
            catch(Exception ex)
            {
                return RedirectToInsufficientAcess(ex.Message);
            }
        }

        public ActionResult Pair_Read([DataSourceRequest] DataSourceRequest request)
        {
            IList<PairViewModel> result = new List<PairViewModel>();
            try
            {
                result = dhPair.GetPairView().Cast<PairViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Pair_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PairViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<PairViewModel>();

            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        dhPair.AddPair(product);
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
        public ActionResult Pair_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PairViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhPair.Update(product);
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
        public ActionResult Pair_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<PairViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhPair.Delete(product);
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



        public JsonResult GetProcessNames(string text)
        {
            IList<ProcessViewModel> result = new List<ProcessViewModel>();
            try
            {
                result = dhProcess.Get().Cast<ProcessViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Process_Read([DataSourceRequest] DataSourceRequest request)
        {
            IList<ProcessViewModel> result = new List<ProcessViewModel>();
            try
            {
                result = dhProcess.Get().Cast<ProcessViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Process_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ProcessViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<ProcessViewModel>();

            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        dhProcess.AddProcess(product);
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
        public ActionResult Process_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ProcessViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhProcess.Update(product);
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
        public ActionResult Process_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ProcessViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhProcess.Delete(product);
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


        public ActionResult ServingList_Read([DataSourceRequest] DataSourceRequest request, string processName)
        {
            IList<ServingListViewModel> result = new List<ServingListViewModel>();
            try
            {
                result = dhServingList.GetServingListView(processName).Cast<ServingListViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }

        public ActionResult ServingListSnapshot_Read([DataSourceRequest] DataSourceRequest request, string snapshotName)
        {
            IList<ServingListSnapshotViewModel> result = new List<ServingListSnapshotViewModel>();
            try
            {
                result = dhServingList.GetServingListSnapshot(snapshotName).Cast<ServingListSnapshotViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }

        public JsonResult ServingListSnapshot_GetSnapshotNames(string text)
        {
            IList<ServingListSnapshotViewModel> result = new List<ServingListSnapshotViewModel>();
            try
            {
                result = dhServingList.GetSnapshotNames().Cast<ServingListSnapshotViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }


            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServingList_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ServingListViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhServingList.Delete(product);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Failed to delete {product.GetDescription()} Entry:{product.ExchangePairName} Error: {ex.Message}");
                    }
                }
            }

            if (sb.Length != 0)
            {
                ModelState.AddModelError(string.Empty, sb.ToString());
            }

            return Json(products.ToDataSourceResult(request, ModelState));
        }


        public ActionResult Events_Upload(IEnumerable<HttpPostedFileBase> files)
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

                ServingList a = new ServingList();
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

                FileHelper fh = new FileHelper();
                fh.CleanupFolder(filePath, "*.*", 3);

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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ServingList_Snap(string processName)
        {
            //dhServingList.SnapshotServingList(processName);
            return Json("Method is not supported at this time");
        }



    }
}