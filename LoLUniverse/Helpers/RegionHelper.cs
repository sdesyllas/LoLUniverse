using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using RiotApi.Net.RestClient.Configuration;
using RiotApi.Net.RestClient.Helpers;

namespace LoLUniverse.Helpers
{
    public class RegionHelper
    {
        public static IList<SelectListItem> SummonerRegions
        {
            get
            {
                var regions = EnumHelper.GetSelectList(typeof(RiotApiConfig.Regions));
                //remove PBE
                regions.Remove(regions.FirstOrDefault(x => x.Text == RiotApiConfig.Regions.PBE.ToString()));
                //remove Global
                regions.Remove(regions.FirstOrDefault(x => x.Text == RiotApiConfig.Regions.Global.ToString()));
                return regions;
            }
        }
    }
}