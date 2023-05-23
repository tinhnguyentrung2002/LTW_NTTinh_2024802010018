using LTW_NTTinh_2024802010018.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using PagedList;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Web.UI;

namespace LTW_NTTinh_2024802010018.Controllers
{
    [SessionTimeout]
    public class ProviderController : Controller
    {
        // GET: Provider
        dbMinimartDataContext db = new dbMinimartDataContext();
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult provider_manage(int ?page)
        {      
           
            int size = 10;
            int PageNum = (page ?? 1);
            ViewBag.page = PageNum;
            var result = from s in db.Providers orderby (s.STT) select s;
            return View(result.ToPagedList(PageNum,size));
        }
        public ActionResult SearchProvider(string strSearch)
        {           
            ViewBag.Count = 0;
            ViewBag.check = 0;
            ViewBag.Search = strSearch;
            if (!String.IsNullOrEmpty(strSearch) && strSearch != null)
            {
                var result1 = from s in db.Providers where s.ProviderID.Contains(strSearch) orderby (s.STT) select s;
                var result2 = from s in db.Providers where s.ProviderName.Contains(strSearch) orderby (s.STT) select s;
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
            Provider p = new Provider();
            p.STT = db.Providers.Count() + 1;
            p.ProviderID = "P00" + p.STT;
            ViewBag.IDP = p.ProviderID;
            return PartialView();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Provider p, FormCollection f, HttpPostedFileBase fFileUpload)
        {
            if (fFileUpload == null)
            {
                p.STT = db.Providers.Count() + 1;
                p.ProviderID = "P00" + p.STT;
                p.ProviderName = f["Name"];
                p.Description = f["Description"];
                p.Logo = "default_image.png";
                ViewBag.IDP = p.ProviderID;
                db.Providers.InsertOnSubmit(p);
                db.SubmitChanges();
                return RedirectToAction("provider_manage", new { page = ViewBag.page });
            }
            else
            {
                if (ModelState.IsValid)
                {
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), sFileName);
                    if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                    }
                    p.STT = db.Providers.Count() + 1;
                    p.ProviderID = "P00" + p.STT;
                    p.ProviderName = f["Name"];
                    p.Description = f["Description"];
                    p.Logo = sFileName;
                    ViewBag.IDP = p.ProviderID;
                    db.Providers.InsertOnSubmit(p);
                    db.SubmitChanges();
                    return RedirectToAction("provider_manage", new {page = ViewBag.page});
                }
            }       
            return View();
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var p = db.Providers.SingleOrDefault(n => n.ProviderID.CompareTo(id)==0);
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
            var p = db.Providers.SingleOrDefault(n => n.ProviderID == f["ProviderID"].ToString());
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
                    p.Logo = sFileName;
                }
               
                p.ProviderName = f["Name"].ToString();
                p.Description = f["Description"].ToString();
                db.SubmitChanges();
                return RedirectToAction("provider_manage");
            }
            return View(p);
        }
        [HttpGet]
        public ActionResult Delete(string id)
        {
            var p = db.Providers.SingleOrDefault(n => n.ProviderID.CompareTo(id) == 0);
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
            var p = db.Providers.SingleOrDefault(n => n.ProviderID.CompareTo(id) == 0);
            if (p == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.Providers.DeleteOnSubmit(p);
            db.SubmitChanges();
            return RedirectToAction("provider_manage");
        }
    }
}