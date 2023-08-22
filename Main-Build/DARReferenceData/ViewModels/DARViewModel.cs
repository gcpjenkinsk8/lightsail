using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DARReferenceData.ViewModels
{
    public abstract class DARViewModel
    {

        public DARViewModel()
        {
           
            CreateTime = LastEditTime = DateTime.Now;

        }
        [JsonIgnore]
        public bool IsActive { get; set; }
        [JsonIgnore]
        [ScaffoldColumn(false)]
        public string CreateUser { get; set; }
        [JsonIgnore]
        [ScaffoldColumn(false)]
        public string LastEditUser { get; set; }
        [JsonIgnore]
        [DataType(DataType.Date)]
        [ScaffoldColumn(false)]
        public DateTime CreateTime { get; set; }
        [JsonIgnore]
        [DataType(DataType.Date)]
        [ScaffoldColumn(false)]
        public DateTime LastEditTime { get; set; }

        public abstract string GetDescription();

    }
}
