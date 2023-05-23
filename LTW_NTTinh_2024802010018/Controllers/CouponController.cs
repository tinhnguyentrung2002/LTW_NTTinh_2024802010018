using LTW_NTTinh_2024802010018.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
namespace LTW_NTTinh_2024802010018.Controllers
{
    [SessionTimeout]
    public class CouponController : Controller
    {
        // GET: Coupons
        dbMinimartDataContext db = new dbMinimartDataContext();
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult Coupon_manage(int? page, int id)
        {

            int size = 10;
            int PageNum = (page ?? 1);
            ViewBag.page = PageNum;
            if (id == 1)
            {
                var result = from s in db.Coupons where (DateTime.Now < s.CouponExpire) select s;
                return View(result.ToPagedList(PageNum, size));
            }
            else if (id == 2)
            {
                var result = from s in db.Coupons where (DateTime.Now >= s.CouponExpire) select s;
                return View(result.ToPagedList(PageNum, size));
            }
            else
            {
                var result = from s in db.Coupons select s;
                return View(result.ToPagedList(PageNum, size));
            }
        }
        public ActionResult SearchCoupon(string strSearch)
        {
            ViewBag.Count = 0;
            ViewBag.check = 0;
            ViewBag.Search = strSearch;
            if (!String.IsNullOrEmpty(strSearch) && strSearch != null)
            {
                var result1 = from s in db.Coupons where s.CouponID.Contains(strSearch)  select s;
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
            return PartialView();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Coupon p, FormCollection f, HttpPostedFileBase fFileUpload)
        {
         
                p.CouponID = f["CouponId"];
                p.CouponValue = int.Parse(f["CouponValue"]);
                p.CouponDescrption = f["Description"];
                p.CouponExpire = Convert.ToDateTime( f["CouponExpire"]);
                db.Coupons.InsertOnSubmit(p);
                db.SubmitChanges();
            return RedirectToAction("Coupon_manage", new { id = 1 });

        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var p = db.Coupons.SingleOrDefault(n => n.CouponID.CompareTo(id) == 0);
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
            var p = db.Coupons.SingleOrDefault(n => n.CouponID == f["CouponID"].ToString());
            if (ModelState.IsValid)
            {
                p.CouponID = f["CouponID"];
                p.CouponDescrption = f["Description"].ToString();
                p.CouponValue = int.Parse(f["CouponValue"]);
                p.CouponExpire = Convert.ToDateTime(f["CouponExpire"]);
                db.SubmitChanges();
                return RedirectToAction("Coupon_manage",new { id = 1 });
            }
            return View(p);
        }
        [HttpGet]
        public ActionResult Delete(string id)
        {
            var p = db.Coupons.SingleOrDefault(n => n.CouponID.CompareTo(id) == 0);
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
            var p = db.Coupons.SingleOrDefault(n => n.CouponID.CompareTo(id) == 0);
            if (p == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.Coupons.DeleteOnSubmit(p);
            db.SubmitChanges();
            return RedirectToAction("Coupon_manage", new { id = 1 });
        }
    }
}