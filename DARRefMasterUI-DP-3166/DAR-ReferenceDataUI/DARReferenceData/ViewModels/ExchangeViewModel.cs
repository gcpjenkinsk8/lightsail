using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class ExchangeViewModel: DARViewModel
    {
        public string UniqueExchangeId { get; set; }
        public string DARExchangeID { get; set; }
        public string ShortName { get; set; }
        public string LegalName { get; set; }
        public string LegalNameSource { get; set; }
        public string ExchangeType { get; set; }
        public string ExchangeTypeSource { get; set; }
        public string ExchangeStatus { get; set; }
        public string ExternalClassification { get; set; }
        public string InternalClassification { get; set; }
        public string ClassificationFolder { get; set; }
        public DateTime? ClassificationDate { get; set; }
        public int? ClassificationVersion { get; set; }
        public string DomicileCountry { get; set; }
        public string IncorporationCountry { get; set; }
        public string ExchangeSLA { get; set; }
        public int? FoundingYear { get; set; }
        public string Ownership { get; set; }
        public string LEI { get; set; }
        public string Chairman { get; set; }
        public string CEO { get; set; }
        public string President { get; set; }
        public string CTO { get; set; }
        public string CISO { get; set; }
        public string CCO { get; set; }
        public string PrimaryPhone { get; set; }
        public string PrimaryEmail { get; set; }
        public string SupportURL { get; set; }
        public string SupportPhone { get; set; }
        public string SupportEmail { get; set; }
        public string HQAddress1 { get; set; }
        public string HQAddress2 { get; set; }
        public string HQCity { get; set; }
        public string HQState { get; set; }
        public string HQCountry { get; set; }
        public string HQPostalCode { get; set; }
        public string Licenses { get; set; }
        public string Wikipedia { get; set; }
        public string MICCode { get; set; }
        public bool? KnownRegulatoryIssues { get; set; }
        public bool? TradeMonitoringSystem { get; set; }
        public bool? BlockchainSurveillanceSystem { get; set; }
        public bool? ThirdPartyAudit { get; set; }
        public bool? KnownSecurityIncidences { get; set; }
        public string InsuranceProviders { get; set; }
        public bool? InsuranceonCryptoAssets { get; set; }
        public string Wherethebankisdomiciled { get; set; }
        public bool? SelfInsurance { get; set; }
        public bool? MandatoryGovtIDPriortoTrading { get; set; }
        public string TradingLimitExKYC { get; set; }
        public string TradingLimitExKYCsource { get; set; }
        public string DepositLimitExKYC { get; set; }
        public string DepositLimitExKYCsource { get; set; }
        public string WithdrawalLimitExKYC { get; set; }
        public string WithdrawalLimitExKYCsource { get; set; }
        public bool? KYCReqGovernmentID { get; set; }
        public bool? KYCReqDigitalSelfPortrait { get; set; }
        public string CorporateActionsPolicy { get; set; }
        public string PoliciesOnListing { get; set; }
        public string FeeSchedule { get; set; }
        public string TradingHours { get; set; }
        public bool? Leverage { get; set; }
        public bool? Staking { get; set; }
        public bool? IEOPlatform { get; set; }
        public bool? NativeToken { get; set; }
        public bool? ColdStorageCustody { get; set; }
        public bool? CustodyInsurance { get; set; }
        public bool? PercentOfAssetsinColdStorage { get; set; }
        public bool? StablecoinPairs { get; set; }
        public bool? FiatTrading { get; set; }
        public bool? Futures { get; set; }
        public bool? Options { get; set; }
        public bool? Swaps { get; set; }
        public string APIType { get; set; }
        public string APIDocumentation { get; set; }

        public string PrimaryURL { get; set; }
        public string Twitter { get; set; }
        public string LinkedIn { get; set; }
        public string Reddit { get; set; }
        public string Facebook { get; set; }
        public string Operation { get; set; }
        public long Deleted { get; set; }


        public int LegacyId { get; set; }
        public override string GetDescription()
        {
            return $"{DARExchangeID} - {ShortName} - {ExchangeType}";
        }
    }
}
