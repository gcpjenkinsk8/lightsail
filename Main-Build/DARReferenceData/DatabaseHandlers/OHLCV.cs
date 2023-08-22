using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.DatabaseHandlers
{
    public class OHLCV : RefDataHandler
    {
        public override long Add(DARViewModel i)
        {
            throw new NotImplementedException();
        }

        public override bool Delete(DARViewModel i)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<DARViewModel> Get()
        {
            throw new NotImplementedException();
        }

        public override DARViewModel Get(string key)
        {
            throw new NotImplementedException();
        }

        public override string GetNextId()
        {
            throw new NotImplementedException();
        }

        public override bool IdExists(string nextId)
        {
            throw new NotImplementedException();
        }

        public override bool LoadDataFromExcelFile(string fileName, out string errors)
        {
            throw new NotImplementedException();
        }

        public override bool Update(DARViewModel i)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OHLCVViewModel> GetHighLowPrice(string clientId, string[] assetIdentifiers, string windowStart, string windowEnd)
        {
            Client c = new Client();
            var hasAccess = c.HasAccessToOHLCV(clientId);
            if (hasAccess)
            {
                List<OHLCVViewModel> result = new List<OHLCVViewModel>();

                // Convert the list to a comma-separated string with single quotes around each element
                string assetIdentifiersString = string.Join(",", assetIdentifiers.Select(identifier => $"'{identifier}'"));

                // Wrap the formatted string in parentheses
                string formattedAssetIdentifiers = $"({assetIdentifiersString})";

                // CALL SQL Here to get the data from the databse
                string sql = $@"
                        select priceIdentifier, darAssetID, darTicker, tier, open, high, low, close, volume, windowStart, windowEnd, effectiveTime  from {DARApplicationInfo.CalcPriceDatabase}.OHLCV WHERE
                            (darAssetID IN {formattedAssetIdentifiers}
                            OR darTicker IN {formattedAssetIdentifiers})
                            AND windowStart >= '{windowStart}' AND windowEnd <= '{windowEnd}'
                            ORDER BY windowStart;
                        ";

                using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
                {
                    result = connection.Query<OHLCVViewModel>(sql).ToList();
                }

                return result;
            }
            else
            {
                throw new Exception($"Client does not have access to OHLCV");
            }
        }

    }
}
