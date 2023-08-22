using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DARReferenceData.ViewModels
{
    public class VerifiableVolumeViewModel : DARViewModel
    {
        public string darAssetID { get; set; }

        private string _darAssetTicker;

        public string darAssetTicker
        {
            get
            {
                return _darAssetTicker;
            }
            set
            {
                _darAssetTicker = value.ToUpper();

            }
        }

        public decimal verifiableVolume { get; set; }

        public string windowStart { get; set; }

        public string windowEnd { get; set; }

        public string effectiveTime { get; set; }

        public string error { get; set; }

        public override string GetDescription()
        {
            throw new NotImplementedException();
        }
    }
}
