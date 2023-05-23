using LTW_NTTinh_2024802010018.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.IO;

namespace LTW_NTTinh_2024802010018.Controllers
{
    [SessionTimeout]
    public class NewsController : Controller
    {
        // GET: News
        dbMinimartDataContext db = new dbMinimartDataContext();
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult News_manage(int? page, int id)
        {

            int size = 10;
            int PageNum = (page ?? 1);
            ViewBag.page = PageNum;
            if (id == 1)
            {
                var result = from s in db.News orderby (s.Updated) descending select s;
                return View(result.ToPagedList(PageNum, size));
            }
            else
            {
                var result = from s in db.News orderby (s.Updated) ascending select s;
                return View(result.ToPagedList(PageNum, size));
            }
        }
        public ActionResult SearchNews(string strSearch)
        {
            ViewBag.Count = 0;
            ViewBag.check = 0;
            ViewBag.Search = strSearch;
            if (!String.IsNullOrEmpty(strSearch) && strSearch != null)
            {
                var result1 = from s in db.News where s.NewsID.Contains(strSearch) orderby (s.Updated) select s;
                var result2 = from s in db.News where s.NewsTitle.Contains(strSearch) orderby (s.Updated) select s;
                var result3 = from s in db.News where s.NewsAuthor.Contains(strSearch) orderby (s.Updated) select s;
                if (result1 != null && result1.Count() != 0)
                {
                    ViewBag.Count = result1.Count();
                    ViewBag.check = 1;
                    return View(result1);
                }
                else if (result2 != null && result2.Count() != 0)
                {
                    ViewBag.Count = result2.Count();
                    ViewBag.check = 1;
                    return View(result2);
                }
                else if (result3 != null && result3.Count() != 0)
                {
                    ViewBag.Count = result3.Count();
                    ViewBag.check = 1;
                    return View(result3);
                }
                else
                {
                    ViewBag.Count = 0;
                    ViewBag.check = 0;
                    return View();
                }

            }
            return View();
        }
        [HttpGet]
        public ActionResult Create()
        {
            New p = new New();
            int temp = 100 + db.News.Count() + 1;
            p.NewsID = "News" + temp;
            ViewBag.IDP = p.NewsID;
            return PartialView();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(New p, FormCollection f, HttpPostedFileBase fFileUpload)
        {

            if (ModelState.IsValid)
            {
                if (fFileUpload == null)
                {
                    int temp = 100 + db.News.Count() + 1;
                    p.NewsID = "News" + temp;
                    p.NewsAuthor = f["Name"];
                    p.NewsTitle = f["Title"];
                    p.NewsContent = f["Description"];
                    p.Updated = DateTime.Now;
                    p.Thumbnail = "default.jpg";
                    ViewBag.IDP = p.NewsID;
                    db.News.InsertOnSubmit(p);
                    db.SubmitChanges();
                    return RedirectToAction("News_manage", new { id = 1 });
                }
                else
                {
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), sFileName);
                    if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                    }
                    int temp = 100 + db.News.Count() + 1;
                    p.NewsID = "News" + temp;
                    p.NewsAuthor = f["Name"];
                    p.NewsTitle = f["Title"];
                    p.NewsContent = f["Description"];
                    p.Updated = DateTime.Now;
                    p.Thumbnail = sFileName;
                    ViewBag.IDP = p.NewsID;
                    db.News.InsertOnSubmit(p);
                    db.SubmitChanges();
                    return RedirectToAction("News_manage", new { id = 1 });
                }
                    
            }
            return RedirectToAction("News_manage", new { id = 1 });
        }
      
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var p = db.News.SingleOrDefault(n => n.NewsID.CompareTo(id) == 0);
            if (p == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return PartialView(p);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection f, HttpPostedFileBase fFileUpload)
        {
            var p = db.News.SingleOrDefault(n => n.NewsID == f["NewsID"].ToString());
            if (ModelState.IsValid)
            {
                if (ModelState.IsValid)
                {
                    if (fFileUpload != null)
                    {
                        var sFileName = Path.GetFileName(fFileUpload.FileName);
                        var path = Path.Combine(Server.MapPath("~/Images"), sFileName);
                        if (!System.IO.File.Exists(path))
                        {
                            fFileUpload.SaveAs(path);
                        }
                        p.Thumbnail = sFileName;
                    }
                    p.NewsAuthor = f["Name"];
                    p.NewsTitle = f["Title"];
                    p.NewsContent = f["Description"];
                    p.Updated = DateTime.Now;
                    db.SubmitChanges();
                    return RedirectToAction("News_manage", new {id = 1});
                }
            }
            return RedirectToAction("News_manage", new { id = 1 });
        }
        [HttpGet]
        public ActionResult Delete(string id)
        {
            var p = db.News.SingleOrDefault(n => n.NewsID.CompareTo(id) == 0);
            if (p == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return PartialView(p);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(string id)
        {
            var p = db.News.SingleOrDefault(n => n.NewsID.CompareTo(id) == 0);
            if (p == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.News.DeleteOnSubmit(p);
            db.SubmitChanges();
            return RedirectToAction("News_manage", new {id = 1});
        }
    }
}