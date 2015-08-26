namespace LoLUniverse.Helpers
{
    public class CacheKeys
    {
        internal static readonly string AllStaticChampionsByRegionKey = "AllStaticChampions_{0}";
        internal static readonly string SummonerByRegionAndIdCacheKey = "SummonerById_{0}_{1}";
        internal static readonly string TeamByRegionAndIdCacheKey = "TeamById_{0}_{1}";
        internal static readonly string SummonerByRegionAndNameCacheKey = "SummonerByName_{0}_{1}";
        internal static readonly string SummonerLeagueByRegionAndIdCacheKey = "SummonerLeagues_{0}_{1}";
        internal static readonly string SummonerRecentGamesbyRegionAndIdCacheKey = "SummonerRecentGames_{0}_{1}";
        internal static readonly string PlayerStatsByRegionAndIdCacheKey = "PlayerStats_{0}_{1}";
        internal static readonly string PlayerRankedStatsByRegionAndIdCacheKey = "PlayerRankedStats_{0}_{1}";
        internal static readonly string DataDragonVersionByRegionKey = "DataDragonVersions_{0}";
        internal static readonly string ChampionStaticByIdKey = "ChampionStaticById_{0}";
        internal static readonly string ChallengerTierLeagueKey = "ChallengerTierLeague_{0}_{1}";
    }
}