using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using LTW_NTTinh_2024802010018.Models;
using System.Security.Cryptography;
using System.Text;

namespace LTW_NTTinh_2024802010018.Controllers
{
    [SessionTimeout]
    public class CustomerController : Controller
    {
        public static string EncryptPassword(string password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(password);
            byte[] targetData = md5.ComputeHash(fromData);
            string bytetostring = null;
            for (int i = 0; i < targetData.Length; i++)
            {
                bytetostring += targetData[i].ToString("X2");
            }
            return bytetostring;
        }
        // GET: Customer
        dbMinimartDataContext db = new dbMinimartDataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult customer_detail(string id)
        {
            return PartialView(db.Customers.Where(s => s.CustomerID == id).SingleOrDefault());
        }
        public ActionResult customer_manage(int id, int? page)
        {
            ViewBag.id = id;
            int size = 10;
            int PageNum = (page ?? 1);
            if (id == 1)
            {
                var result = from s in db.Customers where (s.Sex.CompareTo("Nam") == 0) orderby (s.STT) select s;
                return View(result.ToPagedList(PageNum, size));
            }
            else if (id == 2)
            {
                var result = from s in db.Customers where (s.Sex.CompareTo("Nữ") == 0) orderby (s.STT) select s;
                return View(result.ToPagedList(PageNum, size));
            }          
            else
            {
                var result = from s in db.Customers orderby (s.STT) select s;
                return View(result.ToPagedList(PageNum, size));
            }
        }
        public ActionResult SearchCustomer(string strSearch)
        {
            ViewBag.Count = 0;
            ViewBag.check = 0;
            if (!String.IsNullOrEmpty(strSearch) && strSearch != null)
            {
                var result1 = from s in db.Customers where s.CustomerID.Contains(strSearch) orderby (s.STT) select s;
                var result2 = from s in db.Customers where s.CustomerName.Contains(strSearch) orderby (s.STT) select s;
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
            return PartialView();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Customer e, User u, FormCollection f)
        {
          
                    e.STT = db.Customers.Count() + 1;
                    int? temp = 100 + e.STT;
                    e.CustomerID = "KH3" + DateTime.Now.Year.ToString() + temp;
                    e.CustomerName = f["Name"].ToString();
                    e.Address = f["Address"].ToString();
                    e.Birthday = Convert.ToDateTime(f["Birthday"].ToString());
                    e.Sex = f["Sex"].ToString();
                    e.Phone = f["Phone"].ToString();
                    e.Email = f["Email"].ToString();
                    e.Updated = DateTime.Now;
                    u.STT = db.Users.Count() + 1;
                    u.UserName = e.CustomerID;
                    u.Password = EncryptPassword(e.CustomerID + "@");
                    u.Permission = 3;
                    u.CustomerID = e.CustomerID;
                    db.Users.InsertOnSubmit(u);
                    db.Customers.InsertOnSubmit(e);
                    db.SubmitChanges();
                    return RedirectToAction("customer_manage", new { id = 3 });
                      
            //return View();
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var e = db.Customers.SingleOrDefault(s => s.CustomerID == id);
            ViewBag.Sex = e.Sex;
            if (e == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return PartialView(e);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(FormCollection f)
        {
            var e = db.Customers.SingleOrDefault(n => n.CustomerID == f["CustomerID"].ToString());
            ViewBag.Sex = e.Sex;
            if (ModelState.IsValid)
            {
                e.CustomerName = f["Name"].ToString();
                e.Address = f["Address"].ToString();
                e.Birthday = Convert.ToDateTime(f["Birthday"].ToString());
                e.Sex = f["Sex"].ToString();
                e.Phone = f["Phone"].ToString();
                e.Email = f["Email"].ToString();
                e.Updated = DateTime.Now;
                db.SubmitChanges();
                return RedirectToAction("customer_manage", new { id = 3 });
            }
            return View(e);
        }
        [HttpGet]
        public ActionResult Delete(string id)
        {
            var e = db.Customers.SingleOrDefault(s => s.CustomerID == id);
            if (e == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return PartialView(e);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(string id, FormCollection f)
        {
            var e = db.Customers.SingleOrDefault(s => s.CustomerID == id);
            if (e == null)
            {
                Response.StatusCode = 404;
                return null;
            }

            var u = db.Users.Where(s => s.CustomerID == id);
            if (u != null)
            {
                db.Users.DeleteAllOnSubmit(u);
                db.SubmitChanges();
            }
            db.Customers.DeleteOnSubmit(e);
            db.SubmitChanges();
            return RedirectToAction("customer_manage", new { id = 3 });
        }
    }
}