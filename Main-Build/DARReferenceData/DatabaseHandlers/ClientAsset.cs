using Confluent.Kafka;
using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace DARReferenceData.DatabaseHandlers
{
    public class ClientAsset : RefDataHandler
    {

        public override long Add(DARViewModel i)
        {
            var a = (ClientAssetsViewModel)i;

            if (string.IsNullOrEmpty(a.DARAssetID))
            {
                Asset o = new Asset(a.AssetName);
                if (o.CurrentAsset == null)
                    throw new Exception("Invalid asset");

                a.DARAssetID = o.CurrentAsset.DARAssetID;
            }

            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            ClientViewModel client = (ClientViewModel)(new Client()).Get(a.ClientName);

            if(client == null)
            {
                throw new Exception($"Invalid client {a.ClientName}. Pick a valid client");
            }

            a.DARClientID = client.DARClientID;
            a.ClientName = client.ClientName;

            
            a.Operation = "INSERT";
            a.Deleted = 0;
            string publishstatus = ClientAssetPublish(a);

            return 1;

        }

        public override bool Delete(DARViewModel i)
        {
            var a = (ClientAssetsViewModel)i;

            ClientViewModel client = (ClientViewModel)(new Client()).Get(a.ClientName);

            a.DARClientID = client.DARClientID;
            a.ClientName = client.ClientName;


            a.Operation = "DELETE";
            a.Deleted = 1;
            string publishstatus = ClientAssetPublish(a);

            return true;
        }



        public bool HasFullAccess(string callerID)
        {

            string sql = $@"
                            SELECT HasFullAccess
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Clients c
                            WHERE c.APIKey = '{callerID}'
                            AND  HasFullAccess = 1
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var result  = connection.Query<bool>(sql);

                if (result == null)
                    return false;
                if (result.Any() && result.FirstOrDefault() == true)
                    return true;

            }
            return false;

        }



        public IEnumerable<DARViewModel> GetAuthorizedAssets(string callerID)
        {
            var l = new List<ClientAssetsViewModel>();

            string sql = $@"
                            SELECT *
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.vClientAssetsClientID
                            WHERE CallerID = @CallerID
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<ClientAssetsViewModel>(sql, new { CallerID = callerID }).ToList();
            }

            return l;
        }

        public IEnumerable<DARViewModel> GetAuthorizedAssetsReference(string callerID, string[] assetIdentifiers)
        {
            string identifiers = $"'{String.Join("','", assetIdentifiers)}'";

            var l = new List<ClientAssetsViewModel>();

            var p = new DynamicParameters();
            p.Add("@callerId", callerID);

            string sql = $@"
                            SELECT *
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.vClientAssetsClientID
                            WHERE CallerID = @callerId
                              AND ReferenceData = 1
                              AND DARAssetID in (ASSET_LIST)
                            ";

            sql = sql.Replace("ASSET_LIST", identifiers);

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<ClientAssetsViewModel>(sql, p).ToList();
            }

            return l;
        }

        public IEnumerable<DARViewModel> GetAuthorizedAssetsPrice(string callerID, string[] assetIdentifiers)
        {
            string identifiers = $"'{String.Join("','", assetIdentifiers)}'";

            var l = new List<ClientAssetsViewModel>();

            var p = new DynamicParameters();
            p.Add("@callerId", callerID);

            string sql = $@"
                            SELECT *
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.vClientAssetsClientID
                            WHERE CallerID = @callerId
                              AND Price = 1
                              AND DARAssetID in (ASSET_LIST)
                            ";

            sql = sql.Replace("ASSET_LIST", identifiers);

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<ClientAssetsViewModel>(sql, p).ToList();
            }

            return l;
        }

        public override IEnumerable<DARViewModel> Get()
        {
            var l = new List<ClientAssetsViewModel>();

            string sql = $@"
                            SELECT *
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.ClientAssets where deleted = 0
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<ClientAssetsViewModel>(sql)
                    .ToList();
            }

            return l;
        }

        public IEnumerable<DARViewModel> GetClientAssets(string clientName)
        {
            var cv = new List<ClientAssetsViewModel>();

            string sql = $@"SELECT  concat(DARAssetID, ClientID) as ClientAssetID
                            , ClientID as DARClientID
                            , ClientName
                            , DARAssetID
                            , DARTicker
                            , AssetName
                            , ReferenceData
                            , Price
                            from vClientAssetsClientID where ClientName = @ClientName";


            //string sql = $@"
            //                SELECT ca.*, ClientName, a.DARAssetID, a.DARTicker, a.Name as AssetName, concat(ca.DARClientID,ca.DARAssetID) as ClientAssetID
            //                FROM {DARApplicationInfo.SingleStoreCatalogInternal}.vClientAssetsClientID ca
            //                JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.Clients c ON c.DARClientID = ca.DARClientID
            //                JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a ON a.DARAssetID = ca.DARAssetID
            //                WHERE ClientName = @ClientName
            //                  AND ca.Deleted = 0

            //                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                cv = connection.Query<ClientAssetsViewModel>(sql, new { ClientName = clientName }).ToList();
            }

            return cv;
        }

        public ClientAssetsViewModel GetClientAsset(ClientAssetsViewModel ca)
        {
            var cv = new ClientAssetsViewModel();

            string sql = $@"SELECT  concat(DARAssetID, ClientID) as ClientAssetID
                            , ClientID as DARClientID
                            , ClientName
                            , DARAssetID
                            , DARTicker
                            , AssetName
                            , ReferenceData
                            , Price
                            , AssetName
                            from vClientAssetsClientID where concat(DARAssetID, ClientID) = '{ca.ClientAssetID}'";




            //string sql = $@"
            //                SELECT ca.*, ClientName, a.DARAssetID, a.DARTicker, a.Name as AssetName
            //                FROM {DARApplicationInfo.SingleStoreCatalogInternal}.ClientAssets ca
            //                JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.Clients c ON c.DARClientID = ca.DARClientID
            //                JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a ON a.DARAssetID = ca.DARAssetID
            //                WHERE concat(ca.DARClientID,ca.DARAssetID) = '{ca.ClientAssetID}'
            //                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                cv = connection.Query<ClientAssetsViewModel>(sql).ToList().FirstOrDefault();
            }

            return cv;

        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            throw new NotImplementedException();
        }
        public ClientAssetsViewModel GetOriginalID(long assetid, long clientid)
        {
            List<ClientAssetsViewModel> l = new List<ClientAssetsViewModel>();
            string sql = $@"select
                                * from
                                {DARApplicationInfo.SingleStoreCatalogInternal}.ClientAssets 
                                where AssetID = {assetid} and DARClientID = {clientid}
                    ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<ClientAssetsViewModel>(sql).ToList();
            }

            return l.FirstOrDefault();
        }

        public override bool Update(DARViewModel i)
        {
            var a = (ClientAssetsViewModel)i;

            var old = GetClientAsset(a);
            Delete(old);
          
            Asset a1 = new Asset(a.AssetName);
            if (a1.CurrentAsset != null)
            {
                a.DARAssetID = a1.CurrentAsset.DARAssetID;
            }
                

            ClientViewModel client = (ClientViewModel)(new Client()).Get(a.ClientName);

            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;


            a.DARClientID = client.DARClientID;
            a.ClientName = client.ClientName;


            a.Deleted = 0;
            Add(a);

            return true;


        }

        public override bool IdExists(string nextId)
        {
            throw new NotImplementedException();
        }

        public override string GetNextId()
        {
            throw new NotImplementedException();
        }

        public string ClientAssetPublish(ClientAssetsViewModel l)
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
                    producer.Produce("client_assets", new Message<Null, string> { Value = jsondata });
                    producer.Flush();
                }
                return "Message published without error";
            }
            catch (Exception ex)
            {
                return $"Failed to publish message {ex.Message}";
            }

        }

        public override DARViewModel Get(string key)
        {
            throw new NotImplementedException();
        }
    }
}