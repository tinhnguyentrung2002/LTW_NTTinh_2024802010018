using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LTW_NTTinh_2024802010018.Models
{
    public class shopping_cart
    {
        dbMinimartDataContext db = new dbMinimartDataContext();

        public string sProductID { get; set; }
        public string sProductName { get; set; }
        public string sPicture { get; set; }
        public string sUnit { get; set; }
        public double dPrice { get; set; }
        public int iQuantity { get; set; }
        public int iTax { get; set; }
        public double dTotal
        {
            get { return iQuantity * dPrice; }
        }   
        public shopping_cart(string productid)
        {
            sProductID = productid;
            Product p = db.Products.Single(n => n.ProductID == sProductID);
            sProductName = p.ProductName;
            sPicture = p.Picture;
            sUnit = p.Unit;
            dPrice = double.Parse(p.Price.ToString());
            iQuantity= 1;
        }
    }
}