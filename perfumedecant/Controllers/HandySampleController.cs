using perfumedecant.Models;
using perfumedecant.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace perfumedecant.Controllers
{
    public class HandySampleController : Controller
    {
        PD_DB db = new PD_DB();
        Log log = new Log();
        LogStatus logStatus = new LogStatus();
        //
        // GET: /HandySample/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddHandySample()
        {
            String Message = "";

            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddhandySample", "DashboardHandySample", logStatus.EventLog);
                return RedirectToAction("Index", "Account");
            }
            else if (Session["RoleName"].ToString() == "Admin")
            {
                InitDropdownLists();
                return View();
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "AddHandySample", "DashboardHandySample", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddHandySample(Tbl_HandySample sample)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddHandySample", "DashboardHandySample", logStatus.EventLog);
                return RedirectToAction("Index", "Account");
            }
            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    InitDropdownLists();
                    Tbl_HandySample s = new Tbl_HandySample();
                    s.HandySample_Perfume_ID = sample.HandySample_Perfume_ID;
                    s.HandySample_PricePerMil = sample.HandySample_PricePerMil;
                    s.HandySample_AllWeight = sample.HandySample_AllWeight;
                    db.Tbl_HandySample.Add(s);
                    if (Convert.ToBoolean(db.SaveChanges() > 0))
                    {
                        Message = "handy sample with perfume ID " + sample.HandySample_Perfume_ID + " added successfully.";
                        log.addLog(Message, "AddHandySample", "DashboardHandySample", logStatus.EventLog);
                        return RedirectToAction("Index", "HandySample");
                    }
                    else
                    {
                        Message = "added handy sample with perfume ID " + sample.HandySample_Perfume_ID + " failed.";
                        log.addLog(Message, "AddHandySample", "DashboardHandySample", logStatus.ErrorLog);
                        ViewBag.Error = "سمپل جدید ثبت نشد، لطفا دوباره تلاش کنید.";
                        return View();
                    }
                }
                catch
                {
                    Message = "added handy sample with perfume ID " + sample.HandySample_Perfume_ID + " failed.";
                    log.addLog(Message, "AddHandySample", "DashboardHandySample", logStatus.ErrorLog);
                    ViewBag.Error = "سمپل جدید ثبت نشد، لطفا دوباره تلاش کنید.";
                    return View();
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "AddHandySample", "DashboardHandySample", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }
        }

        public ActionResult EditHandySample(int sampleID)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "EditHandySample", "DashboardHandySample", logStatus.EventLog);
                return RedirectToAction("Index", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    InitDropdownLists();
                    var sample = db.Tbl_HandySample.Where(a => a.HandySample_ID == sampleID).SingleOrDefault();
                    if (sample != null)
                    {
                        return View(sample);
                    }
                    else
                    {
                        Message = "handy sample with ID" + sampleID + "not found.";
                        log.addLog(Message, "EditHandySample", "DashboardHandySample", logStatus.EventLog);
                        ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                        return RedirectToAction("Index", "HandySample");
                    }

                }
                catch
                {
                    Message = "handy sample with ID" + sampleID + "not found.";
                    log.addLog(Message, "EditHandySample", "DashboardHandySample", logStatus.EventLog);
                    ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                    return View();
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "EditHandySample", "DashboardHandySample", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditHandySample(Tbl_HandySample s)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "EditHandySample", "DashboardHandySample", logStatus.EventLog);
                return RedirectToAction("Index", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    InitDropdownLists();
                    var sample = db.Tbl_HandySample.Where(a => a.HandySample_ID == s.HandySample_ID).SingleOrDefault();
                    if (sample != null)
                    {
                        sample.HandySample_Perfume_ID = s.HandySample_Perfume_ID;
                        sample.HandySample_AllWeight = s.HandySample_AllWeight;
                        sample.HandySample_PricePerMil = s.HandySample_PricePerMil;
                        db.Tbl_HandySample.Attach(sample);
                        db.Entry(sample).State = System.Data.Entity.EntityState.Modified;
                        if (Convert.ToBoolean(db.SaveChanges() > 0))
                        {
                            Message = "handy sample with id " + s.HandySample_ID + " edited successfully.";
                            log.addLog(Message, "EditHandySample", "DashboardHandySample", logStatus.EventLog);
                            return RedirectToAction("Index", "HandySample");
                        }
                        else
                        {
                            Message = "edited handy sample with id " + s.HandySample_ID + " failed.";
                            log.addLog(Message, "EditHandySample", "DashboardHandySample", logStatus.ErrorLog);
                            return RedirectToAction("Index", "HandySample");

                        }
                    }
                    else
                    {
                        Message = "handy sample with ID" + s.HandySample_ID + "not found.";
                        log.addLog(Message, "EditHandySample", "DashboardHandySample", logStatus.EventLog);
                        ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                        return RedirectToAction("Index", "HandySample");
                    }

                }
                catch
                {
                    Message = "handy sample with ID" + s.HandySample_ID + "not found.";
                    log.addLog(Message, "EditHandySample", "DashboardHandySample", logStatus.EventLog);
                    ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                    return View();
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "EditHandySample", "DashboardHandySample", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }
        }

        public ActionResult DeleteHandySample(int sampleID)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddHandySample", "DashboardHandySample", logStatus.EventLog);
                return RedirectToAction("Index", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    var sample = db.Tbl_HandySample.Where(a => a.HandySample_ID == sampleID).SingleOrDefault();
                    if (sample != null)
                    {
                        db.Tbl_HandySample.Remove(sample);
                        if (Convert.ToBoolean(db.SaveChanges() > 0))
                        {
                            Message = "handy sample with id " + sample.HandySample_ID + " deleted successfully.";
                            log.addLog(Message, "Index", "DashboardHandySample", logStatus.EventLog);
                            ViewBag.Success = "محصول شما با موفقیت حذف شد.";
                            return RedirectToAction("Index", "HandySample");
                        }
                        else
                        {
                            Message = "delete handy sample with ID " + sampleID + " failed.";
                            log.addLog(Message, "DeleteHandySample", "DashboardHandySample", logStatus.ErrorLog);
                            ViewBag.Error = "محصول حذف نشد، لطفا دوباره تلاش کنید.";
                            return RedirectToAction("Index", "HandySample");
                        }
                    }
                    else
                    {
                        Message = "handy sample with ID" + sampleID + "not found.";
                        log.addLog(Message, "DeleteHandySample", "DashboardHandySamplee", logStatus.EventLog);
                        ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                        return RedirectToAction("Index", "HandySample");
                    }
                }
                catch
                {
                    Message = "delete handy sample with ID " + sampleID + " failed.";
                    log.addLog(Message, "DeleteHandySample", "DashboardHandySample", logStatus.ErrorLog);
                    ViewBag.Error = "محصول حذف نشد، لطفا دوباره تلاش کنید.";
                    return RedirectToAction("Index", "HandySample");
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "DeleteHandySample", "DashboardHandySample", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }
        }

        public void InitDropdownLists()
        {
            var perfumeList = db.Tbl_Perfume.OrderBy(r => r.Perfume_Name).ToList().Select(rr =>
                  new SelectListItem { Value = rr.Perfume_ID.ToString(), Text = rr.Perfume_Name }).ToList();
            ViewBag.perfumeList = perfumeList;
        }




    }

}

