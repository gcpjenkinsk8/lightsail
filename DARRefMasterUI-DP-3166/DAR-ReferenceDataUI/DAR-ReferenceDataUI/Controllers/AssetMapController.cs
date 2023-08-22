using DARReferenceData.DatabaseHandlers;
using DARReferenceData.ViewModels;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Script.Services;

namespace DAR_ReferenceDataUI.Controllers
{
    public class ElectricityProduction
    {
        public ElectricityProduction()
        {
        }

        public ElectricityProduction(string year, int solar, int hydro, int wind, int nuclear)
        {
            Year = year;
            Solar = solar;
            Hydro = hydro;
            Wind = wind;
            Nuclear = nuclear;
        }

        public string Year { get; set; }
        public int Solar { get; set; }
        public int Nuclear { get; set; }
        public int Hydro { get; set; }
        public int Wind { get; set; }
    }
    public class TestModel
    {
        public string channel { get; set; }
        public int conversion { get; set; }

        public int users { get; set; }

    }




    public class AssetMapController : DARController
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        // GET: AssetMap
        private Asset dh = new Asset();
        ExchangePairs ep = new ExchangePairs();
       
        public ActionResult AssetMapIndex()
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

        public JsonResult Get_Fluctuating_Assets(string clients)
        {
            string result = @"{channel: 'Organic Search', conversion: 8232, users: 70500 },
                              {channel: 'Direct', conversion: 6574, users: 24900 },
                              { channel: 'Referral', conversion: 4932, users: 20000 },
                              { channel: 'Social Media', conversion: 2928, users: 19500 },
                              { channel: 'Email', conversion: 2456, users: 18100 },
                              { channel: 'Other', conversion: 1172, users: 16540 },}";
            try
            {
            }
            catch (Exception ex)
            {
            }
            var jsonWrapper = new
            {
                clients = $"hello"
            };

            return Json(result, JsonRequestBehavior.AllowGet);         
        }






        [HttpGet]
        [ScriptMethod(UseHttpGet = true, ResponseFormat = ResponseFormat.Json)]
        public JsonResult Get_Fluctuating_Assets_NP()
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();

            List<TestModel> result = new List<TestModel>();
            result.Add(new TestModel() { channel = "Ch1", conversion = 500, users=100});
            result.Add(new TestModel() { channel = "Ch2", conversion = 500, users = 100 });

           
            try
            {
            }
            catch (Exception ex)
            {
            }
            
           
            return Json(jss.Serialize(result), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            List<AssetViewModel> assets = new List<AssetViewModel>();
            try
            {
                assets = dh.GetAssetView().Cast<AssetViewModel>().ToList();
                assets.Sort((a, b) => { return a.DARTicker.CompareTo(b.DARTicker); });
            }
            catch (Exception ex)
            {

            }

            return Json(assets.ToDataSourceResult(request));
        }


        [HttpPost]
        public ActionResult _SpainElectricityProduction()
        {
            // Data is usually read from a service that communicates with the an instance of the DbContext. 
            // Refer to the MSDN documentation for further details on connecting to a data base and using a DBContext:
            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext?view=efcore-5.0
            //
            // For clarity, in this example static data is generated and returned from the remote-endpoint.
            var result = new ElectricityProduction[] {
                new ElectricityProduction("2000", 18, 31807, 4727, 62206),
                new ElectricityProduction("2001", 24, 43864, 6759, 63708),
                new ElectricityProduction("2002", 30, 26270, 9342, 63016),
                new ElectricityProduction("2003", 41, 43897, 12075, 61875),
                new ElectricityProduction("2004", 56, 34439, 15700, 63606),
                new ElectricityProduction("2005", 41, 23025, 21176, 57539),
                new ElectricityProduction("2006", 119, 29831, 23297, 60126),
                new ElectricityProduction("2007", 508, 30522, 27568, 55103),
                new ElectricityProduction("2008", 2578, 26112, 32203, 58973)
            };
            return Json(result);
        }


        #region Exchange Pair Map
        public ActionResult Editing_ExchangePair_Read([DataSourceRequest] DataSourceRequest request)
        {
            IList<ExchangePairsViewModel> result = new List<ExchangePairsViewModel>();
            try
            {
                result = ep.Get().Cast<ExchangePairsViewModel>().ToList();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
            }

            return Json(result.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_ExchangePair_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ExchangePairsViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            var results = new List<ExchangePairsViewModel>();

            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    //Create and return
                    try
                    {
                        ep.Add(product);
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
        public ActionResult Editing_ExchangePair_Update([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ExchangePairsViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products != null && ModelState.IsValid)
            {
                foreach (var product in products)
                {
                    try
                    {
                        ep.Update(product);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Failed to update {product.GetDescription()}  Error: {ex.Message}");
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
        public ActionResult Editing_ExchangePair_Destroy([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")] IEnumerable<ExchangePairsViewModel> products)
        {
            StringBuilder sb = new StringBuilder();
            if (products.Any())
            {
                foreach (var product in products)
                {
                    try
                    {
                        ep.Delete(product);
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine($"Failed to delete {product.GetDescription()} {product.GetDescription()} link. Error: {ex.Message}");
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

    }
}