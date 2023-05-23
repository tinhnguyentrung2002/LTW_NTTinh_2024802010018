using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using LTW_NTTinh_2024802010018.Models;
using System.ComponentModel.DataAnnotations;
using System.Web.UI;
using PagedList;
using LTW_NTTinh_2024802010018;
namespace LTW_NTTinh_2024802010018.Controllers
{
    public class MartController : Controller
    {
        // GET: Mart
        dbMinimartDataContext db = new dbMinimartDataContext();
        private List<Product> takeProduct(int count)
        {
            return db.Products.OrderByDescending(a => a.Sale).Take(count).ToList();
        }
        private List<Product> takeNewProduct(int count)
        {

            return db.Products.OrderByDescending(a => a.Updated).Take(count).ToList();
        }
        private List<Product> takeASCProduct(int count)
        {

            return db.Products.OrderBy(a => a.Price).Take(count).ToList();
        }
        private List<Product> takeDESProduct(int count)
        {

            return db.Products.OrderByDescending(a => a.Price).Take(count).ToList();
        }
        public ActionResult Index(int ?page)
        {
            int size = 8;
            int PageNum = (page ?? 1);
            var new_product = takeNewProduct(8);
            var quantity = db.Products.Where(s => s.CategoryID == "C2").Count();
            var quantity1 = db.Products.Where(s => s.CategoryID == "C8").Count();
            var quantity2 = db.Products.Where(s => s.CategoryID == "C7").Count();
            var quantity3 = db.Products.Where(s => s.CategoryID == "C1").Count();
            var quantity4 = db.Products.Where(s => s.CategoryID == "C9").Count();
            var quantity5 = db.Products.Where(s => s.CategoryID == "C11").Count();
            ViewBag.quantity = quantity;
            ViewBag.quantity1 = quantity1;
            ViewBag.quantity2 = quantity2;
            ViewBag.quantity3 = quantity3;
            ViewBag.quantity4 = quantity4;
            ViewBag.quantity5 = quantity5;
            return View(new_product.ToPagedList(PageNum,size));
        }
        public ActionResult category_partial()
        {

            var category = from s in db.Categories orderby(s.CategoryID)ascending select s;
            return PartialView(category);
        }
        public ActionResult slider_partial()
        {
            return PartialView();
        }
        public ActionResult footer_partial()
        {
            return PartialView();
        }
        public ActionResult provider_partial()
        {
            var provider = from s in db.Providers select s;
            return PartialView(provider);
        }
        public ActionResult navtop_partial()
        {
            return PartialView();
        }
        public ActionResult provider_category()
        {       
            var provider = db.Providers;
            return PartialView(provider);
        }
        public ActionResult categorypage_partial()
        {
            return PartialView();
        }
        public ActionResult hotproduct_partial(int ?page)
        {
            int size = 8;
            int PageNum = (page ?? 1);
            var product = takeProduct(6);
            return PartialView(product.ToPagedList(PageNum,size));
        }
        public ActionResult user_detail(string id)
        {
            return View(db.Customers.Where(s => s.CustomerID.CompareTo(id) == 0).SingleOrDefault());
        }
        [HttpGet]
        public ActionResult user_edit(string id)
        {
            var c = db.Customers.SingleOrDefault(s => s.CustomerID.CompareTo(id) == 0);
            ViewBag.Sex = c.Sex;
            if (c == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(c);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult user_edit(FormCollection f)
        {
            var c = db.Customers.SingleOrDefault(n => n.CustomerID == Session["CustomerID"].ToString());
            ViewBag.Sex = c.Sex;
            if (ModelState.IsValid)
            {                
                c.CustomerName = f["Name"].ToString();
                c.Address = f["Address"].ToString();
                c.Birthday = DateTime.Parse(f["Birthday"].ToString());
                c.Sex = f["Sex"].ToString();
                c.Phone = f["Phone"].ToString();
                c.Updated = DateTime.Now;
                db.SubmitChanges();
                return RedirectToAction("user_detail", "Mart", new
                {
                    id = Session["CustomerID"].ToString(),
                });
            }
            return View(c);
        }
        public ActionResult mart_product(int id,int ?page)
        {
            ViewBag.id = id;
            int size = 12;
            int PageNum = (page ?? 1);
            if (id == 1)
            {
                var product = db.Products.OrderByDescending(a => a.Updated).ToList();
                return View(product.ToPagedList(PageNum,size));
            }
            else if( id == 2)
            {
                var product = db.Products.OrderBy(a => a.Price).ToList(); 
                return View(product.ToPagedList(PageNum, size));
            }
            else 
            {
                var product = db.Products.OrderByDescending(a => a.Price).ToList(); ;
                return View(product.ToPagedList(PageNum, size));
            }    
        }
        public ActionResult category_product(string id, int i,int ?page)
        {
            ViewBag.id = id;
            ViewBag.i = i;
            int size = 12;
            int PageNum = (page ?? 1);
            if (db.Products.Where(p => p.CategoryID == id).Count() == 0)
            {
                ViewBag.P = "Không tồn tại sản phẩm thuộc danh mục này";
            }
            if (i == 1)
            {
                var product = db.Products.OrderByDescending(p => p.Updated).Where(p => p.CategoryID == id).Select(p => p);
                return View(product.ToPagedList(PageNum, size));
            }
            else if (i == 2)
            {
                var product = db.Products.OrderBy(p => p.Price).Where(p => p.CategoryID == id).Select(p => p);
                return View(product.ToPagedList(PageNum, size));
            }
            else if(i == 3)
            {
                var product = db.Products.OrderByDescending(p => p.Price).Where(p => p.CategoryID == id).Select(p => p);
                return View(product.ToPagedList(PageNum, size));
            }
            var product1 = db.Products.OrderBy(p => p.Updated).Where(p => p.CategoryID == id).Select(p => p);
   
            
            return View();
        }
        public ActionResult provider_product(string id, int i, int ?page)
        {
            ViewBag.id = id;
            ViewBag.i = i;
            int size = 12;
            int PageNum = (page ?? 1);
            if (db.Products.Where(p => p.ProviderID == id).Count() == 0)
            {
                ViewBag.P = "Không tồn tại sản phẩm thuộc danh mục này";
            }
            if (i == 1)
            {
                var product = db.Products.OrderByDescending(p => p.Updated).Where(p => p.ProviderID == id).Select(p => p);
                return View(product.ToPagedList(PageNum, size));
            }
            else if (i == 2)
            {
                var product = db.Products.OrderBy(p => p.Price).Where(p => p.ProviderID == id).Select(p => p);
                return View(product.ToPagedList(PageNum, size));
            }
            else if (i == 3)
            {
                var product = db.Products.OrderByDescending(p => p.Price).Where(p => p.ProviderID == id).Select(p => p);
                return View(product.ToPagedList(PageNum, size));
            }
            var product1 = db.Products.OrderBy(p => p.Updated).Where(p => p.ProviderID == id).Select(p => p);


            return View();
        }
        [HttpGet]
        public ActionResult contact()
        {
            if (Session["UserName"] != null)
            {
                var email_customer = db.Customers.Where(s => s.CustomerID == Session["CustomerID"].ToString()).Select(s => s.Email).SingleOrDefault();
                ViewBag.email = email_customer;
            }
            return View();
        }
        [HttpPost]
        public ActionResult contact(string receiver, string subject, string message, string name,Response resp, FormCollection f)
        {
            if (Session["UserName"] != null)
            {         
                try
                {             
                    if (ModelState.IsValid)
                    {
                        int temp = 1000 + db.Responses.Count() + 1;
                        resp.ResponseID = "Resp" + temp;
                        resp.ResponseEmail = f["receiver"].ToString();
                        resp.ResponseContent = f["message"].ToString();
                        resp.ResponseGuestName = f["name"].ToString();
                        resp.ResponseStatus = false;
                        resp.Updated = DateTime.Now;
                        Customer cus = new Customer();
                        var email_customer = db.Customers.Where(s => s.CustomerID == Session["CustomerID"].ToString()).Select(s => s.Email).SingleOrDefault();
                        ViewBag.email = email_customer;
                        var senderEmail = new MailAddress("2024802010018@student.tdmu.edu.vn", "Minimart - Phản hồi từ khách hàng có mã #" + Session["CustomerID"].ToString());
                        var receiverEmail = new MailAddress("nguyentrungtinhlop9a8@gmail.com", "Temp Mail");
                        var password = "nqnwdgkgndiokdju";
                        var sub = subject;
                        var body = message;
                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(senderEmail.Address, password)
                        };
                        using (var mess = new MailMessage(senderEmail, receiverEmail)
                        {
                            Subject = subject,
                            Body = "MAIL CỦA KHÁCH: " +  email_customer + "\n" + "TÊN CỦA KHÁCH: " +  name + "\n" + "NỘI DUNG: " +  body
                        })
                        {
                            smtp.Send(mess);
                            ViewBag.success = "Gửi thành công!";
                        }
                        db.Responses.InsertOnSubmit(resp);
                        db.SubmitChanges();
                        return View();
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Gửi không thành công! + " + ex.Message;
                }
            }
            else
            {         
                try
                {

                    if (ModelState.IsValid)
                    {
                        int temp = 1000 + db.Responses.Count() + 1;
                        resp.ResponseID = "Resp" + temp;
                        resp.ResponseEmail = f["receiver"].ToString();
                        resp.ResponseContent = f["message"].ToString();
                        resp.ResponseGuestName = f["name"].ToString();
                        resp.ResponseStatus = false;
                        resp.Updated = DateTime.Now;
                        var senderEmail = new MailAddress("2024802010018@student.tdmu.edu.vn", "Minimart - Phản hồi từ khách hàng có mã GUEST#" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second);
                        var receiverEmail = new MailAddress("nguyentrungtinhlop9a8@gmail.com", "Temp Mail");
                        var password = "nqnwdgkgndiokdju";
                        var sub = subject;
                        var body = message;
                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(senderEmail.Address, password)
                        };
                        using (var mess = new MailMessage(senderEmail, receiverEmail)
                        {
                            Subject = subject,
                            Body = "MAIL CỦA KHÁCH: " + receiver + "\n" + "TÊN CỦA KHÁCH: " + name + "\n"  + "NỘI DUNG: " + body
                        })
                        {
                            smtp.Send(mess);
                            ViewBag.success = "Gửi thành công!";
                        }
                        db.Responses.InsertOnSubmit(resp);
                        db.SubmitChanges();
                        return View();
                    }
                }
                catch (Exception ex )
                {
                    ViewBag.Error = "Gửi không thành công! " + ex.Message;
                }
            }

            return View();
        }
        public ActionResult SearchProduct(string strSearch,int ?page)
        {
            ViewBag.Count = 0;
            ViewBag.strSearch = strSearch;
            int size = 12;
            int PageNum = (page ?? 1);
            ViewBag.Search = "Không tồn tại sản phẩm này";
            if (!String.IsNullOrEmpty(strSearch) && strSearch!= null)
            {             
                ViewBag.Search = null;
                var result = from s in db.Products where s.ProductName.Contains(strSearch) orderby (s.Updated) descending select s;
                ViewBag.Count = db.Products.Where(s=>s.ProductName.Contains(strSearch)).Select(s=>s).Count();
                return View(result.ToPagedList(PageNum, size));

            }
            return View();
        }
        public ActionResult product_detail(string id)
        { 
            return View(db.Products.Where(s => s.ProductID == id).Select(s=>s).Single());
        }
        public ActionResult my_order(string id,int id1,int ?page)
        {
            ViewBag.id= id;
            ViewBag.id1 = id1;
            int size = 10;
            int PageNum = (page ?? 1);
            ViewBag.cnt = 1;
            if (db.Orders.Where(s => s.CustomerID == id).Count() == 0) ViewBag.cnt = 0;
            if (id1 == 1)
            {
                return View(db.Orders.Where(s => s.CustomerID == id).OrderByDescending(s => s.Updated).Select(s => s).ToPagedList(PageNum,size));
            }
            else
            {
                return View(db.Orders.Where(s => s.CustomerID == id).OrderBy(s => s.Updated).Select(s => s).ToPagedList(PageNum,size));
            }
        }
        public ActionResult my_order_detail(string id)
        {

            return View(db.OrderDetails.Where(s => s.OrderID == id).OrderBy(s => s.ProductID).Select(s => s));
        }
        public ActionResult SearchOrder(string strId, int? page)
        {
            ViewBag.strId = strId;
            int size = 10;
            int PageNum = (page ?? 1);
            ViewBag.cnt1 = 1;
            if(db.Orders.Where(s => s.OrderID.Contains(strId) && s.CustomerID == Session["CustomerID"].ToString()).OrderByDescending(s => s.Updated).Count() == 0) ViewBag.cnt1 = 0;
            return View(db.Orders.Where(s=>s.OrderID.Contains(strId) && s.CustomerID == Session["CustomerID"].ToString()).OrderByDescending(s => s.Updated).ToPagedList(PageNum, size));
        }
        public ActionResult filter_order(DateTime start, DateTime end, int ?page)
        {
            ViewBag.date = start;
            ViewBag.end = end;
            ViewBag.Count = 0;
            int size = 10;
            int PageNum = (page ?? 1);
            ViewBag.cnt2 = 1;
            ViewBag.temp1 = start;
            ViewBag.temp1 = end;
   
                if (db.Orders.Where(s => s.CustomerID == Session["CustomerID"].ToString() && (s.InvoiceDate >= start && s.InvoiceDate <= end)).OrderByDescending(s => s.Updated).Select(s => s).Count() == 0) ViewBag.cnt2 = 0;
                ViewBag.Count = db.Orders.Where(s => s.CustomerID == Session["CustomerID"].ToString() && (s.InvoiceDate >= start && s.InvoiceDate <= end)).OrderByDescending(s => s.Updated).Select(s => s).Count();
                return View(db.Orders.Where(s => s.CustomerID == Session["CustomerID"].ToString() && (s.InvoiceDate >= start && s.InvoiceDate <= end || (s.InvoiceDate == start && s.InvoiceDate == end))).OrderByDescending(s => s.Updated).Select(s => s).ToPagedList(PageNum, size));

           
        }
        public ActionResult news_mart( int? page)
        {
            int size = 6;
            int PageNum = (page ?? 1);
            return View(db.News.OrderByDescending(n =>n.Updated).Select(n=>n).ToPagedList(PageNum, size));
        }
        public ActionResult read_news(string id)
        {
            return View(db.News.Where(n=>n.NewsID == id).Select(n => n).Single());
        }
        public ActionResult mart_warranty(string id)
        {
            return View();
        }
        public ActionResult mart_delivery(string id)
        {
            return View();
        }
        public ActionResult mart_checkout(string id)
        {
            return View();
        }
    }
}