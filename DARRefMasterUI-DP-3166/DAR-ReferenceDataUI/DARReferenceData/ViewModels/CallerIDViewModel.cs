using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DARReferenceData.Models;

namespace DARReferenceData.ViewModels
{
    public class CallerIDViewModel : DARViewModel
    {
        public string DARClientID { get; set; }

        [Required(ErrorMessage = "CallerID cannot be blank.")]
        [Display(Name = "IP Address")]
        public string CallerID { get; set; }

        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        public override string GetDescription()
        {
            return $"{this.GetType()}: {DARClientID}-{CallerID}, {ClientName}";
        }
    }
}