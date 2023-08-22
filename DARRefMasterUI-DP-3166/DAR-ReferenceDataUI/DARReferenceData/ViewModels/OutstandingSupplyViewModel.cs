using System;

namespace DARReferenceData.ViewModels
{
    public class OutstandingSupplyViewModel : DARViewModel
    {
        public string OustandingSupplyID { get; set; }
        public string DARSourceID { get; set; }

        public string ProcessID { get; set; }

        public string Error { get; set; }

        public string darAssetID { get; set; }
        public string DARTicker { get; set; }

        public string Source { get; set; }

        public decimal OutstandingSupply { get; set; }

        public decimal? OutstandingSupplyReviewed { get; set; }

        public long Reviewed { get; set; }

        public bool BaseDataAvailable { get; set; }

        public string MappedDARAssetId { get; set; }

        public string LegacyDARAssetId { get; set; }

        public int? LegacyId { get; set; }

        public bool PassedValidation { get; set; }

        public string Operation { get; set; }

        public DateTime CollectedTimeStamp { get; set; }

        public long Deleted { get; set; }

        public DateTime LoadTimestamp { get; set; }

        public override string GetDescription()
        {
            return $"{darAssetID} - {DARTicker} - {Source}";
        }
    }
}