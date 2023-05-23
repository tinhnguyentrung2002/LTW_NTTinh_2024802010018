using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LTW_NTTinh_2024802010018.Models;
using PagedList;
namespace LTW_NTTinh_2024802010018.Controllers
{
    [SessionTimeout]
    public class PermissionController : Controller
    {
        // GET: Permission
        dbMinimartDataContext db = new dbMinimartDataContext();
        public ActionResult Index(int? page)
        {
            int size = 10;
            int PageNum = (page ?? 1);
            var per = db.Users.OrderBy(p=>p.STT).Select(p=>p).ToList();
            return View(per.ToPagedList(PageNum, size));
        }
        public ActionResult filter(int id,int? page)
        {
            int size = 10;
            int PageNum = (page ?? 1);
            if(id == 0)
            {
                var per = db.Users.Where(p=>p.Permission == 0).OrderBy(p => p.STT).Select(p => p).ToList();
                return View(per.ToPagedList(PageNum, size));
            }
            else if (id == 1)
            {
                var per = db.Users.Where(p => p.Permission == 1).OrderBy(p => p.STT).Select(p => p).ToList();
                return View(per.ToPagedList(PageNum, size));
            }
            else if (id == 2)
            {
                var per = db.Users.Where(p => p.Permission == 2).OrderBy(p => p.STT).Select(p => p).ToList();
                return View(per.ToPagedList(PageNum, size));
            }
            else if (id == 3)
            {
                var per = db.Users.Where(p => p.Permission == 3).OrderBy(p => p.STT).Select(p => p).ToList();
                return View(per.ToPagedList(PageNum, size));
            }
            else
            {
                var per = db.Users.OrderBy(p => p.STT).Select(p => p).ToList();
                return View(per.ToPagedList(PageNum, size));
            }
        }
    }
}