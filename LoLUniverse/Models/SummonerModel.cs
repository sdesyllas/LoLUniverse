using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RiotApi.Net.RestClient.Dto.League;
using RiotApi.Net.RestClient.Dto.Summoner;

namespace LoLUniverse.Models
{
    public class SummonerModel
    {
        public string SummonerKey { get; set; }

        public SummonerDto SummonerDto { get; set; }

        public List<LeagueModel> LeagueModels { get; set; }

        public string ProfileImagePath { get; set; }

        public class LeagueModel
        {
            public string LeagueKey { get; set; }

            public LeagueDto LeagueDto { get; set; }

            public string LeagueName { get; set; }
        }
    }
}