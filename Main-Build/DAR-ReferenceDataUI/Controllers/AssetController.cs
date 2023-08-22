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
    public class AssetController : DARController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        private Asset dh = new Asset();
        private RefDataHandler tHandler = new Theme();
        private RefDataHandler utHandler = new UrlType();
        private BlockChain dhBlockchain = new BlockChain();
        private Token dhToken = new Token();
        private AssetToken dhAssetToken = new AssetToken();
        private ProcessDar dhProcess = new ProcessDar();
        private VettingStatus dhVettingStatus = new VettingStatus();
        private Custodian dhCustodian = new Custodian();
        private AssetCustodian dhAssetCustodian = new AssetCustodian();

        // GET: Asset
        public ActionResult AssetIndex()
        {
            try
            {
                var r = ValidateUser();

                if (r == null)
                {
                    var themes = tHandler.Get().Cast<ThemeViewModel>();
                    ViewData["Themes"] = themes;
                    var urlTypes = utHandler.Get().Cast<UrlTypeViewModel>();
                    ViewData["UrlTypes"] = urlTypes;
                    //ViewData["Process"] = dhProcess.Get();
                    ViewData["Asset"] = dh.Get().Cast<AssetViewModel>().OrderBy(x => x.Name);
                    // ViewData["VettingStatus"] = dhVettingStatus.Get();
                    ViewData["Custodian"] = dhCustodian.Get().Cast<CustodianViewModel>().OrderBy(x => x.Name);

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

        #region Asset


        public JsonResult GetAssetNames(string text)
        {
            IList<AssetViewModel> result = new List<AssetViewModel>();

           
            try
            {
                result = dh.GetAssets().Cast<AssetViewModel>().ToList();
                //if (!string.IsNullOrWhiteSpace(text))
                //{
                //    result = result.Where(x => x.Name.StartsWith(text, StringComparison.InvariantCultureIgnoreCase)).ToList();
                //}
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Editing_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<AssetViewModel> assets = new List<AssetViewModel>();
            try
            {
                assets = dh.GetAssetView().Cast<AssetViewModel>().ToList();
                assets.Sort((a, b) => { return a.DARTicker.CompareTo(b.DARTicker); });
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(assets.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<AssetViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<AssetViewModel>();

            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        dh.AddAsset(product);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Failed to add {product.GetDescription()} Error:{ex.Message}");
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
        public ActionResult Editing_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<AssetViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    try
                    {
                        var action = "Manual";
                        dh.UpdateAsset(product, action);
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
        public ActionResult Editing_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<AssetViewModel> products)
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

        #endregion Asset

        #region Blockchain

        public ActionResult Blockchain_Read([DataSourceRequest] DataSourceRequest request)
        {
            IList<BlockChainViewModel> result = new List<BlockChainViewModel>();
            try
            {
                result = dhBlockchain.Get().Cast<BlockChainViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Blockchain_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<BlockChainViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<BlockChainViewModel>();

            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        dhBlockchain.Add(product);
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
        public ActionResult Blockchain_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<BlockChainViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhBlockchain.Update(product);
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
        public ActionResult Blockchain_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<BlockChainViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhBlockchain.Delete(product);
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

        #endregion Blockchain

        #region AssetToken

        public ActionResult AssetToken_Read([DataSourceRequest] DataSourceRequest request)
        {
            IList<AssetTokenViewModel> result = new List<AssetTokenViewModel>();
            try
            {
                result = dhAssetToken.GetAssetTokenView().Cast<AssetTokenViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetToken_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<AssetTokenViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<AssetTokenViewModel>();

            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        dhToken.UpsertAssetToken(product.DARTokenID, product.DARAssetID, product.DARTicker, product.BlockChain, product.TokenContractAddress, 0);
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
        public ActionResult AssetToken_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<AssetTokenViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhToken.UpsertAssetToken(product.DARTokenID, product.DARAssetID, product.DARTicker, product.BlockChain, product.TokenContractAddress, 0);
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
        public ActionResult AssetToken_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<AssetTokenViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhAssetToken.Delete(product);
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

        public ActionResult Token_Upload(IEnumerable<HttpPostedFileBase> filesTokens)
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

                string e;
                if (filesTokens != null)
                {
                    foreach (var file in filesTokens)
                    {
                        // Some browsers send file names with full path.
                        // We are only interested in the file name.
                        try
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var physicalPath = $@"{filePath}\{fileName}";
                            file.SaveAs(physicalPath);
                            if (!dhToken.LoadDataFromExcelFile(physicalPath, out e))
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

        #endregion AssetToken

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

                Asset a = new Asset();
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


        #region Custodian

        public ActionResult Custodian_Read([DataSourceRequest] DataSourceRequest request)
        {
            IList<CustodianViewModel> result = new List<CustodianViewModel>();
            try
            {
                result = dhCustodian.Get().Cast<CustodianViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Custodian_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<CustodianViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<CustodianViewModel>();

            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        dhCustodian.Add(product);
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
        public ActionResult Custodian_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<CustodianViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhCustodian.Update(product);
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
        public ActionResult Custodian_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<CustodianViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhCustodian.Delete(product);
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

        #endregion Custodian

        #region Asset Custodian

        public ActionResult AssetCustodian_Read([DataSourceRequest] DataSourceRequest request, string assetName)
        {
            IList<AssetCustodianViewModel> result = new List<AssetCustodianViewModel>();
            try
            {
                result = dhAssetCustodian.GetAssetCustodianList(assetName).Cast<AssetCustodianViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AssetCustodian_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<AssetCustodianViewModel> products, string assetName)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<AssetCustodianViewModel>();



            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        dhAssetCustodian.Add(product);
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
        public ActionResult AssetCustodian_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<AssetCustodianViewModel> products, string assetName)
        {
            //if (string.IsNullOrWhiteSpace(assetName) || assetName.Equals("TBD"))
            //{
            //    ModelState.AddModelError(String.Empty, $"Please select a valid asset name. Received input {assetName}");
            //    return Json(products.ToDataSourceResult(request, ModelState));
            //}

            StringBuilder sb = new StringBuilder();
            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    try
                    {
                        product.DARAssetID = assetName;
                        dhAssetCustodian.Update(product);
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
        public ActionResult AssetCustodian_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<AssetCustodianViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        dhAssetCustodian.Delete(product);
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

        #endregion Asset Custodian
    }
}