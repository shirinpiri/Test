using perfumedecant.Models;
using perfumedecant.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace perfumedecant.Controllers
{
    public class DashboardOrderController : Controller
    {

        PD_DB db = new PD_DB();
        Log log = new Log();
        LogStatus logStatus = new LogStatus();

        public ActionResult Index()
        {
            InitDropdownLists();
            return View();  
        }

        public ActionResult EditOrder(int id = 0)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "EditOrder", "DashboardOrder", logStatus.EventLog);
                return RedirectToAction("Login", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    InitDropdownLists();
                    var qUserBillEdit = db.Tbl_UserBills.Where(a => a.UserBills_ID == id).SingleOrDefault();
                    if (qUserBillEdit != null)
                    {
                        return View(qUserBillEdit);
                    }
                    else
                    {
                        Message = "UserBill with ID" + id + "not found.";
                        log.addLog(Message, "EditOrder", "DashboardOrder", logStatus.EventLog);
                        ViewBag.result = "سفارش پیدا نشد، لطفا دوباره تلاش کنید.";
                        return RedirectToAction("Index", "DashboardOrder");
                    }
                }
                catch
                {
                    Message = "edit UserBill with ID " + id + " failed.";
                    log.addLog(Message, "EditOrder", "DashboardOrder", logStatus.ErrorLog);
                    ViewBag.result = "سفارش ویرایش نشد. لطفاً دوباره تلاش کنید.";
                    return View();
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "EditOrder", "DashboardOrder", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditOrder(Tbl_UserBills newUserBill)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "EditOrder", "DashboardOrder", logStatus.EventLog);
                return RedirectToAction("Login", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                Tbl_UserBills oldUserBill = new Tbl_UserBills();
                try
                {
                  

                    InitDropdownLists();
                    oldUserBill = db.Tbl_UserBills.Where(a => a.UserBills_ID == newUserBill.UserBills_ID).SingleOrDefault();


                    if (oldUserBill == null)
                    {
                        Message = "UserBill with ID" + newUserBill.UserBills_ID + "not found.";
                        log.addLog(Message, "EditUser", "DashboardUser", logStatus.ErrorLog);
                        ViewBag.result = "سفارش یافت نشد.لطفاً دوباره تلاش کنید.";
                        return RedirectToAction("Index", "DashboardOrder");
                    }

                    oldUserBill.UserBills_Bill_ID = newUserBill.UserBills_Bill_ID;
                    oldUserBill.UserBills_InterimBill_ID = newUserBill.UserBills_InterimBill_ID;

                    db.Tbl_UserBills.Attach(oldUserBill);
                    db.Entry(oldUserBill).State = System.Data.Entity.EntityState.Modified;
                    if (Convert.ToBoolean(db.SaveChanges() > 0))
                    {
                            Message = "edited UserBill with id " + newUserBill.UserBills_ID + " done successfully.";
                            log.addLog(Message, "EditOrder", "DashboardOrder", logStatus.EventLog);
                            ViewBag.result = "سفارش با موفقیت ویرایش شد.";
                            return View(newUserBill);
                    }
                    else
                    {
                        Message = "edited UserBill with id " + newUserBill.UserBills_ID + " failed.";
                        log.addLog(Message, "EditOrder", "DashboardOrder", logStatus.ErrorLog);
                        ViewBag.result = "سفارش ویرایش نشد، لطفا دوباره تلاش کنید.";
                        return View(newUserBill);
                    }

                }
                catch
                {
                    Message = "edited UserBill with id " + newUserBill.UserBills_ID + " failed.";
                    log.addLog(Message, "EditOrder", "DashboardOrder", logStatus.ErrorLog);
                    ViewBag.result = "سفارش ویرایش نشد، لطفا دوباره تلاش کنید.";
                    return View(newUserBill);
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "EditOrder", "DashboardOrder", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }

        }

        public ActionResult OrderDetails(int id = 0)
        {
            string Message = "";
            var qUserBillDetails = db.Tbl_UserBills.Where(a => a.UserBills_ID == id).SingleOrDefault();

            int post_price = 0;
            if (qUserBillDetails.Tbl_InterimBill.InterimBill_Price > 300000)
                post_price = 0;
            else
                post_price = (int)qUserBillDetails.Tbl_Bill.Tbl_PostType.PostType_Price;

            ViewBag.PostPrice = post_price;

            if (qUserBillDetails != null)
            {
                var sumPrice = qUserBillDetails.Tbl_InterimBill.InterimBill_Price +
                               post_price +
                               qUserBillDetails.Tbl_Bill.Bill_OtherPrice -
                               qUserBillDetails.Tbl_Bill.Bill_Off;
                ViewBag.finalPrice = sumPrice;
                return View(qUserBillDetails);
            }
            else
            {
                Message = "UserBill with ID" + id + "not found.";
                log.addLog(Message, "OrderDetails", "DashboardOrder", logStatus.EventLog);
                ViewBag.result = "سفارش پیدا نشد، لطفا دوباره تلاش کنید.";
                return RedirectToAction("Index", "DashboardOrder");
            }
        }

        public ActionResult DeleteOrder(int id = 0)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "DeleteOrder", "DashboardOrder", logStatus.EventLog);
                return RedirectToAction("Login", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    var userBill = db.Tbl_UserBills.Where(a => a.UserBills_ID == id).SingleOrDefault();
                    if (userBill != null)
                    {
                        db.Tbl_UserBills.Remove(userBill);
                        if (Convert.ToBoolean(db.SaveChanges() > 0))
                        {
                            Message = "UserBill with id " + userBill.UserBills_ID + " deleted successfully.";
                            log.addLog(Message, "Index", "DashboardOrder", logStatus.EventLog);
                            ViewBag.Success = "سفارش با موفقیت حذف شد.";
                            return RedirectToAction("Index", "DashboardOrder");
                        }
                        else
                        {
                            Message = "delete UserBill with ID " + id + " failed.";
                            log.addLog(Message, "DeleteOrder", "DashboardOrder", logStatus.ErrorLog);
                            ViewBag.result = "سفارش حذف نشد، لطفا دوباره تلاش کنید.";
                            return RedirectToAction("Index", "DashboardOrder");
                        }
                    }
                    else
                    {
                        Message = "UserBill with ID" + id + "not found.";
                        log.addLog(Message, "DeleteOrder", "DashboardOrder", logStatus.EventLog);
                        ViewBag.result = "سفارش پیدا نشد، لطفا دوباره تلاش کنید.";
                        return RedirectToAction("Index", "DashboardOrder");
                    }
                }
                catch
                {
                    Message = "delete UserBill with ID " + id + " failed.";
                    log.addLog(Message, "DeleteOrder", "DashboardOrder", logStatus.ErrorLog);
                    ViewBag.result = "سفارش حذف نشد، لطفا دوباره تلاش کنید.";
                    return RedirectToAction("Index", "DashboardOrder");
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "DeleteOrder", "DashboardOrder", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }

        }

        public ActionResult ChangeOrderStatus(int statusID = 0, int id = 0)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "ChangeOrderStatus", "DashboardOrder", logStatus.EventLog);
                return RedirectToAction("Login", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    //var userBill = db.Tbl_UserBills.Where(a => a.UserBills_Bill_ID == id).SingleOrDefault();
                    var oldBill = db.Tbl_Bill.Where(a => a.Bill_ID == id).SingleOrDefault();
                    //var postStatus = db.Tbl_PostStatus.Where(a => a.PostStatus_ID == statusID).SingleOrDefault();

                    if (oldBill != null)
                    {
                        oldBill.Bill_PostStatus_ID = statusID;

                        db.Tbl_Bill.Attach(oldBill);
                        db.Entry(oldBill).State = System.Data.Entity.EntityState.Modified;

                        if (Convert.ToBoolean(db.SaveChanges() > 0))
                        {
                            Message = "Post status of Bill with id " + id + " changed successfully.";
                            log.addLog(Message, "ChangeOrderStatus", "DashboardOrder", logStatus.EventLog);
                            ViewBag.Success = "وضعیت سفارش با موفقیت تغییر کرد.";
                            return RedirectToAction("Index", "DashboardOrder");
                        }
                        else
                        {
                            Message = "change Post status of Bill with ID " + id + " failed.";
                            log.addLog(Message, "ChangeOrderStatus", "DashboardOrder", logStatus.ErrorLog);
                            ViewBag.result = "تغییر وضعیت سفارش با خطا مواجه شده است.";
                            return RedirectToAction("Index", "DashboardOrder");
                        }
                    }
                    else
                    {
                        Message = "Bill with ID" + id + "not found.";
                        log.addLog(Message, "ChangeOrderStatus", "DashboardOrder", logStatus.EventLog);
                        ViewBag.result = "تغییر وضعیت سفارش با خطا مواجه شده است.";
                        return RedirectToAction("Index", "DashboardOrder");
                    }
                }
                catch
                {
                    Message = "change Post status of Bill with ID " + id + " failed.";
                    log.addLog(Message, "ChangeOrderStatus", "DashboardOrder", logStatus.ErrorLog);
                    ViewBag.result = "تغییر وضعیت سفارش با خطا مواجه شده است.";
                    return RedirectToAction("Index", "DashboardOrder");
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "DeleteOrder", "DashboardOrder", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }

        }

        public void InitDropdownLists()
        {
            //Post Type list
            var PostTypeList = db.Tbl_PostType.OrderBy(r => r.PostType_Title).ToList().Select(rr =>
                new SelectListItem { Value = rr.PostType_ID.ToString(), Text = rr.PostType_Title }).ToList();
            ViewBag.postTypes = PostTypeList;

            //Post Status list
            var PostStatusList = db.Tbl_PostStatus.OrderBy(r => r.PostStatus_ID).ToList().Select(rr =>
                new SelectListItem { Value = rr.PostStatus_ID.ToString(), Text = rr.PostStatus_Title }).ToList();
            ViewBag.postStatuses = PostStatusList;
        }
    }
}