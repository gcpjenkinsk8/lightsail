using DARReferenceData.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace DARReferenceData.DatabaseHandlers
{
    public class Configuration
    {
        public static List<DropDownItem> GetVettingTypes()
        {
            var list = new List<DropDownItem>();
            list.Add(new DropDownItem() { Id = "1", Name = "Exchange Status" });
            list.Add(new DropDownItem() { Id = "2", Name = "Asset Tier" });

            return list;
        }

        public static List<DropDownItem> GetTimeUnits()
        {
            var list = new List<DropDownItem>();
            list.Add(new DropDownItem() { Id = "1", Name = "hh" });
            list.Add(new DropDownItem() { Id = "2", Name = "mm" });
            list.Add(new DropDownItem() { Id = "2", Name = "ss" });
            list.Add(new DropDownItem() { Id = "2", Name = "ms" });
            return list;
        }

        public static List<DropDownItem> GetDARMnemonicFamily()
        {
            var list = new List<DropDownItem>();
            list.Add(new DropDownItem() { Id = "1", Name = "ftse" });
            list.Add(new DropDownItem() { Id = "2", Name = "dar" });

            return list;
        }

        public static List<DropDownItem> GetVettingStatus()
        {
            var list = new List<DropDownItem>();
            list.Add(new DropDownItem() { Id = "1", Name = "vetted /participating (DAR/FTSE)" });
            list.Add(new DropDownItem() { Id = "2", Name = "watch list" });
            list.Add(new DropDownItem() { Id = "3", Name = "disregarded" });
            list.Add(new DropDownItem() { Id = "9", Name = "penalized" });



            return list;
        }

        public static List<DropDownItem> GetDARDMnemonics()
        {
     
            var list = new List<DropDownItem>();
            list.Add(new DropDownItem() { Id = "1", Name = "dar-std-400ms-vw" });
            list.Add(new DropDownItem() { Id = "2", Name = "dar-std-15s-vw" });
            list.Add(new DropDownItem() { Id = "3", Name = "ftse-15s-vw" });
            list.Add(new DropDownItem() { Id = "4", Name = "dar-std-1s-pair-vw" });
            list.Add(new DropDownItem() { Id = "5", Name = "dar-principal-market-price-1h"});



            return list;
        }
        public static List<DropDownItem> GetPricingTier()
        {
            var list = new List<DropDownItem>();
            list.Add(new DropDownItem() { Id = "1", Name = "Tier1" });
            list.Add(new DropDownItem() { Id = "2", Name = "Tier2" });
            list.Add(new DropDownItem() { Id = "3", Name = "Tier3" });

            return list;
        }
    }
}
