using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class StagedCryptoEventViewModel : DARViewModel
    {
        public StagedCryptoEventViewModel()
        {
            ValidationTime = DateTime.Now.ToUniversalTime();
        }

        public string StageEventID { get; set; }

        [DataType(DataType.Date)]
        public DateTime DateofReview { get; set; }

        public string ExchangeAssetTicker { get; set; }

        public string ExchangeAssetName { get; set; }
        public string DARAssetID { get; set; }
        public string DAREventID { get; set; }
        public string EventType { get; set; }

        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime AnnouncementDate { get; set; }

        public string EventDescription { get; set; }
        public string SourceURL { get; set; }
        public string EventStatus { get; set; }
        public string Notes { get; set; }
        public string Exchange { get; set; }

        [DataType(DataType.Date)]
        [ScaffoldColumn(false)]
        public DateTime ValidationTime { get; set; }
        public string SourceID { get; set; }
        public string DARSourceID { get; set; }
        public string EventTypeID { get; set; }

        public int BlockHeight { get; set; }

        public string Error { get; set; }
        public string Operation { get; set; }
        public long Deleted { get; set; }

        public override string GetDescription()
        {
            return $"{ExchangeAssetTicker} - {ExchangeAssetName} - {DARAssetID} - {EventType} - {EventDate.ToShortDateString()} - {EventDescription} - {SourceURL}";
        }
    }
}