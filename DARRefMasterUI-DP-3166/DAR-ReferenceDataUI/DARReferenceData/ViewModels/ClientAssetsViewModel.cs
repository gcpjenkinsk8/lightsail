using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DARReferenceData.ViewModels
{
    public class ClientAssetsViewModel : DARViewModel
    {
        public string ClientAssetID { get; set; }
        public string DARClientID { get; set; }

        public string DARAssetID { get; set; }

        [Display(Name = "Asset Name")]
        public string AssetName { get; set; }

        [Display(Name = "DAR Ticker")]
        public string DARTicker { get; set; }

        
        [Display(Name = "Allow Reference Data")]
        public bool ReferenceData { get; set; }

        [DisplayName("Allow Price")]
        public bool Price { get; set; }

        [DisplayName("Client Name")]
        public string ClientName { get; set; }

        public string Operation { get; set; }

        public long Deleted { get; set; }


        public override string GetDescription()
        {
            return $"{this.GetType()}: {DARClientID}, {AssetName}-{DARTicker}";
        }
    }
}