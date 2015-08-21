using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RiotApi.Net.RestClient.Configuration;
using RiotApi.Net.RestClient.Dto.Summoner;

namespace LoLUniverse.Models.Search
{
    public class SearchSummoner
    {
        [Required]
        [Display(Name = "Summoner", Prompt = "")]
        public string SummonerName { get; set; }

        [Required]
        [Display(Name = "Region")]
        public RiotApiConfig.Regions Region { get; set; }

        public List<SummonerModel> SummonerModels { get; set; } 

    }
}