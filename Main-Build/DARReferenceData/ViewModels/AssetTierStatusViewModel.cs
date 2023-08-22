using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class AssetTierStatusViewModel : DARViewModel
    {
        public long ID { get; set; }

        public string ProcessName { get; set; }
        public long ProcessId { get; set; }

        public string Asset { get; set; }
        public long AssetId { get; set; }
        public string DARAssetID { get; set; }

        public string AssetTier { get; set; }
        public int AssetTierId { get; set; }

        public int IndexStatus { get; set; }

        public override string GetDescription()
        {
            return $"{ProcessName} - {Asset} - {AssetTier} - {IndexStatus}";
        }
    }
}