using DARReferenceData.DatabaseHandlers;
using DARReferenceData.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DAR_ReferenceDataUI.Controllers
{
    public class OutstandingSupplyController : DARController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);

        // GET: OutstandingSupply
        private OutstandingSupplySource dhSource = new OutstandingSupplySource();

        private OutstandingSupply dhPublished = new OutstandingSupply();

        public ActionResult OutstandingSupplyIndex()
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
            catch (Exception ex)
            {
                return RedirectToInsufficientAcess(ex.Message);
            }
        }

        public ActionResult Editing_CS_Raw_Read([DataSourceRequest] DataSourceRequest request)
        {
            IList<OutstandingSupplyRawViewModel> result = new List<OutstandingSupplyRawViewModel>();
            try
            {
                result = dhSource.GetRawCirculatingSupply().Cast<OutstandingSupplyRawViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }

        public ActionResult Editing_CS_Source_Read([DataSourceRequest] DataSourceRequest request)
        {
            IList<OutstandingSupplySourceViewModel> result = new List<OutstandingSupplySourceViewModel>();
            try
            {
                result = dhSource.Get().Cast<OutstandingSupplySourceViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_CS_Source_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<OutstandingSupplySourceViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<OutstandingSupplySourceViewModel>();

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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_CS_Source_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<OutstandingSupplySourceViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhSource.Update(product);
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
        public ActionResult Editing_CS_Source_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<OutstandingSupplySourceViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhSource.Delete(product);
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

        public ActionResult Editing_Published_Read([DataSourceRequest] DataSourceRequest request, DateTime startDate)
        {
            IList<OutstandingSupplyViewModel> result = new List<OutstandingSupplyViewModel>();
            try
            {
                result = dhPublished.GetFinalizedOutStandingSupply(startDate).Cast<OutstandingSupplyViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Published_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<OutstandingSupplyViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<OutstandingSupplyViewModel>();

            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        dhPublished.Add(product);
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
        public ActionResult Editing_Published_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<OutstandingSupplyViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhPublished.Update(product);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Failed to {product.GetDescription()} Error: {ex.Message}");
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
        public ActionResult Editing_Published_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<OutstandingSupplyViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhPublished.Delete(product);
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
        public ActionResult Publish_Data(string startDate)
        {
            return Content("Obsolete method", MediaTypeNames.Text.Plain);
           }
    }
}