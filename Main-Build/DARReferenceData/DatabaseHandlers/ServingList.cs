using Dapper;
using DARReferenceData.ViewModels;
using log4net;
using MySql.Data.MySqlClient;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace DARReferenceData.DatabaseHandlers
{
    internal enum PairOfferingsColumns
    {
        exchange = 0
        , exchangePairName = 1
        , assetTicker = 2
        , currencyTicker = 3
        , process_name = 4
        , assetDarId = 5
        , currencyDarId = 6
        , assetTier = 7
        , exchangeTier = 8
        , startTime = 9
        , endTime = 10
    }

    public class ServingList : RefDataHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);

        public override long Add(DARViewModel i)
        {
            var a = (ServingListViewModel)i;

            /*
                        string sql = $@"
                                INSERT INTO {DARApplicationInfo.SingleStoreCatalogInternal}.[ServingList]
                                           ([PairID]
                                           ,[SourceID]
                                           ,[ProcessID]
                                           ,[Start]
                                           ,[End]
                                           ,[IsActive]
                                           ,[CreateUser]
                                           ,[LastEditUser]
                                           ,[CreateTime]
                                           ,[LastEditTime])
                                     VALUES
                                           (@PairID
                                           ,@SourceID
                                           ,@ProcessID
                                           ,@Start
                                           ,@End
                                           ,@IsActive
                                           ,@CreateUser
                                           ,@LastEditUser
                                           ,@CreateTime
                                           ,@LastEditTime)

                                    SELECT CAST(SCOPE_IDENTITY() as int)
                                    ";
            */
            string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLServingList(
                    'UPSERT'
                    , @ProcessID
                    , @Start
                    , @ID
                    , @End
                    , @PairID
                    , @SourceID
                    , @IsActive
                    , @CreateUser
                    , @LastEditUser
                    )";

            var p = new DynamicParameters();

            p.Add("@PairID", a.PairID);
            p.Add("@SourceID", a.SourceID);
            p.Add("@ProcessID", a.ProcessID);
            p.Add("@Start", a.Start);
            p.Add("@End", a.End);
            p.Add("@IsActive", a.IsActive);
            p.Add("@CreateUser", string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name);
            p.Add("@LastEditUser", string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name);
            p.Add("@ID", a.ID);

            long iid = 0;

            //using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            //{
            //    var result = (IDictionary<string, object>)connection.Query<object>(query, p).FirstOrDefault();
            //    iid = (long)result.Values.FirstOrDefault();

            //    if (iid > 0) a.ID = iid;
            //}

            return iid; 
        }

        public bool Exists(ServingListViewModel sl)
        {
            if (!sl.IsValid())
                throw new Exception("Invalid serving list.");

            if (Get().Cast<ServingListViewModel>().Where(x => x.PairID == sl.PairID &&
                                                 x.SourceID == sl.SourceID &&
                                                 x.ProcessID == sl.ProcessID
                                                 ).FirstOrDefault() != null)
                return true;
            else
                return false;
        }

        public override bool Delete(DARViewModel i)
        {
            var a = (ServingListViewModel)i;
            DateTime updateTime = DateTime.Now.ToUniversalTime();
            /*
                        string sql = $@"DELETE [{CatalogName}].{SchemaName}.[ServingList]
                                       WHERE ID = @Id";
            */
            string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLServingList(
                    'DELETE'
                    , @ProcessID
                    , @Start
                    , @ID
                    , @End
                    , @PairID
                    , @SourceID
                    , @IsActive
                    , @CreateUser
                    , @LastEditUser
                    )";

            var p = new DynamicParameters();
            Type t = a.GetType();
            PropertyInfo[] props = t.GetProperties();

            foreach (var prop in props)
            {
                object value = prop.GetValue(a, new object[] { });
                p.Add($"@{prop.Name}", value);
            }

            long deleteId = 0;

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var result = (IDictionary<string, object>)connection.Query<object>(query, p).FirstOrDefault();
                deleteId = (long)result.Values.FirstOrDefault();
            }

            return true;
        }

        public override IEnumerable<DARViewModel> Get()
        {
            var l = new List<ServingListViewModel>();

            string sql = $@"
                        SELECT ID
                              , PairID
                              , SourceID
                              , ProcessID
                              , Start
                              , End
                              , IsActive
                              , CreateUser
                              , LastEditUser
                              , CreateTime
                              , LastEditTime
                          FROM {DARApplicationInfo.SingleStoreCatalogInternal}.ServingList
                    ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<ServingListViewModel>(sql).ToList();
            }

            return l;
        }

        public IEnumerable<DARViewModel> GetServingListView(string processName)
        {
            var l = new List<ServingListViewModel>();

            string sql = $@"
                          select *
	                        ,cast( start as nvarchar) as StartDisplay
	                        ,cast( end as nvarchar) as EndDisplay
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.vServingList
                            where ProcessName='{processName}'";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<ServingListViewModel>(sql).ToList();
            }

            return l;
        }

        public override DARViewModel Get(string key)
        {
            throw new NotImplementedException();
        }

        public static string RemoveUnwantedCharacters(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            return input.Replace("_", "").Replace("-", "").Replace(@"\", "").Replace(@"/", "").Replace(":", "").Replace(".", "").Replace(" ", "");
        }

        public static string RemoveQuoteAsset(string pair, string quoteAsset, int rowCount)
        {
            string result = pair.ToUpper().Replace(quoteAsset.ToUpper(), "");
            if ((result + quoteAsset).Length != pair.Length)
                result = pair.Substring(0, pair.Length - quoteAsset.Length);

            if ((result + quoteAsset).Length != pair.Length)
            {
                throw new Exception($"Can't determine other asset from pair {pair} using input asset {quoteAsset}. RowNumber:{rowCount}");
            }

            return result;
        }

        public static string GetAssetFromPair(string productId, string asset, AssetViewModel knownAssetInstance, int rowCount)
        {
            if (productId.Contains(asset))
                return RemoveUnwantedCharacters(RemoveQuoteAsset(productId, asset, rowCount));
            else if (productId.ToUpper().Contains(knownAssetInstance.DARTicker.ToUpper()))
            {
                return RemoveUnwantedCharacters(RemoveQuoteAsset(productId, knownAssetInstance.DARTicker, rowCount));
            }
            else if (productId.ToUpper().Contains(knownAssetInstance.Name.ToUpper()))
            {
                return RemoveUnwantedCharacters(RemoveQuoteAsset(productId, knownAssetInstance.Name, rowCount));
            }
            else
            {
                throw new Exception($"Can't determine other asset from pair {productId} using input asset {asset}. RowNumber:{rowCount}");
            }
        }

        private string LoadPair(string inputExchange, string productId, string asset, string currencyAsset, string processName, int rowCount, string assetDarId, string currencyDarId, string assetTier, string exchangeVettingStatus, string startTime, string endTime)
        {
            /*
                SHIBUSDT: SHIB => Asset USDT => CurrencyAsset
                LBC-BTC: LBC => Asset  BTC => CurrencyAsset
                skyeth:   SKY => Asset ETH => CurrencyAsset

                ExchangeTier: If value in

             */

            DateTime dtStart;
            DateTime dtEnd;

            if (!DateTime.TryParseExact(startTime, new string[] { "yyyy-MM-dd HH:mm tt" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dtStart))
            {
                dtStart = DateTime.Parse("01/01/1753");
            }

            if (!DateTime.TryParseExact(endTime, new string[] { "yyyy-MM-dd HH:mm tt" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dtEnd))
            {
                dtEnd = DateTime.MaxValue;
            }

            var exchange = new Exchange(inputExchange);
            if (exchange.CurrentExchange == null)
            {
                throw new Exception($"Exchange {inputExchange} does not exist. Please setup the exchange first. RowNumber:{rowCount} Product ID:{productId} ");
                /*
                Exchange e = new Exchange();
                ExchangeViewModel evm = new ExchangeViewModel();
                evm.ShortName = inputExchange;
                e.Add(evm);
                */
            }

            var aAsset = new Asset(assetDarId);

            if (aAsset.CurrentAsset == null)
            {
                throw new Exception($"Asset {assetDarId} does not exist. Please setup the asset first. RowNumber:{rowCount} Product ID:{productId} ");
            }

            var cAsset = new Asset(currencyDarId);

            if (cAsset.CurrentAsset == null)
            {
                if (string.IsNullOrWhiteSpace(currencyAsset))
                {
                    currencyAsset = GetAssetFromPair(productId, asset, aAsset.CurrentAsset, rowCount);
                }
            }

            PairViewModel p = (new Pair()).PairExists(aAsset.CurrentAsset.DARAssetID, cAsset.CurrentAsset.DARAssetID);
            if (p == null)
            {
                p = new PairViewModel();
                p.AssetID = aAsset.CurrentAsset.DARAssetID;
                p.QuoteAssetID = cAsset.CurrentAsset.DARAssetID;
                p.DARName = productId;
                p.ID = (new Pair()).AddPair(p);
            }

            ExchangePairViewModel ep = new ExchangePairViewModel();
            ep.DARExchangeId = exchange.CurrentExchange.DARExchangeID;
            ep.PairID = p.ID;
            ep.ExchangePairName = productId;
            if (!(new ExchangePair()).ExchangePairExists(ep.DARExchangeId, ep.ExchangePairName))
            {
                ep.ID = (new ExchangePair()).Add(ep);
            }

          


            if (!string.IsNullOrWhiteSpace(processName))
            {
                ProcessDar pd = new ProcessDar(processName);
                if (pd.CurrentProcess == null)
                {
                    throw new Exception($"Process {processName} does not exist");
                }

                ServingListViewModel sl = new ServingListViewModel();
                sl.PairID = p.ID;
                sl.SourceID = exchange.CurrentExchange.DARExchangeID;
                sl.ProcessID = pd.CurrentProcess.DARProcessID;
                sl.Start = dtStart;
                sl.End = dtEnd;
                sl.IsActive = true;

                if (!Exists(sl))
                    Add(sl);
                else
                {
                    throw new Exception($"Serving List Process:{pd.CurrentProcess.Name} AssetID:{sl.AssetID} PairID:{sl.PairID} SourceID:{sl.SourceID} exists already");
                }
            }

            return p.ID;
        }

        public bool SnapshotServingList(string processName)
        {
            try
            {
                string sql = $@"
                            declare @SnapshotVersion nvarchar(10)
                            select @SnapshotVersion = cast(coalesce(MAX(SnapshotVersion),0) + 1 as nvarchar) from {DARApplicationInfo.SingleStoreCatalogInternal}.ServingListSnapshot where ProcessName = @processName
                            update {DARApplicationInfo.SingleStoreCatalogInternal}.ServingListSnapshot set IsActive = 0 where ProcessName = @processName

                            INSERT INTO {DARApplicationInfo.SingleStoreCatalogInternal}.ServingListSnapshot
                                       ( SnapshotName
                                       , SnapshotVersion
                                       , ProcessName
                                       , ProcessId
                                       , PairName
                                       , PairId
                                       , AssetName
                                       , AssetId
                                       , Exchange
                                       , ExchangeId
                                       , ExchangePairName
                                       , AssetTicker
                                       , ExchangeVettingStatus
                                       , ExchangeVettingStatusCode
                                       , AssetTierDescription
                                       , AssetTierCode
                                       , Start
                                       , End
	                                   , AssetLegacyId
                                       , AssetLegacyDARAssetId
                                       , ExchangeLegacyId
                                       , IsActive
                                       , CreateUser
                                       , CreateTime
                                       , Lookback
                                       , LookbackUnit
                                       , Frequency
                                       , FrequencyUnit
                                                )
                            select
	                            ProcessName + '.' +  @SnapshotVersion + '.' + FORMAT([Start],'yyyy-MM-dd-HH:MM') + ' to  ' + FORMAT([End],'yyyy-MM-dd-HH:MM')   as SnapshotName
	                            ,@SnapshotVersion as SnapshotVersion
	                            ,ProcessName
	                            ,ProcessId
	                            ,DARName as PairName
	                            ,PairID
	                            ,AssetName
	                            ,AssetID
	                            ,Exchange
	                            ,ExchangeId
	                            ,ExchangePairName
	                            ,AssetTicker
	                            ,ExchangeVettingStatusDescription as  ExchangeVettingStatus
	                            ,ExchangeVettingStatus as ExchangeVettingStatusCode
	                            ,AssetTierDescription
	                            ,AssetTierCode
	                            , Start
	                            , End
	                            ,AssetLegacyId
                                ,AssetLegacyDARAssetId
                                ,ExchangeLegacyId
	                            ,1 as IsActive
	                            ,'tzaman'
	                            ,GETUTCDATE()
                                ,Lookback
                                ,LookbackUnit
                                ,Frequency
                                ,FrequencyUnit
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.vServingList
                            where ProcessName = @processName
                            ";

                var p = new DynamicParameters();
                p.Add("@processName", processName);

                using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
                {
                    connection.Execute(sql, p);
                }
            }
            catch (Exception ex)
            {
                Logger.Info($"Failed to take a snapshot of service list. Error:{ex.Message}");
                return false;
            }

            return true;
        }

        public bool DeleteStagedData(string processName)
        {
            try
            {
                string sql = $@"
                            delete l
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.ServingList l
                            inner join {DARApplicationInfo.SingleStoreCatalogInternal}.Process p on l.ProcessID = p.id
                            where p.Name = @processName
                            ";

                var p = new DynamicParameters();
                p.Add("@processName", processName);
                using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
                {
                    connection.Execute(sql, p);
                }
            }
            catch (Exception ex)
            {
                Logger.Info($"Failed to delete staged service list. Error:{ex.Message}");
                return false;
            }

            return true;
        }

        public IEnumerable<DARViewModel> GetServingList(string processName, DateTime? startDate = null, DateTime? endDate = null)
        {
            var l = new List<ServingListSnapshotViewModel>();

            var p = new DynamicParameters();
            p.Add("@processName", processName);

            if (startDate == null && endDate == null)
            {
                p.Add("@isActive", true);
                p.Add("@start", null);
                p.Add("@end", null);
            }
            else
            {
                p.Add("@isActive", null);

                p.Add("@start", ((DateTime)startDate).ToString("yyyy-MM-dd HH:mm"));
                p.Add("@end", ((DateTime)endDate).ToString("yyyy-MM-dd HH:mm"));
            }
            string sql = $@"
                        SELECT *
                        FROM {DARApplicationInfo.SingleStoreCatalogInternal}.ServingListSnapshot s
                        inner join (
		                        select ProcessName,max(SnapshotVersion) as SnapshotVerison
		                        from {DARApplicationInfo.SingleStoreCatalogInternal}.ServingListSnapshot
		                        where ProcessName = @processName
		                            and (IsActive=@isActive or (FORMAT([Start], 'yyyy-MM-dd HH:mm')= @start AND FORMAT(End, 'yyyy-MM-dd HH:mm')=@end))
		                        group by ProcessName
	                        ) l on s.ProcessName = l.ProcessName
	                          and s.SnapshotVersion = l.SnapshotVerison
                        where s.ProcessName = @processName
                        and (IsActive=@isActive or (FORMAT([Start], 'yyyy-MM-dd HH:mm')= @start AND FORMAT(End, 'yyyy-MM-dd HH:mm')=@end))
                    ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<ServingListSnapshotViewModel>(sql, p).ToList();
            }

            return l;
        }

        public IEnumerable<DARViewModel> GetServingListSnapshot(string snapshotName)
        {
            var l = new List<ServingListSnapshotViewModel>();

            string sql = $@"
                        SELECT *
                          FROM {DARApplicationInfo.SingleStoreCatalogInternal}.ServingListSnapshot
                        WHERE snapshotname = '{snapshotName}'
                    ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<ServingListSnapshotViewModel>(sql).ToList();
            }

            return l;
        }

        public IEnumerable<DARViewModel> GetSnapshotNames()
        {
            var l = new List<ServingListSnapshotViewModel>();

            string sql = $@"
                        select distinct snapshotname
                          FROM {DARApplicationInfo.SingleStoreCatalogInternal}.ServingListSnapshot
                        order by snapshotname desc

                    ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<ServingListSnapshotViewModel>(sql).ToList();
            }

            return l;
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            errors = string.Empty;
            Workbook workbook = new Workbook();
            workbook.LoadFromFile(fileName);
            Worksheet sheet = workbook.Worksheets[0];

            Logger.Info($"PairUpload: Loading Filename {fileName} RowCount:{sheet.Rows.Count()}");
            int rowCount = 0;
            string productId;
            string asset;
            string exchange;
            string currencyAsset;
            string processName = String.Empty;
            string assetDarId;
            string currencyDarId;
            string assetTier;
            string exchangeTier;
            string startTime;
            string endTime;

            int rowsLoaded = 0;
            int errorCount = 0;

            var processes = from items in sheet.Rows
                            group items by items.Columns[(int)PairOfferingsColumns.process_name].Value into q
                            select new { q.Key };
            if (processes.Count() > 2)
            {
                errors = "Only one serving list is allowed per upload file";
                return false;
            }

            var startDate = from items in sheet.Rows
                            group items by items.Columns[(int)PairOfferingsColumns.startTime].Value into q
                            select new { q.Key };
            if (startDate.Count() > 2)
            {
                errors = "Only one start time is allowed per upload file";
                return false;
            }
            var endDate = from items in sheet.Rows
                          group items by items.Columns[(int)PairOfferingsColumns.endTime].Value into q
                          select new { q.Key };
            if (endDate.Count() > 2)
            {
                errors = "Only one end time is allowed per upload file";
                return false;
            }

            if (sheet.Rows.Count() > 1)
            {
                processName = sheet.Rows[1].Columns[(int)PairOfferingsColumns.process_name].Value;
                if (!DeleteStagedData(processName))
                {
                    errors = "Failed to delete staged data from previous run. Terminated data serving list upload";
                    return false;
                }
            }

            foreach (var row in sheet.Rows)
            {
                try
                {
                    if (rowCount == 0)
                    {
                        rowCount++;
                        continue;
                    }
                    exchange = row.Columns[(int)PairOfferingsColumns.exchange].Value;
                    productId = row.Columns[(int)PairOfferingsColumns.exchangePairName].Value;
                    asset = row.Columns[(int)PairOfferingsColumns.assetTicker].Value;
                    currencyAsset = row.Columns[(int)PairOfferingsColumns.currencyTicker].Value;
                    processName = row.Columns[(int)PairOfferingsColumns.process_name].Value;

                    assetDarId = row.Columns[(int)PairOfferingsColumns.assetDarId].Value;
                    currencyDarId = row.Columns[(int)PairOfferingsColumns.currencyDarId].Value;
                    assetTier = row.Columns[(int)PairOfferingsColumns.assetTier].Value;
                    exchangeTier = row.Columns[(int)PairOfferingsColumns.exchangeTier].Value;
                    startTime = row.Columns[(int)PairOfferingsColumns.startTime].Value;
                    endTime = row.Columns[(int)PairOfferingsColumns.endTime].Value;

                    LoadPair(exchange, productId, asset, currencyAsset, processName, rowCount, assetDarId, currencyDarId, assetTier, exchangeTier, startTime, endTime);
                    rowsLoaded++;
                }
                catch (Exception ex)
                {
                    Logger.Error($"PairUpload: {ex.Message}");
                    errorCount++;
                }
                finally
                {
                    rowCount++;
                    if (rowCount % 100 == 0)
                        Logger.Info($"PairUpload: Processed {rowCount} of {sheet.Rows.Count()} ");
                }
            }

            if (!string.IsNullOrWhiteSpace(processName))
            {
                if (!SnapshotServingList(processName))
                {
                    errors = $"Skipped loading this file. Failed to snapshot servic list for process {processName}";
                    return false;
                }
            }

            Logger.Info($"PairUpload: File row count: {sheet.Rows.Count()} Loaded:{rowsLoaded} rows. Failed:{errorCount} rows");
            errors = $"Total {errorCount} errors found during upload.";

            if (errorCount > 0)
                return false;
            return true;
        }

        public override bool Update(DARViewModel i)
        {
            var a = (ServingListViewModel)i;
            DateTime updateTime = DateTime.Now.ToUniversalTime();
            /*
                        string sql = $@"
                                    UPDATE [dbo].[ServingList]
                                       SET [AssetID] = @AssetID
                                          ,[PairID] = @PairID
                                          ,[SourceID] = @SourceID
                                          ,[ProcessID] = @ProcessID
                                          ,[Start] = @Start
                                          ,[End] = @End
                                          ,[IsActive] = @IsActive
                                          ,[LastEditUser] = @LastEditUser
                                          ,[LastEditTime] = @LastEditTime
                                    WHERE ID = @Id";
            */
            string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLServingList(
                    'UPSERT'
                    , @ProcessID
                    , @Start
                    , @ID
                    , @End
                    , @PairID
                    , @SourceID
                    , @IsActive
                    , @CreateUser
                    , @LastEditUser
                    )";

            var p = new DynamicParameters();
            Type t = a.GetType();
            PropertyInfo[] props = t.GetProperties();

            foreach (var prop in props)
            {
                object value = prop.GetValue(a, new object[] { });
                p.Add($"@{prop.Name}", value);
            }

            p.Add("@LastEditUser", string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name);

            long recordId = 0;

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var result = (IDictionary<string, object>)connection.Query<object>(query, p).FirstOrDefault();
                recordId = (long)result.Values.FirstOrDefault();
            }

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
    }
}