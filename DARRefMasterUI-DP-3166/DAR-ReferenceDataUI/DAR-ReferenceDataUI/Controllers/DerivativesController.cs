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
    public class DerivativesController : DARController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        Derivatives dhDerivatives = new Derivatives();
        // GET: Derivatives
        public ActionResult DerivativesIndex()
        {
            try
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
            catch(Exception ex)
            {
                return RedirectToInsufficientAcess(ex.Message);
            }
        }

        public ActionResult DervativesUpload()
        {
            return View();
        }



        public ActionResult Derivatives_Read([DataSourceRequest] DataSourceRequest request)
        {
            IList<DerivativesViewModel> result = new List<DerivativesViewModel>();
            try
            {
                result = dhDerivatives.Get().Cast<DerivativesViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Derivatives_Create([DataSourceRequest] DataSourceRequest request, DerivativesViewModel product)
        {
            var results = new List<DerivativesViewModel>();

            //Create and return
            try
            {
                dhDerivatives.Add(product);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.IsNullOrWhiteSpace(product.DARContractID) ? string.Empty: product.DARContractID , $"Failed to add {product.GetDescription()} Error: {ex.Message}");
            }
            results.Add(product);
       
            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Derivatives_Update([DataSourceRequest] DataSourceRequest request, DerivativesViewModel product)
        {
            try
            {
                dhDerivatives.Update(product);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.IsNullOrWhiteSpace(product.DARContractID) ? string.Empty : product.DARContractID, $"Failed to update {product.GetDescription()} Error: {ex.Message}");
            }

            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Derivatives_Destroy([DataSourceRequest] DataSourceRequest request, DerivativesViewModel product)
        {
       
            try
            {
                dhDerivatives.Delete(product);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.IsNullOrWhiteSpace(product.DARContractID) ? string.Empty : product.DARContractID, $"Failed to delete {product.GetDescription()} Error: {ex.Message}");
            }

            return Json(new[] { product }.ToDataSourceResult(request, ModelState));
        }


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

                Derivatives a = new Derivatives();
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

    }
}