using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Data;
using System.Data.SqlTypes;
using Confluent.Kafka;
using System.Net;

namespace DARReferenceData.DatabaseHandlers
{
    public class AssetTheme : RefDataHandler
    {

        public AssetThemeViewModel Kafka_message_create(AssetThemeViewModel a)
        {
            AssetThemeViewModel l = new AssetThemeViewModel()
            {
                DARAssetID = a.DARAssetID,
                AssetName = a.AssetName,
                DARThemeID = a.DARThemeID,
                ThemeName = a.ThemeName,
                CreateTime = a.CreateTime,
                CreateUser = a.CreateUser,
                LastEditTime = a.LastEditTime,
                LastEditUser = a.LastEditUser,
                Operation = a.Operation,
                Deleted = a.Deleted,
                ThemeType = a.ThemeType,
            };
            return l;
        }


        public override long Add(DARViewModel i)
        {
            var a = (AssetThemeViewModel)i;

            if (string.IsNullOrWhiteSpace(a.DARAssetID))
            {
                var o = (new Asset()).Get(a.AssetName);
                if (o == null)
                {
                    throw new Exception($"Invalid Asset: {a.AssetName}");
                }
                a.DARAssetID = ((AssetViewModel)o).DARAssetID;
            }

            if(string.IsNullOrWhiteSpace(a.ThemeType))
            {
                throw new Exception("ThemeType is a required field");
            }


            if (string.IsNullOrWhiteSpace(a.DARThemeID))
            {
                var o = (new Theme()).Get(a.ThemeName,a.ThemeType);
                if (o == null)
                {
                    throw new Exception($"Invalid Theme: {a.ThemeName}");
                }
                a.DARThemeID = ((ThemeViewModel)o).DARThemeID;
            }

            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;


            a.Operation = "INSERT";
            a.Deleted = 0;
            var kafka_message = Kafka_message_create(a);
            string publishstatus = AssetThemePublish(kafka_message);

            return 1;
        }

        public bool Delete(string theme, string assetId, string themeType)
        {
            string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLAssetThemeDEL(@AssetID, @ThemeType, @Theme)";
            var p = new DynamicParameters();
            p.Add("@AssetID", assetId);
            p.Add("@Theme", theme);
            p.Add("@ThemeType", themeType);

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var result = (IDictionary<string, object>)connection.Query<object>(query, p).FirstOrDefault();

                if (!string.IsNullOrEmpty(result.Values.ElementAtOrDefault(0).ToString()) && result.Values.ElementAtOrDefault(0).ToString().Contains("Deleted"))
                {
                    return true;
                }
            }

            return false;
        }

        public override bool Delete(DARViewModel i)
        {
            var a = (AssetThemeViewModel)i;
           
            a.Operation = "DELETE";

            var length_ID = a.ID.Length;


            a.DARThemeID = a.ID.Substring(0, (length_ID - 7));
            a.Deleted = 1;

            
            var kafka_message = Kafka_message_create(a);

           

            string publishstatus = AssetThemePublish(kafka_message);


            return true;
        }

        public override IEnumerable<DARViewModel> Get()
        {
            List<AssetThemeViewModel> l = new List<AssetThemeViewModel>();

            string sql = $@"
                               SELECT concat(t.DARThemeID,t.DARAssetID) as ID
                                , a.DARAssetID
                                , a.Name as AssetName
                                , th.Name as ThemeName
                                , th.ThemeType
                                , t.DARThemeID 
                                , t.CreateUser
                                , t.LastEditUser
                                , t.CreateTime
                                , t.LastEditTime
                              FROM {DARApplicationInfo.SingleStoreCatalogInternal}.AssetTheme t
                              INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on t.DARAssetID = a.DARAssetID
                              INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.Theme th on t.DARThemeID = th.DARThemeID
                              where t.deleted = 0

                          ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<AssetThemeViewModel>(sql).ToList();
            }

            return l;
        }

        public bool ThemeExists(string assetId, string themeId)
        {
            string sql = $@"
                            SELECT count(*)
                              FROM {DARApplicationInfo.SingleStoreCatalogInternal}.AssetTheme t
                              WHERE t.DARAssetId = @assetId and t.DARThemeID = @themeId
                          ";

            var p = new DynamicParameters();
            p.Add("@assetId", assetId);
            p.Add("@themeId", themeId);

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var l = connection.Query<int>(sql, p).FirstOrDefault();

                if (l > 0)
                    return true;
            }

            return false;
        }

        public Dictionary<string, bool> GetAssetThemes(string darAssetID, string themeType)
        {
            List<AssetThemeViewModel> l = new List<AssetThemeViewModel>();
            string sql = $@"
                              SELECT th.Name as ThemeName
                                  ,0 as IsActive
                              FROM {DARApplicationInfo.SingleStoreCatalogInternal}.AssetTheme t
                              INNER JOIN {DARApplicationInfo.SingleStoreCatalogInternal}.Theme th on t.DARThemeID = th.DARThemeID
                              WHERE t.DARAssetID = @DARAssetID
                                AND th.ThemeType = @ThemeType
                          ";

            var p = new DynamicParameters();
            p.Add("@DARAssetID", darAssetID);
            p.Add("@ThemeType", themeType);

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<AssetThemeViewModel>(sql, p).ToList();
            }
            return l.ToDictionary(x => x.ThemeName, x => x.IsActive);
        }

        public override DARViewModel Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            return Get().Cast<AssetThemeViewModel>().Where(x => x.AssetName.ToUpper().Equals(key.ToUpper()) || x.ThemeName.ToUpper().Equals(key.ToUpper())).FirstOrDefault();
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            throw new NotImplementedException();
        }

        public override bool Update(DARViewModel i)
        {
            var a = (AssetThemeViewModel)i;

            a.DARAssetID = string.Empty;
            if (string.IsNullOrWhiteSpace(a.AssetName))
            {
                throw new Exception($"Invalid AssetName:{a.AssetName}");
            }

            if (!string.IsNullOrWhiteSpace(a.AssetName))
            {
                var oan = (AssetViewModel)(new Asset()).Get(a.AssetName);
                if (oan != null)
                    a.DARAssetID = oan.DARAssetID;
            }

            if (string.IsNullOrWhiteSpace(a.DARAssetID))
            {
                throw new Exception($"Can't lookup Asset using AssetName:{a.AssetName}");
            }

            a.DARThemeID = String.Empty;
            if (string.IsNullOrWhiteSpace(a.ThemeName))
            {
                throw new Exception($"Invalid Theme Name:{a.ThemeName}");
            }

            var o = (ThemeViewModel)(new Theme()).Get(a.ThemeName);
            if (o != null)
                a.DARThemeID = o.DARThemeID;
            else
            {
                throw new Exception($"Can't lookup Theme using Name:{a.ThemeName}. Please make sure that this Theme exists in DAR Database");
            }


            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            var new_theme_id = a.DARThemeID;
            Delete(a);

            a.DARThemeID = new_theme_id;
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

        public string AssetThemePublish(AssetThemeViewModel l)
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
                    producer.Produce("asset_theme", new Message<Null, string> { Value = jsondata });
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