using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
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
        public List<LeagueDto.LeagueEntryDto> LeagueEntries { get; set; }
    }
}