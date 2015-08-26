using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoLUniverse.Helpers;
using LoLUniverse.Models;
using LoLUniverse.Services;
using Ninject;
using RiotApi.Net.RestClient;
using RiotApi.Net.RestClient.ApiCalls;
using RiotApi.Net.RestClient.Configuration;
using RiotApi.Net.RestClient.Dto.League;
using RiotApi.Net.RestClient.Dto.Match.Generic;
using RiotApi.Net.RestClient.Helpers;

namespace LoLUniverse.Controllers
{
    public class LeaderboardController : Controller
    {
        private IRiotClient _riotClient;
        private ICacheManager _cacheManager;
        private IMemoryCache _memoryCache;

        public LeaderboardController(IRiotClient riotClient, ICacheManager cacheManager, IMemoryCache memoryCache)
        {
            _riotClient = riotClient;
            _cacheManager = cacheManager;
            _memoryCache = memoryCache;
        }

        // GET: Leaderboard
        public ActionResult Index()
        {
            var cacheKey = string.Format(CacheKeys.ChallengerTierLeagueKey, RiotApiConfig.Regions.EUNE,
                Enums.GameQueueType.RANKED_SOLO_5x5);

            var challengers = _cacheManager.Get(cacheKey, DateTime.Now.AddMinutes(60),
                () =>
                    _riotClient.League.GetChallengerTierLeagues(RiotApiConfig.Regions.EUNE,
                        Enums.GameQueueType.RANKED_SOLO_5x5));
            //get top 40 leaderboard using LINQ
            var top40 = challengers.Entries.OrderByDescending(x => x.LeaguePoints).Take(40).ToList();
            LeaderboardModel model = new LeaderboardModel
            {
                Region = RiotApiConfig.Regions.EUNE,
                GameQueueType = Enums.GameQueueType.RANKED_SOLO_5x5,
                LeagueDto = challengers
            };
            PrepareModel(model.Region, model.GameQueueType, model, top40);
            //top 100 leaderboard
            return View(model);
        }

        public ActionResult FilterBoard(LeaderboardModel model)
        {
            var cacheKey = string.Format(CacheKeys.ChallengerTierLeagueKey, model.Region, model.GameQueueType);

            var challengers = _cacheManager.Get(cacheKey, DateTime.Now.AddMinutes(60),
                () =>
                    _riotClient.League.GetChallengerTierLeagues(model.Region, model.GameQueueType));

            //get top 40 leaderboard using LINQ
            var top40 = challengers.Entries.OrderByDescending(x => x.LeaguePoints).Take(40).ToList();
            model.LeagueDto = challengers;
            PrepareModel(model.Region, model.GameQueueType, model, top40);
            //top 100 leaderboard
            return View("Index", model);
        }

        public void PrepareModel(RiotApiConfig.Regions region, Enums.GameQueueType gameQueueType, LeaderboardModel model, List<LeagueDto.LeagueEntryDto> topEntries)
        {
            var ddragonVersions = _memoryCache.Get(CacheKeys.DataDragonVersionByRegionKey, DateTime.Now.AddDays(1),
                () => _riotClient.LolStaticData.GetVersionData(region));

            var summonerOrTeamIds = topEntries.Select(x => x.PlayerOrTeamId).ToArray();
            var summonerCacheKey = string.Format(CacheKeys.SummonerByRegionAndIdCacheKey, region, summonerOrTeamIds);

            var summoners = _cacheManager.Get(summonerCacheKey, DateTime.Now.AddDays(1),
                () => _riotClient.Summoner.GetSummonersById(region, summonerOrTeamIds));
           
            model.LeagueEntryModels = new List<LeaderboardModel.SummonerEntryModel>();
            foreach (var topEntry in topEntries)
            {
                LeaderboardModel.SummonerEntryModel summonerEntryModel = new LeaderboardModel.SummonerEntryModel();
                summonerEntryModel.LeagueEntry = topEntry;
                if (gameQueueType == Enums.GameQueueType.RANKED_SOLO_5x5)
                {
                    var summonerDto = summoners[topEntry.PlayerOrTeamId];
                    if (summonerDto != null)
                    {
                        summonerEntryModel.SummonerIcon =
                            $"http://ddragon.leagueoflegends.com/cdn/{ddragonVersions.FirstOrDefault()}/img/profileicon/{summonerDto.ProfileIconId}.png";
                    }
                }
                model.LeagueEntryModels.Add(summonerEntryModel);
            }
        }
    }
}