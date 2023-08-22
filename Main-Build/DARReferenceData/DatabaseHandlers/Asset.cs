using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using CsvHelper;
using CsvHelper.Configuration;
using Dapper;
using DARReferenceData.DatabaseHandlers.Validators;
using DARReferenceData.ViewModels;
using FluentValidation;
using FluentValidation.Results;
using log4net;
using MySql.Data.MySqlClient;
using Spire.Xls;
using System.Text;
using System.Text.Json;

using System.ComponentModel;

using System.Runtime.Caching;
using CsvHelper.TypeConversion;
using Confluent.Kafka;
using log4net.Repository.Hierarchy;
using System.Windows.Interop;
using System.Net;
using CsvHelper.Configuration.Attributes;
using static DARReferenceData.ViewModels.AssetViewModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.Collections;
using System.Threading;

namespace DARReferenceData.DatabaseHandlers
{
    public class Asset : RefDataHandler
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Environment.MachineName);
        private ObjectCache cache = MemoryCache.Default;

        private enum AssetVettingStatusUploadColumns
        {
            ProcessName = 0
             , Asset = 1
             , AssetTier = 2
             , IndexStatus = 3
        }

        public AssetViewModel CurrentAsset { get; set; }

        private enum AssetUploadColumns
        {
            DARAssetID = 0
            , DARTicker = 1
            , Name = 2
            , Description = 3
            , Sponsor = 4
            , DevelopmentStage = 5
            , CodeRepositoryURL = 6
            , PrimaryURL = 7
            , AssetType = 8
            , Twitter = 9
            , Reddit = 10
            , Blog = 11
            , WhitePaper = 12
            , MaxSupply = 13
            , MessariTaxonomySector = 14
            , MessariTaxonomyCategory = 15
            , CoinGeckoURL = 16
            , CoinMarketCapURL = 17
            , InstitutionalCustodyAvailable = 18
            , Custodians = 19
            , DARSuperSector = 20
            , DARSuperSectorCode = 21
            , DARSector = 22
            , DARSectorCode = 23
            , DARSubSector = 24
            , DARSubsectorCode = 25
            , DARTaxonomyVersion = 26
            , DATSSuperSector = 27
            , DATSSuperSectorCode = 28
            , DATSSector = 29
            , DATSSectorCode = 30
            , DATSSubSector = 31
            , DATSSubsectorCode = 32
            , DATSTaxonomyVersion = 33
            , DARThemes = 34
            , DATSThemes = 35
            , HasERC20Version = 36
            , HasNYDFSCustody = 37
            , GovernanceToken = 38
            , LegacyOne = 39
            , DATSGovernance = 40
            , DATSLayer1 = 41
            , isoCurrencyCode = 42
        }

        public Asset(string asset)
        {
            if (!string.IsNullOrEmpty(asset))
            {
                Get(asset);
            }
        }

        public Asset()
        {
        }

        public override bool IdExists(string nextId)
        {
            string sql = $@"
                            select DARAssetID
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Asset
                            where DARAssetID = '{nextId}'
                            union
                            select DARAssetID
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Asset
                            where LegacyDARAssetId = '{nextId}'
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                var r = connection.Query<string>(sql);

                if (r != null && r.Any())
                    return true;
            }

            return false;
        }

        public long GetMaxIDFromAsset()
        {
            string sql = $@"
                            select max(ID) + 1
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Asset
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var r = connection.Query<long>(sql);

                return r.FirstOrDefault();
            }
        }

        public long GetMaxIDFromAssetAudit()
        {
            string sql = $@"
                            select max(ChangeID) + 1
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Asset_Audit
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var r = connection.Query<long>(sql);

                return r.FirstOrDefault();
            }
        }

        public long GetLegacyIDFromDB()
        {
            string sql = $@"
                            select max(legacyID) + 1
                            from {DARApplicationInfo.SingleStoreCatalogPublic}.token2
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                var r = connection.Query<long>(sql);

                return r.FirstOrDefault();
            }
        }

        public static List<DropDownItem> GetAssetList()
        {
            var l = new List<DropDownItem>();

            string sql = $@"
                            select DARAssetID as Id,Name
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Asset
                            where Name is not null
                              and LTRIM(RTRIM(Name)) != ''

                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<DropDownItem>(sql).ToList();
                l.Sort((x, y) =>
                {
                    return x.Name.CompareTo(y.Name);
                });
            }

            return l;
        }


        public ClientAccessViewModel GetClientAccess(string clientName)
        {
            var l = new List<ClientAccessViewModel>();

            string sql = $@"
                            select 
                              distinct ClientName
                              ,case when HasFullAccess = 1 then 'True' else 'False' end as HasAccessToAllPricedAssets
                              ,case when HourlyPrice = 1 then 'True' else 'False' end as HasAccessToHourlyPrice
                              ,case when LatestPrice = 1 then 'True' else 'False' end as hasAccessToLatestPrice
                              ,case when Derivatives = 1 then 'True' else 'False' end as hasAccessToDerivativePrices
                              ,'False' as HasAccessToLPTokenPrices
                              ,case when NFT = 1 then 'True' else 'False' end as HasAccessToNFTPrices
                              ,DARAssetID,AssetName,DARTicker
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.vClientAssetsClientID
                            where ClientName = '{clientName}'
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<ClientAccessViewModel>(sql).ToList();
            }

            ClientAccessViewModel result = new ClientAccessViewModel();

            if (l.Any())
            {
                result.ClientName = l.ElementAtOrDefault(0).ClientName;
                result.HasAccessToAllPricedAssets = l.ElementAtOrDefault(0).HasAccessToAllPricedAssets;
                result.HasAccessToHourlyPrice = l.ElementAtOrDefault(0).HasAccessToHourlyPrice;
                result.HasAccessToLatestPrice = l.ElementAtOrDefault(0).HasAccessToLatestPrice;
                result.HasAccessToDerivativePrices = l.ElementAtOrDefault(0).HasAccessToDerivativePrices;
                result.HasAccessToLPTokenPrices = l.ElementAtOrDefault(0).HasAccessToLPTokenPrices;
                result.HasAccessToNFTPrices = l.ElementAtOrDefault(0).HasAccessToNFTPrices;


                if (result.HasAccessToAllPricedAssets.Equals("True"))
                {
                    var allAssets = GetAssetView().Cast<AssetViewModel>().ToList();
                    result.Assets = allAssets.Select(f => new ShortAsset() { DARAssetID = f.DARAssetID, Name = f.Name, Ticker = f.DARTicker }).Cast<ShortAsset>();
                }
                else
                {
                    result.Assets = l.Select(f => new ShortAsset() { DARAssetID = f.DARAssetID, Name = f.AssetName, Ticker = f.DARTicker }).Cast<ShortAsset>();
                }
                
            }
            return result;
        }


        public static int GetCount()
        {
            string sql = $@"
                           SELECT count(*)
                            FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Asset";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                var result = connection.Query<int>(sql);
                return result.FirstOrDefault();
            }
        }

        public List<string> LookupAssetIdFromDB_obsolete(string inputId)
        {
            // Console.WriteLine($"Lookup {inputId} from DB");
            List<string> l;
            string sql = $@"
                            select DARAssetid
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Asset
                            where DARAssetId = '{inputId}'
                            union
                            select LegacyDarAssetId
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Asset
                            where DARAssetId  = '{inputId}'
                              and LegacyDarAssetId is not null
                            union
                            select LegacyDarAssetId
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Asset
                            where LegacyDarAssetId = '{inputId}'
                              and LegacyDarAssetId is not null
                            union
                            select DARAssetId
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Asset
                            where LegacyDarAssetId  = '{inputId}'
                            union
                            select DARAssetId
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Asset
                            where DARTicker  = '{inputId}'
                            union
                            select LegacyDarAssetId
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Asset
                            where DARTicker  = '{inputId}'
                              and LegacyDarAssetId is not null
                            union
                            select DARAssetID
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.vAssetToken
                            where concat(TokenContractAddress,'+',BlockChain)  = '{inputId}'
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<string>(sql).ToList();
            }

            if (l == null || l.Count() == 0)
            {
                l = new List<string>() { "Not available" };
            }
            return l;
        }

        public List<string> LookupAssetIdFromDB(string inputId)
        {
            // Console.WriteLine($"Lookup {inputId} from DB");
            List<string> l;
            string sql = $@"
                            select DARAssetID from {DARApplicationInfo.SingleStoreCatalogInternal}.AssetIdMap where InputId ='{inputId}'
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<string>(sql).ToList();
            }

            if (l == null || l.Count() == 0)
            {
                return null;
            }
            return l;
        }

        public IEnumerable<string> GetAllAssetIds(string[] assetIdentifiers)
        {
            List<string> l = new List<string>();

            foreach (string assetIdentifier in assetIdentifiers)
            {
                IEnumerable<string> mappedIDs = cache[assetIdentifier] as List<string>;

                if (mappedIDs == null)
                {
                    CacheItemPolicy policy = new CacheItemPolicy();
                    policy.AbsoluteExpiration = new DateTimeOffset(DateTime.Now.AddMinutes(1440));
                    mappedIDs = LookupAssetIdFromDB(assetIdentifier);
                    if(mappedIDs != null)
                        cache.Set(assetIdentifier, mappedIDs, policy);
                }

                if (mappedIDs != null)
                {
                    foreach (string mappedId in mappedIDs)
                    {
                        l.Add(mappedId);
                    }
                }
                else
                {
                    l.Add(assetIdentifier);
                }
            }

            return l;
        }

        public IEnumerable<string> GetAllAssetIdsOld(string[] assetIdentifiers)
        {
            string identifiers = $"'{string.Join("','", assetIdentifiers)}'";

            IEnumerable<string> l;

            string sql = $@"
                            select DARAssetid
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Asset
                            where DARAssetId in (ASSET_LIST)
                            union
                            select LegacyDarAssetId
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Asset
                            where DARAssetId in (ASSET_LIST)
                              and LegacyDarAssetId is not null
                            union
                            select LegacyDarAssetId
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Asset
                            where LegacyDarAssetId in (ASSET_LIST)
                              and LegacyDarAssetId is not null
                            union
                            select DARAssetId
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Asset
                            where LegacyDarAssetId in (ASSET_LIST)
                            union
                            select DARAssetId
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Asset
                            where DARTicker in (ASSET_LIST)
                            union
                            select LegacyDarAssetId
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Asset
                            where DARTicker in (ASSET_LIST)
                              and LegacyDarAssetId is not null
                            union
                            select DARAssetID
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.vAssetToken
                            where concat(TokenContractAddress,'+',BlockChain) in (ASSET_LIST)
                            ";

            sql = sql.Replace("ASSET_LIST", identifiers);

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<string>(sql);
            }

            return l;
        }

        public string GetDARIdentifierPrice(string[] assetIdentifiers, string clientID)
        {
            bool clientHasFullAccess = (new ClientAsset()).HasFullAccess(clientID);
            string result = String.Empty;

            IEnumerable<string> l = GetAllAssetIds(assetIdentifiers);

            if (l != null && l.Any())
            {
                if (!clientHasFullAccess)
                {
                    var authorizedAssets = (new ClientAsset()).GetAuthorizedAssetsPrice(clientID, l.ToArray()).Cast<ClientAssetsViewModel>().ToList().Select(x => x.DARAssetID).ToArray();
                    if (authorizedAssets != null && authorizedAssets.Any())
                    {
                        result = $"'{String.Join("','", authorizedAssets)}'";
                    }
                }
                else
                {
                    result = $"'{String.Join("','", l)}'";
                }
            }

            return result;
        }

        public string GetDARIdentifierReference(string[] assetIdentifiers, string clientID)
        {
            bool clientHasFullAccess = (new ClientAsset()).HasFullAccess(clientID);
            string result = String.Empty;

            IEnumerable<string> l = GetAllAssetIds(assetIdentifiers);

            if (l != null && l.Any())
            {
                if (!clientHasFullAccess)
                {
                    var authorizedAssets = (new ClientAsset()).GetAuthorizedAssetsReference(clientID, l.ToArray()).Cast<ClientAssetsViewModel>().ToList().Select(x => x.DARAssetID).ToArray();
                    if (authorizedAssets != null && authorizedAssets.Any())
                    {
                        result = $"'{String.Join("','", authorizedAssets)}'";
                    }
                }
                else
                {
                    result = $"'{String.Join("','", l)}'";
                }
            }

            return result;
        }

        public void ReplicateAsset(DARViewModel asset)
        {
            if (!DARApplicationInfo.CurrentEnvironment.Equals("PROD"))
                return;

            if (asset == null)
                throw new ArgumentNullException($"ERROR: Invalid asset.");

            var a = (AssetViewModel)asset;
            string error = String.Empty;

            if (asset != null)
            {
                int retryCount = 0;
                int maxRetry = 3;

                var par = new DynamicParameters();
                par.Add("@DARTicker", a.DARTicker.ToLower());
                par.Add("@Name", a.Name);
                par.Add("@DARAssetID", a.DARAssetID);

                while (retryCount < maxRetry)
                {
                    try
                    {
                        using (var connection = new MySqlConnection(DARApplicationInfo.ReferenceDB))
                        {
                            string sql = $@"CALL {DARApplicationInfo.SingleStoreCatalogPublic}.sp_upsert_asset
                                (@DARTicker
                                ,@Name
                                ,0
                                ,0
                                ,3
                                ,0
                                ,@DARAssetID
                                )";

                            var x = connection.Query<int>(sql, par).Single();
                            if (x == -1)
                            {
                                error = $"Failed to replicate asset.Asset exists in public token table";
                                Logger.Error(error);
                                retryCount = maxRetry;
                                throw new Exception(error);
                            }
                        }
                        break;
                    }
                    catch (Exception ex)
                    {
                        error = $"Failed to replicate asset {a.DARAssetID}. error:{ex.Message} retry {retryCount} of {maxRetry}";
                        Logger.Error(ex);
                    }
                    finally
                    {
                        retryCount++;
                    }
                }

                if (!string.IsNullOrWhiteSpace(error) && retryCount > maxRetry - 1)
                    throw new Exception(error);
            }
            else
            {
                throw new Exception($"Invalid darAssetId {a.DARAssetID}");
            }
        }

        public AssetViewModel Kafka_message_create(AssetViewModel a)
        {
            AssetViewModel l = new AssetViewModel()
            {
                DARAssetID = a.DARAssetID,
                DARTicker = a.DARTicker,
                UniqueID = a.UniqueID,
                Name = a.Name,
                AssetType = a.AssetType,
                Description = a.Description,
                Sponsor = a.Sponsor,
                IsBenchmarkAsset = a.IsBenchmarkAsset,
                SEDOL = a.SEDOL,
                ISIN = a.ISIN,
                CUSIP = a.CUSIP,
                DTI = a.DTI,
                DevelopmentStage = a.DevelopmentStage,
                DARSuperSector = a.DARSuperSector,
                DARSuperSectorCode = a.DARSuperSectorCode,
                DARSector = a.DARSector,
                DARSectorCode = a.DARSectorCode,
                DARSubSector = a.DARSubSector,
                DARSubSectorCode = a.DARSubSectorCode,
                DarTaxonomyVersion = a.DarTaxonomyVersion,
                DATSSuperSector = a.DATSSuperSector,
                DATSSuperSectorCode = a.DATSSuperSectorCode,
                DATSSector = a.DATSSector,
                DATSGovernance = a.DATSGovernance,
                DATSLayer1 = a.DATSLayer1,
                DATSSectorCode = a.DATSSectorCode,
                DATSSubSector = a.DATSSubSector,
                DATSSubSectorCode = a.DATSSubSectorCode,
                DATSTaxonomyVersion = a.DATSTaxonomyVersion,
                IssuanceFramework = a.IssuanceFramework,
                IsRestricted = a.IsRestricted,
                CirculatingSupply = a.CirculatingSupply,
                MaxSupply = a.MaxSupply,
                MessariTaxonomySector = a.MessariTaxonomySector,
                MessariTaxonomyCategory = a.MessariTaxonomyCategory,
                InstitutionalCustodyAvailable = a.InstitutionalCustodyAvailable,
                DarTheme = a.DarTheme,
                PrimaryURL = a.PrimaryURL,
                Twitter = a.Twitter,
                Reddit = a.Reddit,
                Blog = a.Blog,
                WhitePaper = a.WhitePaper,
                CodeRepositoryURL = a.CodeRepositoryURL,
                CoinGeckoURL = a.CoinGeckoURL,
                CoinMarketCapURL = a.CoinMarketCapURL,
                LegacyDARAssetId = a.LegacyDARAssetId,
                LegacyId = a.LegacyId,
                Custodians = a.Custodians,
                DatsTheme = a.DatsTheme,
                CoinMarketCapId = a.CoinMarketCapId,
                HasERC20Version = a.HasERC20Version,
                HasNYDFSCustoday = a.HasNYDFSCustoday,
                CoinGeckoId = a.CoinGeckoId,
                GovernanceToken = a.GovernanceToken,
                LayerOne = a.LayerOne,
                IsoCurrencyCode = a.IsoCurrencyCode,
                CreateTime = a.CreateTime,
                CreateUser = a.CreateUser,
                LastEditTime = a.LastEditTime,
                LastEditUser = a.LastEditUser,

            };

        return l;
        }

        public AssetViewModel GetTheExistingRecord(string values, string operation)
        {
            List <AssetViewModel> r = new List<AssetViewModel>();
            string sql = $@"
                            select *
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.Asset
                            where {operation} = '{values}' and deleted = 0
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                r = connection.Query<AssetViewModel>(sql).ToList();

                return r.FirstOrDefault();
            }
        }

        public string AddAsset(DARViewModel i)
        {
            string error = String.Empty;
            var a = (AssetViewModel)i;

            // DateTime updateTime = DateTime.Now.ToUniversalTime();

            if (string.IsNullOrWhiteSpace(a.DARTicker.Trim()))
            {
                throw new Exception("Invalid Ticker");
            }

            if (string.IsNullOrWhiteSpace(a.Name.Trim()))
            {
                throw new Exception("Invalid Name");
            }

            if (!string.IsNullOrWhiteSpace(a.Description))
            {
                a.Description = a.Description.Trim();
            }

            // Punctuation in 'name' creates unhanled exception in non-refmaster processes
            a.Name = a.Name.Replace("'", String.Empty);
            a.DARAssetID = GetNextId();

            //Checking duplicate reocrds in the DB

            var duplicate_darassetid = GetTheExistingRecord(a.DARAssetID.Trim(), "DARAssetID");
            var duplicate_ticker = GetTheExistingRecord(a.DARTicker.Trim(), "DARTicker");
            var duplicate_name = GetTheExistingRecord(a.Name.Trim(), "Name");

            if (duplicate_darassetid != null)
            {
                error = "Assets cannot be created as Table already has a record with same DARAssetID";
                Logger.Error(error);
                throw new Exception(error);
            }

            if (duplicate_name != null)
            {
                error = "Assets cannot be created as Table already has a record with same Name";
                Logger.Error(error);
                throw new Exception(error);
            }

            if (duplicate_ticker != null)
            {
                error = "Assets cannot be created as Table already has a record with same DARTicker";
                Logger.Error(error);
                throw new Exception(error);
            }

            
            
            if (string.IsNullOrWhiteSpace(a.AssetType))
                a.AssetType = "Token";

            a.CreateUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;
            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            a.AssetType = a.AssetType.Replace(" ", "");

            if (a.CoinMarketCapId != null)
            {
                try
                {
                    long.Parse(a.CoinMarketCapId.Trim());
                }
                catch (Exception ex)
                {
                    error = "Assets can only have the numeric CMC ID.";
                    Logger.Error(error);
                    throw new Exception(error);
                }
                var cmc = GetAssetByCMC_ID(a.CoinMarketCapId);

                if (cmc != null)
                {
                    throw new Exception("Two Assets cannot have the same CMC ID.");
                }
            }

            if (a.CoinGeckoId != null)
            {
                var cg = GetAssetByCG_ID(a.CoinGeckoId);

                if (cg != null)
                {
                    throw new Exception("Two Assets cannot have the same CG ID.");
                }
            }
            if (a.CoinGeckoId != null && a.CoinMarketCapId != null && a.CoinMarketCapId == a.CoinGeckoId)
            {
                throw new Exception("Assets cannot have the same CG and CMC ID");
            }
            
            //Publishing in Asset Table
            a = Kafka_message_create(a);
            a.Operation = "INSERT";
            a.Deleted = 0;
            a.LegacyId = null;
            string publishStatus = AssetPublish(a);


            return a.DARAssetID;
        }

        public AssetViewModel GetAssetByCMC_ID(string id)
        {
            List<AssetViewModel> l = new List<AssetViewModel>();

            string sql = $@"select *
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.vAssetMaster
                            where CoinMarketCapId = '{id}'
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<AssetViewModel>(sql).ToList();
            }

            return l.FirstOrDefault();
        }

        public AssetViewModel GetAssetByCG_ID(string id)
        {
            List<AssetViewModel> l = new List<AssetViewModel>();

            string sql = $@"select *
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.vAssetMaster
                            where CoinGeckoId = '{id}'
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<AssetViewModel>(sql).ToList();
            }

            return l.FirstOrDefault();
        }

        public AssetViewModel GetAssetfromDB(string id)
        {
            List<AssetViewModel> l = new List<AssetViewModel>();

            string sql = $@"select *
                            from {DARApplicationInfo.SingleStoreCatalogInternal}.vAssetMaster
                            where DARAssetID = '{id}'
                ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<AssetViewModel>(sql).ToList();
            }

            return l.FirstOrDefault();
        }


        public bool UpdateAsset(DARViewModel i, string action)
        {
            string error = String.Empty;
            var a = (AssetViewModel)i;

            var duplicate_ticker = GetTheExistingRecord(a.DARTicker.Trim(), "DARTicker");
            var duplicate_name = GetTheExistingRecord(a.Name.Trim(), "Name");

            if (duplicate_name != null && !string.Equals(duplicate_name.DARAssetID, a.DARAssetID))
            {
                error = "Assets cannot be created as Table already has a record with same Name";
                Logger.Error(error);
                throw new Exception(error);
            }

            if (duplicate_ticker != null && !string.Equals(duplicate_ticker.DARAssetID, a.DARAssetID))
            {
                error = "Assets cannot be created as Table already has a record with same DARTicker";
                Logger.Error(error);
                throw new Exception(error);
            }


            if (string.IsNullOrWhiteSpace(a.DARTicker.Trim()))
            {
                throw new Exception("Invalid Ticker");
            }

            if (!string.IsNullOrWhiteSpace(a.Description))
            {
                a.Description = a.Description.Trim();
            }

            var x = GetAssetfromDB(a.DARAssetID);

            if (x != null)
            {
                if ((string.IsNullOrWhiteSpace(x.CoinMarketCapId) && !string.IsNullOrWhiteSpace(a.CoinMarketCapId)) || (!string.IsNullOrWhiteSpace(x.CoinMarketCapId) && !x.CoinMarketCapId.Equals(a.CoinMarketCapId)))
                {
                    if (!string.IsNullOrWhiteSpace(a.CoinMarketCapId))
                    {
                        long l;
                        if (!long.TryParse(a.CoinMarketCapId.Trim(), out l))
                        {
                            error = "Assets can only have the numeric CMC ID.";
                            Logger.Error(error);
                            throw new Exception(error);
                        }

                        var cmc = GetAssetByCMC_ID(a.CoinMarketCapId);

                        if (cmc != null)
                        {
                            throw new Exception("Two Assets cannot have the same CMC ID.");
                        }
                    }
                    else
                    {
                        if(!string.IsNullOrWhiteSpace(x.CoinMarketCapId) && !action.Equals("Manual"))
                        {
                            a.CoinMarketCapId= x.CoinMarketCapId;
                        }
                    }
                }
                if ((string.IsNullOrWhiteSpace(x.CoinGeckoId) && !string.IsNullOrWhiteSpace(a.CoinGeckoId)) || (!string.IsNullOrWhiteSpace(x.CoinGeckoId) && !x.CoinGeckoId.Equals(a.CoinGeckoId)))
                {
                    if (!string.IsNullOrWhiteSpace(a.CoinGeckoId))
                    {
                        var cg = GetAssetByCG_ID(a.CoinGeckoId);

                        if (cg != null)
                        {
                            throw new Exception("Two Assets cannot have the same CG ID");
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(x.CoinGeckoId) && !action.Equals("Manual"))
                        {
                            a.CoinGeckoId = x.CoinGeckoId;
                        }
                    }
                }
            }
            
            
            

            if (!string.IsNullOrWhiteSpace(a.CoinGeckoId) && !string.IsNullOrWhiteSpace(a.CoinMarketCapId) && a.CoinMarketCapId.Equals(a.CoinGeckoId))
            {
                throw new Exception("Assets cannot have the same CG and CMC ID");
            }


            if (a.MaxSupply == -1) a.MaxSupply = null;
            if (a.CirculatingSupply == -1) a.CirculatingSupply = null;

            a.AssetType = a.AssetType.Replace(" ", "");

            a.LastEditUser = string.IsNullOrWhiteSpace(HttpContext.Current.User.Identity.Name) ? Environment.UserName : HttpContext.Current.User.Identity.Name;

            a = Kafka_message_create(a);
            a.Operation = "UPDATE";
            a.Deleted = 0;
            string publishStatus = AssetPublish(a);

            
            return true;
        }

        public override bool Delete(DARViewModel vm)
        {
            var a = (AssetViewModel)vm;

            try
            {
                a = Kafka_message_create(a);
                a.Operation = "DELETE";
                a.Deleted = 1; 
                string publishStatus = AssetPublish(a);


                //a.AssetID = GetMaxIDFromAssetAudit().ToString();
                //string audit_publishstatus = AssetPublish_Audit(a);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<DARViewModel> GetAssets()
        {
            var l = new List<AssetViewModel>();

            string sql = $@"
                            SELECT
                                  DARAssetID
                                  ,DARTicker
                                  ,Name
                                 FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Asset";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<AssetViewModel>(sql).ToList();
            }

            return l;
        }

        public IEnumerable<DARViewModel> GetAssetView(string id = null)
        {
            var l = new List<AssetViewModel>();

            string sql = $@"
                        SELECT a.* , c.Custodians,a.DARAssetId as UniqueID
                        FROM {DARApplicationInfo.SingleStoreCatalogInternal}.vAssetMaster a
                        LEFT JOIN (SELECT  DARAssetID, GROUP_CONCAT(Custodian ORDER BY Custodian ASC SEPARATOR ',') as Custodians
                        FROM {DARApplicationInfo.SingleStoreCatalogInternal}.vAssetCustodian
                        GROUP BY DARAssetID) c on a.DARAssetID = c.DARAssetId";

            if (!string.IsNullOrWhiteSpace(id))
            {
                sql = string.Concat(sql, $@" WHERE (LTRIM(RTRIM(a.DARTicker))) = '{id}'
                                            OR LTRIM(RTRIM(a.Name)) = '{id}'
                                            OR LTRIM(RTRIM(a.DARAssetID)) = '{id}'
                                            OR LTRIM(RTRIM(COALESCE(a.LegacyDARAssetId, 'X'))) = '{id}'
                                            OR COALESCE(a.LegacyId, '-1') = '{id}';");
            }

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
            {
                l = connection.Query<AssetViewModel>(sql).ToList();
            }

            return l;
        }

        public override IEnumerable<DARViewModel> Get()
        {
            return GetAssetView();
        }

        public AssetViewModel GetAssetById(string id)
        {
            return GetAssetView(id).Cast<AssetViewModel>().FirstOrDefault();
        }

        public override DARViewModel Get(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                return null;

            CurrentAsset = GetAssetById(key);
            return CurrentAsset;
        }

        private void UpdateCustodians(string custodians, string assetId, bool? institutionalCustodyAvailable)
        {
            AssetCustodian ac = new AssetCustodian();

            var l = new List<AssetCustodianViewModel>();

            if (custodians.Equals("Null", StringComparison.OrdinalIgnoreCase) || (institutionalCustodyAvailable != null && !institutionalCustodyAvailable.Value))
            {
                l = (List<AssetCustodianViewModel>)ac.GetList(assetId);
                if (l != null)
                {
                    foreach (var item in l)
                    {
                        ac.Delete(item);
                    }
                    return;

                }
            }


                
            

            //if (custodians.Equals("Null", StringComparison.OrdinalIgnoreCase) || (institutionalCustodyAvailable != null && !institutionalCustodyAvailable.Value))
            //{
            //    ac.Delete(assetId);
            //    return;
            //}
            if (!string.IsNullOrWhiteSpace(custodians))
            {
                l = (List<AssetCustodianViewModel>)ac.GetList(assetId);
                if (l != null)
                {
                    foreach (var item in l)
                    {
                        ac.Delete(item);
                    }

                }
                var custodianList = custodians.Split(',');
                if (custodianList.Any())
                {
                    var existingCustodians = ac.GetAssetCustodians(assetId);

                    foreach (var item in custodianList)
                    {

                        Custodian c = new Custodian(item);

                        if (c.CurrentCustodian == null)
                        {
                            throw new Exception("Invalid custodian code");
                        }

                        AssetCustodianViewModel acVM = new AssetCustodianViewModel();

                        acVM.DARCustodianID = c.CurrentCustodian.DARCustodianID;
                        acVM.DARAssetID = assetId;
                        ac.Add(acVM);
                    }

                    

                    //foreach (var item in existingCustodiansList)
                    //{
                    //    //if (item.Value == false)
                    //    //{
                    //    //    ac.Delete(item.Key, assetId);
                    //    //}
                    //}
                }
            }
        }

        private void UpdateThemes(string themes, string assetId, string themeType)
        {
            AssetTheme at = new AssetTheme();
            if (string.IsNullOrWhiteSpace(themes))
                return;

            if (string.IsNullOrWhiteSpace(themes))
                return;

            var themeList = themes.Split(',');

            if (themeList.Any())
            {
                var existingThemes = at.GetAssetThemes(assetId, themeType);

                foreach (var i in themeList)
                {
                    var item = i.Trim();
                    if (existingThemes.ContainsKey(item))
                        existingThemes[item] = true;

                    Theme c = new Theme(item, themeType);

                    if (c.CurrentTheme == null)
                    {
                        throw new Exception("Invalid Theme code");
                    }

                    AssetThemeViewModel atVM = new AssetThemeViewModel();

                    if (!at.ThemeExists(assetId, c.CurrentTheme.DARThemeID))
                    {
                        atVM.DARThemeID = c.CurrentTheme.DARThemeID;
                        atVM.DARAssetID = assetId;
                        atVM.ThemeType = themeType;
                        at.Add(atVM);
                    }
                }

                foreach (var item in existingThemes)
                {
                    if (item.Value == false)
                    {
                        at.Delete(item.Key, assetId, themeType);
                    }
                }
            }
        }

        private void UploadAssetURL(ICollection<AssetViewModel> records)
        {
            var urls = (new UrlType()).Get().Cast<UrlTypeViewModel>().ToList();
            var urlByString = urls.ToDictionary(keySelector: m => m.Name, elementSelector: m => m.DARURLTypeID);

            var url = new AssetURL();
            foreach (var record in records)
            {
                url.Upsert(urlByString[UT_TWITTER], record.Twitter, record.DARAssetID);
                url.Upsert(urlByString[UT_REDDIT], record.Reddit, record.DARAssetID);
                url.Upsert(urlByString[UT_BLOG], record.Blog, record.DARAssetID);
                url.Upsert(urlByString[UT_WHITE_PAPER], record.WhitePaper, record.DARAssetID);
                url.Upsert(urlByString[UT_CODE_REPOSITORY_URL], record.CodeRepositoryURL, record.DARAssetID);
                url.Upsert(urlByString[UT_PRIMARY], record.PrimaryURL, record.DARAssetID);
                url.Upsert(urlByString[UT_COIN_GECKO_URL], record.CoinGeckoURL, record.DARAssetID);
                url.Upsert(urlByString[UT_COIN_MARKETCAP_URL], record.CoinMarketCapURL, record.DARAssetID);
            }
        }
        
        public bool ConfirmAssetUpload(string ticker)
        {
            for (int i = 0; i < 30; i++)
            {
                var duplicate_ticker = GetTheExistingRecord(ticker.Trim(), "DARTicker");
                if (duplicate_ticker != null)
                {
                    return true;
                }
                Thread.Sleep(1000);
                
            }
            return false;
        }

        private int PostFromExcelFile(IEnumerable<AssetViewModel> records, BindingList<string> errors)
        {
            AssetValidator validator = new AssetValidator();
            int rowCount = 0;

            foreach (var r in records)
            {
                try
                {
                    FluentValidation.Results.ValidationResult results = validator.Validate(r);

                    if (results.IsValid == false)
                    {
                        foreach (ValidationFailure failure in results.Errors)
                        {
                            errors.Add($"{failure.PropertyName}: {failure.ErrorMessage}");
                        }
                    }
                    else
                    {
                        r.DARAssetID = AddAsset(r);
                        if(!ConfirmAssetUpload(r.DARTicker))
                        {
                            throw new Exception("Failed to create the asset. Possible reason - Pipeline issue. Please Check!!");
                        }
                    }

                    if (!string.IsNullOrEmpty(r.Custodians))
                    {
                        UpdateCustodians(r.Custodians, r.DARAssetID, r.InstitutionalCustodyAvailable);
                    }

                    if (!string.IsNullOrEmpty(r.DarTheme))
                    {
                        UpdateThemes(r.DarTheme, r.DARAssetID, Theme.THEME_TYPE_DAR);
                    }
                    if (!string.IsNullOrEmpty(r.DatsTheme))
                    {
                        UpdateThemes(r.DatsTheme, r.DARAssetID, Theme.THEME_TYPE_DATS);
                    }

                    rowCount++;
                }
                catch (Exception ex)
                {
                    Logger.Fatal($"AssetUpdate: Failed to post {r.GetDescription()}.", ex);
                    errors.Add($"{r.DARTicker} : Asset cannot be created because the entered DAR Ticker is already assigned to another asset");
                }
            }

            return rowCount;
        }

        public int PatchFromExcelFile(List<AssetViewModel> records, List<string> headers, BindingList<string> errors)
        {
            int rowCount = 0;

            foreach (var r in records)
            {
                AssetViewModel curAsset = null;

                try
                {
                    // curAsset = (Get(r.DARAssetID) as AssetViewModel) ?? throw new Exception("Invalid DARAssetID");

                    string query = $"SELECT * FROM {DARApplicationInfo.SingleStoreCatalogInternal}.Asset WHERE @DARAssetID = DARAssetID";

                    using (IDbConnection connection = new MySqlConnection(DARApplicationInfo.SingleStoreInternalDB))
                    {
                        curAsset = connection.QuerySingle<AssetViewModel>(query, new { DARAssetID = r.DARAssetID });
                    }

                    r.DARAssetID = curAsset != null ? curAsset.DARAssetID : throw new ArgumentNullException("Invalid DARAssetID");

                    curAsset.CopyTo(r);
                    var action = "File Upload";
                    UpdateAsset(r, action);

                    if (!string.IsNullOrEmpty(r.Custodians) || r.InstitutionalCustodyAvailable != null)
                    {
                        UpdateCustodians(r.Custodians, curAsset.DARAssetID, r.InstitutionalCustodyAvailable);
                    }

                    if (!string.IsNullOrEmpty(r.DarTheme))
                    {
                        UpdateThemes(r.DarTheme, curAsset.DARAssetID, Theme.THEME_TYPE_DAR);
                    }

                    if (!string.IsNullOrEmpty(r.DatsTheme))
                    {
                        UpdateThemes(r.DatsTheme, curAsset.DARAssetID, Theme.THEME_TYPE_DATS);
                    }

                    rowCount++;
                }
                catch (Exception ex)
                {
                    Logger.Fatal($"AssetUpdate: Failed to update {r.DARAssetID}.", ex);
                    errors.Add($"{r.DARAssetID} : {ex.Message}");
                }
            }

            UploadAssetURL(records);

            return rowCount;
        }

        public override bool LoadDataFromExcelFile(string filePath, out string errors)
        {
            errors = string.Empty;
            StringBuilder sbError = new StringBuilder();
            int rowCount = 0;

            BindingList<string> errorslist = new BindingList<string>();
            var headerRow = new List<string>();
            IEnumerable<AssetViewModel> records;

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                HeaderValidated = null,
                MissingFieldFound = null,
                IgnoreBlankLines = true,
                TrimOptions = TrimOptions.Trim
            };

            using (var streamReader = new StreamReader(filePath))
            {
                using (var csvReader = new CsvReader(streamReader, config))
                {
                    csvReader.Context.TypeConverterOptionsCache.GetOptions<decimal?>().NullValues.Add("Null");
                    csvReader.Context.TypeConverterOptionsCache.GetOptions<bool?>().NullValues.Add("Null");
                    // csvReader.Context.RegisterClassMap<AssetViewModelMapConverter>();
                    records = csvReader.GetRecords<AssetViewModel>().ToList();
                }
            }

            Logger.Info($"AssetUpdate: Loading Filename {filePath} RowCount:{records.Count()}");

            List<AssetViewModel> posts = new List<AssetViewModel>();
            List<AssetViewModel> patches = new List<AssetViewModel>();

            foreach (var r in records)
            {
                if (string.IsNullOrEmpty(r.DARAssetID))
                {
                    posts.Add(r);
                }
                else
                {
                    patches.Add(r);
                }
            }
            int postCount = PostFromExcelFile(posts, errorslist);

            int patchCount = PatchFromExcelFile(patches, headerRow, errorslist);

            rowCount = postCount + patchCount;

            string summaryMessage = $"AssetUpdate: {rowCount} rows loaded from file {filePath}";

            Logger.Info(summaryMessage);

            sbError.AppendLine(summaryMessage);

            errorslist.ToList().ForEach(error => sbError.AppendLine(error));

            errors = sbError.ToString();

            return errorslist.Count() == 0;
        }

  
        public IEnumerable<DARViewModel> GetRefMasterPublicAssets()
        {
            var l = new List<AssetRefMasterPublic>();

            string sql = $@"
                           select *
                           from {DARApplicationInfo.SingleStoreCatalogPublic}.token2
                            ";

            using (var connection = new MySqlConnection(DARApplicationInfo.SingleStorePublicDB))
            {
                l = connection.Query<AssetRefMasterPublic>(sql).ToList();
            }

            return l;
        }

        public override string GetNextId()
        {
            return GetNextId("DA", 5, 100);
        }

        public override bool Update(DARViewModel i)
        {
            throw new NotImplementedException();
        }

        public string AssetPublish(AssetViewModel l)
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
                    producer.Produce("refmaster_asset", new Message<Null, string> { Value = jsondata });
                    producer.Flush();
                }
                return "Message published without error";
            }
            catch (Exception ex)
            {
                return $"Failed to publish message {ex.Message}";
            }

        }

        public string AssetPublish_Audit(AssetViewModel l)
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
                    producer.Produce("refmaster_asset_audit", new Message<Null, string> { Value = jsondata });
                    producer.Flush();
                }
                return "Message published without error";
            }
            catch (Exception ex)
            {
                return $"Failed to publish message {ex.Message}";
            }

        }
        public string AssetPublish_token(TokenTableViewModel l)
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
                    producer.Produce("refmaster_public_token", new Message<Null, string> { Value = jsondata });
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
            throw new NotImplementedException();
        }
    }
}