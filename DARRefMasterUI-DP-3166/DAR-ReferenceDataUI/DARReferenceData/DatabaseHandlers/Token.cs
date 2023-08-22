using Confluent.Kafka;
using Dapper;
using DARReferenceData.ViewModels;
using log4net;
using MySql.Data.MySqlClient;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace DARReferenceData.DatabaseHandlers
{
    public class Token : RefDataHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);

        private enum TokenUploadColumns
        {
            DARTokenID = 0
            , DARTAssetID = 1
            , DARTicker = 2
            , TokenBlockchainBase = 3
            , TokenContractAddress = 4
        }

        public Token(string name)
        {
            Get(name);
        }

        public Token()
        {
        }

        public TokenViewModel CurrentToken { get; set; }

        public string AddToken(DARViewModel i)
        {
            var a = (TokenViewModel)i;
                       

            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.IsActive = true;

            if(string.IsNullOrWhiteSpace(a.DARBlockchainID))
            {
                var block = new BlockChain(a.TokenBlockchainBase);
                if (block.CurrentBlockChain == null)
                {
                    throw new Exception($"Invalid blockchainbase {a.TokenBlockchainBase}. Setup blockchain first");
                }
                a.DARBlockchainID = block.CurrentBlockChain.DARBlockchainID;

            }


            a.Operation = "INSERT";
            a.Deleted = 0;
            if (string.IsNullOrWhiteSpace(a.DARTokenID))
                a.DARTokenID = GetNextId();
            string publishstatus = TokenPublish(a);


            return a.DARTokenID;
        }

        public override bool Delete(DARViewModel i)
        {
            var a = (TokenViewModel)i;

            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.IsActive = true;

            a.Operation = "DELETE";
            a.Deleted = 1;
            string publishstatus = TokenPublish(a);

            return true;
        }

        public override IEnumerable<DARViewModel> Get()
        {
            List<TokenViewModel> l = new List<TokenViewModel>();

            string sql = $@"
                            SELECT DARTokenID
                                  , TokenName
                                  , TokenDescription
                                  , IsActive
                                  , CreateUser
                                  , LastEditUser
                                  , CreateTime
                                  , LastEditTime
                              FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Token where DELETED = 0
                          ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<TokenViewModel>(sql).ToList();
            }

            return l;
        }

        public override DARViewModel Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            CurrentToken = Get().Cast<TokenViewModel>().Where(x => x.TokenName.ToUpper().Equals(key.ToUpper()) || x.DARTokenID.ToUpper().Equals(key.ToUpper())).FirstOrDefault();
            return CurrentToken;
        }

        public string Publish(TokenViewModel l)
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
                    producer.Produce("token", new Message<Null, string> { Value = jsondata });
                    producer.Flush();
                }
                return "Message published without error";
            }
            catch (Exception ex)
            {
                return $"Failed to publish message {ex.Message}";
            }

        }
        public bool UpsertAssetToken(string darTokenId, string darAssetId, string darTicker, string tokenBlockchainBase, string tokenContractAddress, int currentRow)
        {
            Asset asset;
            Token token;
            BlockChain block;

            string tokenName;

            token = new Token();
            asset = new Asset(darAssetId);
            if (asset.CurrentAsset == null)
            {
                throw new Exception($"Invalid asset {token.CurrentToken.DARAssetID}. Setup asset first");
            }

            tokenName = $"{asset.CurrentAsset.DARAssetID} on {tokenBlockchainBase}";
            if (!string.IsNullOrEmpty(darTokenId))
            {
                token = new Token(darTokenId);
            }

            // Lookup by token name
            if (token.CurrentToken == null)
            {
                token = new Token(tokenName);
            }

            if (token.CurrentToken == null)
            {
                token = new Token();
                token.CurrentToken = new TokenViewModel();
                token.CurrentToken.DARTokenID = GetNextId();
                token.CurrentToken.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
                token.CurrentToken.CreateTime = DateTime.Now;
                token.CurrentToken.Operation = "INSERT";
            }
            else
            {
                token.CurrentToken.Operation = "UPDATE";
            }

            
            token.CurrentToken.DARAssetID = asset.CurrentAsset.DARAssetID;
            token.CurrentToken.DARTicker = asset.CurrentAsset.DARTicker;
            token.CurrentToken.TokenBlockchainBase = tokenBlockchainBase;
            token.CurrentToken.TokenContractAddress = tokenContractAddress;

            if (string.IsNullOrWhiteSpace(token.CurrentToken.DARAssetID) && string.IsNullOrWhiteSpace(token.CurrentToken.TokenBlockchainBase))
            {
                throw new Exception($"TokenUpload: Invalid input Row:{currentRow} DAR Asset Id: {token.CurrentToken.DARAssetID} Blockchain: {token.CurrentToken.TokenBlockchainBase} ");
            }

          


            block = new BlockChain(token.CurrentToken.TokenBlockchainBase);
            if (block.CurrentBlockChain == null)
            {
                throw new Exception($"Invalid blockchainbase {token.CurrentToken.TokenBlockchainBase}. Setup blockchain first");
            }

            token.CurrentToken.TokenName = token.CurrentToken.TokenDescription = tokenName;
            token.CurrentToken.DARBlockchainID = block.CurrentBlockChain.DARBlockchainID;

            token.CurrentToken.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            token.CurrentToken.LastEditTime = DateTime.Now;

            string publishStatus = Publish(token.CurrentToken);

            return true;
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            errors = string.Empty;
            StringBuilder sbError = new StringBuilder();
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(fileName);
            Worksheet sheet = workbook.Worksheets[0];

            int rowCount = 0;
            int processedRowCount = 0;

            string darTokenId;
            string darAssetId;
            string darTicker;
            string tokenBlockchainBase;
            string tokenContractAddress;

            Logger.Info($"TokenUpload: Loading Filename {fileName} RowCount:{sheet.Rows.Count()}");

            foreach (var row in sheet.Rows)
            {
                if (rowCount == 0)
                {
                    rowCount++;
                    continue;
                }
                try
                {
                    darTokenId = row.Columns[(int)TokenUploadColumns.DARTokenID].Value;
                    darAssetId = row.Columns[(int)TokenUploadColumns.DARTAssetID].Value;
                    darTicker = row.Columns[(int)TokenUploadColumns.DARTicker].Value;
                    tokenBlockchainBase = row.Columns[(int)TokenUploadColumns.TokenBlockchainBase].Value;
                    tokenContractAddress = row.Columns[(int)TokenUploadColumns.TokenContractAddress].Value;

                    UpsertAssetToken(darTokenId, darAssetId, darTicker, tokenBlockchainBase, tokenContractAddress, rowCount);

                    processedRowCount++;
                }
                catch (Exception ex)
                {
                    Logger.Fatal($"TokenUpload: Failed to load row {processedRowCount}. {ex.Message}");
                    sbError.AppendLine(ex.Message);
                }
                finally
                {
                    rowCount++;
                    if (rowCount % 100 == 0)
                        Logger.Info($"TokenUpload: Loading {rowCount} of {sheet.Rows.Count()}");
                }
            }

            string summaryMessage = $"TokenUpload: {processedRowCount} of {sheet.Rows.Count()} rows loaded from file {fileName}";
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
            var a = (TokenViewModel)i;

            //Delete(a);
            Add(a);
            return true;
        }

        public override bool IdExists(string nextId)
        {
            string sql = $@"
                            select DARTokenID
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Token
                            where DARTokenID = '{nextId}'
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
            return GetNextId("DT", 5, 1000);
        }

        public override long Add(DARViewModel i)
        {
            throw new NotImplementedException();
        }

        public string TokenPublish(TokenViewModel l)
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
                    producer.Produce("token", new Message<Null, string> { Value = jsondata });
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