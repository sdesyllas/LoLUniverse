using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoLUniverse.Models;
using LoLUniverse.Services;
using Ninject;
using RiotApi.Net.RestClient;
using RiotApi.Net.RestClient.Configuration;
using RiotApi.Net.RestClient.Helpers;

namespace LoLUniverse.Controllers
{
    public class LeaderboardController : Controller
    {
        private static readonly string ChallengerTierLeagueKey = "ChallengerTierLeague_{0}_{1}";

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
            var cacheKey = string.Format(ChallengerTierLeagueKey, RiotApiConfig.Regions.EUNE,
                Enums.GameQueueType.RANKED_SOLO_5x5);

            var challengers = _cacheManager.Get(cacheKey, DateTime.Now.AddMinutes(60),
                () =>
                    _riotClient.League.GetChallengerTierLeagues(RiotApiConfig.Regions.EUNE,
                        Enums.GameQueueType.RANKED_SOLO_5x5));
           
            //get top 20 leaderboard using LINQ
            var top100 = challengers.Entries.OrderByDescending(x => x.LeaguePoints).Take(100).ToList();
            LeaderboardModel model = new LeaderboardModel
            {
                Region = RiotApiConfig.Regions.EUNE,
                GameQueueType = Enums.GameQueueType.RANKED_SOLO_5x5,
                LeagueEntries = top100,
                LeagueDto = challengers
            };
            //top 100 leaderboard
            return View(model);
        }

        public ActionResult FilterBoard(LeaderboardModel model)
        {
            var cacheKey = string.Format(ChallengerTierLeagueKey, model.Region, model.GameQueueType);

            var challengers = _cacheManager.Get(cacheKey, DateTime.Now.AddMinutes(60),
                () =>
                    _riotClient.League.GetChallengerTierLeagues(model.Region, model.GameQueueType));

            //get top 20 leaderboard using LINQ
            var top100 = challengers.Entries.OrderByDescending(x => x.LeaguePoints).Take(100).ToList();
            
            model.LeagueEntries = top100;
            model.LeagueDto = challengers;
            
            //top 100 leaderboard
            return View("Index", model);
        }
    }
}