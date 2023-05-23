using LTW_NTTinh_2024802010018.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Net.Mail;
using System.Net;
using System.Web.Services.Description;
using LTW_NTTinh_2024802010018;
namespace LTW_NTTinh_2024802010018.Controllers
{
    [SessionTimeout]
    public class ContactController : Controller
    {
        // GET: Contact
        dbMinimartDataContext db = new dbMinimartDataContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult contact_manage(int id, int? page)
        {
            ViewBag.id = id;
            int size = 10;
            int PageNum = (page ?? 1);
            if (id == 1)
            {
                var result = db.Responses.Where(s=>s.ResponseStatus == true).OrderByDescending(s => s.Updated).Select(s => s);
                return View(result.ToPagedList(PageNum, size));
            }
            else if (id == 2)
            {
                var result = db.Responses.Where(s => s.ResponseStatus == false).OrderByDescending(s => s.Updated).Select(s => s);
                return View(result.ToPagedList(PageNum, size));
            }
            else
            {
                var result = db.Responses.OrderByDescending(s => s.Updated).Select(s => s);
                return View(result.ToPagedList(PageNum, size));
            }
            
        }
        [HttpGet]
        public ActionResult contact_reply(string Id)
        {
            var reps = db.Responses.SingleOrDefault(c => c.ResponseID == Id);
            return PartialView(reps);
        }
        [HttpPost]
        public ActionResult contact_reply(string Id,string name,string email,string question,string rep)
        {
                try
                {
                    if (ModelState.IsValid)
                    {
                        var reps = db.Responses.SingleOrDefault(c => c.ResponseID == Id);
                        reps.ResponseStatus = true;
                        
                        var senderEmail = new MailAddress("2024802010018@student.tdmu.edu.vn", "Minimart - Hồi đáp thắc mắc có mã " + Id);
                        var receiverEmail = new MailAddress(email,"Mail nhận");
                        var password = "nqnwdgkgndiokdju";
                        var sub = "Minimart - " +"Hồi đáp thắc mắc";
                        var body = rep;
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
                            Subject = sub,
                            Body = "Chào anh/chị " + name + "\n"+"Câu hỏi của anh/chị: " + question + "\n" +"Giải đáp: " + body +"\n"+ "\n" + "Chúc quý khách mua sắm vui vẻ tại Minimart"
                        })
                        {
                            smtp.Send(mess);
                         
                        }
                        db.SubmitChanges();
                        return RedirectToAction("contact_manage", "Contact", new { id = 3 });
                    }
                   
                }
                catch (Exception ex)
                {
                    return View(ex);
                }
            return View();
        }
        [HttpGet]
        public ActionResult Delete(string Id)
        {
            var reps = db.Responses.SingleOrDefault(c => c.ResponseID == Id);
            if (reps == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return PartialView(reps);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult delete_confirm(string Id)
        {
            var reps = db.Responses.SingleOrDefault(c => c.ResponseID == Id);
            if (reps == null)
            {
                Response.StatusCode = 404;
                return null;
            }   
            db.Responses.DeleteOnSubmit(reps);
            db.SubmitChanges();
            return RedirectToAction("contact_manage", "Contact", new { id = 3 });
        }
    }
}