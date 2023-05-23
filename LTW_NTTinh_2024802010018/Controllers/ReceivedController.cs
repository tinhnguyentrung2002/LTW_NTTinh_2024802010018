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
    public class ReceivedController : Controller
    {
        // GET: Received
        dbMinimartDataContext db = new dbMinimartDataContext();
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult GRN_manage(int? page)
        {
            int size = 10;
            int PageNum = (page ?? 1);
            ViewBag.cnt = 1;
            if (db.GoodsReceivedNotes.Count() == 0) ViewBag.cnt = 0;
                return View(db.GoodsReceivedNotes.OrderByDescending(s => s.STT).Select(s => s).ToPagedList(PageNum, size));

       
        }
        public ActionResult GRN_detail_manage(string id)
        {

            return PartialView(db.GoodsReceivedNotes.Where(s => s.GRNID == id).OrderBy(s => s.STT).Select(s => s).Single());
        }
        public ActionResult SearchGRN(string strSearch)
        {
            ViewBag.Count = 0;
            ViewBag.check = 0;
            ViewBag.Search = strSearch;
            if (!String.IsNullOrEmpty(strSearch) && strSearch != null)
            {
                var result1 = from s in db.GoodsReceivedNotes where s.GRNID.Contains(strSearch) orderby (s.STT) select s;
                if (result1 != null && result1.Count() != 0)
                {
                    ViewBag.Count = result1.Count();
                    ViewBag.check = 1;
                    return View(result1);
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
            ViewBag.Product = new SelectList(db.Products.ToList().OrderBy(n => n.ProductName), "ProductID", "ProductName");
            ViewBag.Provider = new SelectList(db.Providers.ToList().OrderBy(n => n.ProviderName), "ProviderID", "ProviderName");
            GoodsReceivedNote p = new GoodsReceivedNote();
            p.STT = db.GoodsReceivedNotes.Count() + 1;
            p.GRNID = "R00" + p.STT;
            ViewBag.IDP = p.GRNID;
            return PartialView();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(GoodsReceivedNote p, FormCollection f)
        {
            ViewBag.Product = new SelectList(db.Products.ToList().OrderBy(n => n.ProductName), "ProductID", "ProductName");
            ViewBag.Provider = new SelectList(db.Providers.ToList().OrderBy(n => n.ProviderName), "ProviderID", "ProviderName");

                p.STT = db.GoodsReceivedNotes.Count() + 1;
                p.GRNID = "R00" + p.STT;
                p.GRNName = f["Name"];
                p.ProductID = f["Product"];
                var p1 = db.Products.Where(n => n.ProductID == p.ProductID).Single();
                p.ProviderID = f["Provider"];
                var p2 = db.Providers.Where(n => n.ProviderID == p.ProviderID).Single();
                p.ProductName = p1.ProductName;
                p.ProviderName = p2.ProviderName;
                p.Quantity = int.Parse(f["Quantity"]);
                p.Date = Convert.ToDateTime(f["Date"]);
                p.Status = true;              
                int? temp = p.Quantity;
                p1.Quantity = p1.Quantity + (int)temp;
                p.Amount = p1.Price * p.Quantity;
                db.GoodsReceivedNotes.InsertOnSubmit(p);
                db.SubmitChanges();
                return RedirectToAction("GRN_manage");
             
        }
        
        [HttpGet]
        public ActionResult Delete(string id)
        {
            var p = db.GoodsReceivedNotes.SingleOrDefault(n => n.GRNID.CompareTo(id) == 0);
         
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
            var p = db.GoodsReceivedNotes.SingleOrDefault(n => n.GRNID.CompareTo(id) == 0);
            var p1 = db.Products.Where(n => n.ProductID == p.ProductID);
            if (p == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            int? temp = p.Quantity;
            var product = db.Products.SingleOrDefault(p2 => p2.ProductID.CompareTo(p.ProductID) == 0);
            product.Quantity = product.Quantity - (int)temp;
            p.Status = false;
            db.SubmitChanges();
            return RedirectToAction("GRN_manage");
        }
    }
}