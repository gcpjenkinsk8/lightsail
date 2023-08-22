using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DAR_ReferenceDataUI.Models
{
    public class StagedCryptoEventViewModel_delete
    {
        public long ID { get; set; }
        [DataType(DataType.Date)]
        public DateTime DateofReview { get; set;}

        public string ExchangeAssetTicker { get; set; }

        public string ExchangeAssetName { get; set; }
        public string DARAssetID { get; set; }
        public string DAREventID { get; set; }
        public string EventType { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime AnnouncementDate { get; set; }
        public string EventDescription { get; set; }
        public string SourceURL { get; set; }
        public string EventStatus { get; set; }
        public string Notes { get; set; }
        public string  Deleted { get; set; }
        public string Exchange { get; set; }
        public DateTime ValidationTime { get; set; }
        public long AssetID { get; set; }
        public int SourceID { get; set; }
        public int EventTypeID { get; set; }
        public DateTime CreateTime { get; set; }



    }
}