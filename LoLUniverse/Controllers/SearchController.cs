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
using WebGrease.Css.Extensions;

namespace LoLUniverse.Controllers
{
    public class SearchController : Controller
    {
        IRiotClient _riotClient;
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public SearchController(IRiotClient riotClient)
        {
            _riotClient = riotClient;
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

                var summoners = _riotClient.Summoner.GetSummonersByName(searchSummoner.Region,
                    searchSummoner.SummonerName);

                Dictionary<string, IEnumerable<LeagueDto>> summonerLeagues = null;
                try
                {
                    logger.Debug($"League.GetSummonerLeaguesByIds({searchSummoner.Region}, {summoners.Values.Select(x => x.Id).ToArray()})");
                    summonerLeagues = _riotClient.League.GetSummonerLeaguesByIds(searchSummoner.Region,
                        summoners.Values.Select(x => x.Id).ToArray());
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
            
            foreach (var key in summonersDto.Keys)
            {
                SummonerModel summonerModel = new SummonerModel();

                var summonerDto = summonersDto[key];

                summonerModel.SummonerKey = key;
                summonerModel.SummonerDto = summonerDto;
                summonerModel.ProfileImagePath =
                    $"http://ddragon.leagueoflegends.com/cdn/5.2.1/img/profileicon/{summonerDto.ProfileIconId}.png";

                summonerModel.LeagueModels = new List<SummonerModel.LeagueModel>();
                if (summonerLeagues!=null && summonerLeagues.Count > 0 && summonerLeagues.ContainsKey(summonerDto.Id.ToString()))
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
                searchSummoner.SummonerModels.Add(summonerModel);
            }
        }
    }
}