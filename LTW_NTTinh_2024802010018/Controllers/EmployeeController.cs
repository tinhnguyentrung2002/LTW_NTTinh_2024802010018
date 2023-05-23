using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using LTW_NTTinh_2024802010018.Models;
using PagedList;
namespace LTW_NTTinh_2024802010018.Controllers
{
    [SessionTimeout]
    public class EmployeeController : Controller
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
        dbMinimartDataContext db = new dbMinimartDataContext();
        // GET: Employee
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult user_detail(string id)
        {
            return PartialView(db.Employees.Where(s => s.EmployeeID == id).SingleOrDefault());
        }
        public ActionResult user_manage(int id, int ?page)
        {
            ViewBag.id = id;
            int size = 10;
            int PageNum = (page ?? 1);
            if (id == 1)
            {
                var result = from s in db.Employees where(s.Title.CompareTo("Nhân viên") == 0) orderby (s.STT) select s;
                return View(result.ToPagedList(PageNum, size));
            }
            else if (id == 2)
            {
                var result = from s in db.Employees where (s.Title.CompareTo("Quản lý") == 0) orderby (s.STT) select s;
                return View(result.ToPagedList(PageNum, size));
            }
            else if (id == 3)
            {
                var result = from s in db.Employees where (s.Title.CompareTo("Admin") == 0) orderby (s.STT) select s;
                return View(result.ToPagedList(PageNum, size));
            }
            else
            {
                var result = from s in db.Employees orderby (s.STT) select s;
                return View(result.ToPagedList(PageNum, size));
            }
        }
        public ActionResult SearchEmployee(string strSearch)
        {
            ViewBag.Count = 0;
            ViewBag.check = 0;
            if (!String.IsNullOrEmpty(strSearch) && strSearch != null)
            {
                var result1 = from s in db.Employees where s.EmployeeID.Contains(strSearch) orderby (s.STT) select s;
                var result2 = from s in db.Employees where s.EmployeeName.Contains(strSearch) orderby (s.STT) select s;
                if(result1 != null && result1.Count() != 0)
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
        public ActionResult Create(Employee e, User u, FormCollection f)
        {
           
                if (f["Title"].ToString().CompareTo("Quản lý") == 0)
                {
                    e.Title = f["Title"].ToString();
                    e.STT = db.Employees.Count() + 1;
                    int? temp = 100 + e.STT;
                    e.EmployeeID = "QL1" + DateTime.Now.Year.ToString()+ temp;
                    e.EmployeeName = f["Name"].ToString();
                    e.Address = f["Address"].ToString();
                    e.Birthday = Convert.ToDateTime(f["Birthday"].ToString());
                    e.Sex = f["Sex"].ToString();
                    e.Phone = f["Phone"].ToString();
                    e.Email = f["Email"].ToString();
                    e.Updated = DateTime.Now;
                    u.STT = db.Users.Count() + 1;
                    u.UserName =  e.EmployeeID;
                    u.Password = EncryptPassword( e.EmployeeID + "@");
                    u.Permission = 1;
                    u.EmployeeID = e.EmployeeID;
                    db.Users.InsertOnSubmit(u);
                    db.Employees.InsertOnSubmit(e);
                    db.SubmitChanges();
                    return RedirectToAction("user_manage", new { id = 4 });
                }
                if (f["Title"].ToString().CompareTo("Nhân viên") == 0)
                {
                    e.Title = f["Title"].ToString();
                    e.STT = db.Employees.Count() + 1;
                    int? temp = 100 + e.STT;
                    e.EmployeeID = "NV2" + DateTime.Now.Year.ToString()+ temp;
                    e.EmployeeName = f["Name"].ToString();
                    e.Address = f["Address"].ToString();
                    e.Birthday = Convert.ToDateTime(f["Birthday"].ToString());
                    e.Sex = f["Sex"].ToString();
                    e.Phone = f["Phone"].ToString();
                    e.Email = f["Email"].ToString();
                    e.Updated = DateTime.Now;
                    u.STT = db.Users.Count() + 1;
                    u.UserName = e.EmployeeID;
                    u.Password = EncryptPassword(e.EmployeeID+"@");
                    u.Permission = 2;
                    u.EmployeeID = e.EmployeeID;
                    db.Users.InsertOnSubmit(u);
                    db.Employees.InsertOnSubmit(e);
                    db.SubmitChanges();
                    return RedirectToAction("user_manage", new {id = 4});
                        
            }
            return View();
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            var e = db.Employees.SingleOrDefault(s => s.EmployeeID == id);
            ViewBag.Title = e.Title;
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
            var e = db.Employees.SingleOrDefault(n => n.EmployeeID == f["EmployeeID"].ToString());
            ViewBag.Title = e.Title;
            ViewBag.Sex = e.Sex;
            if (ModelState.IsValid)
            {

                e.Title = f["Title"].ToString();
                e.EmployeeName = f["Name"].ToString();
                e.Address = f["Address"].ToString();
                e.Birthday = Convert.ToDateTime(f["Birthday"].ToString());
                e.Sex = f["Sex"].ToString();
                e.Phone = f["Phone"].ToString();
                e.Email = f["Email"].ToString();
                e.Updated = DateTime.Now;
                db.SubmitChanges();
                return RedirectToAction("user_manage",new { id = 4 });
            }
            return View(e);
        }
        [HttpGet]
        public ActionResult Delete(string id)
        {
            var e = db.Employees.SingleOrDefault(s => s.EmployeeID == id);
            if (e == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return PartialView(e);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(string id)
        {
            var e = db.Employees.SingleOrDefault(s => s.EmployeeID == id);
            if (e == null)
            {
                Response.StatusCode = 404;
                return null;
            }
           
            var u = db.Users.Where(s => s.EmployeeID == id);
            if (u != null)
            {
                db.Users.DeleteAllOnSubmit(u);
                db.SubmitChanges();
            }
            db.Employees.DeleteOnSubmit(e);
            db.SubmitChanges();
            return RedirectToAction("user_manage",new { id = 4 });
        }
    }
}