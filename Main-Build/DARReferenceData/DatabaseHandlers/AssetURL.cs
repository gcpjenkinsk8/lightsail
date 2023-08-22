using Confluent.Kafka;
using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace DARReferenceData.DatabaseHandlers
{
    public class AssetURL : RefDataHandler
    {
        public override long Add(DARViewModel i)
        {
            var a = (AssetURLViewModel)i;

            if (string.IsNullOrWhiteSpace(a.DARAssetID))
            {
                var o = (new Asset()).Get(a.Asset);
                if (o == null)
                {
                    throw new Exception($"Invalid asset {a.Asset}");
                }
                a.AssetID = ((AssetViewModel)o).DARAssetID;
                a.DARAssetID = ((AssetViewModel)o).DARAssetID;
            }

            if (string.IsNullOrWhiteSpace(a.DARURLTypeID))
            {
                var o = (new UrlType()).Get(a.URLType);
                if (o == null)
                {
                    throw new Exception($"Invalid URL {a.URLType}");
                }
                a.DARURLTypeID = ((UrlTypeViewModel)o).DARURLTypeID;
            }

            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            a.Operation = "INSERT";
            a.Deleted = 0;
            AssetURLPublish(a);


            return 0;
        }

        public long Add(string urlType, string urlPath, string assetID)
        {
            if (string.IsNullOrWhiteSpace(urlPath))
                return 0;

            UrlType ut = new UrlType();
            var utvm = (UrlTypeViewModel)ut.Get(urlType);
            if (utvm == null)
                throw new Exception($"Invalid URL type {urlType}");
            AssetURLViewModel auvm = new AssetURLViewModel
            {
                AssetID = assetID,
                DARURLTypeID = utvm.DARURLTypeID,
                URLPath = urlPath
            };
            return Add(auvm);
        }

        public void Upsert(string urlType, string urlPath, string assetID)
        {
            if (string.IsNullOrWhiteSpace(urlPath))
                return;

            if (urlType == null)
                throw new Exception($"Invalid URL Type");

            AssetURLViewModel auvm;

            // Get entry from asset url by AssetID and URL Type ID
            // if it exists then call update otherwise call add

            auvm = (AssetURLViewModel)Get(assetID, urlType);

            if (auvm != null && !string.IsNullOrWhiteSpace(auvm.DARURLTypeID))
            {
                auvm.URLPath = urlPath;
                Update(auvm);
            }
            else
            {
                auvm = new AssetURLViewModel
                {
                    DARAssetID = assetID,
                    DARURLTypeID = urlType,
                    URLPath = urlPath.Equals("Null") ? DBNull.Value.ToString() : urlPath
                };
                Add(auvm);
            }
        }

        public DARViewModel Get(string assetId, string urlTypeId)
        {
            var l = new AssetURLViewModel();

            string sql = $@"
                      SELECT a.Name as Asset
                                      ,a.DARAssetID
	                                  ,ut.Name as URLType
                                      ,au.DARURLTypeID
                                      ,au.URLPath
                      FROM  {DARApplicationInfo.SingleStoreCatalogInternal}.AssetURL au
                      INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on au.DARAssetID =  a.DARAssetID
                      INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.URLType ut on au.DARURLTypeID =  ut.DARURLTypeID
	                  WHERE au.DARAssetID = @assetId
					    AND au.DARURLTypeID = @urlTypeId
                          ";

            var p = new DynamicParameters();
            p.Add("@assetId", assetId);
            p.Add("@urlTypeId", urlTypeId);

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<AssetURLViewModel>(sql, p).FirstOrDefault();
            }

            return l;
        }

        public override bool Delete(DARViewModel i)
        {
            var a = (AssetURLViewModel)i;

            a.Operation = "DELETED";
            a.Deleted = 1;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            AssetURLPublish(a);

            return true;
                 
        }

        public override IEnumerable<DARViewModel> Get()
        {
            List<AssetURLViewModel> l = new List<AssetURLViewModel>();

            string sql = $@"
                     SELECT au.DARURLTypeID as DARURLTypeID
                                      ,a.Name as Asset
                                      ,a.DARAssetID
                                      ,au.DARAssetID as AssetID
	                                  ,ut.Name as URLType
                                      ,au.URLPath
                      FROM {DARApplicationInfo.SingleStoreCatalogInternal}.AssetURL au
                      INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on au.DARAssetID =  a.DARAssetID
                      INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.URLType ut on au.DARURLTypeID =  ut.DARURLTypeID
                      where au.Deleted = 0
                          ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<AssetURLViewModel>(sql).ToList();
            }

            return l;
        }

        public AssetURLViewModel Get(AssetURLViewModel a)
        {
            AssetURLViewModel l = new AssetURLViewModel();

            string sql = $@"
                      SELECT a.Name as Asset
                                      ,a.DARAssetID
                                      ,au.DARAssetID as AssetID
	                                  ,ut.Name as URLType
                                      ,au.DARURLTypeID
                                      ,au.URLPath
                      FROM {DARApplicationInfo.SingleStoreCatalogInternal}.AssetURL au
                      INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on au.DARAssetID =  a.DARAssetID
                      INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.URLType ut on au.DARURLTypeID =  ut.DARURLTypeID
                      WHERE au.DARAssetID = '{a.DARAssetID}'
                        AND au.DARURLTypeID = {a.DARURLTypeID}
                      AND au.DELETED = 0
                          ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<AssetURLViewModel>(sql).ToList().FirstOrDefault();
            }

            return l;
        }

        public override DARViewModel Get(string key)
        {
            throw new NotImplementedException();
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            throw new NotImplementedException();
        }

        public override bool Update(DARViewModel i)
        {
            var a = (AssetURLViewModel)i;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            var oldAssetURL = Get(a);

            Delete(oldAssetURL);

            a.DARURLTypeID = ((UrlTypeViewModel)((new UrlType()).Get(a.URLType))).DARURLTypeID;
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

        public string AssetURLPublish(AssetURLViewModel l)
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
                    producer.Produce("asset_url", new Message<Null, string> { Value = jsondata });
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