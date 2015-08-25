using Ninject;
using RiotApi.Net.RestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LoLUniverse.Services
{
    public class StaticContext
    {
        private static IEnumerable<string> _dataDragonVersions = null;

        [Inject]
        private static IRiotClient _riotClient;

        public IEnumerable<string> DataDragonVersions
        {
            get
            {
                if(_dataDragonVersions == null)
                {
                    _dataDragonVersions = _riotClient.LolStaticData.GetVersionData(RiotApi.Net.RestClient.Configuration.RiotApiConfig.Regions.Global);
                }
                return _dataDragonVersions;
            }
        }
    }
}