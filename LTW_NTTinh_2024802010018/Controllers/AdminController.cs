using LTW_NTTinh_2024802010018.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using LTW_NTTinh_2024802010018;

namespace LTW_NTTinh_2024802010018.Controllers
{
    [SessionTimeout]
    public class AdminController : Controller
    {
        // GET: Admin
       
        dbMinimartDataContext db = new dbMinimartDataContext();

        public ActionResult Index()
        {
            decimal? temp = 0;
            if (Session["UserName"] == null)
            {
                return RedirectToAction("user_login", "User");
            }
            var p1 = db.Orders.Where(p=>p.PayStatus == true && p.InvoiceDate.Value.Month == DateTime.Now.Month);
            
            foreach(var item in p1)
            {
                temp = temp + item.Amount;
                ViewBag.EarnMonth = temp;
            }
            var p2 = db.Orders.Where(p => p.PayStatus == true && p.InvoiceDate.Value.Year == DateTime.Now.Year).OrderByDescending(p=>p.Updated);
            decimal? temp1 = 0;
            foreach (var item in p2)
            {
                temp1 = temp1 + item.Amount;
                ViewBag.EarnYear = temp1;
            }
           
            ViewBag.User = db.Users.Count();
            ViewBag.Order = db.Orders.Where(p=>p.PayStatus == true).Count();
            ViewBag.Target = Math.Round((((double)temp / 50000000) * 100), 2);
           
            return View();
        }
        public ActionResult nav_partial()
        {
                var mess = db.Responses.Where(s => s.ResponseStatus == false).Select(s => s);
                ViewBag.count = db.Responses.Where(s => s.ResponseStatus == false).Count();
                return PartialView(mess);
                  
        }
        public ActionResult left_nav_partial()
        {
            return PartialView();
        }
        public ActionResult footer_partial()
        {
            return PartialView();
        }
        public ActionResult user_detail(string id)
        {
            return PartialView(db.Employees.Where(s => s.EmployeeID == id).SingleOrDefault());
        }
        public ActionResult order()
        {
            return PartialView(db.Orders.Where(s=>s.PayStatus == true).OrderByDescending(s=>s.InvoiceDate).Select(s => s));
        }
        public ActionResult employee()
        {
            return PartialView(db.Employees.Select(s=>s));
        }

    }
}