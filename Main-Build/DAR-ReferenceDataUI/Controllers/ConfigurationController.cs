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
    public class ConfigurationController : DARController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        private RefDataHandler dhVettingStatus = new VettingStatus();
        private RefDataHandler _dhClient = new Client();
        private ClientAsset _dhClientAsset = new ClientAsset();
        private CallerID _dhCaller = new CallerID();

        /*        internal class CallerIdMap : EntityMap<CallerIDViewModel>
                {
                    internal CallerIdMap()
                    {
                        Map(u => u.CallerID).ToColumn("ID");
                        Map(u => u.IPAddress).ToColumn("CallerID");
                    }
                }
        */

        // GET: VettingStatus
        public ActionResult ConfigurationIndex()
        {
            try
            {
                var r = ValidateUser();

                if (r == null)
                {
                    /*                    FluentMapper.Initialize(config =>
                                        {
                                            config.AddMap(new CallerIdMap());
                                        });
                    */
                    ViewData["VettingTypes"] = Configuration.GetVettingTypes();
                    ViewData["Assets"] = Asset.GetAssetList();
                    ViewData["Clients"] = Client.GetClientList();
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

        public JsonResult GetClientNames(string text)
        {
            IList<ClientViewModel> result = new List<ClientViewModel>();
            try
            {
                result = _dhClient.Get().Cast<ClientViewModel>().GroupBy(elem => elem.ClientName).Select(group => group.First()).ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #region Client

        public ActionResult Client_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<ClientViewModel> clients = new List<ClientViewModel>();
            try
            {
                clients = _dhClient.Get().Cast<ClientViewModel>().ToList();
                clients.Sort((x, y) =>
                {
                    return string.Compare(x.ClientName.ToLower(), y.ClientName.ToLower());
                });
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(clients.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Client_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ClientViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<ClientViewModel>();

            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        _dhClient.Add(product);
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
        public ActionResult Client_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ClientViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    try
                    {
                        _dhClient.Update(product);
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
        public ActionResult Client_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ClientViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        _dhClient.Delete(product);
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

        #endregion Client

        #region Client Assets

        public ActionResult ClientAssets_Read([DataSourceRequest] DataSourceRequest request, string clientName)
        {
            IList<ClientAssetsViewModel> result = new List<ClientAssetsViewModel>();
            try
            {
                //result = dhClientAsset.GetClientAssets(clientName).Cast<ClientAssetsViewModel>().ToList();
                result = _dhClientAsset.GetClientAssets(clientName).Cast<ClientAssetsViewModel>().OrderBy(e => e.AssetName).ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }

        [HttpPost]
        public ActionResult ClientAssets_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ClientAssetsViewModel> products, string clientName)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<ClientAssetsViewModel>();

            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        product.ClientName = clientName;
                        _dhClientAsset.Add(product);
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
        public ActionResult ClientAssets_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ClientAssetsViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    try
                    {
                        _dhClientAsset.Update(product);
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
        public ActionResult ClientAssets_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ClientAssetsViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        _dhClientAsset.Delete(product);
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

        #endregion Client Assets

        #region CallerID

        public ActionResult CallerID_Read([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<CallerIDViewModel> callers = new List<CallerIDViewModel>();
            try
            {
                var results = _dhCaller.Get().Cast<CallerIDViewModel>();
                callers = results.GroupBy(x => x.ClientName)
                    .Select(e => e.OrderBy(y => y.CallerID))
                    .SelectMany(x => x); // flatten the group
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }
            return Json(callers.ToList().ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CallerID_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<CallerIDViewModel> payload)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<CallerIDViewModel>();

            if (payload.Any() && ModelState.IsValid)
            {
                foreach (var item in payload)
                {
                    try
                    {
                        _dhCaller.Add(item);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Failed to add {item.GetDescription()} Error: {ex.Message}");
                    }

                    results.Add(item);
                }
            }

            if (sb.Length > 0)
            {
                ModelState.AddModelError(string.Empty, sb.ToString());
            }
            return Json(results.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CallerID_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<CallerIDViewModel> payload)
        {
            StringBuilder sb = new StringBuilder();
            if (payload.Any() && ModelState.IsValid)
            {
                foreach (var item in payload)
                {
                    try
                    {
                        _dhCaller.Update(item);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Failed to updated {item.GetDescription()} Error: {ex.Message}");
                    }
                }
            }
            if (sb.Length != 0)
            {
                ModelState.AddModelError(string.Empty, sb.ToString());
            }

            return Json(payload.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CallerID_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<CallerIDViewModel> payload)
        {
            StringBuilder sb = new StringBuilder();
            if (payload.Any())
            {
                foreach (var item in payload)
                {
                    try
                    {
                        _dhCaller.Delete(item);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Failed to delete {item.GetDescription()} Error: {ex.Message}");
                    }
                }
            }
            if (sb.Length > 0)
            {
                ModelState.AddModelError(string.Empty, sb.ToString()); ;
            }

            return Json(payload.ToDataSourceResult(request, ModelState));
        }

        #endregion CallerID

        #region Vetting Status

        public ActionResult VettingStatus_Read([DataSourceRequest] DataSourceRequest request)
        {
            IList<VettingStatusViewModel> result = new List<VettingStatusViewModel>();
            try
            {
                result = dhVettingStatus.Get().Cast<VettingStatusViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VettingStatus_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<VettingStatusViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<VettingStatusViewModel>();

            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        dhVettingStatus.Add(product);
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
        public ActionResult VettingStatus_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<VettingStatusViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhVettingStatus.Update(product);
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
        public ActionResult VettingStatus_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<VettingStatusViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhVettingStatus.Delete(product);
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

        #endregion Vetting Status

        #region File Download

        public ActionResult TemplateDownload()
        {
            return View();
        }

        #endregion File Download
    }
}