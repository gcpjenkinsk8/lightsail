using System;

namespace DARReferenceData.ViewModels
{
    public class OutstandingSupplySourceViewModel : DARViewModel
    {
        public string supplysourceID { get; set; }
        public string DARAssetID { get; set; }

        public string DARTicker { get; set; }

        public string DARSourceID { get; set; }

        public string ShortName { get; set; }
        
        public string SourceType { get; set; }

        public string Creator { get; set; }

        public decimal? ManualValue { get; set; }

        public decimal LoadTimestamp { get; set; }

        public string Operation { get; set; }

        public long Deleted { get; set; }


        public override string GetDescription()
        {
            return $"{DARAssetID} - {DARSourceID} - {ShortName}";
        }
    }
}