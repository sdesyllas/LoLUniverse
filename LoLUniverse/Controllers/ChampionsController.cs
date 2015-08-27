using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoLUniverse.Helpers;
using LoLUniverse.Models;
using LoLUniverse.Services;
using Ninject;
using Raven.Abstractions.Extensions;
using Raven.Client;
using RiotApi.Net.RestClient;
using RiotDtos = RiotApi.Net.RestClient.Dto;
using static RiotApi.Net.RestClient.Configuration.RiotApiConfig;

namespace LoLUniverse.Controllers
{
    public class ChampionsController : Controller
    {
        private static readonly DateTime CacheExpiry = DateTime.Now.AddDays(1);

        [Inject]
        public IDocumentStore DocumentStore { get; set; }

        private IRiotClient _riotClient;
        private ICacheManager _cacheManager;
        private IMemoryCache _memoryCache;

        public ChampionsController(IRiotClient riotClient, ICacheManager cacheManager, IMemoryCache memoryCache)
        {
            _riotClient = riotClient;
            _cacheManager = cacheManager;
            _memoryCache = memoryCache;
        }
        
        // GET: Champions
        public ActionResult Index(ChampionsModel model)
        {
            var allChampionsKey = string.Format(CacheKeys.AllChampionsByRegionKey, model.Region);
            var allChampions = _cacheManager.Get(allChampionsKey, CacheExpiry,
                () => _riotClient.Champion.RetrieveAllChampions(model.Region));

            var allStaticChampionsKey = string.Format(CacheKeys.AllStaticChampionsByRegionKey, model.Region);
            var allStaticChampions = _cacheManager.Get(allStaticChampionsKey, CacheExpiry,
                () => _riotClient.LolStaticData.GetChampionList(model.Region, champData: "all"));

            if (model.F2pOnly)
            {
                allChampions.Champions = allChampions.Champions.Where(x => x.FreeToPlay).ToList();
            }
            //model.Region = Regions.NA;
            PrepareModel(model, allChampions, allStaticChampions);
            return View(model);
        }

        public ActionResult Details(ChampionsModel.ChampionModel model)
        {
            //var champion = _riotClient.Champion.RetrieveChampionById(model.Region, model.ChampionId);

            var championKey = string.Format(CacheKeys.StaticChampionByRegionAndIdKey, model.Region, model.ChampionId);

            var staticChampion = _memoryCache.Get(championKey, CacheExpiry,
                () => _riotClient.LolStaticData.GetChampionById(model.Region, model.ChampionId, champData: "all"));
            
            PrepareDetailsModel(model, null, staticChampion);
            ViewBag.Title = $"Champion {model.StaticChampionDto.Name}";
            return View(model);
        }

        private void PrepareDetailsModel(ChampionsModel.ChampionModel championModel,
            RiotDtos.Champion.ChampionListDto.ChampionDto championDto,
            RiotDtos.LolStaticData.Champion.ChampionDto staticChampionDto)
        {
            var ddragonKeyVersionsKey = string.Format(CacheKeys.DataDragonVersionByRegionKey, championModel.Region);
            var ddragonVersions = _memoryCache.Get(ddragonKeyVersionsKey, DateTime.Now.AddDays(1),
                () => _riotClient.LolStaticData.GetVersionData(championModel.Region));

            var ddVersions = ddragonVersions as IList<string> ?? ddragonVersions.ToList();
            PrepareDetailsModel(championModel, championDto, staticChampionDto, ddVersions);
        }

        private void PrepareDetailsModel(ChampionsModel.ChampionModel championModel, RiotDtos.Champion.ChampionListDto.ChampionDto championDto,
            RiotDtos.LolStaticData.Champion.ChampionDto staticChampionDto, IEnumerable<string> ddVersions)
        {
            championModel.ChampionDto = championDto;
            championModel.StaticChampionDto = staticChampionDto;
            championModel.ChampionImage =
                 $"http://ddragon.leagueoflegends.com/cdn/{ddVersions.FirstOrDefault()}/img/champion/{championModel.StaticChampionDto.Image.Full}";
            championModel.ChampionImage =
                 $"http://ddragon.leagueoflegends.com/cdn/{ddVersions.FirstOrDefault()}/img/champion/{championModel.StaticChampionDto.Image.Full}";
            championModel.SplashImage =
                $"http://ddragon.leagueoflegends.com/cdn/img/champion/splash/{championModel.StaticChampionDto.Key}_{championModel.CurrentSkinId}.jpg";
            championModel.LoadingImage =
                $"http://ddragon.leagueoflegends.com/cdn/img/champion/loading/{championModel.StaticChampionDto.Key}_{championModel.CurrentSkinId}.jpg";
        }

        private void PrepareModel(ChampionsModel model, RiotDtos.Champion.ChampionListDto championListDto,
            RiotDtos.LolStaticData.Champion.ChampionListDto staticChampionListDto)
        {
            var ddragonKeyVersionsKey = string.Format(CacheKeys.DataDragonVersionByRegionKey, model.Region);
            var ddragonVersions = _memoryCache.Get(ddragonKeyVersionsKey, DateTime.Now.AddDays(1),
                () => _riotClient.LolStaticData.GetVersionData(model.Region));

            model.ChampionModels = new List<ChampionsModel.ChampionModel>();
            
            foreach (var championDto in championListDto.Champions)
            {
                ChampionsModel.ChampionModel championModel = new ChampionsModel.ChampionModel();
                var ddVersions = ddragonVersions as IList<string> ?? ddragonVersions.ToList();
                PrepareDetailsModel(championModel, championDto,
                    staticChampionListDto.Data.Values.FirstOrDefault(x => x.Id == championDto.Id), ddVersions);
                model.ChampionModels.Add(championModel);
            }
        }
    }
}