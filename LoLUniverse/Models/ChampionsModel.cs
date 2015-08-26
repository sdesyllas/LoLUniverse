using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using RiotApi.Net.RestClient.Configuration;
using RiotApi.Net.RestClient.Dto.LolStaticData.Generic;
using RiotDtos = RiotApi.Net.RestClient.Dto;

namespace LoLUniverse.Models
{
    public class ChampionsModel
    {
        [Display(Name = "Free 2 play")]
        public bool F2pOnly { get; set; }

        public RiotApiConfig.Regions Region { get; set; }

        public List<ChampionModel> ChampionModels { get; set; }

        public class ChampionModel
        {
            public RiotDtos.Champion.ChampionListDto.ChampionDto ChampionDto { get; set; }

            public RiotDtos.LolStaticData.Champion.ChampionDto StaticChampionDto { get; set; }

            public string ChampionImage { get; set; }

            public RiotApiConfig.Regions Region { get; set; }

            public int ChampionId { get; set; }

            public string SplashImage { get; set; }

            public string LoadingImage { get; set; }

            public int CurrentSkinId { get; set; }

            public string CurrentSkinName { get; set; }
        }
    }
}