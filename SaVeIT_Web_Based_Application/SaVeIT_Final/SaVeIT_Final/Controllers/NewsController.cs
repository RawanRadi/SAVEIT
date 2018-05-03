using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Net;
using SaVeIT_Final.Models;
using System.IO;
using System.Globalization;

namespace Application2.Controllers
{
    public class NewsController : Controller
    {
        private Entities db = new Entities();

        // GET: Index
        public ActionResult Index()
        {
            return View(db.News.ToList());
        }
        // GET: News
        public ActionResult News(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }
        // GET: Admin
        public ActionResult Admin()
        {
            return View(db.News.ToList());
        }

        // GET: NewsController/Create
        public ActionResult Create()
        {
            News news = new News();
            return View(news);
        }
        // POST: NewsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "newId,newName,newContent,newImg,newDate")] News news, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    news.newImg = new byte[image.ContentLength];
                    image.InputStream.Read(news.newImg, 0, image.ContentLength);

                }


                news.newDate = @DateTime.Now.Date;
                
                    db.News.Add(news);
                db.SaveChanges();
                return RedirectToAction("Admin");
            }

            return View(news);
        }

        // GET: NewsController/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }
        // POST: NewsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "newId,newName,newAbst,newContent,newImg,newDate")] News news, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                var db = new Entities();
                if (image != null)
                {
                    news.newImg = new byte[image.ContentLength];
                    image.InputStream.Read(news.newImg, 0, image.ContentLength);

                }
                else
                {
                    news.newImg = news.newImg;
                }
                news.newDate = DateTime.Now;
                db.Entry(news).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Admin");
            }
            return View(news);
        }

        // GET: NewsController/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }
        // POST: NewsController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            News news = db.News.Find(id);
            db.News.Remove(news);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}