using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace DARReferenceData.ViewModels
{
    public class SourceViewModel : DARViewModel
    {
        public string DARSourceID { get; set; }
        public string ShortName { get; set; }
        public string SourceType { get; set; }
        public override string GetDescription()
        {
            return $"{DARSourceID} - {ShortName} - {SourceType}";
        }
    }
}