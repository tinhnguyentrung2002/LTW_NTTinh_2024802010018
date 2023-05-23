using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Cryptography;
using LTW_NTTinh_2024802010018.Models;
using System.Text;

namespace LTW_NTTinh_2024802010018.Controllers
{
    public class UserController : Controller
    {
        dbMinimartDataContext data = new dbMinimartDataContext();
        public static string EncryptPassword(string password)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(password);
            byte[] targetData = md5.ComputeHash(fromData);
            string bytetostring = null;
            for(int i =0;i<targetData.Length;i++)
            {
                bytetostring += targetData[i].ToString("X2");
            }
            return bytetostring;
        }
        public bool ValidEmail(string mail)
        {
            string format = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            Regex reg = new Regex(format);
            if (!reg.IsMatch(mail))
            {
                return false;

            }
            return true;

        }
        [HttpGet]
        public ActionResult user_register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult user_register(FormCollection register, User user, Customer customer)
        {
            var sName = register["CustomerName"];
            var suserName = register["UserName"];
            var sPassword = register["Password"];
            var dBirthday = String.Format("{0:MM/dd/yyyy}", register["Birthday"]); 
            var sre_password = register["rePassword"];
            var sAddress = register["Address"];
            var sphoneNum = register["Phone"];
            var semail = register["Email"];
            var sex = register["Sex"];
            ViewBag.a = sName;
            ViewBag.b = dBirthday;
            ViewBag.c = sAddress;
            ViewBag.d = semail;
            ViewBag.e = sphoneNum;
            if (String.IsNullOrEmpty(sName)) ViewData["err1"] = "Họ tên không được rỗng!";
            else if (String.IsNullOrEmpty(suserName)) ViewData["err2"] = "Tên đăng nhập không được rỗng!";
            else if (String.IsNullOrEmpty(sPassword)) ViewData["err3"] = "Phải nhập mật khẩu!";
            else if (String.IsNullOrEmpty(sre_password)) ViewData["err4"] = "Phải nhập lại mật khẩu!";
            else if (sPassword != sre_password) ViewData["err4"] = "Mật khẩu nhập lại không khớp";   
            else if (String.IsNullOrEmpty(sphoneNum)) ViewData["err5"] = "Số điện thoại được rỗng";
            else if (String.IsNullOrEmpty(sphoneNum)) ViewData["err6"] = "Địa chỉ không được rỗng";
            else if (sphoneNum.Length != 10) ViewData["err7"] = "Số điện thoại không phù hợp";
            else if (String.IsNullOrEmpty(semail)) ViewData["err8"] = "Email không được rỗng";
            else if (ValidEmail(semail.ToString()) == false) ViewData["err9"] = "Email sai định dạng";
            else if (data.Users.SingleOrDefault(n => n.UserName == suserName) != null) ViewBag.ThongBao = "Tên đăng nhập đã tồn tại";
            else
            {
                if (ModelState.IsValid)
                {
                    user.STT = data.Users.Count() + 1;
                    user.UserName = suserName;
                    user.Password = EncryptPassword(sPassword);
                    user.Permission = 3;
                    user.EmployeeID = null;
                    customer.STT = data.Customers.Count() + 1;
                    int? temp = 100 + customer.STT;
                    customer.CustomerID = "KH" + user.Permission + DateTime.Now.Year + temp;
                    customer.CustomerName = sName;
                    customer.Address = sAddress;
                    customer.Sex = sex;
                    customer.Phone = sphoneNum;
                    customer.Email = semail;
                    customer.Updated = DateTime.Now;
                    customer.Birthday = DateTime.Parse(dBirthday);
                    user.CustomerID = "KH" + user.Permission + DateTime.Now.Year + temp;
                    data.Users.InsertOnSubmit(user);
                    data.Customers.InsertOnSubmit(customer);
                    data.SubmitChanges();
                    ViewBag.ThongBao1 = "Chúc mừng đăng ký thành công!";
                    return RedirectToAction("user_login");
                }
            }
            return this.user_register();    
        }
        // GET: User
        [HttpGet]
        public ActionResult user_login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult user_login(FormCollection login)
        {
            var saccountName = login["UserName"];
            var sPassword = login["Password"];
            if (String.IsNullOrEmpty(saccountName)) ViewData["Err1"] = " Bạn chưa nhập tên đăng nhập!";
            else if (String.IsNullOrEmpty(sPassword)) ViewData["Err2"] = " Phải nhập mật khẩu!";
            else
            {
                User us = data.Users.SingleOrDefault(n => n.UserName == saccountName && n.Password == EncryptPassword(sPassword)) ;
                if (us != null && us.Permission == 0)
                {
                    ViewBag.ThongBao = "Chúc mừng đăng nhập thành công!";
                    //Session["UserName"] = us.UserName;
                    Session["Password"] = us.Password;
                    Session["EmployeeID"] = us.EmployeeID;
                    Session["Permission"] = us.Permission;
                    HttpContext.Session["UserName"] = us.UserName;
                    if (login["remember"].Contains("true"))
                    {
                        Response.Cookies["UserName"].Value = saccountName;
                        Response.Cookies["Password"].Value = sPassword;
                        Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(1);
                        Response.Cookies["Password"].Expires = DateTime.Now.AddDays(1);

                    }
                    else
                    {
                        Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies["Password"].Expires = DateTime.Now.AddDays(-1);
                    }
                    return RedirectToAction("Index", "Admin");
                }
                else if(us != null && us.Permission == 3)
                {
                    if (Session["shopping_cart"]==null)
                    {
                        ViewBag.ThongBao = "Chúc mừng đăng nhập thành công!";
                        Session["UserName"] = us.UserName;
                        Session["Password"] = us.Password;
                        Session["Permission"] = us.Permission;
                        Session["CustomerID"] = us.CustomerID;
                        if (login["remember"].Contains("true"))
                        {
                            Response.Cookies["UserName"].Value = saccountName;
                            Response.Cookies["Password"].Value = sPassword;
                            Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(1);
                            Response.Cookies["Password"].Expires = DateTime.Now.AddDays(1);

                        }
                        else
                        {
                            Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(-1);
                            Response.Cookies["Password"].Expires = DateTime.Now.AddDays(-1);
                        }
                        return RedirectToAction("Index", "Mart");
                    }
                    else
                    {
                        ViewBag.ThongBao = "Chúc mừng đăng nhập thành công!";
                        HttpContext.Session["UserName"] = us.UserName;
                        Session["Password"] = us.Password;
                        Session["Permission"] = us.Permission;
                        Session["CustomerID"] = us.CustomerID;
                        if (login["remember"].Contains("true"))
                        {
                            Response.Cookies["UserName"].Value = saccountName;
                            Response.Cookies["Password"].Value = sPassword;
                            Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(1);
                            Response.Cookies["Password"].Expires = DateTime.Now.AddDays(1);

                        }
                        else
                        {
                            Response.Cookies["UserName"].Expires = DateTime.Now.AddDays(-1);
                            Response.Cookies["Password"].Expires = DateTime.Now.AddDays(-1);
                        }
                        return RedirectToAction("shopping_cart", "ShoppingCart");
                    }
           
                }
                else
                {
                    ViewBag.ThongBao = "Sai tài khoản hoặc mật khẩu!";

                }
            }
            return View();
        }
        public ActionResult user_logout()
        {
            FormsAuthentication.SignOut();
            if (Convert.ToInt32(Session["Permission"].ToString()) == 3)
            {
                Session["UserName"] = null;
                Session["Password"] = null;
                Session["Permission"] = null;
                Session["shopping_cart"] = null;
                return RedirectToAction("Index","Mart");
            }
            Session["UserName"] = null;
            Session["Password"] = null;
            Session["EmployeeID"] = null;
            Session["Permission"] = null;
            return RedirectToAction("user_login");
        }
    }
}