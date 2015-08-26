using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using LoLUniverse.Helpers;
using LoLUniverse.Models;
using LoLUniverse.Models.Search;
using NLog;
using RiotApi.Net.RestClient;
using RiotApi.Net.RestClient.Dto.League;
using RiotApi.Net.RestClient.Dto.Summoner;
using RiotApi.Net.RestClient.Helpers;
using LoLUniverse.Services;
using RiotApi.Net.RestClient.Dto.Stats;
using System;

namespace LoLUniverse.Controllers
{
    public class SearchController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private IRiotClient _riotClient;
        private ICacheManager _cacheManager;
        private IMemoryCache _memoryCache;

        public SearchController(IRiotClient riotClient, ICacheManager cacheManager, IMemoryCache memoryCache)
        {
            _riotClient = riotClient;
            _cacheManager = cacheManager;
            _memoryCache = memoryCache;
        }

        // GET: Search
        public ActionResult SearchBox()
        {
            return View();
        }

        public ActionResult SummonerResults(SearchSummoner searchSummoner)
        {
            try
            {
                logger.Debug($"Summoner.GetSummonersByName({searchSummoner.Region}, {searchSummoner.SummonerName})");
                var summonerKey = string.Format(CacheKeys.SummonerByRegionAndNameCacheKey, searchSummoner.Region,
                    searchSummoner.SummonerName);
                var summoners = _cacheManager.Get(summonerKey, DateTime.UtcNow.AddMinutes(60),
                    () => _riotClient.Summoner.GetSummonersByName(searchSummoner.Region,
                        searchSummoner.SummonerName));

                Dictionary<string, IEnumerable<LeagueDto>> summonerLeagues = null;
                try
                {
                    var summonerLeagueLey = string.Format(CacheKeys.SummonerLeagueByRegionAndIdCacheKey, searchSummoner.Region,
                        string.Join(",", summoners.Values.Select(x => x.Id).ToArray()));
                    logger.Debug($"League.GetSummonerLeaguesByIds({searchSummoner.Region}, {summoners.Values.Select(x => x.Id).ToArray()})");
                    summonerLeagues = _cacheManager.Get(summonerLeagueLey, DateTime.UtcNow.AddMinutes(60),
                        () => _riotClient.League.GetSummonerLeaguesByIds(searchSummoner.Region,
                            summoners.Values.Select(x => x.Id).ToArray()));

                }
                catch (RiotExceptionRaiser.RiotApiException exception)
                {
                    if (exception.RiotErrorCode == RiotExceptionRaiser.RiotErrorCode.DATA_NOT_FOUND)
                    {
                        logger.Debug(
                            $"League.GetSummonerLeaguesByIds({searchSummoner.Region}, {summoners.Values.Select(x => x.Id).ToArray()}) - - {exception.RiotErrorCode}");
                    }
                }

                PrepareSearchSummonerModel(searchSummoner, summoners, summonerLeagues);
                return View(searchSummoner);
            }
            catch (RiotExceptionRaiser.RiotApiException exception)
            {
                logger.Debug($"Summoner.GetSummonersByName({searchSummoner.Region}, {searchSummoner.SummonerName}) - {exception.RiotErrorCode}");
                if (exception.RiotErrorCode == RiotExceptionRaiser.RiotErrorCode.DATA_NOT_FOUND)
                {
                    return new HttpNotFoundResult("Summoner not found");
                }
                throw;
            }
        }

        public void PrepareSearchSummonerModel(SearchSummoner searchSummoner, Dictionary<string, SummonerDto> summonersDto, Dictionary<string, IEnumerable<LeagueDto>> summonerLeagues)
        {
            searchSummoner.SummonerModels = new List<SummonerModel>();

            var allChampionsKey = string.Format(CacheKeys.AllStaticChampionsByRegionKey, searchSummoner.Region);
            var allChampions =
                _cacheManager.Get(allChampionsKey, DateTime.Now.AddDays(1),
                    () => _riotClient.LolStaticData.GetChampionList(searchSummoner.Region, champData: "all"));

            var ddragonKeyVersionsKey = string.Format(CacheKeys.DataDragonVersionByRegionKey, searchSummoner.Region);

            var ddragonVersions = _memoryCache.Get(ddragonKeyVersionsKey, DateTime.Now.AddDays(1),
                () => _riotClient.LolStaticData.GetVersionData(searchSummoner.Region));

            foreach (var key in summonersDto.Keys)
            {
                SummonerModel summonerModel = new SummonerModel();
                summonerModel.PlayedChampions = new List<SummonerModel.PlayedChampionModel>();
                var summonerDto = summonersDto[key];

                summonerModel.SummonerKey = key;
                summonerModel.SummonerDto = summonerDto;
                summonerModel.ProfileImagePath =
                    $"http://ddragon.leagueoflegends.com/cdn/{ddragonVersions.FirstOrDefault()}/img/profileicon/{summonerDto.ProfileIconId}.png";

                summonerModel.LeagueModels = new List<SummonerModel.LeagueModel>();
                if (summonerLeagues != null && summonerLeagues.Count > 0 && summonerLeagues.ContainsKey(summonerDto.Id.ToString()))
                {
                    var summonerLeagueDtos = summonerLeagues[summonerDto.Id.ToString()];
                    foreach (var summonerLeagueDto in summonerLeagueDtos)
                    {
                        SummonerModel.LeagueModel leagueModel = new SummonerModel.LeagueModel();
                        leagueModel.LeagueKey = summonerDto.Id.ToString();
                        leagueModel.LeagueDto = summonerLeagueDto;
                        leagueModel.LeagueName = RiotApiEnumsDisplay.GetDisplayForQueueType(summonerLeagueDto.Queue);

                        summonerModel.LeagueModels.Add(leagueModel);
                    }
                }

                //summoner spells
                var recentGamesKey = string.Format(CacheKeys.SummonerRecentGamesbyRegionAndIdCacheKey, searchSummoner.Region,
                    summonerDto.Id);
                var recentGames = _cacheManager.Get(recentGamesKey, DateTime.Now.AddMinutes(60),
                    () => _riotClient.Game.GetRecentGamesBySummonerId(searchSummoner.Region, summonerDto.Id));
                var orderedRecentGames = recentGames.Games.OrderByDescending(x => x.CreateDate).ToList();
                summonerModel.RecentGames = orderedRecentGames;

                //summoner stats
                var playersStatskey = string.Format(CacheKeys.PlayerStatsByRegionAndIdCacheKey, searchSummoner.Region, summonerDto.Id);
                var playerStats = _cacheManager.Get(playersStatskey, DateTime.UtcNow.AddMinutes(60),
                    () => _riotClient.Stats.GetPlayerStatsBySummonerId(searchSummoner.Region, summonerDto.Id));
                summonerModel.PlayerStats = playerStats;

                //summoner ranked stats
                RankedStatsDto rankedStats = null;
                try
                {
                    var rankedStatsKey = string.Format(CacheKeys.PlayerRankedStatsByRegionAndIdCacheKey, searchSummoner.Region, summonerDto.Id);
                    rankedStats = _cacheManager.Get(rankedStatsKey, DateTime.UtcNow.AddMinutes(60),
                        () => _riotClient.Stats.GetRankedStatsBySummonerId(searchSummoner.Region, summonerDto.Id));
                }
                catch (RiotExceptionRaiser.RiotApiException exception)
                {
                    if (exception.RiotErrorCode == RiotExceptionRaiser.RiotErrorCode.DATA_NOT_FOUND)
                    {
                        logger.Debug(
                            $"Stats.GetRankedStatsBySummonerId({searchSummoner.Region}, {summonerDto.Id}) - - {exception.RiotErrorCode}");
                    }
                }

                //champion data
                if (rankedStats != null)
                {
                    var mostPlayedChampions =
                        rankedStats.Champions.OrderByDescending(x => x.Stats.TotalSessionsPlayed).ToList();
                    foreach (var statChampion in mostPlayedChampions)
                    {
                        var staticChampion = allChampions.Data.Values.FirstOrDefault(x => x.Id == statChampion.Id);
                        if (staticChampion != null)
                        {
                            SummonerModel.PlayedChampionModel playedChampion = new SummonerModel.PlayedChampionModel();
                            playedChampion.StaticChampion = staticChampion;
                            playedChampion.RankedStats = statChampion.Stats;
                            playedChampion.ChampionSpriteImage =
                                $"http://ddragon.leagueoflegends.com/cdn/{ddragonVersions.FirstOrDefault()}/img/champion/{staticChampion.Image.Full}";
                            summonerModel.PlayedChampions.Add(playedChampion);
                        }
                    }
                }

                searchSummoner.SummonerModels.Add(summonerModel);
            }
        }
    }
}