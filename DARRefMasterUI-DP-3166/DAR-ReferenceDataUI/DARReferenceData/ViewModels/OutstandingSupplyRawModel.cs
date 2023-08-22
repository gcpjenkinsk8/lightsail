using System;

namespace DARReferenceData.ViewModels
{
    public class OutstandingSupplyRawViewModel : DARViewModel
    {
        public string DARAssetID { get; set; }
        public string ExternalSourceTicker { get; set; }

        public string ExternalSourceTickerName { get; set; }
        public string ExternalSource { get; set; }
        public string Category { get; set; }

        public string Note { get; set; }
        public string SourceType { get; set; }
        public string EndpointURL { get; set; }
        public string EndpointField { get; set; }

        public string LoadTS { get; set; }

        public string CirculatingSupply { get; set; }


        public override string GetDescription()
        {
            return $"{DARAssetID} - {ExternalSourceTicker} - {ExternalSource}";
        }
    }
}