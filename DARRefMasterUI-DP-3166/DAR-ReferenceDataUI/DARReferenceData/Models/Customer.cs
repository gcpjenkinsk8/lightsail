using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DARReferenceData.Models
{
    public class Customer
    {
        [Required(ErrorMessage = "Client Name cannot be blank.")]
        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        public string Description { get; set; }

        public bool HasFullAccess { get; set; }

        public string APIKey { get; set; }

        public bool HourlyPrice { get; set; }

        public bool LatestPrice { get; set; } = false;

        public bool WebSocket { get; set; } = false;

        public bool NFT { get; set; } = false;

        public bool Derivatives { get; set; } = false;

        // public ICollection<CallerIDViewModel> CallerIDs { get; set; }

        public string GetDescription()
        {
            return $"{ClientName}: {Description} ";
        }
    }
}