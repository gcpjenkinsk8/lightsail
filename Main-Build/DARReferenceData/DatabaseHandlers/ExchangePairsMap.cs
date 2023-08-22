using Confluent.Kafka;
using Dapper;
using DARReferenceData.ViewModels;
using log4net;
using MySql.Data.MySqlClient;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.HtmlControls;
using static log4net.Appender.RollingFileAppender;

namespace DARReferenceData.DatabaseHandlers

{
    public class ExchangePairsMap : RefDataHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);

        public override long Add(DARViewModel i)
        {
            var a = (ExchangePairsV2ViewModel)i;

            var asset = (new Asset()).Get(a.DARAssetID) as AssetViewModel;
            if (asset == null)
                throw new Exception($"Invalid asset {a.DARAssetID}");

            var exchangeID = (new Exchange()).Get(a.DARExchangeID) as ExchangeViewModel;
            if (exchangeID == null)
                throw new Exception($"Invalid Exchange ID {a.DARExchangeID}");


            a.darPairID = GetNextId();

            Thread.Sleep(1000);

            //checking duplicate record in the Database.
            var existing_record = GetDuplicateRecord_exchangepairsv2(a.ExchangeName, a.ExchangePair, a.AssetTicker, a.CurrencyTicker);
            if (existing_record != null)
            {
                throw new Exception("Record already exists in the Database.");
            }
            
            ExchangePairsV2ViewModel l = new ExchangePairsV2ViewModel()
            {
                legacyExchangeID = a.legacyExchangeID,
                ExchangeName = a.ExchangeName.Trim(),
                DARExchangeID = a.DARExchangeID.Trim(),
                ExchangePair = a.ExchangePair,
                legacyAssetID = a.legacyAssetID,
                AssetTicker = a.AssetTicker.Trim(),
                AssetName = a.AssetName.Trim(),
                DARAssetID = a.DARAssetID.Trim(),
                legacyCurrencyID = a.legacyCurrencyID,
                CurrencyTicker = a.CurrencyTicker.Trim(),
                CurrencyName = a.CurrencyName.Trim(),
                DARCurrencyID = a.DARCurrencyID.Trim(),
                blockchain = a.blockchain.Trim(),
                contractAddress = a.contractAddress.Trim(),
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                darPairID = a.darPairID
            };

            var end_date_year = l.EndDate.Year.ToString();
            var start_date_year = l.StartDate.Year.ToString();

            if (string.Equals(end_date_year, "9999"))
            {
                l.EndDate = new DateTime(l.EndDate.Year, l.EndDate.Month, l.EndDate.Day, 00, 00, 00);
            }

            if (string.Equals(start_date_year, "1900"))
            {
                l.StartDate = new DateTime(l.StartDate.Year, l.StartDate.Month, l.StartDate.Day, 00, 00, 00);
            }

            if (string.Equals(start_date_year, "1899"))
            {
                l.StartDate = DateTime.Parse("1900-01-01 00:00:00.000000");
            }

            l.Operation = "INSERT";
            l.Deleted = 0;

            if (l.legacyCurrencyID == 0)
                throw new Exception($"Enter valid Values for the Currency fields as its LegacyID is 0");
            if (l.legacyAssetID == 0)
                throw new Exception($"Enter valid Values for the Asset fields as its LegacyID is 0");
            if (l.legacyExchangeID == 0 && !l.ExchangeName.Equals("Bkex", StringComparison.InvariantCultureIgnoreCase))
                throw new Exception($"Enter valid Values for the Exchange fields as its LegacyID is 0");

            string publishStatus = ExchangPairsPublish(l);


            return 0;
        }

        public override bool Delete(DARViewModel i)
        {
            throw new NotImplementedException();
        }

        public bool DeleteExchangePairsV2(ExchangePairsV2ViewModel record)
        {
            record.Operation = "DELETE";
            record.Deleted = 1;
            
            string publishStatus = ExchangPairsPublish(record);
            return true;
        }

        public override bool Update(DARViewModel i)
        {
            throw new NotImplementedException();
        }


        public override IEnumerable<DARViewModel> Get()
        {
            List<ExchangePairsV2ViewModel> l = new List<ExchangePairsV2ViewModel>();

            string sql = $@"select
                            CONCAT(legacyExchangeID, exchangePair, startDate) as ExchangePairID
                            ,e.legacyExchangeID
                            ,e.exchangeName
                            ,e.darExchangeID
                            ,e.exchangePair
                            ,e.legacyAssetID
                            ,e.assetTicker
                            ,e.assetName
                            ,e.darAssetID
                            ,e.legacyCurrencyID
                            ,e.currencyTicker
                            ,e.currencyName
                            ,e.darCurrencyID
                            ,e.blockchain
                            ,e.contractAddress
                            ,e.startDate
                            ,e.endDate
                            ,e.darPairID
                            ,unix_timestamp(e.LoadTime) as LoadTimestamp
                            from {DARApplicationInfo.SingleStoreCatalogPublic}.exchangePairsV2 e
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<ExchangePairsV2ViewModel>(sql).ToList();
            }

            return l;
        }

        public ExchangePairsV2ViewModel GetExchangePairsV2(string id)
        {
            List<ExchangePairsV2ViewModel> l = new List<ExchangePairsV2ViewModel>();

            string sql = $@"select
                            CONCAT(legacyExchangeID, exchangePair, startDate) as ExchangePairID
                            ,e.legacyExchangeID
                            ,e.exchangeName
                            ,e.darExchangeID
                            ,e.exchangePair
                            ,e.legacyAssetID
                            ,e.assetTicker
                            ,e.assetName
                            ,e.darAssetID
                            ,e.legacyCurrencyID
                            ,e.currencyTicker
                            ,e.currencyName
                            ,e.darCurrencyID
                            ,e.blockchain
                            ,e.contractAddress
                            ,e.startDate
                            ,e.endDate
                            ,e.darPairID
                            ,unix_timestamp(e.LoadTime) as LoadTimestamp
                            from {DARApplicationInfo.SingleStoreCatalogPublic}.exchangePairsV2 e
                            WHERE CONCAT (legacyExchangeID, exchangePair, startDate) = '{id}'
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<ExchangePairsV2ViewModel>(sql).ToList();
            }

            return l.FirstOrDefault();
        }

        public bool Adjust_exchange_pairs_v2(DARViewModel i)
        {
            var a = (ExchangePairsV2ViewModel)i;

            var old_record = GetExchangePairsV2(a.ExchangePairID);

            DeleteExchangePairsV2(old_record);
            try
            {
                Add(a);
            }
            catch (Exception ex)
            {
                Add(old_record);
                throw new Exception(ex.Message);
            }
            
            return true;

        }


        public bool Roll_exchange_pairs_v2(DARViewModel i)
        {
            var a = (ExchangePairsV2ViewModel)i;
            var lastItem = GetExchangePairsV2(a.ExchangePairID);
            var lastItem_copy = lastItem;
            if (lastItem.StartDate.ToString("yyyyMMddHHmmss").Equals(a.StartDate.ToString("yyyyMMddHHmmss")))
            {
                a.StartDate = DateTime.Now;
            }

            lastItem.EndDate = a.StartDate.AddSeconds(-1);
            if (lastItem.StartDate > lastItem.EndDate)
            {
                throw new Exception("Start date can not be greater than end date");
            }
            if (a.StartDate > a.EndDate)
            {
                throw new Exception("Start date can not be greater than end date");
            }

            var old_record = GetExchangePairsV2(lastItem_copy.ExchangePairID);

            DeleteExchangePairsV2(old_record);
            try
            {
                Add(lastItem);
                Add(a);
            }
            catch (Exception ex)
            {
                DeleteExchangePairsV2(lastItem);
                Add(old_record);
                throw new Exception(ex.Message);
            }
            
            return true;
        }

        public bool remove_exchange_pairs_v2(DARViewModel i)
        {
            var a = (ExchangePairsV2ViewModel)i;
            
            DeleteExchangePairsV2(a);
            return true;
        }

        public IEnumerable<DARViewModel> GetExchangeStatus()
        {
            List<ExchangeStatusViewModel> l = new List<ExchangeStatusViewModel>();

            string sql = $@"select
                            CONCAT(darExchangeID, darMnemonicFamily,endTime) as ExchangeStatusID
                            ,e.DARExchangeID 
                            ,e.DARMnemonicFamily
                            ,e.vettedStatus
                            ,e.StartTime
                            ,e.EndTime
                            ,unix_timestamp(e.loadTime) as LoadTimestamp
                            from {DARApplicationInfo.SingleStoreCatalogPublic}.exchangeStatus e
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<ExchangeStatusViewModel>(sql).ToList();
            }

            return l;
        }
        public ExchangeStatusViewModel GetExchangeStatus(string id)
        {
            List<ExchangeStatusViewModel> l = new List<ExchangeStatusViewModel>();

            string sql = $@"select
                            CONCAT(darExchangeID, darMnemonicFamily,endTime) as ExchangeStatusID
                            ,e.DARExchangeID 
                            ,e.DARMnemonicFamily
                            ,e.vettedStatus
                            ,e.StartTime
                            ,e.EndTime
                            ,unix_timestamp(e.loadTime) as LoadTimestamp
                            from {DARApplicationInfo.SingleStoreCatalogPublic}.exchangeStatus e
                            where CONCAT(darExchangeID, darMnemonicFamily,endTime) = '{id}'
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<ExchangeStatusViewModel>(sql).ToList();
            }

            return l.FirstOrDefault();
        }
        public bool CreateExchangeStatus(DARViewModel i)
        {
            var a = (ExchangeStatusViewModel)i;

            var exchangeID = (new Exchange()).Get(a.DARExchangeID) as ExchangeViewModel;
            if (exchangeID == null)
                throw new Exception($"Invalid Exchange ID {a.DARExchangeID}");

            ExchangeStatusViewModel l = new ExchangeStatusViewModel()
            {
                DARExchangeID = a.DARExchangeID.Trim(),
                DARMnemonicFamily = a.DARMnemonicFamily,
                vettedStatus = a.vettedStatus,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                CreateUser = string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.User.Identity.Name) ? Environment.UserName : System.Web.HttpContext.Current.User.Identity.Name,
                LastEditUser = string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.User.Identity.Name) ? Environment.UserName : System.Web.HttpContext.Current.User.Identity.Name
            };

            var end_date_year = l.EndTime.Year.ToString();
            var start_date_year = l.StartTime.Year.ToString();

            if (string.Equals(end_date_year, "9999"))
            {
                l.EndTime = new DateTime(l.EndTime.Year, l.EndTime.Month, l.EndTime.Day, 00, 00, 00);
            }

            if (string.Equals(start_date_year, "1900"))
            {
                l.StartTime = new DateTime(l.StartTime.Year, l.StartTime.Month, l.StartTime.Day, 00, 00, 00);
            }

            if (string.Equals(start_date_year, "1899"))
            {
                l.StartTime = DateTime.Parse("1900-01-01 00:00:00.000000");
            }

            l.Operation = "INSERT";
            l.Deleted = 0;

            string publishStatus = ExchangStatusPublish(l);
            return true;
        }

        public bool AdjustExchangeStatus(DARViewModel i)
        {
            var a = (ExchangeStatusViewModel)i;

            var old_record = GetExchangeStatus(a.ExchangeStatusID);

            DeleteExchangeStatus(old_record);
            CreateExchangeStatus(a);
            return true;

        }

        public bool remove_exchange_status(DARViewModel i)
        {
            var a = (ExchangeStatusViewModel)i;
            DeleteExchangeStatus(a);
            return true;
        }
        

        public bool RollExchangeStatus(DARViewModel i)
        {
            var a = (ExchangeStatusViewModel)i;
            var lastItem = GetExchangeStatus(a.ExchangeStatusID);
            var lastItem_copy = lastItem;
            if (a.StartTime.ToString("yyyyMMddHHmmss").Equals(lastItem.StartTime.ToString("yyyyMMddHHmmss")))
            {
                a.StartTime = DateTime.Now;
            }
            lastItem.EndTime = a.StartTime.AddSeconds(-1);
            if (lastItem.StartTime > lastItem.StartTime)
            {
                throw new Exception("Start date can not be greater than end date");
            }
            if (a.StartTime > a.EndTime)
            {
                throw new Exception("Start date can not be greater than end date");
            }

            var old_record = GetExchangeStatus(lastItem_copy.ExchangeStatusID);

            DeleteExchangeStatus(old_record);
            CreateExchangeStatus(lastItem);
            CreateExchangeStatus(a);
            return true;
        }

        public bool DeleteExchangeStatus(ExchangeStatusViewModel record)
        {
            record.Operation = "DELETE";
            record.Deleted = 1;

            string publishStatus = ExchangStatusPublish(record);

            return true;
        }

        public IEnumerable<DARViewModel> GetExcludeFromPricing()
        {
            List<ExcludeFromPricingViewModel> l = new List<ExcludeFromPricingViewModel>();
            string sql = $@"select
                            CONCAT(e.darExchangeID, e.exchangePair, e.endDate) as  ExcludefromPricingID
                            ,e.darExchangeID 
                            ,ex.name as ExchangeName
                            ,e.exchangePair
                            ,e.startDate
                            ,e.endDate
                            from {DARApplicationInfo.SingleStoreCatalogPublic}.excludeFromPricing e
                            join {DARApplicationInfo.SingleStoreCatalogPublic}.exchange ex on e.darExchangeID = ex.DARExchangeID
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<ExcludeFromPricingViewModel>(sql).ToList();
            }

            return l;
        }

        public ExcludeFromPricingViewModel GetExcludeFromPricing(string id)
        {
            ExcludeFromPricingViewModel l = new ExcludeFromPricingViewModel();
            string sql = $@"select
                             CONCAT(e.darExchangeID, e.exchangePair, e.endDate) as  ExcludefromPricingID
                            ,e.darExchangeID
                            ,ex.name as ExchangeName
                            ,e.exchangePair
                            ,e.startDate
                            ,e.endDate
                            from {DARApplicationInfo.SingleStoreCatalogPublic}.excludeFromPricing e
                            join {DARApplicationInfo.SingleStoreCatalogPublic}.exchange ex on e.darExchangeID = ex.DARExchangeID
                            where  CONCAT(e.darExchangeID, e.exchangePair, e.endDate) = '{id}'
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<ExcludeFromPricingViewModel>(sql).ToList().FirstOrDefault();
            }

            return l;
        }

        public bool CreateExcludefromPricing(DARViewModel i)
        {
            var a = (ExcludeFromPricingViewModel)i;

            var exchangeID = (new Exchange()).Get(a.DARExchangeID) as ExchangeViewModel;
            if (exchangeID == null)
                throw new Exception($"Invalid Exchange ID {a.DARExchangeID}");

            Thread.Sleep(1000);

            var existing_record = GetDuplicateRecord_excludeFromPricing(a.DARExchangeID, a.ExchangePair);
            if (existing_record != null)
            {
                throw new Exception("Record already exists in the Database.");
            }

            DateTime start_utc = DateTime.SpecifyKind(a.StartDate, DateTimeKind.Utc);
            DateTime end_utc = DateTime.SpecifyKind(a.EndDate, DateTimeKind.Utc);

            ExcludeFromPricingViewModel l = new ExcludeFromPricingViewModel()
            {
                DARExchangeID = a.DARExchangeID.Trim(),
                ExchangePair = a.ExchangePair.Trim(),
                StartDate = a.StartDate,
                EndDate = a.EndDate,
                CreateUser = string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.User.Identity.Name) ? Environment.UserName : System.Web.HttpContext.Current.User.Identity.Name,
                LastEditUser = string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.User.Identity.Name) ? Environment.UserName : System.Web.HttpContext.Current.User.Identity.Name
            };


            var end_date_year = l.EndDate.Year.ToString();
            var start_date_year = l.StartDate.Year.ToString();

            if (string.Equals(end_date_year, "9999"))
            {
                l.EndDate = new DateTime(l.EndDate.Year, l.EndDate.Month, l.EndDate.Day, 00, 00, 00);
            }

            if (string.Equals(start_date_year, "1900"))
            {
                l.StartDate = new DateTime(l.StartDate.Year, l.StartDate.Month, l.StartDate.Day, 00, 00, 00);
            }

            if (string.Equals(start_date_year, "1899"))
            {
                l.StartDate = DateTime.Parse("1900-01-01 00:00:00.000000");
            }

            l.Operation = "INSERT";
            l.Deleted = 0;

            string publishStatus = ExcludefromPricingPublish(l);
            return true;
        }

        public bool remove_exclude_from_pricing(DARViewModel i)
        {
            var a = (ExcludeFromPricingViewModel)i;
            DeleteExcludefromPricing(a);
            return true;

        }
        public bool Adjust_exclude_from_pricing(DARViewModel i)
        {
            var a = (ExcludeFromPricingViewModel)i;

            var old_record = GetExcludeFromPricing(a.ExcludefromPricingID);

            DeleteExcludefromPricing(old_record);
            try
            {
                CreateExcludefromPricing(a);
            }
            catch (Exception ex)
            {
                CreateExcludefromPricing(old_record);
                throw new Exception(ex.Message);
            }

            return true;

        }


        public bool Roll_exclude_from_pricing(DARViewModel i)
        {
            var a = (ExcludeFromPricingViewModel)i;
            var lastItem = GetExcludeFromPricing(a.ExcludefromPricingID);
            var lastItem_copy = lastItem;
            if (lastItem.StartDate.ToString("yyyyMMddHHmmss").Equals(a.StartDate.ToString("yyyyMMddHHmmss")))
            {
                a.StartDate = DateTime.Now;
            }
            
            lastItem.EndDate = a.StartDate.AddSeconds(-1);
            if (lastItem.StartDate > lastItem.EndDate)
            {
                throw new Exception("Start date can not be greater than end date");
            }
            if (a.StartDate > a.EndDate)
            {
                throw new Exception("Start date can not be greater than end date");
            }

            var old_record = GetExcludeFromPricing(lastItem_copy.ExcludefromPricingID);


            DeleteExcludefromPricing(old_record);
            
            try
            {
                CreateExcludefromPricing(lastItem);
                CreateExcludefromPricing(a);
            }
            catch (Exception ex)
            {
                DeleteExcludefromPricing(lastItem);
                CreateExcludefromPricing(old_record);
                throw new Exception(ex.Message);
            }

            return true;
        }

        public bool DeleteExcludefromPricing(ExcludeFromPricingViewModel record)
        {
            record.Operation = "DELETE";
            record.Deleted = 1;

            string publishStatus = ExcludefromPricingPublish(record);
            return true;
        }

        public ExcludeFromPricingViewModel GetDuplicateRecord_excludeFromPricing(string DARExchangeID, string ExchangePair)
        {
            List<ExcludeFromPricingViewModel> l = new List<ExcludeFromPricingViewModel>();

            string sql = $@"select *
                           from {DARApplicationInfo.SingleStoreCatalogPublic}.excludeFromPricing
                           where (darExchangeID = '{DARExchangeID}' and  exchangePair = '{ExchangePair}')
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<ExcludeFromPricingViewModel>(sql).ToList();
            }

            return l.FirstOrDefault();
        }


        public IEnumerable<DARViewModel> GetServListV2()
        {
            List<ServListV2ViewModel> l = new List<ServListV2ViewModel>();
                string sql = $@"select
                                CONCAT(e.darAssetID, darMnemonic,endTime) as Servlistv2ID
                                ,e.darMnemonic 
                                ,e.darAssetID
                                ,a.darTicker
                                ,e.priceTier
                                ,e.startTime
                                ,e.endTime
                                ,unix_timestamp(e.loadTime) as LoadTimestamp
                                from ({DARApplicationInfo.SingleStoreCatalogPublic}.servListV2 e
                                join {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on e.darAssetID = a.DARAssetID)
                    ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<ServListV2ViewModel>(sql).ToList();
            }

            return l;
        }

        public ServListV2ViewModel GetServListV2(string id)
        {
            List<ServListV2ViewModel> l = new List<ServListV2ViewModel>();

            string sql = $@"select
                            CONCAT(e.darAssetID, darMnemonic,endTime) as servlistv2ID
                            ,e.DARMnemonic
                            ,e.DARAssetID
                            ,a.darTicker
                            ,e.priceTier
                            ,e.StartTime
                            ,e.EndTime
                            ,unix_timestamp(e.loadTime) as LoadTimestamp
                            from ({DARApplicationInfo.SingleStoreCatalogPublic}.servListV2 e
                            join {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on e.darAssetID = a.DARAssetID)
                            where CONCAT(e.darAssetID, darMnemonic,endTime) = '{id}'
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<ServListV2ViewModel>(sql).ToList();
            }

            return l.FirstOrDefault();
        }

        public ServListV2ViewModel GetServListV2_Duplicate(string id)
        {
            List<ServListV2ViewModel> l = new List<ServListV2ViewModel>();

            string sql = $@"select
                            CONCAT(e.darAssetID, darMnemonic) as servlistv2ID
                            ,e.DARMnemonic
                            ,e.DARAssetID
                            ,a.darTicker
                            ,e.priceTier
                            ,e.StartTime
                            ,e.EndTime
                            ,unix_timestamp(e.loadTime) as LoadTimestamp
                            from ({DARApplicationInfo.SingleStoreCatalogPublic}.servListV2 e
                            join {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on e.darAssetID = a.DARAssetID)
                            where CONCAT(e.darAssetID, darMnemonic) = '{id}'
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<ServListV2ViewModel>(sql).ToList();
            }

            return l.FirstOrDefault();
        }

        public bool CreateServListV2(DARViewModel i)
        {
            var a = (ServListV2ViewModel)i;

            var DARassetID = (new Asset()).Get(a.darAssetID) as AssetViewModel;
            if (DARassetID == null)
                throw new Exception($"Invalid DARAsset ID {a.darAssetID}");

            ServListV2ViewModel l = new ServListV2ViewModel()
            {
                darMnemonic = a.darMnemonic.Trim(),
                darAssetID = a.darAssetID.Trim(),
                priceTier = a.priceTier,
                startTime = a.startTime,
                endTime = a.endTime,
                CreateUser = string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.User.Identity.Name) ? Environment.UserName : System.Web.HttpContext.Current.User.Identity.Name,
                LastEditUser = string.IsNullOrWhiteSpace(System.Web.HttpContext.Current.User.Identity.Name) ? Environment.UserName : System.Web.HttpContext.Current.User.Identity.Name
            };

            var end_date_year = l.endTime.Year.ToString();
            var start_date_year = l.startTime.Year.ToString();

            if (string.Equals(end_date_year, "9999"))
            {
                l.endTime = new DateTime(l.endTime.Year, l.endTime.Month, l.endTime.Day, 00, 00, 00);
            }

            if (string.Equals(start_date_year, "1900"))
            {
                l.startTime = new DateTime(l.startTime.Year, l.startTime.Month, l.startTime.Day, 00, 00, 00);
            }

            if (string.Equals(start_date_year, "1899"))
            {
                l.startTime = DateTime.Parse("1900-01-01 00:00:00.000000");
            }

            l.Operation = "INSERT";
            l.Deleted = 0;

            string publishStatus = ServListV2Publish(l);
            return true;
        }

        public bool Adjust_servlist_v2(DARViewModel i)
        {
            var a = (ServListV2ViewModel)i;

            var old_record = GetServListV2(a.Servlistv2ID);

            DeleteServlistV2(old_record);
            CreateServListV2(a);
            return true;

        }
        public bool remove_servlist_v2(DARViewModel i)
        {
            var a = (ServListV2ViewModel)i;
            DeleteServlistV2(a);
            return true;
        }

        public bool Roll_servlist_v2(DARViewModel i)
        {
            var a = (ServListV2ViewModel)i;
            var lastItem = GetServListV2(a.Servlistv2ID);
            var lastItem_copy = lastItem;
            if (lastItem.startTime.ToString("yyyyMMddHHmmss").Equals(a.startTime.ToString("yyyyMMddHHmmss")))
            {
                a.startTime = DateTime.Now;
            }
            lastItem.endTime = a.startTime.AddSeconds(-1);
            if (lastItem.startTime > lastItem.endTime)
            {   
                throw new Exception("Start date can not be greater than end date");
            }
            if (a.startTime > a.endTime)
            {
                throw new Exception("Start date can not be greater than end date");
            }

            var old_record = GetServListV2(lastItem_copy.Servlistv2ID);


            DeleteServlistV2(old_record);
            CreateServListV2(lastItem);
            CreateServListV2(a);
            return true;
        }

        public bool CleanUnmappedTrades(string topic)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = DARApplicationInfo.KafkaServerName,
                ClientId = Dns.GetHostName(),
                EnableIdempotence = false,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = DARApplicationInfo.KafkaSaslUsername,
                SaslPassword = DARApplicationInfo.KafkaPassword,
                SslCaLocation = DARApplicationInfo.KafkaCertificateFileName,
                BatchNumMessages = 1,
                BatchSize = 1000000,

            };


            Dictionary<string, string> message = new Dictionary<string, string>() { { "clean",$"{DateTimeOffset.Now}" } };
           
            string jsondata = JsonSerializer.Serialize(message);
            try
            {
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    producer.Produce("unmapped_clean_prod", new Message<Null, string> { Value = jsondata });
                    
                    producer.Flush();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
  
        public bool DeleteServlistV2(ServListV2ViewModel record)
        {
            record.Operation = "DELETE";
            record.Deleted = 1;

            string publishStatus = ServListV2Publish(record);
            return true;
        }

        public override DARViewModel Get(string key)
        {
            return Get().Cast<ExchangePairsV2ViewModel>().Where(x => x.DARAssetID.Equals(key)).FirstOrDefault();
        }




        public override bool IdExists(string nextId)
        {
            string sql = $@"
                            select darPairID
                            from {DARApplicationInfo.SingleStoreCatalogPublic}.exchangePairsV2
                            where darPairID = '{nextId}'
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                var r = connection.Query<string>(sql);

                if (r != null && r.Any())
                    return true;
            }

            return false;
        }

        public override string GetNextId()
        {
            return GetNextId("DP", 7, 100);
        }

        public string ExchangPairsPublish(ExchangePairsV2ViewModel l)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = DARApplicationInfo.KafkaServerName,
                ClientId = Dns.GetHostName(),
                EnableIdempotence = false,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = DARApplicationInfo.KafkaSaslUsername,
                SaslPassword = DARApplicationInfo.KafkaPassword,
                SslCaLocation = DARApplicationInfo.KafkaCertificateFileName,
                BatchNumMessages = 1,
                BatchSize = 1000000,

            };
            l.LoadTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();


            string jsondata = JsonSerializer.Serialize(l);
            try
            {
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    producer.Produce("refmaster_exchange_pairs_v2", new Message<Null, string> { Value = jsondata });
                    producer.Flush();
                }
                return "Message published without error";
            }
            catch (Exception ex)
            {
                return $"Failed to publish message {ex.Message}";
            }

        }

        public string ExchangStatusPublish(ExchangeStatusViewModel l)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = DARApplicationInfo.KafkaServerName,
                ClientId = Dns.GetHostName(),
                EnableIdempotence = false,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = DARApplicationInfo.KafkaSaslUsername,
                SaslPassword = DARApplicationInfo.KafkaPassword,
                SslCaLocation = DARApplicationInfo.KafkaCertificateFileName,
                BatchNumMessages = 1,
                BatchSize = 1000000,

            };
            l.LoadTimestamp = DateTimeOffset.Now.ToUnixTimeSeconds();


            string jsondata = JsonSerializer.Serialize(l);
            try
            {
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    producer.Produce("refmaster_exchange_status", new Message<Null, string> { Value = jsondata });
                    producer.Flush();
                }
                return "Message published without error";
            }
            catch (Exception ex)
            {
                return $"Failed to publish message {ex.Message}";
            }

        }

        public string ExcludefromPricingPublish(ExcludeFromPricingViewModel l)
        {
            string fatalErrorFile = @"C:\temp\UploadedFiles\fatalerror.txt";
            var config = new ProducerConfig
            {
                BootstrapServers = DARApplicationInfo.KafkaServerName,
                ClientId = Dns.GetHostName(),
                EnableIdempotence = false,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = DARApplicationInfo.KafkaSaslUsername,
                SaslPassword = DARApplicationInfo.KafkaPassword,
                SslCaLocation = DARApplicationInfo.KafkaCertificateFileName,
                BatchNumMessages = 1,
                BatchSize = 1000000,

            };


            string jsondata = JsonSerializer.Serialize(l);
            try
            {
                var current_dir = Directory.GetCurrentDirectory();

                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    producer.Produce("refmaster_exclude_from_pricing", new Message<Null, string> { Value = jsondata });
                    producer.Flush();
                }


                return "Message published without error";
            }
            catch (Exception ex)
            {
                using (StreamWriter sw = File.AppendText(fatalErrorFile))
                {
                    sw.WriteLine($"[{DateTime.Now}] Publish Error: {ex.Message}");

                }

                return $"Failed to publish message {ex.Message}";
            }

        }

        public string ServListV2Publish(ServListV2ViewModel l)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = DARApplicationInfo.KafkaServerName,
                ClientId = Dns.GetHostName(),
                EnableIdempotence = false,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = DARApplicationInfo.KafkaSaslUsername,
                SaslPassword = DARApplicationInfo.KafkaPassword,
                SslCaLocation = DARApplicationInfo.KafkaCertificateFileName,
                BatchNumMessages = 1,
                BatchSize = 1000000,

            };
            l.LoadTimeStamp = DateTimeOffset.Now.ToUnixTimeSeconds();


            string jsondata = JsonSerializer.Serialize(l);
            try
            {
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    producer.Produce("refmaster_servlist_v2", new Message<Null, string> { Value = jsondata });
                    producer.Flush();
                }
                return "Message published without error";
            }
            catch (Exception ex)
            {
                return $"Failed to publish message {ex.Message}";
            }

        }




        public IEnumerable<DARViewModel> GetNonPricingServList()
        {
            List<NonPricingServListViewModel> l = new List<NonPricingServListViewModel>();
            string sql = $@"select
                             e.darMnemonic
                            ,e.darAssetID
                            ,e.note
                            from {DARApplicationInfo.SingleStoreCatalogPublic}.non_pricing_serv_list e
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<NonPricingServListViewModel>(sql).ToList();
            }

            return l;
        }

        public IEnumerable<DARViewModel> GetIncludePricingCurrency()
        {
            List<IncludePricingCurrencyViewModel> l = new List<IncludePricingCurrencyViewModel>();
            string sql = $@"select
                             e.darMnemonicFamily
                            ,e.priceTier
                            ,e.currencyTicker
                            ,e.startDate
                            ,e.endDate
                            from {DARApplicationInfo.SingleStoreCatalogPublic}.includePricingCurrency e
                            order by startDate desc
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<IncludePricingCurrencyViewModel>(sql).ToList();
            }

            return l;
        }

        public ServListV2ViewModel GetExistingRecord(string darMnemonic, string DARAssetID, string pricetier)
        {
            List<ServListV2ViewModel> l = new List<ServListV2ViewModel>();

            
            string sql = $@"select
                             e.DARMnemonic
                            ,e.DARAssetID
                            ,e.priceTier
                            ,e.StartTime
                            ,e.EndTime
                            ,unix_timestamp(e.loadTime) as LoadTimestamp
                            from {DARApplicationInfo.SingleStoreCatalogPublic}.servListV2 e
                           where darAssetID = '{DARAssetID}' and  darMnemonic = '{darMnemonic}' and priceTier = '{pricetier}' and loadTime = (select max(loadTime) from {DARApplicationInfo.SingleStoreCatalogPublic}.servListV2
                           where darAssetID = '{DARAssetID}' and  darMnemonic = '{darMnemonic}' and priceTier = '{pricetier}')
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<ServListV2ViewModel>(sql).ToList();
            }

            return l.FirstOrDefault();
        }

        public ServListV2ViewModel GetExistingRecordforward(string darMnemonic, string DARAssetID, string endtime)
        {
            List<ServListV2ViewModel> l = new List<ServListV2ViewModel>();

            string sql = $@"select *
                           from {DARApplicationInfo.SingleStoreCatalogPublic}.servListV2
                           where (darAssetID = '{DARAssetID}' and  darMnemonic = '{darMnemonic}' and endtime like '%{endtime}%')
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<ServListV2ViewModel>(sql).ToList();
            }

            return l.FirstOrDefault();
        }

        public ExchangePairsV2ViewModel GetDuplicateRecord_exchangepairsv2(string ExchangeName, string ExchangePair, string AssetTicker, string CurrencyTicker)
        {
            List<ExchangePairsV2ViewModel> l = new List<ExchangePairsV2ViewModel>();

            string sql = $@"select *
                           from {DARApplicationInfo.SingleStoreCatalogPublic}.exchangePairsV2
                           where (exchangeName = '{ExchangeName}' and  exchangePair = '{ExchangePair}' and assetTicker = '{AssetTicker}' and currencyTIcker = '{CurrencyTicker}')
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<ExchangePairsV2ViewModel>(sql).ToList();
            }

            return l.FirstOrDefault();
        }

        private bool IsInputValid(ref ExchangePairsV2ViewModel input, string effective_time)
        {
            StringBuilder error_list = new StringBuilder();
            
            if (string.IsNullOrWhiteSpace(input.ExchangeName) || string.IsNullOrWhiteSpace(input.ExchangePair) || string.IsNullOrWhiteSpace(input.AssetTicker) || string.IsNullOrWhiteSpace(input.CurrencyTicker))
            {
                throw new Exception("Exchange Name, Exchange Pair, Asset Ticker and Pair Currency are required Fields for File Upload.");
            }

            TokenTable a = new TokenTable();
            TokenTableViewModel avm = a.GetTokenDetails(input.AssetTicker) as TokenTableViewModel;
            TokenTable c = new TokenTable();
            TokenTableViewModel cvm = c.GetTokenDetails(input.CurrencyTicker) as TokenTableViewModel;
            Exchange e = new Exchange();
            ExchangeViewModel evm = e.GetExchangePublic(input.ExchangeName) as ExchangeViewModel;
            if (avm == null)
            {
                throw new Exception("Couldn't resolve input asset");
            }
            if (cvm == null)
            {
                throw new Exception("Couldn't resolve input currency");
            }
            if (evm == null)
            {
                throw new Exception("Couldn't resolve input exchange");
            }

            if (avm != null && cvm != null && evm != null)
            {
                //check duplicate entry in database
                var duplicate_record = GetDuplicateRecord_exchangepairsv2(input.ExchangeName, input.ExchangePair, input.AssetTicker, input.CurrencyTicker);
                if (duplicate_record != null)
                {
                    throw new Exception("Duplicate Entry Entered.");
                }
                input.DARAssetID = avm.darAssetID;
                input.AssetName = avm.Name;
                input.AssetTicker = avm.darTicker;
                input.legacyAssetID = avm.legacyID;
                input.DARCurrencyID = cvm.darAssetID;
                input.CurrencyName = cvm.Name;
                input.CurrencyTicker = cvm.darTicker;
                input.legacyCurrencyID = cvm.legacyID;
                input.DARExchangeID = evm.DARExchangeID;
                input.legacyExchangeID = evm.LegacyId;
                input.ExchangeName = evm.ShortName;
                
                DateTime defaultdate = Convert.ToDateTime("01-01-0001 00:00:00");
                if (effective_time == "Now()")
                {
                    input.StartDate = DateTime.Now;
                }
                else
                {
                    input.StartDate = DateTime.Parse("1900-01-01 00:00:00.000000");
                }

                if (input.EndDate != defaultdate)
                    input.EndDate = input.EndDate;
                else
                    input.EndDate = DateTime.Parse("9999-12-31 00:00:00.000000");

                if (!string.IsNullOrEmpty(input.blockchain))
                    input.blockchain = input.blockchain;
                else
                    input.blockchain = "na";

                if (!string.IsNullOrEmpty(input.contractAddress))
                    input.contractAddress = input.contractAddress;
                else
                    input.contractAddress = "na";

                return true;
            }
            else
                return false;

        }

        private bool IsInputValid_for_exclude_from_pricing(ref ExcludeFromPricingViewModel input)
        {
            StringBuilder error_list = new StringBuilder();

            Exchange e = new Exchange();
            ExchangeViewModel evm = e.GetExchangePublic(input.DARExchangeID) as ExchangeViewModel;
            
            if (evm == null)
            {
                throw new Exception("Couldn't resolve input exchange");
            }

            else
            {
                
                input.DARExchangeID = evm.DARExchangeID;
                

                DateTime defaultdate = Convert.ToDateTime("01-01-0001 00:00:00");
                if (input.StartDate != defaultdate)
                    input.StartDate = input.StartDate;
                else
                    input.StartDate = DateTime.Now;

                if (input.EndDate != defaultdate)
                    input.EndDate = input.EndDate;
                else
                    input.EndDate = DateTime.Parse("9999-12-31 00:00:00.000000");

                return true;
            }
        }

        private bool IsInputValid_for_servlistv2(ref ServListV2ViewModel input)
        {
            StringBuilder error_list = new StringBuilder();

            TokenTable a = new TokenTable();
            TokenTableViewModel avm = a.GetTokenDetails(input.darAssetID) as TokenTableViewModel;
           
            if (avm == null)
            {
                throw new Exception("Couldn't resolve input asset");
            }
           

            if (avm != null)
            {
                input.darAssetID = avm.darAssetID;
                
                // Check for duplicate entries
                var servlistv2id = $"{input.darAssetID.Trim()}{input.darMnemonic.Trim()}".Replace("'", "''");

                var servlist_record = GetServListV2_Duplicate(servlistv2id);
                if (servlist_record != null)
                {
                    throw new Exception($"DARMnemonic:{input.darMnemonic} and DARAssetID:{input.darAssetID} already exists");
                }

                return true;
            }
            else
                return false;
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            errors = string.Empty;
            StringBuilder sbError = new StringBuilder();
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(fileName);
            Worksheet sheet = workbook.Worksheets[0];

            int rowCount = 0;
            int failCount = 0;
            int successCount = 0;
            int total_rows_to_upload = sheet.Rows.Count() - 1;

            ExchangePairsV2ViewModel ep;
            DateTime dt;

            LogData.Log(LogDataType.Info, $"[Exchange Pairs Upload] Loading Filename {fileName} RowCount:{total_rows_to_upload}");
            Dictionary<string, bool> message = new Dictionary<string, bool>();

            foreach (var row in sheet.Rows)
            {
                if (rowCount == 0)
                {
                    rowCount++;
                    continue;
                }
                
                ep = new ExchangePairsV2ViewModel();
            
                string effective_time = "";
                try
                {
                    effective_time = row.Columns[4].Value;
                }
                catch (Exception ex)
                {

                    LogData.Log(LogDataType.Error, $"[Exchange Pairs Upload] Failed to load Row[{rowCount}] ERROR:[Effective time missing.] Input:[{ep.GetDescription()}] ");
                    sbError.AppendLine($"Failed to load the File. ERROR: Effective time missing.");
                    errors = sbError.ToString();
                    return false;
                }
                try
                {
                    ep.ExchangeName = row.Columns[0].Value;
                    ep.ExchangePair = row.Columns[1].Value;
                    ep.AssetTicker = row.Columns[2].Value;
                    ep.CurrencyTicker = row.Columns[3].Value;
                    ep.blockchain = row.Columns[5].Value;
                    ep.contractAddress = row.Columns[6].Value;

                   

                    string current_record = $"{ep.ExchangeName.Trim()}{ep.ExchangePair.Trim()}{ep.AssetTicker.Trim()}{ep.CurrencyTicker.Trim()}".Replace("'", "''");

                    if (message.ContainsKey(current_record))
                    {
                        LogData.Log(LogDataType.Error, $"[Exchange Pairs Upload] Failed to load Row[{rowCount}] ERROR:[Duplicate recirds present in the file] Input:[{ep.GetDescription()}] ");
                        sbError.AppendLine($"Failed to load row {rowCount} :  {ep.GetDescription()} ERROR. Duplicate records present in the file.");
                        failCount++;
                        continue;
                    }

                    if (IsInputValid(ref ep, effective_time))
                    {
                        Add(ep);
                        message.Add($"{ep.ExchangeName.Trim()}{ep.ExchangePair.Trim()}{ep.AssetTicker.Trim()}{ep.CurrencyTicker.Trim()}".Replace("'", "''"), true); 
                    }
                    
                    successCount++;
                    

                }
                catch (Exception ex)
                {

                    LogData.Log(LogDataType.Error, $"[Exchange Pairs Upload] Failed to load Row[{rowCount}] ERROR:[{ex.Message}] Input:[{ep.GetDescription()}] ");
                    sbError.AppendLine($"Failed to load row {rowCount} :  {ep.GetDescription()} ERROR:{ex.Message}.");
                    failCount++;
                }
                finally
                {
                    if (rowCount % 100 == 0)
                    {
                        LogData.Log(LogDataType.Error, $"[Exchange Pairs Upload] Processed Row[{rowCount} of {total_rows_to_upload}] ");
                    }
                    rowCount++;
                }
            }

            LogData.Log(LogDataType.Info, $"[Exchange Pairs Upload] File:{fileName} Total Rows: {total_rows_to_upload} Success:{successCount} Failed:{failCount}");

            if (sbError.Length != 0)
            {
                errors = sbError.ToString();
                return false;
            }
            else
                return true;
        }

        public bool exclude_pairs_from_servlist_LoadDataFromExcelFile(string fileName, out string errors)
        {
            errors = string.Empty;
            StringBuilder sbError = new StringBuilder();
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(fileName);
            Worksheet sheet = workbook.Worksheets[0];

            int rowCount = 0;
            int failCount = 0;
            int successCount = 0;
            int total_rows_to_upload = sheet.Rows.Count() - 1;

            ExcludeFromPricingViewModel ep;
            DateTime dt;

            LogData.Log(LogDataType.Info, $"[Exclude Pairs from Servlist] Loading Filename {fileName} RowCount:{total_rows_to_upload}");


            foreach (var row in sheet.Rows)
            {
                if (rowCount == 0)
                {
                    rowCount++;
                    continue;
                }
                ep = new ExcludeFromPricingViewModel();
                try
                {
                    if (DateTime.TryParse(row.Columns[4].Value, out dt))
                    {
                        ep.EndDate = dt;
                    }
                    ep.DARExchangeID = row.Columns[0].Value;
                    ep.ExchangePair = row.Columns[1].Value;

                    if (IsInputValid_for_exclude_from_pricing(ref ep))
                        CreateExcludefromPricing(ep);

                    successCount++;

                }
                catch (Exception ex)
                {

                    LogData.Log(LogDataType.Error, $"[Exclude Pairs from Servlist] Failed to load Row[{rowCount}] ERROR:[{ex.Message}] Input:[{ep.GetDescription()}] ");
                    sbError.AppendLine($"Failed to load row {rowCount} :  {ep.GetDescription()} ERROR:{ex.Message}.");
                    failCount++;
                }
                finally
                {
                    if (rowCount % 100 == 0)
                    {
                        LogData.Log(LogDataType.Info, $"[Exclude Pairs from Servlist] Processed Row[{rowCount} of {total_rows_to_upload}]");
                    }
                    rowCount++;
                }
            }

            LogData.Log(LogDataType.Info, $"[Exclude Pairs from Servlist] File:{fileName} Total Rows: {total_rows_to_upload} Success:{successCount} Failed:{failCount}");

            if (sbError.Length != 0)
            {
                errors = sbError.ToString();
                return false;
            }
            else
                return true;
        }

        public bool servlistv2_LoadDataFromExcelFile(string fileName, out string errors)
        {
            errors = string.Empty;
            StringBuilder sbError = new StringBuilder();
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(fileName);
            Worksheet sheet = workbook.Worksheets[0];

            int rowCount = 0;
            int failCount = 0;
            int successCount = 0;
            int total_rows_to_upload = sheet.Rows.Count() - 1;

            ServListV2ViewModel ep;
            DateTime dt;

            LogData.Log(LogDataType.Info, $"[Add ServlistV2] Loading Filename {fileName} RowCount:{total_rows_to_upload}");


            foreach (var row in sheet.Rows)
            {
                if (rowCount == 0)
                {
                    rowCount++;
                    continue;
                }
                ep = new ServListV2ViewModel();
                var action = row.Columns[3].Value;
                string tier = row.Columns[1].Value;
                ep.darMnemonic = row.Columns[0].Value;
                ep.priceTier = long.Parse(tier.Substring(tier.Length - 1));
                ep.darAssetID = row.Columns[2].Value;
                var start_time = row.Columns[4].Value;
                var end_time = row.Columns[5].Value;
                try
                {
                    if (action.Equals("Add New Record"))
                    {
                        if (start_time.Equals("Now()"))
                        {
                            ep.startTime = DateTime.Now;
                        }
                        if (end_time.Equals("9999"))
                        {
                            ep.endTime = DateTime.Parse("9999-12-31 00:00:00.000000");
                        }
                        if (IsInputValid_for_servlistv2(ref ep))
                            CreateServListV2(ep);

                        successCount++;
                    }
                    else if (action.Equals("Change Historical"))
                    {
                        var existing_record = GetExistingRecord(ep.darMnemonic, ep.darAssetID, ep.priceTier.ToString());
                        if (existing_record != null)
                        {
                            if (end_time.Equals("Now()"))
                            {
                                ep.endTime = DateTime.Now;
                                ep.startTime = existing_record.startTime;
                            }
                            ep.Servlistv2ID = $"{existing_record.darAssetID.Trim()}{existing_record.darMnemonic.Trim()}{existing_record.endTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff")}".Replace("'", "''");
                            Adjust_servlist_v2(ep);
                            successCount++;

                        }
                    }
                    else
                    {
                        var existing_record_forward = GetExistingRecordforward(ep.darMnemonic, ep.darAssetID, end_time);
                        if (existing_record_forward != null)
                        {
                            if (start_time.Equals("Now()"))
                            {
                                ep.startTime = DateTime.Now;
                                ep.endTime = existing_record_forward.endTime;
                            }
                            ep.Servlistv2ID = $"{existing_record_forward.darAssetID.Trim()}{existing_record_forward.darMnemonic.Trim()}{existing_record_forward.endTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff")}".Replace("'", "''");
                            Roll_servlist_v2(ep);
                            successCount++;
                        }
                    }
                }
                catch (Exception ex)
                {
                    LogData.Log(LogDataType.Error, $"[Add ServlistV2] Failed to load Row[{rowCount}] ERROR:[{ex.Message}] Input:[{ep.GetDescription()}] ");
                    sbError.AppendLine($"Failed to load row {rowCount} :  {ep.GetDescription()} ERROR:{ex.Message}.");
                    failCount++;
                }

                finally
                {
                    if (rowCount % 100 == 0)
                    {
                        LogData.Log(LogDataType.Info, $"[Add ServlistV2] Processed Row[{rowCount} of {total_rows_to_upload}]");
                    }
                    rowCount++;
                }
            }

            LogData.Log(LogDataType.Info, $"[Add ServlistV2] File:{fileName} Total Rows: {total_rows_to_upload} Success:{successCount} Failed:{failCount}");

            if (sbError.Length != 0)
            {
                errors = sbError.ToString();
                return false;
            }
            else
                return true;
        }
    }
}