using LTW_NTTinh_2024802010018.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Antlr.Runtime.Misc;

namespace LTW_NTTinh_2024802010018.Controllers
{
    [SessionTimeout]
    public class ProductController : Controller
    {
        // GET: Products
        dbMinimartDataContext db = new dbMinimartDataContext();
        public ActionResult Index()
        {

            return View();
        }
        public ActionResult product_manage(int? page)
        {
            List<Provider> providerList = db.Providers.ToList();
            List<Category> categorytList = db.Categories.ToList();
            int size = 10;
            int PageNum = (page ?? 1);
            ViewBag.page = PageNum;
            var result = from s in db.Products orderby (s.STT) select s;
            ViewBag.provider = providerList;
            ViewBag.category = categorytList;
            return View(result.ToPagedList(PageNum, size));
        }
        public ActionResult SearchProducts(string strSearch)
        {
            List<Provider> providerList = db.Providers.ToList();
            List<Category> categorytList = db.Categories.ToList();
            ViewBag.provider = providerList;
            ViewBag.category = categorytList;
            ViewBag.Count = 0;
            ViewBag.check = 0;
            ViewBag.Search = strSearch;
            if (!String.IsNullOrEmpty(strSearch) && strSearch != null)
            {
                var result1 = from s in db.Products where s.ProductID.Contains(strSearch) orderby (s.STT) select s;
                var result2 = from s in db.Products where s.ProductName.Contains(strSearch) orderby (s.STT) select s;
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
        public ActionResult filter(string id,int ?page)
        {
            List<Provider> providerList = db.Providers.ToList();
            List<Category> categorytList = db.Categories.ToList();
            ViewBag.provider = providerList;
            ViewBag.category = categorytList;
            ViewBag.id = id;
            int size = 10;
            int PageNum = (page ?? 1);
            var p = db.Products.Where(pr => pr.CategoryID == id).OrderByDescending(pr=>pr.STT).Select(pr => pr);
            ViewBag.count = p.Count();
            return View(p.ToPagedList(PageNum,size));
        }
        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Category = new SelectList(db.Categories.ToList().OrderBy(n => n.CategoryName) ,"CategoryID", "CategoryName");
            ViewBag.Provider = new SelectList(db.Providers.ToList().OrderBy(n => n.ProviderName), "ProviderID", "ProviderName");
            Product p = new Product();
            p.STT = db.Products.Count() + 1;
            p.ProductID = "SP00" + p.STT;
            ViewBag.IDP = p.ProductID;
            return PartialView();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Product p, FormCollection f, HttpPostedFileBase fFileUpload)
        {
            ViewBag.Category = new SelectList(db.Categories.ToList().OrderBy(n => n.CategoryName), "CategoryID", "CategoryName");
            ViewBag.Provider = new SelectList(db.Providers.ToList().OrderBy(n => n.ProviderName), "ProviderID", "ProviderName");

                if (fFileUpload == null)
                {
                p.ProviderID = f["Provider"].ToString();
                p.CategoryID = f["Category"].ToString();
                p.STT = db.Products.Count() + 1;
                p.ProductID = "SP00" + p.STT;
                p.Picture = "default_image.png";
                p.ProductName = f["Name"].ToString();
                p.Unit = f["Unit"];
                p.Price = decimal.Parse(f["Price"]);
                p.Quantity = int.Parse(f["Quantity"]);
                p.Sale = int.Parse(f["Sale"]);
                p.Updated = DateTime.Now;
                p.Description = f["Description"].ToString();
                ViewBag.IDP = p.ProductID;
                db.Products.InsertOnSubmit(p);
                db.SubmitChanges();
                return RedirectToAction("product_manage", new { page = ViewBag.page });

            }
                else
                {
                        var sFileName = Path.GetFileName(fFileUpload.FileName);
                        var path = Path.Combine(Server.MapPath("~/Images"), sFileName);
                        if (!System.IO.File.Exists(path))
                        {
                            fFileUpload.SaveAs(path);
                        }
                        p.ProviderID = f["Provider"].ToString();
                        p.CategoryID = f["Category"].ToString();
                        p.STT = db.Products.Count() + 1;
                        p.ProductID = "SP00" + p.STT;
                        p.Picture = sFileName;
                        p.ProductName = f["Name"].ToString();
                        p.Unit = f["Unit"];
                        p.Price = decimal.Parse(f["Price"]);
                        p.Quantity = int.Parse(f["Quantity"]);
                        p.Sale = int.Parse(f["Sale"]);
                        p.Updated = DateTime.Now;
                        p.Description = f["Description"].ToString();
                        ViewBag.IDP = p.ProductID;
                        db.Products.InsertOnSubmit(p);
                        db.SubmitChanges();
                        return RedirectToAction("product_manage", new { page = ViewBag.page });
                             
                }
        }
        [HttpGet]
        public ActionResult Edit(string id)
        {
            ViewBag.Category = new SelectList(db.Categories.ToList().OrderBy(n => n.CategoryName), "CategoryID", "CategoryName");
            ViewBag.Provider = new SelectList(db.Providers.ToList().OrderBy(n => n.ProviderName), "ProviderID", "ProviderName");
            var p = db.Products.SingleOrDefault(n => n.ProductID.CompareTo(id) == 0);
            if (p == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return PartialView(p);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(string id,FormCollection f, HttpPostedFileBase fFileUpload)
        {
            ViewBag.Category = new SelectList(db.Categories.ToList().OrderBy(n => n.CategoryName), "CategoryID", "CategoryName");
            ViewBag.Provider = new SelectList(db.Providers.ToList().OrderBy(n => n.ProviderName), "ProviderID", "ProviderName");
            var p = db.Products.SingleOrDefault(n => n.ProductID == id);
            if (ModelState.IsValid)
            {
                if (fFileUpload != null)
                {
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Images"), sFileName);
                    if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                    }
                    p.Picture = sFileName;
                }
                p.ProviderID = f["Provider"].ToString();
                p.CategoryID = f["Category"].ToString();
                p.ProductName = f["Name"].ToString();
                p.Unit = f["Unit"];
                p.Price = decimal.Parse(f["Price"]);
                p.Quantity = int.Parse(f["Quantity"]);
                p.Sale = int.Parse(f["Sale"]);
                p.Updated = DateTime.Now;
                p.Description = f["Description"].ToString();
                db.SubmitChanges();
                return RedirectToAction("product_manage");
            }
            return Redirect("~/Error");
        }
        [HttpGet]
        public ActionResult Delete(string id)
        {
            var p = db.Products.SingleOrDefault(n => n.ProductID.CompareTo(id) == 0);
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
            var p = db.Products.SingleOrDefault(n => n.ProductID.CompareTo(id) == 0);
            if (p == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            db.Products.DeleteOnSubmit(p);
            db.SubmitChanges();
            return RedirectToAction("product_manage");
        }
    }
}