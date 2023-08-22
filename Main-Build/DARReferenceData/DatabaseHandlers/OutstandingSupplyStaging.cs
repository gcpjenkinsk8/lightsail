using Dapper;
using DARReferenceData.ViewModels;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.DatabaseHandlers
{
    public class OutstandingSupplyStaging : RefDataHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);

        public override long Add(DARViewModel i)
        {
            var a = (OutstandingSupplyViewModel)i;

            DateTime updateTime = DateTime.Now.ToUniversalTime();

            if (string.IsNullOrWhiteSpace(a.darAssetID))
                throw new Exception("Can't add outstanding supply to staging table. Asset id is 0 and  DARAssetID is empty");
            Asset ah = new Asset();
            var o = ah.Get(a.darAssetID) as AssetViewModel;
            if (o == null)
            {
                throw new Exception($"Can't add outstanding supply to staging table. Invalid asset {a.darAssetID}");
            }

            if (string.IsNullOrWhiteSpace(a.DARSourceID))
            {
                if (string.IsNullOrWhiteSpace(a.Source))
                    throw new Exception("Can't add outstanding supply to staging table. Exchange id is 0 and  Exchange is empty");

                Exchange eh = new Exchange();
                var src = eh.Get(a.Source) as ExchangeViewModel;
                if (src == null)
                {
                    throw new Exception($"Can't add outstanding supply to staging table. Invalid exchange {a.Source}");
                }
                else
                {
                    a.DARSourceID = src.DARExchangeID;
                }
            }

            /*            string sql = $@"
                                    INSERT INTO {DARApplicationInfo.SingleStoreCatalogInternal}.[Staging_OutstandingSupply]
                                               ([SourceID]
                                               ,[AssetID]
                                               ,[ProcessID]
                                               ,[OutstandingSupply]
                                               ,[Error]
                                               ,[CollectedTimeStamp])
                                         VALUES
                                               (@SourceID
                                               ,@AssetID
                                               ,@ProcessID
                                               ,@OutstandingSupply
                                               ,@Error
                                               ,@CollectedTimeStamp)
                                    SELECT CAST(SCOPE_IDENTITY() as int)
                                                                   ";
            */

            a.Reviewed = 0;
            string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLStaging_OutstandingSupply(
                    'UPSERT'
                    , @SourceID
                    , @DARAssetID
                    , @ProcessID
                    , @OutstandingSupplyQuantity
                    , @Error
                    , @PassedValidation
                    , @Reviewed
                    , @OustandingSupplyReviewed
                    , @BaseDataAvailable
                    )";

            var p = new DynamicParameters();

            foreach (var prop in a.GetType().GetProperties())
            {
                object value = prop.GetValue(a, new object[] { });
                p.Add($"@{prop.Name}", value);
            }

            string output;

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var result = (IDictionary<string, object>)connection.Query<object>(query, p).FirstOrDefault();
                output = result.First().Value.ToString();
            }
            if (output.Equals("Insert", StringComparison.OrdinalIgnoreCase))
            {
                return 1;
            }
            else
            {
                throw new Exception(output);
            }
        }

        public override bool Delete(DARViewModel i)
        {
            var a = (OutstandingSupplyViewModel)i;

            // TO DO: assume validation flag is set to true on update?
            string query = $@"CALL {DARApplicationInfo.SingleStoreCatalogInternal}.spDMLStaging_OutstandingSupply(
                    'DELETE'
                    , @SourceID
                    , @DARAssetID
                    , @ProcessID
                    , @OutstandingSupplyQuantity
                    , @Error
                    , @PassedValidation
                    , @Reviewed
                    , @OustandingSupplyReviewed
                    , @BaseDataAvailable
                    )";

            var p = new DynamicParameters();

            foreach (var prop in a.GetType().GetProperties())
            {
                object value = prop.GetValue(a, new object[] { });
                p.Add($"@{prop.Name}", value);
            }

            string output;

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var result = (IDictionary<string, object>)connection.Query<object>(query, p).FirstOrDefault();
                output = result.First().Value.ToString();
            }
            if (output.Equals("Delete", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else
            {
                throw new Exception(output);
            }
        }

        public IEnumerable<DARViewModel> GetSupplyByDate(string startDate)
        {
            if (string.IsNullOrWhiteSpace(startDate))
                startDate = DateTime.Today.ToShortDateString();

            DateTime inputDate;
            if (!DateTime.TryParse(startDate, out inputDate))
                inputDate = DateTime.Today;

            List<OutstandingSupplyViewModel> l = new List<OutstandingSupplyViewModel>();

            string sql = $@"
                            select
	                            o.SourceID
	                            ,o.ProcessID
	                            ,a.DARAssetID
	                            ,e.DARSourceId
	                            ,a.DARTicker
	                            ,e.ShortName as Source
                                ,o.OutstandingSupply as OutstandingSupplyQuantity
                                ,o.OutstandingSupplyReviewed as OutstandingSupplyReviewed
                                ,o.CollectedTimeStamp as CreateTime
                                ,o.Reviewed
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Staging_OutstandingSupply o
                            inner join (
                                            select darAssetID,max(CollectedTimeStamp) as CollectedTimeStamp
                                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Staging_OutstandingSupply
                                            where DATE(CollectedTimeStamp) =   '{inputDate.ToString("yyyy-MM-dd")}'
                                            group by darAssetId
                                        ) l on o.darAssetID = l.darAssetId and o.CollectedTimeStamp = l.CollectedTimeStamp
                            inner join {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on o.DARAssetId = a.darAssetId
                            inner join {DARApplicationInfo.SingleStoreCatalogInternal}.vSource e on o.SourceID = e.ID
                            where o.PassedValidation = 0

                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<OutstandingSupplyViewModel>(sql).ToList();
            }

            return l;
        }

        public override IEnumerable<DARViewModel> Get()
        {
            List<OutstandingSupplyViewModel> l = new List<OutstandingSupplyViewModel>();

            string sql = $@"select
                            o.SourceID
                            ,o.ProcessID
                            ,a.DARAssetID
                            ,e.DARSourceId
                            ,a.DARTicker
                            ,e.ShortName as Source
                            ,o.OutstandingSupply as OutstandingSupplyQuantity
                            ,o.OutstandingSupplyReviewed as OutstandingSupplyReviewed
                            ,o.CollectedTimeStamp as CreateTime
                            ,o.Reviewed
                            ,o.BaseDataAvailable
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Staging_OutstandingSupply o
                            inner join {DARApplicationInfo.SingleStoreCatalogInternal}.Asset a on o.DARAssetID = a.DARAssetID
                            inner join {DARApplicationInfo.SingleStoreCatalogInternal}.vSource e on o.SourceID = e.ID
                            and o.PassedValidation = 0
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<OutstandingSupplyViewModel>(sql).ToList();
            }

            return l;
        }

        public override DARViewModel Get(string key)
        {
            return Get().Cast<OutstandingSupplyViewModel>().Where(x => x.darAssetID.Equals(key)).FirstOrDefault();
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            throw new NotImplementedException();
        }

        public override bool Update(DARViewModel i)
        {
            var a = (OutstandingSupplyViewModel)i;

            if (a.OutstandingSupplyReviewed == null)
                return true;

            if (!a.CreateTime.ToShortDateString().Equals(DateTime.Today.ToUniversalTime().ToShortDateString()))
            {
                throw new Exception("You can override estimated circulating supply for today only");
            }

            a.Reviewed = 1;
            /*
                        string sql = $@"
                                update d
                                set
                                    OutstandingSupplyReviewed = @OutstandingSupplyReviewed
                                    ,[CollectedTimeStamp] = @CollectedTimeStamp
                                    ,Reviewed = @Reviewed
                                from [{CatalogName}].{SchemaName}.Staging_OutstandingSupply d
                                where AssetID in (select AssetId from [{CatalogName}].{SchemaName}.Staging_OutstandingSupply s where ID = @Id and IsActive = 1)
                                  and d.IsActive = 1";
            */

            string query = $@"UPDATE {DARApplicationInfo.SingleStoreCatalogInternal}.Staging_OutstandingSupply
                            SET OutstandingSupplyReviewed=@OutstandingSupplyReviewed
                            WHERE darAssetID=@DARAssetID AND ProcessID=@ProcessID AND SourceID=@SourceID AND CollectedTimeStamp=@CreateTime;";

            var p = new DynamicParameters();

            foreach (var prop in a.GetType().GetProperties())
            {
                object value = prop.GetValue(a, new object[] { });
                p.Add($"@{prop.Name}", value);
            }

            int updatedCount = 0;
            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                updatedCount = connection.Execute(query, p);
            }

            if (updatedCount == 0)
                return false;

            OutstandingSupply osDH = new OutstandingSupply();
            var os = osDH.Get(a.darAssetID) as OutstandingSupplyViewModel;
            if (os != null)
            {
                os.OutstandingSupply = (decimal)a.OutstandingSupplyReviewed;
                os.Reviewed = a.Reviewed;
                osDH.Update(os);
            }
            else
            {
                a.OutstandingSupply = (decimal)a.OutstandingSupplyReviewed;
                osDH.Add(a);
            }

            return true;
        }

        public bool PublishOutstandingSupply(string startDate)
        {
            if (string.IsNullOrWhiteSpace(startDate))
                startDate = DateTime.Today.ToShortDateString();

            try
            {
                /* TO DO: Delete Assets that are being published then insert

                string sql = $@"
                        DELETE [{CatalogName}].{SchemaName}.[OutstandingSupply]
                        where (  Convert(DateTime, DATEDIFF(DAY, 0, CollectedTimeStamp)) =   Convert(DateTime, DATEDIFF(DAY, 0,  @startDate)))

                        INSERT INTO [{CatalogName}].{SchemaName}.[OutstandingSupply]
                                   ([AssetID]
                                   ,[ProcessID]
                                   ,[OutstandingSupply]
                                   ,[CollectedTimeStamp])
                                select
	                                o.AssetID
	                                ,o.ProcessID
	                                ,avg(o.OutstandingSupply) as OutstandingSupplyQuantity
	                                ,GETUTCDATE()
                                from [{CatalogName}].{SchemaName}.[Staging_OutstandingSupply] o
                                inner join [{CatalogName}].{SchemaName}.[Asset] a on o.AssetID = a.ID
                                where  where (  Convert(DateTime, DATEDIFF(DAY, 0, CollectedTimeStamp)) =   Convert(DateTime, DATEDIFF(DAY, 0,  @startDate)))
                                group by o.AssetID, o.ProcessID
                                                       ";

                var p = new DynamicParameters();
                p.Add("@startDate", startDate);

                using (var connection = new SqlConnection(ApplicationInfo.ReferenceDataMasterDB))
                {
                    connection.Execute(sql,p);
                }
                */
            }
            catch (Exception ex)
            {
                Logger.Fatal("Failed to publish outstanding supply.", ex);
                return false;
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