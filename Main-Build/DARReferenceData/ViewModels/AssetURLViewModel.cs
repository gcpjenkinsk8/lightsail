using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration.Attributes;

namespace DARReferenceData.ViewModels
{
    public class AssetURLViewModel : DARViewModel
    {
        public string UniqueID { get; set; }
        public string DARAssetID { get; set; }
        public string Asset { get; set; }
        public string AssetID { get; set; }
        public string URLType { get; set; }
        public string DARURLTypeID { get; set; }
        public string URLPath { get; set; }

        public long Deleted { get; set; }
        public string Operation { get; set; }
        public override string GetDescription()
        {
            return $"{DARAssetID} - {URLType} - {URLPath}";
        }
    }
}