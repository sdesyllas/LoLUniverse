﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using RiotApi.Net.RestClient.Configuration;

namespace LoLUniverse.Helpers
{
    public class RegionHelper
    {
        private static IList<SelectListItem> _summonerRegions;

        public static IList<SelectListItem> SummonerRegions
        {
            get
            {
                if (_summonerRegions == null)
                {
                    var regions = EnumHelper.GetSelectList(typeof (RiotApiConfig.Regions));
                    //remove PBE
                    regions.Remove(regions.FirstOrDefault(x => x.Text == RiotApiConfig.Regions.PBE.ToString()));
                    //remove Global
                    regions.Remove(regions.FirstOrDefault(x => x.Text == RiotApiConfig.Regions.Global.ToString()));
                    _summonerRegions = regions;
                }
                return _summonerRegions;
            }
        }
    }
}