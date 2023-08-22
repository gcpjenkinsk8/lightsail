using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;

namespace DARReferenceData
{
    public class DARTools
    {
        public static string FormatDate_yyyymmdd(string inputDate)
        {
            if (string.IsNullOrWhiteSpace(inputDate))
            {
                return null;
            }
            else
            {
                DateTime sDate;
                sDate = DateTime.ParseExact(inputDate, "MM/dd/yyyy", null);
                return sDate.ToString("yyyy'-'MM'-'dd");
                //if (DateTime.TryParse(inputDate, out sDate))
                //{
                   
                //}
            }
            return null;
        }
    }
}
