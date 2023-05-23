using LTW_NTTinh_2024802010018.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using LTW_NTTinh_2024802010018;
namespace LTW_NTTinh_2024802010018.Controllers
{
    [SessionTimeout]
    public class CategoryController : Controller
    {
     
        // GET: Category
        dbMinimartDataContext db = new dbMinimartDataContext();
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult Category_manage(int? page)
        {

            int size = 10;
            int PageNum = (page ?? 1);
            ViewBag.page = PageNum;
            var result = from s in db.Categories orderby (s.STT) select s;
            return View(result.ToPagedList(PageNum, size));
        }
        public ActionResult SearchCategory(string strSearch)
        {
            ViewBag.Count = 0;
            ViewBag.check = 0;
            ViewBag.Search = strSearch;
            if (!String.IsNullOrEmpty(strSearch) && strSearch != null)
            {
                var result1 = from s in db.Categories where s.CategoryID.Contains(strSearch) orderby (s.STT) select s;
                var result2 = from s in db.Categories where s.CategoryName.Contains(strSearch) orderby (s.STT) select s;
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
            Category p = new Category();
            p.STT = db.Categories.Count() + 1;
            p.CategoryID = "C" + p.STT;
            ViewBag.IDP = p.CategoryID;
            return PartialView();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Category p, FormCollection f, HttpPostedFileBase fFileUpload)
        {          
                if (ModelState.IsValid)
                {                 
                    p.STT = db.Categories.Count() + 1;
                    p.CategoryID = "C" + p.STT;
                    p.CategoryName = f["Name"];
                    p.Description = f["Description"];
                    p.Updated = DateTime.Now;
                    ViewBag.IDP = p.CategoryID;
                    db.Categories.InsertOnSubmit(p);
                    db.SubmitChanges();
                    return RedirectToAction("Category_manage", new { page = ViewBag.page });
                }           
            return View();
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var p = db.Categories.SingleOrDefault(n => n.CategoryID.CompareTo(id) == 0);
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
            var p = db.Categories.SingleOrDefault(n => n.CategoryID == f["CategoryID"].ToString());
            if (ModelState.IsValid)
            {
                p.CategoryID = f["CategoryID"].ToString();
                p.CategoryName = f["Name"].ToString();
                p.Description = f["Description"].ToString();
                db.SubmitChanges();
                return RedirectToAction("Category_manage");
            }
            return View(p);
        }
        [HttpGet]
        public ActionResult Delete(string id)
        {
            var p = db.Categories.SingleOrDefault(n => n.CategoryID.CompareTo(id) == 0);
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
            var p = db.Categories.SingleOrDefault(n => n.CategoryID.CompareTo(id) == 0);
            if (p == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.Categories.DeleteOnSubmit(p);
            db.SubmitChanges();
            return RedirectToAction("Category_manage");
        }
    }
}