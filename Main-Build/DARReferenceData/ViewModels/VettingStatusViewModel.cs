using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public class VettingStatusViewModel : DARViewModel
    {
        public string DARVettingStatusID { get; set; }
        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public string StatusType { get; set; }
        public string Operation { get; set; }
        public long Deleted { get; set; }
        public long IsActive { get; set; }
        
        public override string GetDescription()
        {
            return $"{DARVettingStatusID} - {StatusCode} - {StatusDescription} - {StatusType}";
        }
    }
}
