using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using perfumedecant.Models.Domains;
using perfumedecant.Models;

namespace PerfumeDecant.Controllers
{
    public class DashboardController : Controller
    {
        PD_DB db = new PD_DB();
        Log log = new Log();
        LogStatus logStatus = new LogStatus();

        public ActionResult Index()
        {
            string Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Redirect the user to Login page, because not logined.";
                log.addLog(Message, "Index", "Dashboard", logStatus.EventLog);
                return RedirectToAction("Index", "Account", new { returnUrl = "/Dashboard/Index" });
            }

            var username = Session["UserName"].ToString();
            var role_name = db.Tbl_User.Where(a => a.User_Username == username).SingleOrDefault().Tbl_Role.Role_Name;
            if (role_name == "Admin")
            {
                Message = "Redirect the Admin user to Dashboard page.";
                log.addLog(Message, "Index", "Dashboard", logStatus.EventLog);
                return View();
            }
            else
            {
                Message = "Redirect the user to Home page, because wasn't Admin.";
                log.addLog(Message, "Index", "Dashboard", logStatus.EventLog);
                return RedirectToAction("Index", "Home");
            }

        }
    }
}