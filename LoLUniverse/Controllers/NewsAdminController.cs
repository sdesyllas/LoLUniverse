using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LoLUniverse.Models;
using NLog;
using Raven.Client;
using Ninject;

namespace LoLUniverse.Controllers
{
    [Authorize(Roles = "ContentAdmin")]
    public class NewsAdminController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        [Inject]
        public IDocumentStore DocumentStore { get; set; }
        // GET: NewsAdmin
        public ActionResult Index()
        {
            using (var session = DocumentStore.OpenSession())
            {
                IList<NewsModel> news = session.Query<NewsModel>().OrderByDescending(x => x.CreatedDate).ToList();
                return View(news);
            }
        }

        // GET: NewsAdmin/Details/5
        public ActionResult Details(int id)
        {
            using (var session = DocumentStore.OpenSession())
            {
                NewsModel newModel = session.Load<NewsModel>(id);
                return View(newModel);
            }
        }

        // GET: NewsAdmin/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NewsAdmin/Create
        [HttpPost]
        public ActionResult Create(NewsModel newsModel)
        {
            try
            {
                using (var session = DocumentStore.OpenSession())
                {
                    session.Store(newsModel);
                    session.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: NewsAdmin/Edit/5
        public ActionResult Edit(int id)
        {

            using (var session = DocumentStore.OpenSession())
            {
                NewsModel newModel = session.Load<NewsModel>(id);
                // use System.Net.WebUtility.HtmlDecode() to store unencoded HTML
                // http://r2d2.cc/2011/05/26/ckeditor-a-potentially-dangerous-request-form-value-was-detected-from-the-client/
                newModel.Content = System.Net.WebUtility.HtmlDecode(newModel.Content);
                return View(newModel);
            }
        }

        // POST: NewsAdmin/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, NewsModel newsModel)
        {

            try
            {
                // TODO: Add update logic here
                using (var session = DocumentStore.OpenSession())
                {
                    session.Store(newsModel);
                    session.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: NewsAdmin/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: NewsAdmin/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, NewsModel NewsModel)
        {
            try
            {
                using (var session = DocumentStore.OpenSession())
                {
                    session.Delete<NewsModel>(id);
                    session.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch
            {
                return View();
            }
        }
    }
}
