using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class ServingListSnapshotViewModel : ServingListViewModel
    {
        public long ID { get; set; }
        public string SnapshotName { get; set; }
        public int SnapshotVersion { get; set; }
        public int ProcessId { get; set; }
        public string PairName { get; set; }
        public int PairId { get; set; }
        public int AssetId { get; set; }
        public int ExchangeId { get; set; }
        public string ExchangeVettingStatusCode { get; set; }
    }
}