using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DARReferenceData.ViewModels;
using MySql.Data.MySqlClient;

namespace DARReferenceData.DatabaseHandlers
{
    public class VerifiableVolume : RefDataHandler
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

        public IEnumerable<VerifiableVolumeViewModel> GetVerifiableVolume(string[] assetIdentifiers, string windowStart, string windowEnd, string clientId)
        {
            Client c = new Client();
            var hasAccess = c.HasAccessToProduct(clientId, "1hVerifiableVolume");

            if (hasAccess)
            {
                List<VerifiableVolumeViewModel> result = new List<VerifiableVolumeViewModel>();

                string identifiers = (new Asset()).GetDARIdentifierPrice(assetIdentifiers, clientId);
                // Remove single quotes and split by comma
                string[] values = identifiers.Replace("'", "").Split(',');

                // Convert the array into a collection (e.g., List)
                List<string> assetIdentifiersList = new List<string>(values);

                // Check if client has permissions to the searched assets
                List<string> permissionedAssetIdentifiers = new List<string>();
                var hasFullAccess = c.HasFullAccess(clientId, "1hVerifiableVolume");
                if (hasFullAccess)
                {
                    permissionedAssetIdentifiers = values.ToList();
                }
                else
                {
                    foreach (string asset in assetIdentifiersList)
                    {
                        if (c.HasAccessToAsset(clientId, "1hVerifiableVolume", asset))
                        {
                            permissionedAssetIdentifiers.Add(asset);
                        }
                    }
                }

                if (permissionedAssetIdentifiers.Count != 0)
                {
                    // Convert the list to a comma-separated string with single quotes around each element
                    string assetIdentifiersString = string.Join(",", permissionedAssetIdentifiers.Select(identifier => $"'{identifier}'"));

                    // Wrap the formatted string in parentheses
                    string formattedAssetIdentifiers = $"({assetIdentifiersString})";

                    string sql = $@"
                                  select 
                                          darAssetID
                                          ,darAssetTicker
                                          ,verifiableVolume
                                          ,REPLACE(DATE_FORMAT(windowStart, '%Y-%m-%dT%TZ'),'Z','+00:00') as windowStart
                                          ,REPLACE(DATE_FORMAT(windowEnd, '%Y-%m-%dT%TZ'),'Z','+00:00') as windowEnd
                                          ,REPLACE(DATE_FORMAT(effectiveTime, '%Y-%m-%dT%TZ'),'Z','+00:00') as effectiveTime
                                  from {DARApplicationInfo.CalcPriceDatabase}.vVerifiableVolume
                                  where (darAssetID in {formattedAssetIdentifiers}
                                    or darAssetTicker in {formattedAssetIdentifiers})
                                    and effectiveTime >= '{windowStart}'
                                    and effectiveTime <= '{windowEnd}'
                                    ORDER BY effectiveTime;
                                ";

                    using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
                    {
                        result = connection.Query<VerifiableVolumeViewModel>(sql).ToList();
                    }

                    List<string> upperCaseIdentifiers = assetIdentifiers.Select(identifier => identifier.ToUpper()).ToList();

                    List<string> missingIdentifiers = upperCaseIdentifiers
                    .Except(result.Select(item => (string)item.darAssetTicker))
                    .Except(result.Select(item => (string)item.darAssetID))
                    .ToList();

                    string missingIdentifiersString = string.Join(",", missingIdentifiers.Select(identifier => $"'{identifier}'"));

                    if (missingIdentifiers.Count() > 0)
                    {
                        var i = new VerifiableVolumeViewModel()
                        {
                            error = $"We were unable to return Verifiable Volume information for the following assetIdentifier(s): {missingIdentifiersString}. This can occur if the data is unavailable or if you are not permissioned for this asset. Please contact support@digitalassetresearch.com for assistance."
                        };
                        result.Add(i);
                    }
                    return result;
                }
                else
                {
                    throw new Exception($"Client does not have access to the searched assets. Please contact DAR support team at support@digitalassetresearch.com.");
                }

            }
            else
            {
                throw new Exception($"You have not subscribed to Verifiable Volume data. Please contact DAR support team at support@digitalassetresearch.com.");
            }
        }
    }
}
