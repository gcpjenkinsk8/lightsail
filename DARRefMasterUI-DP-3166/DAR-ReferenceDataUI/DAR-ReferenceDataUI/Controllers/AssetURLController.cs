using DARReferenceData.DatabaseHandlers;
using DARReferenceData.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DAR_ReferenceDataUI.Controllers
{
    public class AssetURLController : DARController
    {

        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        RefDataHandler dh = new AssetURL();
        RefDataHandler utHandler = new UrlType();
        RefDataHandler aHandler = new Asset();
        // GET: AssetURL
        public ActionResult AssetURLIndex()
        {
            try
            {
                var r = ValidateUser();

                if (r == null)
                {
                    var urlTypes = utHandler.Get().Cast<UrlTypeViewModel>();
                    ViewData["UrlTypes"] = urlTypes;
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

        public ActionResult Editing_Read([DataSourceRequest] DataSourceRequest request)
        {
         
           IList<AssetURLViewModel> result = new List<AssetURLViewModel>();
            try
            {
                result = dh.Get().Cast<AssetURLViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<AssetURLViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<AssetURLViewModel>();

            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        dh.Add(product);
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
        public ActionResult Editing_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<AssetURLViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    try
                    {
                        dh.Update(product);
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
        public ActionResult Editing_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<AssetURLViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        dh.Delete(product);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Failed to delete {product.GetDescription()} link. Error: {ex.Message}");
                    }
                }
            }

            if (sb.Length != 0)
            {
                ModelState.AddModelError(string.Empty, sb.ToString());
            }

            return Json(products.ToDataSourceResult(request, ModelState));
        }

    }
}