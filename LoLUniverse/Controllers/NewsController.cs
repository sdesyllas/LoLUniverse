﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;
using LoLUniverse.Models;
using NLog;
using Raven.Client;
using Ninject;

namespace LoLUniverse.Controllers
{
    public class NewsController : ApiController
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [Inject]
        public IDocumentStore DocumentStore { get; set; }

        public IEnumerable<NewsModel> GetLatestNews()
        {
            logger.Debug("/api/news");
            using (var session = DocumentStore.OpenSession())
            {
                List<NewsModel> news =
                    session.Query<NewsModel>().Where(x=> x.CreatedDate <= DateTime.Now)
                        .Take(10)
                        .OrderByDescending(x => x.CreatedDate)
                        .ToList();
                news.ForEach(x => x.Content = System.Net.WebUtility.HtmlDecode(x.Content));
                return news;
            }
        }


        public NewsModel GetNewEntryById(int id)
        {
            logger.Debug("/api/news");
            using (var session = DocumentStore.OpenSession())
            {
                NewsModel newmodel = session.Query<NewsModel>().FirstOrDefault(x => x.Id == id);
                return newmodel;

            }
        }

        [System.Web.Mvc.HttpPost]
        public void CreateNewEntry(NewsModel newsModel)
        {
            using (var session = DocumentStore.OpenSession())
            {
                session.Store(newsModel);
                session.SaveChanges(); // sends all changes to server
            }
        }
    }
}
