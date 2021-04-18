using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using perfumedecant.Models.Domains;
using perfumedecant.Models;

namespace PerfumeDecant.Controllers
{
    public class CologneController : Controller
    {
        PD_DB db = new PD_DB();
        Log log = new Log();
        LogStatus logStatus = new LogStatus();     
        //
        // GET: /Cologne/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddCologne()
        {
            String Message = "";

            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddCologne", "DashboardCologne", logStatus.EventLog);
                return RedirectToAction("Login", "Account");
            }
            else if (Session["RoleName"].ToString() == "Admin")
            {
                InitDropdownLists();
                return View();
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "AddUser", "DashboardUser", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddCologne(Tbl_Cologne cologne)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddCologne", "DashboardCologne", logStatus.EventLog);
                return RedirectToAction("Login", "Account");
            }
            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    InitDropdownLists();
                    Tbl_Cologne c = new Tbl_Cologne();
                    c.Cologne_Perfume_ID = cologne.Cologne_Perfume_ID;
                    c.Cologne_PricePerUnit = cologne.Cologne_PricePerUnit;
                    c.Cologne_AllCount = cologne.Cologne_AllCount;
                    c.Cologne_Weight = cologne.Cologne_Weight;
                    db.Tbl_Cologne.Add(c);
                    if (Convert.ToBoolean(db.SaveChanges() > 0))
                    {
                        Message = "cologne with perfume ID " + cologne.Cologne_Perfume_ID + " added successfully.";
                        log.addLog(Message, "AddCologne", "DashboardCologne", logStatus.EventLog);
                        return RedirectToAction("Index", "Cologne");
                    }
                    else
                    {
                        Message = "added cologne with perfume ID " + cologne.Cologne_Perfume_ID + " failed.";
                        log.addLog(Message, "AddCologne", "DashboardCologne", logStatus.ErrorLog);
                        ViewBag.Error = "ادکلن جدید ثبت نشد، لطفا دوباره تلاش کنید.";
                        return View();
                    }
                }
                catch
                {
                    Message = "added cologne with perfume ID " + cologne.Cologne_Perfume_ID + " failed.";
                    log.addLog(Message, "AddCologne", "DashboardCologne", logStatus.ErrorLog);
                    ViewBag.Error = "ادکلن جدید ثبت نشد، لطفا دوباره تلاش کنید.";
                    return View();
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "AddCologne", "DashboardCologne", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }
        }

        public ActionResult EditCologne(int cologneID)
        {
             String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "EditHandySample", "DashboardHandySample", logStatus.EventLog);
                return RedirectToAction("Login", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    InitDropdownLists();
                    var cologne = db.Tbl_Cologne.Where(a => a.Cologne_ID == cologneID).SingleOrDefault();
                    if (cologne != null)
                    {
                        return View(cologne);
                    }
                    else
                    {
                        Message = "cologne with ID" + cologneID + "not found.";
                        log.addLog(Message, "EditCologne", "DashboardCologne", logStatus.EventLog);
                        ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                        return RedirectToAction("Index", "Cologne");
                    }

                }
                catch
                {
                    Message = "cologne with ID" + cologneID + "not found.";
                    log.addLog(Message, "EditCologne", "DashboardCologne", logStatus.EventLog);
                    ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                    return View();
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "EditCologne", "DashboardCologne", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditCologne(Tbl_Cologne c)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "EditCologne", "DashboardCologne", logStatus.EventLog);
                return RedirectToAction("Login", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    InitDropdownLists();
                    var cologne = db.Tbl_Cologne.Where(a => a.Cologne_ID == c.Cologne_ID).SingleOrDefault();
                    if (cologne != null)
                    {
                        cologne.Cologne_Perfume_ID = c.Cologne_Perfume_ID;
                        cologne.Cologne_AllCount = c.Cologne_AllCount;
                        cologne.Cologne_PricePerUnit = c.Cologne_PricePerUnit;
                        cologne.Cologne_Weight = c.Cologne_Weight;
                        db.Tbl_Cologne.Attach(cologne);
                        db.Entry(cologne).State = System.Data.Entity.EntityState.Modified;
                        if (Convert.ToBoolean(db.SaveChanges() > 0))
                        {
                            Message = "cologne with id " + c.Cologne_ID + " edited successfully.";
                            log.addLog(Message, "EditCologne", "DashboardCologne", logStatus.EventLog);
                            return RedirectToAction("Index", "Cologne");
                        }
                        else
                        {
                            Message = "edited cologne with id " + c.Cologne_ID + " failed.";
                            log.addLog(Message, "EditCologne", "DashboardCologne", logStatus.ErrorLog);
                            return RedirectToAction("Index", "Cologne");

                        }
                    }
                    else
                    {
                        Message = "cologne with ID" + c.Cologne_ID + "not found.";
                        log.addLog(Message, "EditCologne", "DashboardCologne", logStatus.EventLog);
                        ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                        return RedirectToAction("Index", "Cologne");
                    }

                }
                catch
                {
                    Message = "edited cologne with id " + c.Cologne_ID + " failed.";
                    log.addLog(Message, "EditCologne", "DashboardCologne", logStatus.EventLog);
                    ViewBag.Error = "محصول ویرایش نشد، لطفا دوباره تلاش کنید.";
                    return View();
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "EditCologne", "DashboardCologne", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }
        }

        public ActionResult DeleteCologne(int cologneID)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddCologne", "DashboardCologne", logStatus.EventLog);
                return RedirectToAction("Login", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    var cologne = db.Tbl_Cologne.Where(a => a.Cologne_ID == cologneID).SingleOrDefault();
                    if (cologne != null)
                    {
                        db.Tbl_Cologne.Remove(cologne);
                        if (Convert.ToBoolean(db.SaveChanges() > 0))
                        {
                            Message = "cologne with id " + cologne.Cologne_ID + " deleted successfully.";
                            log.addLog(Message, "Index", "DashboardCologne", logStatus.EventLog);
                            ViewBag.Success = "محصول شما با موفقیت حذف شد.";
                            return RedirectToAction("Index", "Cologne");
                        }
                        else
                        {
                            Message = "delete cologne with ID " + cologneID + " failed.";
                            log.addLog(Message, "DeleteCologne", "DashboardCologne", logStatus.ErrorLog);
                            ViewBag.Error = "محصول حذف نشد، لطفا دوباره تلاش کنید.";
                            return RedirectToAction("Index", "Cologne");
                        }
                    }
                    else
                    {
                        Message = "cologne with ID" + cologneID + "not found.";
                        log.addLog(Message, "DeleteCologne", "DashboardCologne", logStatus.EventLog);
                        ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                        return RedirectToAction("Index", "Cologne");
                    }
                }
                catch
                {
                    Message = "delete cologne with ID " + cologneID + " failed.";
                    log.addLog(Message, "DeleteCologne", "DashboardCologne", logStatus.ErrorLog);
                    ViewBag.Error = "محصول حذف نشد، لطفا دوباره تلاش کنید.";
                    return RedirectToAction("Index", "Cologne");
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "DeleteCologne", "DashboardCologne", logStatus.EventLog);
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