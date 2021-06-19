using perfumedecant.Models;
using perfumedecant.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace perfumedecant.Controllers
{
    public class CompanySampleController : Controller
    {
        PD_DB db = new PD_DB();
        Log log = new Log();
        LogStatus logStatus = new LogStatus();

        //
        // GET: /CompanySample/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddCompanySample()
        {
            String Message = "";

            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddCompanySample", "DashboardCompanySample", logStatus.EventLog);
                return RedirectToAction("Index", "Account", new { returnUrl = "/CompanySample/Index" });
            }
            else if (Session["RoleName"].ToString() == "Admin")
            {
                InitDropdownLists();
                return View();
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "AddCompanySample", "DashboardCompanySample", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddCompanySample(Tbl_CompanySample sample)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddCompanySample", "DashboardCompanySample", logStatus.EventLog);
                return RedirectToAction("Index", "Account", new { returnUrl = "/CompanySample/Index" });
            }
            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    InitDropdownLists();
                    Tbl_CompanySample s = new Tbl_CompanySample();
                    s.CompanySample_Perfume_ID = sample.CompanySample_Perfume_ID;
                    s.CompanySample_Price = sample.CompanySample_Price;
                    s.CompanySample_Weight = sample.CompanySample_Weight;
                    s.CompanySample_AllCount = sample.CompanySample_AllCount;
                    db.Tbl_CompanySample.Add(s);
                    if (Convert.ToBoolean(db.SaveChanges() > 0))
                    {
                        Message = "company sample with perfume ID " + sample.CompanySample_Perfume_ID + " added successfully.";
                        log.addLog(Message, "AddCompanySample", "DashboardCompanySample", logStatus.EventLog);
                        return RedirectToAction("Index", "CompanySample");
                    }
                    else
                    {
                        Message = "added company sample with perfume ID " + sample.CompanySample_Perfume_ID + " failed.";
                        log.addLog(Message, "AddCompanySample", "DashboardCompanySample", logStatus.ErrorLog);
                        ViewBag.Error = "سمپل جدید ثبت نشد، لطفا دوباره تلاش کنید.";
                        return View();
                    }
                }
                catch
                {
                    Message = "added company sample with perfume ID " + sample.CompanySample_Perfume_ID + " failed.";
                    log.addLog(Message, "AddCompanySample", "DashboardCompanySample", logStatus.ErrorLog);
                    ViewBag.Error = "سمپل جدید ثبت نشد، لطفا دوباره تلاش کنید.";
                    return View();
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "AddCompanySample", "DashboardCompanySample", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }
        }

        public ActionResult EditCompanySample(int sampleID)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "EditCompanySample", "DashboardCompanySample", logStatus.EventLog);
                return RedirectToAction("Index", "Account", new { returnUrl = "/CompanySample/Index" });
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    InitDropdownLists();
                    var sample = db.Tbl_CompanySample.Where(a => a.CompanySample_ID == sampleID).SingleOrDefault();
                    if (sample != null)
                    {
                        return View(sample);
                    }
                    else
                    {
                        Message = "company sample with ID" + sampleID + "not found.";
                        log.addLog(Message, "EditCompanySample", "DashboardCompanySample", logStatus.EventLog);
                        ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                        return RedirectToAction("Index", "CompanySample");
                    }

                }
                catch
                {
                    Message = "company sample with ID" + sampleID + "not found.";
                    log.addLog(Message, "EditCompanySample", "DashboardCompanySample", logStatus.EventLog);
                    ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                    return View();
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "EditCompanySample", "DashboardCompanySample", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditCompanySample(Tbl_CompanySample s)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "EditCompanySample", "DashboardCompanySample", logStatus.EventLog);
                return RedirectToAction("Index", "Account", new { returnUrl = "/CompanySample/Index" });
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    InitDropdownLists();
                    var sample = db.Tbl_CompanySample.Where(a => a.CompanySample_ID == s.CompanySample_ID).SingleOrDefault();
                    if (sample != null)
                    {
                        sample.CompanySample_Perfume_ID = s.CompanySample_Perfume_ID;
                        sample.CompanySample_AllCount = s.CompanySample_AllCount;
                        sample.CompanySample_Price = s.CompanySample_Price;
                        sample.CompanySample_Weight = s.CompanySample_Weight;
                        db.Tbl_CompanySample.Attach(sample);
                        db.Entry(sample).State = System.Data.Entity.EntityState.Modified;
                        if (Convert.ToBoolean(db.SaveChanges() > 0))
                        {
                            Message = "company sample with id " + s.CompanySample_ID + " edited successfully.";
                            log.addLog(Message, "EditCompanySample", "DashboardCompanySample", logStatus.EventLog);
                            return RedirectToAction("Index", "CompanySample");
                        }
                        else
                        {
                            Message = "edited company sample with id " + s.CompanySample_ID + " failed.";
                            log.addLog(Message, "EditCompanySample", "DashboardCompanySample", logStatus.ErrorLog);
                            return RedirectToAction("Index", "CompanySample");

                        }
                    }
                    else
                    {
                        Message = "company sample with ID" + s.CompanySample_ID + "not found.";
                        log.addLog(Message, "EditCompanySample", "DashboardCompanySample", logStatus.EventLog);
                        ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                        return RedirectToAction("Index", "CompanySample");
                    }

                }
                catch
                {
                    Message = "company sample with ID" + s.CompanySample_ID + "not found.";
                    log.addLog(Message, "EditCompanySample", "DashboardCompanySample", logStatus.EventLog);
                    ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                    return View();
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "EditCompanySample", "DashboardCompanySample", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }
        }

        public ActionResult DeleteCompanySample(int sampleID)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddCompanySample", "DashboardCompanySample", logStatus.EventLog);
                return RedirectToAction("Index", "Account", new { returnUrl = "/CompanySample/Index" });
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    var sample = db.Tbl_CompanySample.Where(a => a.CompanySample_ID == sampleID).SingleOrDefault();
                    if (sample != null)
                    {
                        db.Tbl_CompanySample.Remove(sample);
                        if (Convert.ToBoolean(db.SaveChanges() > 0))
                        {
                            Message = "company sample with id " + sample.CompanySample_ID + " deleted successfully.";
                            log.addLog(Message, "Index", "DashboardCompanySample", logStatus.EventLog);
                            ViewBag.Success = "محصول شما با موفقیت حذف شد.";
                            return RedirectToAction("Index", "CompanySample");
                        }
                        else
                        {
                            Message = "delete company sample with ID " + sampleID + " failed.";
                            log.addLog(Message, "DeleteCompanySample", "DashboardCompanySample", logStatus.ErrorLog);
                            ViewBag.Error = "محصول حذف نشد، لطفا دوباره تلاش کنید.";
                            return RedirectToAction("Index", "CompanySample");
                        }
                    }
                    else
                    {
                        Message = "company sample with ID" + sampleID + "not found.";
                        log.addLog(Message, "DeleteCompanySample", "DashboardCompanySample", logStatus.EventLog);
                        ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                        return RedirectToAction("Index", "CompanySample");
                    }
                }
                catch
                {
                    Message = "delete company sample with ID " + sampleID + " failed.";
                    log.addLog(Message, "DeleteCompanySample", "DashboardCompanySample", logStatus.ErrorLog);
                    ViewBag.Error = "محصول حذف نشد، لطفا دوباره تلاش کنید.";
                    return RedirectToAction("Index", "CompanySample");
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "DeleteCompanySample", "DashboardCompanySample", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }
        }

        public void InitDropdownLists()
        {
            var perfumeList = db.Tbl_Perfume.OrderBy(r => r.Perfume_Name).ToList().Select(rr =>
                  new SelectListItem { Value = rr.Perfume_ID.ToString(), Text = rr.Perfume_Name }).ToList();
            ViewBag.perfumeList = perfumeList;


            var weightList = new List<SelectListItem>
                    {
                        new SelectListItem{ Text="0.7", Value = "0.7" , Selected = true},
                        new SelectListItem{ Text="1", Value = "1" },
                        new SelectListItem{ Text="1.2", Value = "1.2"},
                        new SelectListItem{ Text="1.5", Value = "1.5" },
                        new SelectListItem{ Text="2", Value = "2" },
                        new SelectListItem{ Text="2.5", Value = "2.5" },
                    };
            ViewBag.weightList = weightList;
        }

	}
}