using LTW_NTTinh_2024802010018.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Xml.Linq;

namespace LTW_NTTinh_2024802010018.Controllers
{
    public class ShoppingCartController : Controller
    {
        dbMinimartDataContext db = new dbMinimartDataContext();
        // GET: ShoppingCart
        public ActionResult Index()
        {
            return View();
        }
        public List<shopping_cart> get_shopping_cart()
        {

            List<shopping_cart> shopping_cartList = Session["shopping_cart"] as List<shopping_cart>;
            if (shopping_cartList == null)
            {
                shopping_cartList = new List<shopping_cart>();
                Session["shopping_cart"] = shopping_cartList;
            }
            return shopping_cartList;
        }
        public ActionResult add_shopping_cart(string pId, string url)
        {
            List<shopping_cart> shopping_cartList = get_shopping_cart();
            shopping_cart product = shopping_cartList.Find(n => n.sProductID == pId);
            if (product == null)
            {
                product = new shopping_cart(pId);
                shopping_cartList.Add(product);
            }
            else
            {
                product.iQuantity++;
            }
            return Redirect(url);
        }
        private int total_amount()
        {
            int itotal = 0;
            List<shopping_cart> shopping_cartList = Session["shopping_cart"] as List<shopping_cart>;
            if (shopping_cartList != null)
            {
                itotal = shopping_cartList.Sum(am => am.iQuantity);
            }
            return itotal;
        }
        private double total_prices()
        {
            double dtotal = 0;
            List<shopping_cart> shopping_cartList = Session["shopping_cart"] as List<shopping_cart>;
            if (shopping_cartList != null)
            {
                dtotal = shopping_cartList.Sum(am => am.dTotal);
            }
            return dtotal;
        }
        private double amount()
        {
            int amount = 0;
            List<shopping_cart> shopping_cartList = Session["shopping_cart"] as List<shopping_cart>;
            if (shopping_cartList != null)
            {
                amount = shopping_cartList.Count();
            }
            return amount;
        }
        public ActionResult shopping_cart(string coupon)
        {
                List<shopping_cart> shopping_cartList = get_shopping_cart();
                ViewBag.count = 1;
                ViewBag.code = coupon;
                ViewBag.cp = 0;
                if (shopping_cartList.Count == 0)
                {
                    ViewBag.count = null;
                }
                else
                {
                    if(!String.IsNullOrEmpty(coupon))
                    {
                        if(total_prices()<50000)
                        {
                            ViewBag.cp = 4;
                            ViewBag.TotalAmount = total_amount();
                            ViewBag.ToTalPrices = total_prices();
                        }
                        else
                        {
                            var cp = db.Coupons.Where(c => c.CouponID.CompareTo(coupon) == 0 && c.CouponExpire > DateTime.Now.Date).Select(c => c.CouponValue).SingleOrDefault();
                            int count1 = db.Coupons.Where(c => c.CouponID.CompareTo(coupon) == 0 && c.CouponExpire > DateTime.Now.Date).Select(c => c.CouponValue).Count();
                            int count2 = db.CouponChecks.Where(c => c.CouponID == coupon && c.CustomerID == Session["CustomerID"].ToString()).Select(c => c.CouponID).Count();
                            if (count1 > 0)
                            {
                               if (count2 > 0)
                                {
                                    ViewBag.cp = 3;
                                    ViewBag.TotalAmount = total_amount();
                                    ViewBag.ToTalPrices = total_prices();
                                }
                                else
                                {
                                    ViewBag.cp = 1;
                                    ViewBag.value = cp.ToString();
                                    ViewBag.TotalAmount = total_amount();
                                    ViewBag.ToTalPrices = (total_prices() - (total_prices() * Convert.ToInt32(cp.ToString())) / 100);
                                    ViewBag.code = coupon;
                                }
                              
                            }
                            else if (count1 == 0)
                            {
                                ViewBag.cp = 2;
                                ViewBag.TotalAmount = total_amount();
                                ViewBag.ToTalPrices = total_prices();
                            }
                                    
                        }                       
                    }
                    else
                    {
                        ViewBag.cp = 0;
                        ViewBag.TotalAmount = total_amount();
                        ViewBag.ToTalPrices = total_prices();
                    }
                                  
                }           
                return View(shopping_cartList); 
        }
        public ActionResult shopping_cartPartial()
        {
            ViewBag.Amount = amount();
            ViewBag.TotalAmount = total_amount();
            ViewBag.ToTalPrices = total_prices();
            return PartialView();
        }
        public ActionResult delete_item(string iId)
        {
            ViewBag.count = 1;
            List<shopping_cart> shopping_cartList = get_shopping_cart();
            shopping_cart product = shopping_cartList.SingleOrDefault(n => n.sProductID == iId);
            if (product != null)
            {
                shopping_cartList.RemoveAll(n => n.sProductID == iId);
                if (shopping_cartList.Count == 0)
                {
                    ViewBag.count = null;
                }
            }
            return RedirectToAction("shopping_cart");
        }
        public ActionResult update_shopping_cart(string iId, FormCollection f)
        {
            List<shopping_cart> shopping_cartList = get_shopping_cart();
            shopping_cart product = shopping_cartList.SingleOrDefault(n => n.sProductID == iId);
            if (product != null)
            {
                product.iQuantity = int.Parse(f["txtSoLuong"].ToString());
            }
            return RedirectToAction("shopping_cart");
        }
        public ActionResult delete_shopping_cart()
        {
            List<shopping_cart> shopping_cartList = get_shopping_cart();
            shopping_cartList.Clear();
            return RedirectToAction("Index", "Mart");
        }
        [HttpGet]
        public ActionResult pay_order(string coupon, int value)
        {
            ViewBag.cp = 0;
            if (coupon.CompareTo("0") == 0) coupon = null;
            if (Session["UserName"] == null || Session["UserName"].ToString() == "")
            {
                return RedirectToAction("user_login", "User");
            }
            if (Session["UserName"] != null)
            {
                var email_customer = db.Customers.Where(s => s.CustomerID == Session["CustomerID"].ToString()).Select(s => s.Email).SingleOrDefault();
                var name_customer = db.Customers.Where(s => s.CustomerID == Session["CustomerID"].ToString()).Select(s => s.CustomerName).SingleOrDefault();
                var address_customer = db.Customers.Where(s => s.CustomerID == Session["CustomerID"].ToString()).Select(s => s.Address).SingleOrDefault();
                var phone_customer = db.Customers.Where(s => s.CustomerID == Session["CustomerID"].ToString()).Select(s => s.Phone).SingleOrDefault();
                ViewBag.email = email_customer;
                ViewBag.name = name_customer;
                ViewBag.address = address_customer;
                ViewBag.phone = phone_customer;
                var id_customer = db.Customers.Where(s => s.CustomerID == Session["CustomerID"].ToString()).Select(s => s.CustomerID).SingleOrDefault();
                ViewBag.id = id_customer;

            }
            if (Session["shopping_cart"] == null)
            {
                return RedirectToAction("Index", "Mart");
            }
            List<shopping_cart> shopping_cartList = get_shopping_cart();
            if(!String.IsNullOrEmpty(coupon))
            {
                var cp = db.Coupons.Where(c => c.CouponID.CompareTo(coupon) == 0 && c.CouponExpire > DateTime.Now.Date).Select(c => c.CouponValue).SingleOrDefault();
                ViewBag.value = cp.ToString();
                ViewBag.cp = 1;
                
                ViewBag.TotalAmount = total_amount();
                ViewBag.ToTalPrices = (total_prices() - (total_prices() * Convert.ToInt32(value)) / 100);
            }
            else
            {
                ViewBag.cp =0;
                ViewBag.TotalAmount = total_amount();
                ViewBag.ToTalPrices = total_prices();
            }  
            return View(shopping_cartList);
        }
        [HttpPost]
        public ActionResult pay_order(FormCollection f, string coupon, int value)
        {
            if (coupon.CompareTo("0") == 0) coupon = null;
            Order orderBill = new Order();         
            if (Session["UserName"] != null)
            {
                var id_customer = db.Customers.Where(s => s.CustomerID == Session["CustomerID"].ToString()).Select(s => s.CustomerID).SingleOrDefault();
     
                var email_customer = db.Customers.Where(s => s.CustomerID == Session["CustomerID"].ToString()).Select(s => s.Email).SingleOrDefault();
                var name_customer = db.Customers.Where(s => s.CustomerID == Session["CustomerID"].ToString()).Select(s => s.CustomerName).SingleOrDefault();
                var address_customer = db.Customers.Where(s => s.CustomerID == Session["CustomerID"].ToString()).Select(s => s.Address).SingleOrDefault();
                var phone_customer = db.Customers.Where(s => s.CustomerID == Session["CustomerID"].ToString()).Select(s => s.Phone).SingleOrDefault();
                ViewBag.id = id_customer;
                ViewBag.email = email_customer;
                ViewBag.name = name_customer;
                ViewBag.address = address_customer;
                ViewBag.phone = phone_customer;
            }
            List<shopping_cart> shopping_cartList = get_shopping_cart();
            int temp = 100 + db.Orders.Count() + 1;
            orderBill.OrderID = DateTime.Now.Year.ToString() + temp;
            if (!String.IsNullOrEmpty(coupon))
            {
                double total = (total_prices() - (total_prices() * Convert.ToInt32(value)) / 100);
                orderBill.Amount = (Convert.ToDecimal(total) + Convert.ToDecimal(total) * 8 / 100);
                orderBill.Coupon = value;
                CouponCheck cpc = new CouponCheck();
                cpc.CustomerID = Session["CustomerID"].ToString();
                cpc.CouponID = coupon;
                db.SubmitChanges();
                db.CouponChecks.InsertOnSubmit(cpc);

            }
            else
            {
                orderBill.Amount = (Convert.ToDecimal(total_prices()) + Convert.ToDecimal(total_prices()) * 8 / 100);
            }      
            orderBill.CustomerID = ViewBag.id;
            orderBill.CustomerName = f["Name"];
            orderBill.CustomerPhone = f["Phone"];
            orderBill.CustomerEmail = f["Email"];
            orderBill.Address = f["Address"];
            orderBill.InvoiceDate = DateTime.Now;
           // var DeliveryDate = String.Format("{0:MM/dd/yyyy}", f["DeliveryDate"]);
           // orderBill.DeliveryDate = DateTime.Parse(DeliveryDate);
            orderBill.DeliveryStatus = false;
            orderBill.PayStatus = true;
            orderBill.Tax = 8;
            orderBill.Updated = DateTime.Now;
            orderBill.PaymentID = "pay02";
            db.Orders.InsertOnSubmit(orderBill);
            db.SubmitChanges();
            foreach (var item in shopping_cartList)
            {
                OrderDetail orderDetail = new OrderDetail();
             
                orderDetail.OrderDetailID = "D"+orderBill.OrderID;
                orderDetail.OrderID = orderBill.OrderID;
                orderDetail.ProductID = item.sProductID;
                orderDetail.ProductName = item.sProductName;
                orderDetail.Quantity = item.iQuantity;
                orderDetail.UnitPrice = (Decimal)item.dPrice;
                orderDetail.Amount = (Decimal)item.dTotal;                   
                var product = db.Products.SingleOrDefault(p => p.ProductID.CompareTo(orderDetail.ProductID.ToString()) == 0);
                if((product.Quantity - orderDetail.Quantity) >= 0 )
                {
                    product.Quantity = product.Quantity - orderDetail.Quantity;
                }
                else
                {
                    product.Quantity = 0;
                }
                db.SubmitChanges();
                db.OrderDetails.InsertOnSubmit(orderDetail);            
            }
            Session["shopping_cart"] = null;
            db.SubmitChanges();
            return View("confirm_order");
        }     
        public ActionResult confirm_order()
        {
            var name_customer = db.Customers.Where(s => s.CustomerID == Session["CustomerID"].ToString()).Select(s => s.CustomerName).SingleOrDefault();
            ViewBag.name = name_customer;
            return View();

        }
    }
}