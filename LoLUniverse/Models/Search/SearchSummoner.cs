using System.ComponentModel.DataAnnotations;
using RiotApi.Net.RestClient.Configuration;

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
    }
}