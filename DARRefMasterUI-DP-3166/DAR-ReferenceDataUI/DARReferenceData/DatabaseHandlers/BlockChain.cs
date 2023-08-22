using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Web;
using Confluent.Kafka;
using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;

namespace DARReferenceData.DatabaseHandlers
{
    public class BlockChain : RefDataHandler
    {
        public BlockChain(string name)
        {
            Get(name);
        }

        public BlockChain()
        {
        }

        public BlockChainViewModel CurrentBlockChain { get; set; }


        public string Publish(BlockChainViewModel l)
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
                    producer.Produce("blockchain", new Message<Null, string> { Value = jsondata });
                    producer.Flush();
                }
                return "Message published without error";
            }
            catch (Exception ex)
            {
                return $"Failed to publish message {ex.Message}";
            }

        }
        public override long Add(DARViewModel i)
        {
            var a = (BlockChainViewModel)i;
            if (string.IsNullOrWhiteSpace(a.DARBlockchainID))
                a.DARBlockchainID = GetNextId();

            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.Operation = OPS_INSERT;
            a.Deleted = 0;
            Publish(a);
            
            return 0;
        }

        public override bool Delete(DARViewModel i)
        {
            var a = (BlockChainViewModel)i;
            a.Operation = OPS_DELETE;
            a.Deleted = 1;
            Publish(a);
            return true;
        }

        public override IEnumerable<DARViewModel> Get()
        {
            List<BlockChainViewModel> l = new List<BlockChainViewModel>();

            string sql = $@"
                             SELECT DARBlockchainID
                                  , Name
                                  , Description
                                  , ConsensusMechanism
                                  , HashAlgorithm
                                  , IsActive
                                  , CreateUser
                                  , LastEditUser
                                  , CreateTime
                                  , LastEditTime
                              FROM {DARApplicationInfo.SingleStoreCatalogInternal}.BlockChain
                              WHERE Deleted = 0
                          ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<BlockChainViewModel>(sql).ToList();
            }

            return l;
        }

        public override DARViewModel Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            CurrentBlockChain = Get().Cast<BlockChainViewModel>().Where(x => x.Name.ToUpper().Equals(key.ToUpper()) || x.DARBlockchainID.Equals(key)).FirstOrDefault();
            return CurrentBlockChain;
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            throw new NotImplementedException();
        }

        public override bool Update(DARViewModel i)
        {
            var a = (BlockChainViewModel)i;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            DateTime updateTime = DateTime.Now.ToUniversalTime();
            a.LastEditTime = updateTime;

            var old_record = Get(a.DARBlockchainID);

            Delete(old_record);
            Add(a);

            return true;
        }

        public override bool IdExists(string nextId)
        {
            string sql = $@"
                            select DARBlockchainID
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.BlockChain
                            where DARBlockchainID = '{nextId}'
                            
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
            return GetNextId("DBC", 10, 100);
        }


        public BTCBlockChainTxn get_btc_balockchain_txn(string[] asset_identifiers, string[] wallet_address, string blockchain, string window_start, string window_end,string caller_id, int batch_size)
        {
            List<string> supported_blockchains = new List<string>() { "BITCOIN" };

            if (!supported_blockchains.Contains(blockchain.ToUpper().Trim()))
            {
                throw new Exception($"{blockchain} is not supported at this time.");
            }

            

            BTCBlockChainTxn txn = new BTCBlockChainTxn();
            
            string asset_identifier_string = "*";
            if (!asset_identifiers.Contains("*"))
            {
                asset_identifier_string = (new Asset()).GetDARIdentifierPrice(asset_identifiers, caller_id);
                if (asset_identifiers.Contains("UNKNOWN"))
                    asset_identifier_string = asset_identifier_string + ",'UNKNOWN'";

                if (string.IsNullOrEmpty(asset_identifier_string))
                {
                    throw new Exception($"Invalid asset identifier '{String.Join("','", asset_identifiers)}'");
                }


            }

            string wallet_address_string = $"'{String.Join("','", wallet_address)}'";

            string sql_count = "select count(*) ";
            string sql_select = $@" 
                                   select TxnHash
                                          ,BlockHash
                                          ,BlockNumber
                                          ,blockTS as blockTimestamp
                                          ,BlockIndex
                                          ,SpentTxnHash
                                          ,FromAddress
                                          ,ToAddress
                                          ,CONVERT(Amount, char) as Amount
                                          ,CONVERT(Fee, char) as Fee
                                          ,'DAMFI9C' as DARAssetID
                                          ,'BTC' as DARTicker
                                      ";

            string sql_limit = $" limit {batch_size}";

            string sql_from = $@"
                            from {DARApplicationInfo.BlockChainDatabase}.btcTxn t
                            where blockTS >=  '{window_start}'
                              and blockTS <   '{window_end}'
                              and (FromAddress in (WALLET_ADDRESS) or ToAddress in (WALLET_ADDRESS))
                            order by blockTS
                          ";

            sql_from = sql_from.Replace("WALLET_ADDRESS", wallet_address_string);


        

            sql_count = sql_count + sql_from;
            sql_select = sql_select + sql_from + sql_limit;

            int total_rows = 0;
            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                total_rows = connection.Query<int>(sql_count).FirstOrDefault();
            }

            
            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                txn.Transactions = connection.Query<BTCBlockChainTxnViewModel>(sql_select).ToList();
            }

            if (txn.Transactions.Count == total_rows )
            {
                txn.Summary = $"{txn.Transactions.Count} of {total_rows} records returned for period between {window_start} to {window_end}.";
            }
            else if (txn.Transactions.Count < total_rows && total_rows < batch_size)
            {
                // DISTINCT in select query
                txn.Summary = $"{txn.Transactions.Count} of {txn.Transactions.Count} records returned for period between {window_start} to {window_end}.";
            }
            else
            {
                var lastTime = txn.Transactions.Select(x =>x.blockTimestamp).Last();
                txn.Summary = $"{txn.Transactions.Count} of {total_rows} records returned for period between {window_start} to {window_end}.Adjust your windowStart to {lastTime} to see the next batch of records.";
            }


            return txn;
        }

        public BTCBlockChainPos get_btc_balockchain_pos(string[] asset_identifiers, string[] wallet_address, string blockchain, string as_of_date, string caller_id, int batch_size)
        {
            List<string> supported_blockchains = new List<string>() { "BITCOIN" };

            if (!supported_blockchains.Contains(blockchain.ToUpper().Trim()))
            {
                throw new Exception($"{blockchain} is not supported at this time.");
            }

            BTCBlockChainPos positions = new BTCBlockChainPos();
            positions.Positions = new List<BTCBlockChainBalanceViewModel>();

            string asset_identifier_string = "*";
            if (!asset_identifiers.Contains("*"))
            {
                asset_identifier_string = (new Asset()).GetDARIdentifierPrice(asset_identifiers, caller_id);
                if (asset_identifiers.Contains("UNKNOWN"))
                    asset_identifier_string = asset_identifier_string + ",'UNKNOWN'";

                if(string.IsNullOrEmpty(asset_identifier_string))
                {
                    throw new Exception($"Invalid asset identifier '{String.Join("','", asset_identifiers)}'");
                }
            }

            
            string sql_select = string.Empty;
            foreach (string address in wallet_address)
            {
                sql_select = $@" 
                              select TxnHash
                                  ,BlockHash
                                  ,BlockNumber
                                  ,blockTS  as blockTimestamp
                                  ,t.address
                                  ,CONVERT(Balance, char) as Amount
                                  ,'DAMFI9C' as DARAssetID
                                  ,'BTC' as DARTicker
                                  ,'{as_of_date}' as PositionTimestamp
                                      ";
                string sql_limit = $" limit {batch_size}";

                string sql_from = $@"
                            from {DARApplicationInfo.BlockChainDatabase}.vBtcPos('WALLET_ADDRESS','{as_of_date}') t
                           ";




                sql_select = sql_select + sql_from + sql_limit;

                List<BTCBlockChainBalanceViewModel> w_position = new List<BTCBlockChainBalanceViewModel>();
                using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
                {
                    w_position = connection.Query<BTCBlockChainBalanceViewModel>(sql_select).ToList();
                }

                foreach (var w in w_position)
                {
                    positions.Positions.Add(w);
                }
            }


            



            

            int total_rows = positions.Positions.Count;



            

            foreach (var item in wallet_address)
            {
                if (positions.Positions.Where(x => x.Address.Equals(item)).Count() == 0)
                {
                    positions.Positions.Add(new BTCBlockChainBalanceViewModel() { Address = item, Amount = "0" });
                    total_rows += 1;
                }
            }

            if (positions.Positions.Count == total_rows)
            {
                positions.Summary = $"{positions.Positions.Count} of {total_rows} records returned for period {as_of_date}.";
            }
            else
            {
                positions.Summary = $"{positions.Positions.Count} of {total_rows} records returned for {as_of_date}. Reduce the number assets or wallet addresses in your input parameter to receive the full data set."; 
            }


            return positions;
        }
    }
}