using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Rotativa;
using System.Web.Mvc;
using PagedList;
using LTW_NTTinh_2024802010018.Models;
namespace LTW_NTTinh_2024802010018.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        dbMinimartDataContext db = new dbMinimartDataContext();
        public ActionResult Index(int id)
        {
            if(id == 1)
            {
                ViewBag.id = id;
                return View();
            }
            else if(id == 2)
            {
                ViewBag.id = id;
                return View();
            }
            else if(id==3)
            {
                ViewBag.id = id;
                return View();
            }
            else
            {
                return View();
            }
        }
        public ActionResult report_emp()
        {

                var p2 = db.Employees.Where(s => s.Title == "Admin").OrderBy(s => s.STT).Select(s => s);
                ViewBag.count2 = p2.Count();
                var p3 = db.Employees.Where(s => s.Title == "Quản lý").OrderBy(s => s.STT).Select(s => s);
                ViewBag.count3 = p3.Count();
                var p4 = db.Employees.Where(s => s.Title == "Nhân viên").OrderBy(s => s.STT).Select(s => s);
                ViewBag.count4 = p4.Count();
                var p1 = db.Employees.OrderBy(s => s.STT).Select(s => s);
                ViewBag.count = p1.Count();
                return PartialView(p1);
        }
        public ActionResult report_ord()
        {
                decimal? temp = 0;
                var p1 = db.Orders.Where(p => p.PayStatus == true && p.InvoiceDate.Value.Month == DateTime.Now.Month);
                foreach (var item in p1)
                {
                    temp = temp + item.Amount;
                    ViewBag.EarnMonth = temp;
                    ViewBag.Target = Math.Round((((double)temp / 50000000) * 100), 2);
                }
                var p2 = db.Orders.Where(p => p.PayStatus == true && p.InvoiceDate.Value.Year == DateTime.Now.Year).OrderByDescending(p => p.Updated);
                foreach (var item in p2)
                {
                    temp = temp + item.Amount;
                    ViewBag.EarnYear = temp;
                }
                var p3 = db.Orders.Where(s => s.PayStatus == true).OrderBy(s => s.InvoiceDate).Select(s => s);
                foreach (var item in p3)
                {
                    temp = temp + item.Amount;
                    ViewBag.EarnAll = temp;
                }
                return PartialView(p3);
        }
        public ActionResult report_prod()
        {
               
                var p1 = db.Products.OrderBy(s => s.STT).Select(s => s);
                var p2 = db.Products.Where(s=>s.Quantity > 0).OrderBy(s => s.STT).Select(s => s);
                var p3 = db.Products.Where(s => s.Quantity <= 0).OrderBy(s => s.STT).Select(s => s);
                ViewBag.count = p1.Count();
                ViewBag.count1 = p2.Count();
                ViewBag.count2 = p3.Count();
                return PartialView(p1);            
        }
        public ActionResult Export_Employee()
        {
            return new ActionAsPdf("report_emp")
            {
                FileName = "Minimart_Employee_Report_" +DateTime.Now.ToShortDateString()+".pdf"
            };
           
        }
        public ActionResult Export_Employee1()
        {
            return new ActionAsImage("report_emp")
            {
                FileName = "Minimart_Employee_Report_" + DateTime.Now.ToShortDateString() + ".png"
            };

        }
        public ActionResult Export_Order()
        {
            return new ActionAsPdf("report_ord")
            {
                FileName = "Minimart_Order_Report_" + DateTime.Now.ToShortDateString() + ".pdf"
            };

        }
        public ActionResult Export_Order1()
        {
            return new ActionAsImage("report_ord")
            {
                FileName = "Minimart_Order_Report_" + DateTime.Now.ToShortDateString() + ".png"
            };

        }
        public ActionResult Export_Product()
        {
            return new ActionAsPdf("report_prod")
            {
                FileName = "Minimart_Product_Report_" + DateTime.Now.ToShortDateString() + ".pdf"
            };

        }
        public ActionResult Export_Product1()
        {
            return new ActionAsImage("report_prod")
            {
                FileName = "Minimart_Product_Report_" + DateTime.Now.ToShortDateString() + ".png"
            };

        }
    }
}