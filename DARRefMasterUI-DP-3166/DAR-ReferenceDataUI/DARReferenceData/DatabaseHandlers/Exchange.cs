using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;
using System.Windows.Input;
using Confluent.Kafka;
using Dapper;
using DARReferenceData.ViewModels;
using log4net;
using MySql.Data.MySqlClient;
using Spire.Xls;

namespace DARReferenceData.DatabaseHandlers
{
    public class Exchange : RefDataHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);

        public static int GetCount()
        {
            string sql = $@"
                           SELECT count(*)
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Exchange where DELETED = 0";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                return connection.Query<int>(sql).FirstOrDefault();
            }
        }

        private enum ExchangeUploadColumns
        {
            DARExchangeId = 0
            , ShortName = 1
            , LegalName = 2
            , LegalNameSource = 3
            , ExchangeType = 4
            , ExchangeTypeSource = 5
            , ExchangeStatus = 6
            , ExternalClassification = 7
            , InternalClassification = 8
            , ClassificationFolder = 9
            , ClassificationDate = 10
            , ClassificationVersion = 11
            , DomicileCountry = 12
            , IncorporationCountry = 13
            , ExchangeSLA = 14
            , FoundingYear = 15
            , Ownership = 16
            , LEI = 17
            , Chairman = 18
            , CEO = 19
            , President = 20
            , CTO = 21
            , CISO = 22
            , CCO = 23
            , PrimaryPhone = 24
            , PrimaryEmail = 25
            , SupportURL = 26
            , SupportPhone = 27
            , SupportEmail = 28
            , HQAddress1 = 29
            , HQAddress2 = 30
            , HQCity = 31
            , HQState = 32
            , HQCountry = 33
            , HQPostalCode = 34
            , Licenses = 35
            , Wikipedia = 36
            , MICCode = 37
            , KnownRegulatoryIssues = 38
            , TradeMonitoringSystem = 39
            , BlockchainSurveillanceSystem = 40
            , ThirdPartyAudit = 41
            , KnownSecurityIncidences = 42
            , InsuranceProviders = 43
            , InsuranceonCryptoAssets = 44
            , Wherethebankisdomiciled = 45
            , SelfInsurance = 46
            , MandatoryGovtIDPriortoTrading = 47
            , TradingLimitExKYC = 48
            , TradingLimitExKYCsource = 49
            , DepositLimitExKYC = 50
            , DepositLimitExKYCsource = 51
            , WithdrawalLimitExKYC = 52
            , WithdrawalLimitExKYCsource = 53
            , KYCReqGovernmentID = 54
            , KYCReqDigitalSelfPortrait = 55
            , CorporateActionsPolicy = 56
            , PoliciesOnListing = 57
            , FeeSchedule = 58
            , TradingHours = 59
            , Leverage = 60
            , Staking = 61
            , IEOPlatform = 62
            , NativeToken = 63
            , ColdStorageCustody = 64
            , CustodyInsurance = 65
            , PercentOfAssetsinColdStorage = 66
            , StablecoinPairs = 67
            , FiatTrading = 68
            , Futures = 69
            , Options = 70
            , Swaps = 71
            , APIType = 72
            , APIDocumentation = 73
            , PrimaryURL = 74
            , Twitter = 75
            , LinkedIn = 76
            , Reddit = 77
            , Facebook = 78
        }

        private enum ExchangeVettingStatusUploadColumns
        {
            ProcessName = 0
            , Exchange = 1
            , VettingStatus = 2
        }

        public ExchangeViewModel CurrentExchange { get; set; }

        public Exchange()
        {
        }

        public Exchange(string exchange)
        {
            if (!string.IsNullOrWhiteSpace(exchange))
                Get(exchange);
        }

        public void Upsert(ExchangeViewModel a)
        {
            Get(a.DARExchangeID);

            if (CurrentExchange != null)
            {
                Update(a);
            }
            else
            {
                Add(a);
            }
        }

        public void ReplicateExchange(string darExchangeId)
        {
            if (!DARApplicationInfo.CurrentEnvironment.Equals("PROD"))
                return;

            if (string.IsNullOrWhiteSpace(darExchangeId))
                throw new Exception($"Empty exchangeid  string is invalid");

            Exchange a = new Exchange(darExchangeId);
            if (a.CurrentExchange != null)
            {
                try
                {
                    using (var connection = new MySqlConnection(DARApplicationInfo.ReferenceDB))
                    {
                        var x = connection.Execute($"CALL {DARApplicationInfo.SingleStoreCatalogPublic}.sp_upsert_exchange('{a.CurrentExchange.ShortName}',3,'{a.CurrentExchange.DARExchangeID}')");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to replicate exchange {darExchangeId}. Error:{ex.Message}");
                }
            }
            else
            {
                throw new Exception($"Invalid darExchangeId {darExchangeId}");
            }
        }

        public override long Add(DARViewModel i)
        {
            var exchange = (ExchangeViewModel)i;

            if (!string.IsNullOrWhiteSpace(exchange.DARExchangeID))
            {
                // check for name change
                var name_old = exchange.UniqueExchangeId.Substring(7);
                if (!string.Equals(exchange.ShortName, name_old))
                {
                    var exchange_existing_record = Get().Cast<ExchangeViewModel>().Where(x => x.ShortName.ToUpper().Equals(exchange.ShortName.ToUpper())).FirstOrDefault();

                    if (exchange_existing_record != null)
                    {
                        throw new Exception($"A exchange with same NAME already exists in the database.");

                    }
                }               

            }


            if (string.IsNullOrWhiteSpace(exchange.DARExchangeID))
            {                
                var exchange_existing_record = Get().Cast<ExchangeViewModel>().Where(x => x.ShortName.ToUpper().Equals(exchange.ShortName.ToUpper())).FirstOrDefault();

                if (exchange_existing_record != null)
                {
                    throw new Exception($"A exchange with same NAME already exists in the database.");

                }

                exchange.DARExchangeID = GetNextId();
            }       


            exchange.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            exchange.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            if (exchange.ClassificationDate != null)
                exchange.ClassificationDate = ((DateTime)exchange.ClassificationDate);

            exchange.Operation = "INSERT";
            exchange.Deleted = 0;
            exchange.IsActive = true;
            string publishstatus = ExchangePublish(exchange);

            
            CurrentExchange = exchange;

            return 0;
        }

        public override bool Update(DARViewModel i)
        {
            var a = (ExchangeViewModel)i;

            var older_record = Get(a.UniqueExchangeId);
            //Delete(older_record);
            Add(a);

            //long result = Add(a);

            //if (result <= 0)
            //    return false;

            return true;
        }

        public override bool Delete(DARViewModel i)
        {
            var a = (ExchangeViewModel)i;

            a.Operation = "DELETE";
            a.Deleted = 1;
            a.IsActive = true;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            string publishstatus = ExchangePublish(a);

            //string sql = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLExchangeDEL(@ID)";

            //long deletedId = 0;
            //try
            //{
            //    using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            //    {
            //        var r = connection.Query<object>(sql, new { ID = a.ID });

            //        if (r.Any())
            //        {
            //            var data = (IDictionary<string, object>)r.FirstOrDefault();
            //            deletedId = (long)data.Values.FirstOrDefault();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //}

            return true;
        }

        public override IEnumerable<DARViewModel> Get()
        {
            List<ExchangeViewModel> l = new List<ExchangeViewModel>();

            string sql = $@"

                        SELECT CONCAT(DARExchangeID, ShortName) as UniqueExchangeId
                              ,DARExchangeID
                              ,ShortName
                              ,ExchangeType
                              ,IsActive
                              ,CreateUser
                              ,LastEditUser
                              ,CreateTime
                              ,LastEditTime
                            ,LegalName
                            ,LegalNameSource
                            ,ExchangeTypeSource
                            ,ExchangeStatus
                            ,ExternalClassification
                            ,InternalClassification
                            ,ClassificationFolder
                            ,ClassificationDate
                            ,ClassificationVersion
                            ,DomicileCountry
                            ,IncorporationCountry
                            ,ExchangeSLA
                            ,FoundingYear
                            ,Ownership
                            ,LEI
                            ,Chairman
                            ,CEO
                            ,President
                            ,CTO
                            ,CISO
                            ,CCO
                            ,PrimaryPhone
                            ,PrimaryEmail
                            ,SupportURL
                            ,SupportPhone
                            ,SupportEmail
                            ,HQAddress1
                            ,HQAddress2
                            ,HQCity
                            ,HQState
                            ,HQCountry
                            ,HQPostalCode
                            ,Licenses
                            ,Wikipedia
                            ,MICCode
                            ,KnownRegulatoryIssues
                            ,TradeMonitoringSystem
                            ,BlockchainSurveillanceSystem
                            ,ThirdPartyAudit
                            ,KnownSecurityIncidences
                            ,InsuranceProviders
                            ,InsuranceonCryptoAssets
                            ,Wherethebankisdomiciled
                            ,SelfInsurance
                            ,MandatoryGovtIDPriortoTrading
                            ,TradingLimitExKYC
                            ,TradingLimitExKYCsource
                            ,DepositLimitExKYC
                            ,DepositLimitExKYCsource
                            ,WithdrawalLimitExKYC
                            ,WithdrawalLimitExKYCsource
                            ,KYCReqGovernmentID
                            ,KYCReqDigitalSelfPortrait
                            ,CorporateActionsPolicy
                            ,PoliciesOnListing
                            ,FeeSchedule
                            ,TradingHours
                            ,Leverage
                            ,Staking
                            ,IEOPlatform
                            ,NativeToken
                            ,ColdStorageCustody
                            ,CustodyInsurance
                            ,PercentOfAssetsinColdStorage
                            ,StablecoinPairs
                            ,FiatTrading
                            ,Futures
                            ,Options
                            ,Swaps
                            ,APIType
                            ,APIDocumentation
                            ,PrimaryURL
                            ,Twitter
                            ,LinkedIn
                            ,Reddit
                            ,Facebook
                            ,LegacyId
                          FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Exchange where DELETED = 0

                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<ExchangeViewModel>(sql).ToList();
            }

            return l;
        }


        public DARViewModel GetExchangePublic(string key)
        {
            List<ExchangeViewModel> l = new List<ExchangeViewModel>();

            string sql = $@"
                            select legacyID as LegacyId
                                  ,name as ShortName
                                  ,darExchangeID as DARExchangeID
                            from {DARApplicationInfo.SingleStoreCatalogPublic}.exchange
                            where (name = '{key}' or darExchangeID = '{key}' or literal = '{key}')

                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<ExchangeViewModel>(sql).ToList();
            }

            return l.FirstOrDefault();
        }

        public override DARViewModel Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            CurrentExchange = Get().Cast<ExchangeViewModel>().Where(x => x.DARExchangeID.ToUpper().Equals(key.ToUpper()) || x.ShortName.ToUpper().Equals(key.ToUpper()) || x.UniqueExchangeId.ToUpper().Equals(key.ToUpper())).FirstOrDefault();
            return CurrentExchange;
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            errors = string.Empty;
            StringBuilder sbError = new StringBuilder();
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(fileName);
            Worksheet sheet = workbook.Worksheets[0];

            int rowCount = 0;

            ExchangeViewModel a = new ExchangeViewModel();

            int i;
            DateTime dt;
            bool b;

            Logger.Info($"ExchangeUpload: Loading Filename {fileName} RowCount:{sheet.Rows.Count()}");

            foreach (var row in sheet.Rows)
            {
                if (rowCount == 0)
                {
                    rowCount++;
                    continue;
                }

                try
                {
                    a = Get(row.Columns[(int)ExchangeUploadColumns.DARExchangeId].Value) as ExchangeViewModel;
                    if (a == null)
                        a = new ExchangeViewModel();

                    a.ShortName = row.Columns[(int)ExchangeUploadColumns.ShortName].Value;

                    if (string.IsNullOrWhiteSpace(a.ShortName))
                    {
                        throw new Exception($"Rejecting record at row {rowCount}. ShortName is required. ");
                    }

                    a.LegalName = row.Columns[(int)ExchangeUploadColumns.LegalName].Value;
                    a.LegalNameSource = row.Columns[(int)ExchangeUploadColumns.LegalNameSource].Value;
                    a.ExchangeType = row.Columns[(int)ExchangeUploadColumns.ExchangeType].Value;
                    a.ExchangeTypeSource = row.Columns[(int)ExchangeUploadColumns.ExchangeTypeSource].Value;
                    a.ExchangeStatus = row.Columns[(int)ExchangeUploadColumns.ExchangeStatus].Value;
                    a.ExternalClassification = row.Columns[(int)ExchangeUploadColumns.ExternalClassification].Value;
                    a.InternalClassification = row.Columns[(int)ExchangeUploadColumns.InternalClassification].Value;
                    a.ClassificationFolder = row.Columns[(int)ExchangeUploadColumns.ClassificationFolder].Value;
                    if (DateTime.TryParse(row.Columns[(int)ExchangeUploadColumns.ClassificationDate].Value, out dt))
                        a.ClassificationDate = dt;
                    if (int.TryParse(row.Columns[(int)ExchangeUploadColumns.ClassificationVersion].Value, out i))
                        a.ClassificationVersion = i;
                    a.DomicileCountry = row.Columns[(int)ExchangeUploadColumns.DomicileCountry].Value;
                    a.IncorporationCountry = row.Columns[(int)ExchangeUploadColumns.IncorporationCountry].Value;
                    a.ExchangeSLA = row.Columns[(int)ExchangeUploadColumns.ExchangeSLA].Value;
                    if (int.TryParse(row.Columns[(int)ExchangeUploadColumns.FoundingYear].Value, out i))
                        a.FoundingYear = i;
                    a.Ownership = row.Columns[(int)ExchangeUploadColumns.Ownership].Value;
                    a.LEI = row.Columns[(int)ExchangeUploadColumns.LEI].Value;
                    a.Chairman = row.Columns[(int)ExchangeUploadColumns.Chairman].Value;
                    a.CEO = row.Columns[(int)ExchangeUploadColumns.CEO].Value;
                    a.President = row.Columns[(int)ExchangeUploadColumns.President].Value;
                    a.CTO = row.Columns[(int)ExchangeUploadColumns.CTO].Value;
                    a.CISO = row.Columns[(int)ExchangeUploadColumns.CISO].Value;
                    a.CCO = row.Columns[(int)ExchangeUploadColumns.CCO].Value;
                    a.PrimaryPhone = row.Columns[(int)ExchangeUploadColumns.PrimaryPhone].Value;
                    a.PrimaryEmail = row.Columns[(int)ExchangeUploadColumns.PrimaryEmail].Value;
                    a.SupportURL = row.Columns[(int)ExchangeUploadColumns.SupportURL].Value;
                    a.SupportPhone = row.Columns[(int)ExchangeUploadColumns.SupportPhone].Value;
                    a.SupportEmail = row.Columns[(int)ExchangeUploadColumns.SupportEmail].Value;
                    a.HQAddress1 = row.Columns[(int)ExchangeUploadColumns.HQAddress1].Value;
                    a.HQAddress2 = row.Columns[(int)ExchangeUploadColumns.HQAddress2].Value;
                    a.HQCity = row.Columns[(int)ExchangeUploadColumns.HQCity].Value;
                    a.HQState = row.Columns[(int)ExchangeUploadColumns.HQState].Value;
                    a.HQCountry = row.Columns[(int)ExchangeUploadColumns.HQCountry].Value;
                    a.HQPostalCode = row.Columns[(int)ExchangeUploadColumns.HQPostalCode].Value;
                    a.Licenses = row.Columns[(int)ExchangeUploadColumns.Licenses].Value;
                    a.Wikipedia = row.Columns[(int)ExchangeUploadColumns.Wikipedia].Value;
                    a.MICCode = row.Columns[(int)ExchangeUploadColumns.MICCode].Value;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.KnownRegulatoryIssues].Value, out b))
                        a.KnownRegulatoryIssues = b;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.TradeMonitoringSystem].Value, out b))
                        a.TradeMonitoringSystem = b;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.BlockchainSurveillanceSystem].Value, out b))
                        a.BlockchainSurveillanceSystem = b;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.ThirdPartyAudit].Value, out b))
                        a.ThirdPartyAudit = b;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.KnownSecurityIncidences].Value, out b))
                        a.KnownSecurityIncidences = b;

                    a.InsuranceProviders = row.Columns[(int)ExchangeUploadColumns.InsuranceProviders].Value;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.InsuranceonCryptoAssets].Value, out b))
                        a.InsuranceonCryptoAssets = b;
                    a.Wherethebankisdomiciled = row.Columns[(int)ExchangeUploadColumns.Wherethebankisdomiciled].Value;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.SelfInsurance].Value, out b))
                        a.SelfInsurance = b;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.MandatoryGovtIDPriortoTrading].Value, out b))
                        a.MandatoryGovtIDPriortoTrading = b;
                    a.TradingLimitExKYC = row.Columns[(int)ExchangeUploadColumns.TradingLimitExKYC].Value;
                    a.TradingLimitExKYCsource = row.Columns[(int)ExchangeUploadColumns.TradingLimitExKYCsource].Value;
                    a.DepositLimitExKYC = row.Columns[(int)ExchangeUploadColumns.DepositLimitExKYC].Value;
                    a.DepositLimitExKYCsource = row.Columns[(int)ExchangeUploadColumns.DepositLimitExKYCsource].Value;
                    a.WithdrawalLimitExKYC = row.Columns[(int)ExchangeUploadColumns.WithdrawalLimitExKYC].Value;
                    a.WithdrawalLimitExKYCsource = row.Columns[(int)ExchangeUploadColumns.WithdrawalLimitExKYCsource].Value;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.KYCReqGovernmentID].Value, out b))
                        a.KYCReqGovernmentID = b;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.KYCReqDigitalSelfPortrait].Value, out b))
                        a.KYCReqDigitalSelfPortrait = b;
                    a.CorporateActionsPolicy = row.Columns[(int)ExchangeUploadColumns.CorporateActionsPolicy].Value;
                    a.PoliciesOnListing = row.Columns[(int)ExchangeUploadColumns.PoliciesOnListing].Value;
                    a.FeeSchedule = row.Columns[(int)ExchangeUploadColumns.FeeSchedule].Value;
                    a.TradingHours = row.Columns[(int)ExchangeUploadColumns.TradingHours].Value;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.Leverage].Value, out b))
                        a.Leverage = b;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.Staking].Value, out b))
                        a.Staking = b;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.IEOPlatform].Value, out b))
                        a.IEOPlatform = b;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.NativeToken].Value, out b))
                        a.NativeToken = b;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.ColdStorageCustody].Value, out b))
                        a.ColdStorageCustody = b;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.CustodyInsurance].Value, out b))
                        a.CustodyInsurance = b;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.PercentOfAssetsinColdStorage].Value, out b))
                        a.PercentOfAssetsinColdStorage = b;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.StablecoinPairs].Value, out b))
                        a.StablecoinPairs = b;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.FiatTrading].Value, out b))
                        a.FiatTrading = b;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.Futures].Value, out b))
                        a.Futures = b;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.Options].Value, out b))
                        a.Options = b;
                    if (bool.TryParse(row.Columns[(int)ExchangeUploadColumns.Swaps].Value, out b))
                        a.Swaps = b;
                    a.APIType = row.Columns[(int)ExchangeUploadColumns.APIType].Value;
                    a.APIDocumentation = row.Columns[(int)ExchangeUploadColumns.APIDocumentation].Value;

                    a.PrimaryURL = row.Columns[(int)ExchangeUploadColumns.PrimaryURL].Value;
                    a.Twitter = row.Columns[(int)ExchangeUploadColumns.Twitter].Value;
                    a.LinkedIn = row.Columns[(int)ExchangeUploadColumns.LinkedIn].Value;
                    a.Reddit = row.Columns[(int)ExchangeUploadColumns.Reddit].Value;
                    a.Facebook = row.Columns[(int)ExchangeUploadColumns.Facebook].Value;

                    if (string.IsNullOrWhiteSpace(a.DARExchangeID))
                        Add(a);
                    else
                        Update(a);

                    rowCount++;

                    if (rowCount % 100 == 0)
                        Logger.Info($"ExchangeUpload: Loaded {rowCount} of {sheet.Rows.Count()}");
                }
                catch (Exception ex)
                {
                    Logger.Fatal($"ExchangeUpload: Failed to load row {a.GetDescription()}.", ex);
                    sbError.AppendLine(ex.Message);
                }
            }

            string summaryMessage = $"ExchangeUpload: {rowCount} of {sheet.Rows.Count()} rows loaded from file {fileName}";
            Logger.Info(summaryMessage);

            if (sbError.Length != 0)
            {
                sbError.AppendLine(summaryMessage);
                errors = sbError.ToString();
                return false;
            }

            return true;
        }

        
        public override bool IdExists(string nextId)
        {
            string sql = $@"
                            select DARExchangeID
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Exchange
                            where DARExchangeID = '{nextId}'
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
            return GetNextId("DE", 5, 100);
        }

        public void DeleteReplicatedExchange(string darExchangeId)
        {
            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                var x = connection.Query<int>($"select legacyID from {DARApplicationInfo.SingleStoreCatalogPublic}.exchange where darExchangeID = '{darExchangeId}' ");
                if (x != null && x.Count() > 0)
                {
                    if (x.Single() >= 0)
                    {
                        int deleteID = x.Single();
                        try
                        {
                            connection.Execute($"delete from {DARApplicationInfo.SingleStoreCatalogPublic}.exchange where legacyID={deleteID}");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                        try
                        {
                            connection.Execute($"delete from {DARApplicationInfo.SingleStoreCatalogDax}.Exchange where id={deleteID}");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                        try
                        {
                            connection.Execute($"delete from {DARApplicationInfo.SingleStoreCatalogDardb}.Exchange where id={deleteID}");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                        try
                        {
                            connection.Execute($"delete from {DARApplicationInfo.SingleStoreCatalogMetadata}.Exchange where id={deleteID}");
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                        }
                    }
                }
            }
        }
        public string ExchangePublish(ExchangeViewModel l)
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
                    producer.Produce("exchange", new Message<Null, string> { Value = jsondata });
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