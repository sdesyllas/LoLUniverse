using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoLUniverse.Models.Search;

namespace LoLUniverse.Controllers
{
    public class SearchController : Controller
    {
        // GET: Search
        public ActionResult SearchBox()
        {
            return View();
        }

        public ActionResult SummonerResults(SearchSummoner searchSummoner)
        {
            return View(searchSummoner);
        }
    }
}