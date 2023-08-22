using Confluent.Kafka;
using Dapper;
using DARReferenceData.ViewModels;
using log4net;
using log4net.Repository.Hierarchy;
using MySql.Data.MySqlClient;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace DARReferenceData.DatabaseHandlers
{
    public class Derivatives : RefDataHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);

        private enum DerivativeUploadColumns
        {
            UnderlierDARAssetID = 0
            , ContractType = 1
            , OptionType = 2
            , ContractTicker = 3
             , DARContractID = 4
            , ContractExchange = 5
            , ContractExchangeDARID = 6
             , Status = 7
            , TradingHours = 8
            , MinimumTickSize = 9
            , SettlementTime = 10
            , SettlementType = 11
            , SettlementCurrency = 12
            , ExpirationDate = 13
            , ContractSize = 14
            , InitialMargin = 15
            , MaintenanceMargin = 16
            , MarkPrice = 17
            , DeliveryPrice = 18
            , DeliveryMethod = 19
            , FeesURL = 20
            , PositionLimit = 21
            , PositionLimitURL = 22
            , BlockTradeMinimum = 23
            , LinktoTAndC = 24
            , FundingRates = 25
        }

        public static int GetActiveCount()
        {
            string sql = $@"
                            select count(*)
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Derivatives
                            where ExpirationDate >= timestamp(current_date()) and coalesce(DELETED, 0) = 0
                           ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                return connection.Query<int>(sql).FirstOrDefault();
            }
        }

        public static int GetExpiredCount()
        {
            string sql = $@"
                            select count(*)
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Derivatives
                            where ExpirationDate < timestamp(current_date()) and coalesce(DELETED, 0) = 0
                        ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                return connection.Query<int>(sql).FirstOrDefault();
            }
        }

        public override long Add(DARViewModel i)
        {
            var a = (DerivativesViewModel)i;


          
            a.Operation = "INSERT";
            a.Deleted = 0;
            if(string.IsNullOrWhiteSpace(a.DARContractID))
                a.DARContractID = GetNextId();

            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.IsActive = true;

            string publishstatus = DerivativePublish(a);

       
            return 1;
        }

        public override bool Delete(DARViewModel i)
        {
            var a = (DerivativesViewModel)i;
            DateTime updateTime = DateTime.Now.ToUniversalTime();

            a.Operation = "DELETE";
            a.Deleted = 1;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            string publishstatus = DerivativePublish(a);



      
            return true;
        }

        public override IEnumerable<DARViewModel> Get()
        {
            List<DerivativesViewModel> l = new List<DerivativesViewModel>();

            string sql = $@"
                            SELECT a.DARTicker as UnderlierDARTicker
                                  , UnderlierDARAssetID
                                  , ContractType
                                  , OptionType
                                  , ContractTicker
                                  , DARContractID
                                  , ContractExchange
                                  , ContractExchangeDARID
                                  , case when ExpirationDate >= timestamp(current_date()) then 'active' else 'expired' end as Status
                                  , TradingHours
                                  , MinimumTickSize
                                  , SettlementTime
                                  , SettlementType
                                  , SettlementCurrency
                                  , ExpirationDate
                                  , ContractSize
                                  , InitialMargin
                                  , MaintenanceMargin
                                  , MarkPrice
                                  , DeliveryPrice
                                  , DeliveryMethod
                                  , FeesURL
                                  , PositionLimit
                                  , PositionLimitURL
                                  , BlockTradeMinimum
                                  , LinktoTAndC
                                  , FundingRates
                                  , d.IsActive
                                  , d.CreateUser
                                  , d.LastEditUser
                                  , d.CreateTime
                                  , d.LastEditTime
                              FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Derivatives d
                                inner join  {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on d.UnderlierDARAssetID = a.DARAssetID
                                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<DerivativesViewModel>(sql).ToList();
            }

            return l;
        }

        public DARViewModel LookupContract(string contractExchangeDARID, string exchange)
        {
            return Get().Cast<DerivativesViewModel>().Where(x => x.ContractTicker.ToUpper().Equals(contractExchangeDARID.ToUpper()) && x.ContractExchange.ToUpper().Equals(exchange.ToUpper())).FirstOrDefault();
        }

        public override DARViewModel Get(string key)
        {
            return Get().Cast<DerivativesViewModel>().Where(x => x.DARContractID.ToUpper().Equals(key.ToUpper())).FirstOrDefault();
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            errors = string.Empty;
            StringBuilder sbError = new StringBuilder();
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(fileName);
            Worksheet sheet = workbook.Worksheets[0];

            int rowCount = 0;

            DerivativesViewModel a = new DerivativesViewModel();

            decimal d;
            int i;
            DateTime dt;

            Logger.Info($"DerivativesUpload: Loading Filename {fileName} RowCount:{sheet.Rows.Count()}");

            foreach (var row in sheet.Rows)
            {
                if (rowCount == 0)
                {
                    rowCount++;
                    continue;
                }

                try
                {
                    a = Get(row.Columns[(int)DerivativeUploadColumns.DARContractID].Value) as DerivativesViewModel;
                    if (a == null)
                        a = LookupContract(row.Columns[(int)DerivativeUploadColumns.ContractTicker].Value, row.Columns[(int)DerivativeUploadColumns.ContractExchange].Value) as DerivativesViewModel;
                    if (a == null)
                        a = new DerivativesViewModel();

                    a.UnderlierDARAssetID = row.Columns[(int)DerivativeUploadColumns.UnderlierDARAssetID].Value;
                    a.ContractType = row.Columns[(int)DerivativeUploadColumns.ContractType].Value;
                    a.OptionType = row.Columns[(int)DerivativeUploadColumns.OptionType].Value;
                    a.ContractTicker = row.Columns[(int)DerivativeUploadColumns.ContractTicker].Value;
                    //a.DARContractID = row.Columns[(int)DerivativeUploadColumns.DARContractID].Value;
                    a.ContractExchange = row.Columns[(int)DerivativeUploadColumns.ContractExchange].Value;

                    if (!string.IsNullOrWhiteSpace(a.ContractExchange))
                    {
                        Exchange e = new Exchange(a.ContractExchange);
                        if (e.CurrentExchange != null)
                        {
                            a.ContractExchangeDARID = e.CurrentExchange.DARExchangeID;
                        }
                    }

                    a.Status = row.Columns[(int)DerivativeUploadColumns.Status].Value;
                    a.TradingHours = row.Columns[(int)DerivativeUploadColumns.TradingHours].Value;

                    _ = decimal.TryParse(row.Columns[(int)DerivativeUploadColumns.MinimumTickSize].Value, out d) ? a.MinimumTickSize = d : a.MinimumTickSize = 0;
                    a.SettlementTime = row.Columns[(int)DerivativeUploadColumns.SettlementTime].Value;
                    a.SettlementType = row.Columns[(int)DerivativeUploadColumns.SettlementType].Value;
                    a.SettlementCurrency = row.Columns[(int)DerivativeUploadColumns.SettlementCurrency].Value;
                    _ = DateTime.TryParse(row.Columns[(int)DerivativeUploadColumns.ExpirationDate].Value, out dt) ? a.ExpirationDate = dt : a.ExpirationDate = DateTime.MaxValue;
                    _ = int.TryParse(row.Columns[(int)DerivativeUploadColumns.ContractSize].Value, out i) ? a.ContractSize = i : i = 0;
                    a.InitialMargin = row.Columns[(int)DerivativeUploadColumns.InitialMargin].Value;
                    a.MaintenanceMargin = row.Columns[(int)DerivativeUploadColumns.MaintenanceMargin].Value;
                    a.MarkPrice = row.Columns[(int)DerivativeUploadColumns.MarkPrice].Value;
                    a.DeliveryPrice = row.Columns[(int)DerivativeUploadColumns.DeliveryPrice].Value;
                    a.DeliveryMethod = row.Columns[(int)DerivativeUploadColumns.DeliveryMethod].Value;
                    a.FeesURL = row.Columns[(int)DerivativeUploadColumns.FeesURL].Value;
                    a.PositionLimit = row.Columns[(int)DerivativeUploadColumns.PositionLimit].Value;
                    a.PositionLimitURL = row.Columns[(int)DerivativeUploadColumns.PositionLimitURL].Value;
                    a.BlockTradeMinimum = row.Columns[(int)DerivativeUploadColumns.BlockTradeMinimum].Value;
                    a.LinktoTAndC = row.Columns[(int)DerivativeUploadColumns.LinktoTAndC].Value;
                    a.FundingRates = row.Columns[(int)DerivativeUploadColumns.FundingRates].Value;

                    if (string.IsNullOrWhiteSpace(a.DARDerivativeID))
                        Add(a);
                    else
                        Update(a);

                    rowCount++;

                    if (rowCount % 100 == 0)
                        Logger.Info($"DerivativesUpload: Loaded {rowCount} of {sheet.Rows.Count()}");
                }
                catch (Exception ex)
                {
                    Logger.Fatal($"DerivativesUpload: Failed to load row {a.GetDescription()}.", ex);
                    sbError.AppendLine(ex.Message);
                }
            }

            string summaryMessage = $"DerivativesUpload: {rowCount} of {sheet.Rows.Count()} rows loaded from file {fileName}";
            Logger.Info(summaryMessage);

            if (sbError.Length != 0)
            {
                sbError.AppendLine(summaryMessage);
                errors = sbError.ToString();
                return false;
            }

            return true;
        }

        public override bool Update(DARViewModel i)
        {
            var a = (DerivativesViewModel)i;

            Delete(a);
            Add(a);
            return true;
        }

        public override bool IdExists(string nextId)
        {
            string sql = $@"
                            select DARContractID
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Derivatives
                            where DARContractID = '{nextId}'
                            union 
                            select DARContractID
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.DerivativesContractID
                            where DARContractID = '{nextId}'
                            union 
                            select DARContractID
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.DerivativesRisk
                            where DARContractID = '{nextId}'
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var r = connection.Query<string>(sql);

                if (r != null && r.Any())
                    return true;
            }

            return false;
        }

        public override string GetNextId()
        {
            return GetNextId("DC", 6, 1000);
        }

        public string DerivativePublish(DerivativesViewModel l)
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


            string jsondata = JsonSerializer.Serialize(l);
            try
            {
                using (var producer = new ProducerBuilder<Null, string>(config).Build())
                {
                    producer.Produce("deribit_risk_0", new Message<Null, string> { Value = jsondata });
                    producer.Flush();
                }
                return "Message published without error";
            }
            catch (Exception ex)
            {
                return $"Failed to publish message {ex.Message}";
            }

        }
    }
}