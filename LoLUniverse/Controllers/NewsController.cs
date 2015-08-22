using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using LoLUniverse.Models;
using LoLUniverse.NoSqlContext;
using NLog;

namespace LoLUniverse.Controllers
{
    public class NewsController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public IEnumerable<NewsModel> GetLatestNews()
        {
            logger.Debug("/api/news");
            using (var session = RavenContext.CreateSession())
            {
                IList<NewsModel> news =
                    session.Query<NewsModel>()
                        .Take(10)
                        .OrderByDescending(x => x.CreatedDate)
                        .ToList();

                return news;
            }
        }


        public NewsModel GetNewEntryById(int id)
        {
            logger.Debug("/api/news");
            using (var session = RavenContext.CreateSession())
            {
                NewsModel newmodel = session.Query<NewsModel>().FirstOrDefault(x => x.NewsId == id);
                return newmodel;

            }
        }

        [System.Web.Http.HttpPost]
        public void CreateNewEntry(NewsModel newsModel)
        {
            using (var session = RavenContext.CreateSession())
            {
                session.Store(newsModel);
                session.SaveChanges(); // sends all changes to server
            }
        }
    }
}
