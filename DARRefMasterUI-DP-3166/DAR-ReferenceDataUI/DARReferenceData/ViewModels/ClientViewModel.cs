using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DARReferenceData.ViewModels
{
    public class ClientViewModel : DARViewModel
    {
        public ClientViewModel() { ExpiryDate = DateTime.MaxValue; }
        public string DARClientID { get; set; }

        [Display(Name = "Client Name")]
        public string ClientName { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Full Access")]
        public bool HasFullAccess { get; set; } = true;

        [Display(Name = "API Key")]
        public string APIKey { get; set; }

        [Display(Name = "Hourly Prices")]
        public bool HourlyPrice { get; set; }

        [Display(Name = "Latest Prices")]
        public bool LatestPrice { get; set; } = false;

        [Display(Name = "WebSocket")]
        public bool Websocket { get; set; } = false;

        [Display(Name = "NFT")]
        public bool NFT { get; set; } = false;

        [Display(Name = "Derivatives")]
        public bool Derivatives { get; set; } = false;
        
        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "LookbackDays")]
        public int? LookbackDays { get; set; }

        [Display(Name = "Events")]
        public bool Events { get; set; } = false;

        [Display(Name = "Liquidity Pool Price")]
        public bool LiquidityPoolPrice { get; set; } = false;
        
        [Display(Name = "Domain Name")]
        public string DomainName { get; set; }
        
        [Display(Name = "CirculatingSupply")]
        public bool CirculatingSupply { get; set; } = false;
        
        [Display(Name = "MarketCap")]
        public bool MarketCap { get; set; } = false;
        public bool OHLCV { get; set; } = false;


        [Display(Name = "400ms")]
        public bool Price400ms { get; set; } = false;
        
        [Display(Name = "ExternalClientName")]
        public string ExternalClientName { get; set; }

        public string Operation { get; set; }
        public long Deleted { get; set; }

        public override string GetDescription()
        {
            return $"{this.GetType()}: {DARClientID}, {ClientName}";
        }
    }
}