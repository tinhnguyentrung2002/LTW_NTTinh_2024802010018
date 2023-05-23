using LTW_NTTinh_2024802010018.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Drawing;
using System.Runtime.InteropServices;

namespace LTW_NTTinh_2024802010018.Controllers
{
    public class OrderController : Controller
    {
        // GET: Orders
        dbMinimartDataContext db = new dbMinimartDataContext();
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult order_manage(int? page, int id)
        {

            ViewBag.id = id;
            int size = 10;
            int PageNum = (page ?? 1);
            ViewBag.cnt = 1;
            if (db.Orders.Count() == 0) ViewBag.cnt = 0;
            if (id == 1)
            {
                return View(db.Orders.Where(s=>s.PayStatus == true).OrderByDescending(s => s.Updated).Select(s => s).ToPagedList(PageNum, size));
            }
            else if(id == 2)
            {
                return View(db.Orders.Where(s => s.PayStatus == false).OrderBy(s => s.Updated).Select(s => s).ToPagedList(PageNum, size));
            }
            else
            {
                return View(db.Orders.OrderBy(s => s.Updated).Select(s => s).ToPagedList(PageNum, size));
            }
        }
        public ActionResult order_detail_manage(string id)
        {
  
            return PartialView(db.OrderDetails.Where(s => s.OrderID == id).OrderBy(s => s.ProductID).Select(s => s));
        }
        public ActionResult SearchOrder(string strSearch)
        {
            ViewBag.Count = 0;
            ViewBag.check = 0;
            ViewBag.Search = strSearch;
            if (!String.IsNullOrEmpty(strSearch) && strSearch != null)
            {
                var result1 = from s in db.Orders where s.OrderID.Contains(strSearch) orderby (s.Updated) select s;
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
        public ActionResult Edit(int? page,string id)
        {
            ViewBag.cnt = 1;
            if (db.Orders.Count() == 0) ViewBag.cnt = 0;
            int size = 10;
            int PageNum = (page ?? 1);
            return View(db.Orders.Where(s=>s.OrderID == id).OrderByDescending(s => s.Updated).Select(s => s).ToPagedList(PageNum, size));
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection f, string submitButton)
        {
            var p = db.Orders.SingleOrDefault(n => n.OrderID == f["ID"].ToString());
            switch (submitButton)
            {
                case "Đã giao":
                    {
                        p.DeliveryStatus = true;
                        p.Updated = DateTime.Now;
                        db.SubmitChanges();
                        return RedirectToAction("Edit");
                    }
                case "Đang xử lý":
                    {
                        p.DeliveryStatus = false;
                        p.Updated = DateTime.Now;
                        db.SubmitChanges();
                        return RedirectToAction("Edit");
                    }                
                default:
                    return View("order_manage", new {id = 1});
            }                       
        }
        [HttpGet]
        public ActionResult Delete(string id)
        {
            var p = db.Orders.SingleOrDefault(n => n.OrderID.CompareTo(id) == 0);
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
            var p = db.Orders.SingleOrDefault(n => n.OrderID.CompareTo(id) == 0);
            var p1 = db.OrderDetails.Where(n=> n.OrderID == p.OrderID);
            if (p == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            foreach (var item in p1)
            {
                var product = db.Products.SingleOrDefault(p2 => p2.ProductID.CompareTo(item.ProductID) == 0);
                product.Quantity = product.Quantity + item.Quantity;
            }        
            p.PayStatus = false; 
            p.DeliveryStatus= false;
            p.Updated = DateTime.Now;
            db.SubmitChanges();     
            return RedirectToAction("order_manage", new {id = 1});
        }
        public ActionResult product_order(string id)
        {
            var p = db.OrderDetails.Where(n=>n.OrderID == id);
            return PartialView(p);
        }
    }
}