using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RiotApi.Net.RestClient.Configuration;
using RiotApi.Net.RestClient.Dto.League;
using RiotApi.Net.RestClient.Helpers;

namespace LoLUniverse.Models
{
    public class LeaderboardModel
    {
        [Required]
        [Display(Name = "Region")]
        public RiotApiConfig.Regions Region { get; set; }

        [Required]
        [Display(Name = "Type")]
        public Enums.GameQueueType GameQueueType { get; set; }

        public LeagueDto LeagueDto { get; set; }
        public List<SummonerEntryModel> LeagueEntryModels { get; set; }

        public class SummonerEntryModel
        {
            public string SummonerIcon { get; set; }

            public LeagueDto.LeagueEntryDto LeagueEntry { get; set; }
        } 
    }
}