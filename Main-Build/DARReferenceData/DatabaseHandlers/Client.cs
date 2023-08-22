using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Runtime.Caching;
using Confluent.Kafka;
using System.Net;
using log4net.Repository.Hierarchy;

namespace DARReferenceData.DatabaseHandlers
{
    public class Client : RefDataHandler
    {

        private ObjectCache cache = MemoryCache.Default;
        public static List<DropDownItem> GetClientList()
        {
            var l = new List<DropDownItem>();

            string sql = $@"
                            SELECT ClientName as Name
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Clients where DELETED = 0
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<DropDownItem>(sql).ToList();
            }

            return l;
        }

        public ClientViewModel GetTheExistingRecord(string values, string operation)
        {
            List<ClientViewModel> r = new List<ClientViewModel>();
            string sql = $@"
                            select *
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Clients
                            where {operation} = '{values}' and deleted = 0
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                r = connection.Query<ClientViewModel>(sql).ToList();

                return r.FirstOrDefault();
            }
        }

        public override long Add(DARViewModel i)
        {
            var a = (ClientViewModel)i;
            string error = String.Empty;
            DateTime ts = DateTime.Now.ToUniversalTime();

            if (string.IsNullOrWhiteSpace(a.DARClientID))
            {
                a.DARClientID = GetNextId();
            }

            a.CreateTime = a.LastEditTime = ts;
            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            a.Operation = "INSERT";
            a.Deleted = 0;

            if (string.IsNullOrEmpty(a.ClientName) || string.IsNullOrEmpty(a.Description) || string.IsNullOrEmpty(a.DARClientID))
            {
                throw new Exception("Client Name, ClientID and Description cannot be empty. Please try Again.");
            }

            var duplicate_name = GetTheExistingRecord(a.ClientName.Trim(), "ClientName");
            var duplicate_description = GetTheExistingRecord(a.Description.Trim(), "Description");

            if (duplicate_description != null && !string.Equals(a.DARClientID, duplicate_description.DARClientID))
            {
                error = "Client cannot be created as Table already has a record with same Description";
                throw new Exception(error);
            }

            if (duplicate_name != null && !string.Equals(a.DARClientID, duplicate_name.DARClientID))
            {
                error = "Client cannot be created as Table already has a record with same Name";
                throw new Exception(error);
            }

            if (a.ExpiryDate == null)
            {
                a.ExpiryDate = DateTime.Parse("9999-12-31 23:59:00.000000");
            }
            string publishstatus = ClientPublish(a);

            return 1;
        }

        public override bool Delete(DARViewModel i)
        {
            var a = (ClientViewModel)i;

            a.Operation = "DELETE";
            a.Deleted = 1;
            string publishstatus = ClientPublish(a);



            return true;
        }

        public override IEnumerable<DARViewModel> Get()
        {
            IEnumerable<ClientViewModel> clients;

            string sql = $@"
                            SELECT *,  400ms as Price400ms
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Clients where DELETED = 0
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                clients = connection.Query<ClientViewModel>(sql);
            }
            return clients;
        }

        public override DARViewModel Get(string key)
        {
            ClientViewModel client;

            string sql = $@"
                            SELECT *,  400ms as Price400ms
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Clients
                            WHERE ClientName = @ClientName and DELETED = 0;
                            ";
            var parameters = new { ClientName = key };
            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                client = connection.Query<ClientViewModel>(sql, parameters).FirstOrDefault();
            }

            return client;
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            throw new NotImplementedException();
        }


        public override bool Update(DARViewModel i)
        {
            var a = (ClientViewModel)i;

            DateTime updateTime = DateTime.Now.ToUniversalTime();
            a.LastEditTime = updateTime;

            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;


            //Delete(a);
            Add(a);

            return true;
        }

        public override bool IdExists(string nextId)
        {
            string sql = $@"
                            select DARClientID
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Clients
                            where DARClientID = '{nextId}'
                            
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
            return GetNextId("DCLNT", 5, 100);
        }


        public bool HasAccessToEvents(string callerID)
        {

            string sql = $@"
                            SELECT Events
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Clients c
                            WHERE c.APIKey = '{callerID}'
                            AND  c.Events = 1 and deleted = 0
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var result = connection.Query<object>(sql);

                if (result == null)
                    return false;
                if (result.Count() == 0)
                    return false;
                else
                    return true;

            }


        }

        public bool HasAccessToDerivatives(string callerID)
        {
            
            string sql = $@"
                            SELECT Derivatives
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Clients c
                            WHERE c.APIKey = '{callerID}'
                            AND  Derivatives = 1 and deleted = 0
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var result = connection.Query<bool>(sql);

                if (result == null)
                    return false;
                if (result.Count() == 0)
                    return false;
                else
                    return true;

            }
            

        }

        public bool HasAccessToLiquidityPricing(string callerID)
        {

            string sql = $@"
                            SELECT LiquidityPoolPrice
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Clients c
                            WHERE c.APIKey = '{callerID}'
                            AND  LiquidityPoolPrice = 1 and deleted = 0
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var result = connection.Query<bool>(sql);

                if (result == null)
                    return false;
                if (result.Count() == 0)
                    return false;
                else
                    return true;

            }


        }

        public bool HasAccessToCirculatingSupply(string callerID)
        {

            string sql = $@"
                            SELECT CirculatingSupply
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Clients c
                            WHERE c.APIKey = '{callerID}'
                            AND  CirculatingSupply = 1 and deleted = 0
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var result = connection.Query<bool>(sql);

                if (result == null)
                    return false;
                if (result.Count() == 0)
                    return false;
                else
                    return true;

            }


        }



        public bool HasExpired(string callerID)
        {
            string key = $"{callerID}.HasExpired";
            var hasExpired = cache[key];

            if (hasExpired == null)
            {
                string sql = $@"
                            SELECT ExpiryDate
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Clients c
                            WHERE c.APIKey = '{callerID}' and deleted = 0

                            ";

                using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
                {
                    var result = connection.Query<string>(sql);

                    bool store_result = false;

                    if (result == null)
                        store_result = false;
                    if (result.Count() == 0)
                        store_result = false;
                    else
                    {
                        if (string.IsNullOrWhiteSpace(result.ElementAtOrDefault(0)))
                        {
                            store_result = false;
                        }
                        else
                        {
                            DateTime dt;
                            if (DateTime.TryParse(result.ElementAtOrDefault(0), out dt))
                            {
                                if (dt < DateTime.Today)
                                    store_result = true;
                            }
                        }
                    }

                    CacheItemPolicy policy = new CacheItemPolicy();
                    policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(1440));
                   
                   cache.Set(key, store_result, policy);
                    return store_result;
                }
            }
            else
            {
                bool result;
                if (bool.TryParse(hasExpired.ToString(), out result))
                    return result;
            }
            return false;

        }


        public int LookBackDays(string callerID)
        {
            string key = $"{callerID}.LookBackDays";
            int default_lookback_days = 30;
            var lookBackDays = cache[key];

            if (lookBackDays == null)
            {
                string sql = $@"
                            SELECT LookBackDays
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Clients c
                            WHERE c.APIKey = '{callerID}' and deleted = 0

                            ";

                using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
                {
                    var result = connection.Query<string>(sql);

                    int store_result = default_lookback_days;

                    if (result == null)
                        store_result = default_lookback_days;
                    if (result.Count() == 0)
                        store_result = default_lookback_days;
                    else
                    {
                        if (string.IsNullOrWhiteSpace(result.ElementAtOrDefault(0)))
                        {
                            store_result = default_lookback_days;
                        }
                        else
                        {
                            int i;
                            if (int.TryParse(result.ElementAtOrDefault(0), out i))
                            {
                                store_result = i;
                            }
                        }
                    }

                    CacheItemPolicy policy = new CacheItemPolicy();
                    policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(1440));

                    cache.Set(key, store_result, policy);
                    return store_result;
                }
            }
            else
            {
                int result;
                if (int.TryParse(lookBackDays.ToString(), out result))
                    return result;
            }
            return default_lookback_days;

        }

        public string ClientPublish(ClientViewModel l)
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
                    producer.Produce("clients", new Message<Null, string> { Value = jsondata });
                    producer.Flush();
                }
                return "Message published without error";
            }
            catch (Exception ex)
            {
                return $"Failed to publish message {ex.Message}";
            }

        }

        public bool HasAccessToOHLCV(string callerID)
        {

            string sql = $@"
                            SELECT OHLCV
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Clients c
                            WHERE c.APIKey = '{callerID}'
                            AND OHLCV = 1 AND IsActive = 1 AND Deleted = 0
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var result = connection.Query<bool>(sql);

                if (result == null)
                    return false;
                if (result.Count() == 0)
                    return false;
                else
                    return true;

            }


        }


        public bool HasFullAccess(string callerID, string product)
        {

            string sql = $@"
                            SELECT HasFullAccess
                            FROM {DARApplicationInfo.SingleStoreCatalogPublic}.vClientLicensedProducts c
                            WHERE c.APIKey = '{callerID}'
                            AND c.ProductName = '{product}'
                            AND  HasFullAccess = 1
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var result = connection.Query<bool>(sql);

                if (result == null)
                    return false;
                if (result.Any() && result.FirstOrDefault() == true)
                    return true;

            }
            return false;

        }
        public bool HasAccessToProduct(string callerID, string product)
        {
            if (string.IsNullOrWhiteSpace(callerID) || string.IsNullOrWhiteSpace(product))
                return false;


            string key = $"{callerID}.{product}";
            bool? r = cache[key] as bool?;

            if (r == null)
            {
                string sql = $@"
                            select ClientName 
                            from {DARApplicationInfo.SingleStoreCatalogPublic}.vClientLicensedProducts
                            where ProductName = '{product}'
                              and APIKey = '{callerID}'
                            ";

                bool value_to_store = false;

                using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
                {
                    var result = connection.Query<string>(sql);


                    if (result == null)
                        value_to_store =  false;
                    if (result.Count() == 0)
                        value_to_store = false;
                    else
                        value_to_store =  true;

                }


                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(1440));
                cache.Set(key, value_to_store, policy);
                return value_to_store;
            }
            else
            {
                return (bool)r;
            }
        }


        public bool HasAccessToAsset(string callerID, string product, string dar_asset_id)
        {
            if (string.IsNullOrWhiteSpace(callerID) || string.IsNullOrWhiteSpace(product) || string.IsNullOrWhiteSpace(dar_asset_id))
                return false;

            string key = $"{callerID}.{product}.{dar_asset_id}";
            bool? r = cache[key] as bool?;

            if (r == null)
            {
                bool value_to_store = false;
                if (HasFullAccess(callerID, product))
                    value_to_store = true;
                else
                {

                    string sql = $@"
                            select ClientName 
                            from {DARApplicationInfo.SingleStoreCatalogPublic}.vClientLicensedProductAssets
                            where ProductName = '{product}'
                              and APIKey = '{callerID}'
                              and DARAssetID = '{dar_asset_id}'
                            ";



                    using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
                    {
                        var result = connection.Query<string>(sql);

                        if (result == null)
                            value_to_store = false;
                        if (result.Count() == 0)
                            value_to_store = false;
                        else
                            value_to_store = true;
                    }
                }

                CacheItemPolicy policy = new CacheItemPolicy();
                policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(1440));
                cache.Set(key, value_to_store, policy);
                return value_to_store;
            }
            else
            {
                return (bool)r; 
            }

        }


    }
}