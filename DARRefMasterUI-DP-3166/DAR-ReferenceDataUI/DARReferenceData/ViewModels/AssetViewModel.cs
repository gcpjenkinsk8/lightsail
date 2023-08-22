using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;

namespace DARReferenceData.ViewModels
{
    public class AssetViewModel : DARViewModel
    {
        public class CustomBooleanConverter : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                if (string.IsNullOrEmpty(text) || text.Equals("Null", StringComparison.OrdinalIgnoreCase))
                    return text;

                return text.Equals("Yes", StringComparison.OrdinalIgnoreCase);
            }
        }

     
        [Optional]
        public string UniqueID { get; set; }

        [Name("DAR Asset ID"), Optional]
        [Display(Name = "DAR AssetID")]
        public string DARAssetID { get; set; }

        [Name("DAR Ticker")]
        [Display(Name = "DAR Ticker")]
        public string DARTicker { get; set; }

        [Display(Name = "Asset Name")]
        public string Name { get; set; }

        [Optional]
        public string AssetType { get; set; }

        [Optional]
        public string Description { get; set; }

        [Optional]
        public string Sponsor { get; set; }

        [Optional]
        public bool IsBenchmarkAsset { get; set; }

        [Optional]
        public string SEDOL { get; set; }

        [Optional]
        public string ISIN { get; set; }

        [Optional]
        public string CUSIP { get; set; }

        [Optional]
        public string DTI { get; set; }

        [Optional]
        public string DevelopmentStage { get; set; }

        [Optional]
        public string DARSuperSector { get; set; }

        [Optional]
        public string DARSuperSectorCode { get; set; }

        [Optional]
        public string DARSector { get; set; }

        [Optional]
        public string DARSectorCode { get; set; }

        [Optional]
        public string DARSubSector { get; set; }

        [Optional]
        public string DARSubSectorCode { get; set; }

        [Optional]
        public decimal? DarTaxonomyVersion { get; set; }

        [Optional]
        public string DATSSuperSector { get; set; }

        [Optional]
        public string DATSSuperSectorCode { get; set; }

        [Optional]
        public string DATSSector { get; set; }

        [Optional]
        [CsvHelper.Configuration.Attributes.TypeConverter(typeof(CustomBooleanConverter))]
        [Display(Name = "DATSGovernance")]
        public bool? DATSGovernance { get; set; }

        [Optional]
        [CsvHelper.Configuration.Attributes.TypeConverter(typeof(CustomBooleanConverter))]
        [Display(Name = "DATSLayer1")]
        public bool? DATSLayer1 { get; set; }

        [Optional]
        public string DATSSectorCode { get; set; }

        [Optional]
        public string DATSSubSector { get; set; }

        [Optional]
        public string DATSSubSectorCode { get; set; }

        [Name("DATSTaxonomyVersion"), Optional]
        public decimal? DATSTaxonomyVersion { get; set; }

        [Optional]
        public string IssuanceFramework { get; set; }

        [Optional]
        public bool? IsRestricted { get; set; }

        [Optional]
        public decimal? CirculatingSupply { get; set; }

        [Optional]
        public decimal? MaxSupply { get; set; }

        [Optional]
        public string MessariTaxonomySector { get; set; }

        [Optional]
        public string MessariTaxonomyCategory { get; set; }

        [Name("Institutional Custody"), Optional]
        public bool? InstitutionalCustodyAvailable { get; set; }

        [Name("DAR Themes"), Optional]
        public string DarTheme { get; set; }

        [Name("PrimaryURL"), Optional]
        public string PrimaryURL { get; set; }

        [Name("Twitter"), Optional]
        public string Twitter { get; set; }

        [Name("Reddit"), Optional]
        public string Reddit { get; set; }

        [Name("Blog"), Optional]
        public string Blog { get; set; }

        [Name("WhitePaper"), Optional]
        public string WhitePaper { get; set; }

        [Name("CodeRepositoryURL"), Optional]
        public string CodeRepositoryURL { get; set; }

        [Name("CoinGecko URL"), Optional]
        public string CoinGeckoURL { get; set; }

        [Name("CoinMarketCap URL"), Optional]
        public string CoinMarketCapURL { get; set; }

        [Optional]
        public string LegacyDARAssetId { get; set; }

        [Optional]
        public int? LegacyId { get; set; }

        [Optional]
        public string Custodians { get; set; }

        [Name("DATS Themes"), Optional]
        public string DatsTheme { get; set; }

        [Name("coinmarketcap_id"), Optional]
        public string CoinMarketCapId { get; set; }

        [Optional]
        public bool? HasERC20Version { get; set; }

        [Name("HasNYDFSCustoday"), Optional]
        public bool? HasNYDFSCustoday { get; set; }

        [Name("coingecko_id"), Optional]
        public string CoinGeckoId { get; set; }

        [Optional]
        [CsvHelper.Configuration.Attributes.TypeConverter(typeof(CustomBooleanConverter))]
        [Display(Name = "Governance Token")]
        public bool? GovernanceToken { get; set; }

        [Optional]
        [CsvHelper.Configuration.Attributes.TypeConverter(typeof(CustomBooleanConverter))]
        [Display(Name = "Layer One")]
        public bool? LayerOne { get; set; }


        [Name("isoCurrencyCode"), Optional]
        public string IsoCurrencyCode { get; set; }
        public long EstimatedCirculatingSupply { get; set; }
        public string Operation { get; set; }

        public long Deleted { get; set; }

        public override string GetDescription()
        {
            return $"DARAssetID:{DARAssetID} DARTicker:{DARTicker} Name:{Name} AssetType:{AssetType}";
        }

        public void CopyTo(AssetViewModel toObj)
        {
            if (toObj.GetType() != GetType())
                throw new InvalidCastException();

            foreach (var propertyInfo in GetType().GetProperties())
            {
                var propertyValue = propertyInfo.GetValue(this);

                if (propertyValue == null || propertyInfo.GetValue(toObj) != null) continue;

                if (propertyInfo.PropertyType == typeof(string))
                {
                    if (string.IsNullOrEmpty(propertyValue.ToString()))
                    {
                        continue;
                    }
                }

                propertyInfo.SetValue(toObj, propertyValue, null);
            }
        }
    }

    public class AssetRefMasterPublic : DARViewModel
    {
        public int legacyID { get; set; }
        public string darTicker { get; set; }

        public string name { get; set; }

        public int ftseStatus { get; set; }

        public int indexStatus { get; set; }

        public int exchangeSourceStatus { get; set; }

        public int otherPricing { get; set; }

        public string darAssetID { get; set; }

        public DateTime? createTime { get; set; }

        public override string GetDescription()
        {
            throw new NotImplementedException();
        }
    }
}