using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DARReferenceData.DatabaseHandlers;

namespace DARReferenceData.ViewModels
{
    public class ServListV2ViewModel : DARViewModel
    {
        public string Servlistv2ID { get; set; }
        public string darMnemonic { get; set; }
        public string darAssetID { get; set; }
        public string darTicker { get; set; }
        public long priceTier { get; set; }
        public string Operation { get; set; }
        public long Deleted { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }

        public decimal LoadTimeStamp { get; set; }

        public override string GetDescription()
        {
            return $"{darMnemonic} - {darAssetID}";
        }
    }
}
