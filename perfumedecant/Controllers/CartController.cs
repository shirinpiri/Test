using perfumedecant.Models;
using perfumedecant.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace perfumedecant.Controllers
{
    public class CartController : Controller
    {
        PD_DB db = new PD_DB();
        Log log = new Log();
        LogStatus logStatus = new LogStatus();
        //
        // GET: /Cart/
        public ActionResult Index()
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "Index", "Cart", logStatus.EventLog);
                return RedirectToAction("Index", "Account");
            }
            else
            {
            return View();
            }
        }

        public ActionResult AddCart(int PerfumeID)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddCart", "Cart", logStatus.EventLog);
                return RedirectToAction("Index", "Account");
            }
            var perfume = db.Tbl_Perfume.Where(a => a.Perfume_ID == PerfumeID).SingleOrDefault();
            if (perfume != null)
            {
                InitDropdownLists(PerfumeID);
                return View(perfume);
            }
            else
            {
                Message = "perfume with ID" + PerfumeID + "not found.";
                log.addLog(Message, "AddCart", "Cart", logStatus.EventLog);
                ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                return RedirectToAction("Index", "Home");
            }
        }

        public JsonResult AddToCart(int PerfumeID = 0, string Category_Title = "", float Weight = 0, int Count = 0)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddCart", "Cart", logStatus.EventLog);
                string message = "Login";
                return Json(message, JsonRequestBehavior.AllowGet);
            }
            else if (Category_Title == "")
            {
                Message = "added perfume with perfume ID " + PerfumeID + "failed";
                log.addLog(Message, "AddToCart", "Cart", logStatus.EventLog);
                string message =  "محصول به سبد اضافه نشد، لطفا اطلاعات محصول را با دقت وارد نمایید.";
                return Json(message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                try
                {
                    Tbl_InterimBill ib = new Tbl_InterimBill();
                    ib.InterimBill_Category_ID = db.Tbl_Category.Where(a => a.Category_Title == Category_Title).SingleOrDefault().Category_ID;                                    
                        ib.InterimBill_Weight = Weight;
                        ib.InterimBill_Count = Count;
                    ib.InterimBill_Date = DateTime.Now;
                    ib.InterimBill_ExpDate = DateTime.Now.AddDays(5);


                    Random rnd1 = new Random();
                    int InvoiceNumber = (rnd1.Next(1, 1000) * DateTime.Now.Year) + DateTime.Now.Second;
                    ib.InterimBill_InvoiceNum = InvoiceNumber;
                    ib.InterimBill_Perfume_ID = PerfumeID;
                    if (Category_Title == "ادکلن")
                    {
                        var cologne = db.Tbl_Cologne.Where(a => a.Cologne_Perfume_ID == PerfumeID && a.Cologne_Weight == Weight).SingleOrDefault();
                        if (cologne != null)
                        {
                            ib.InterimBill_Price = (cologne.Cologne_PricePerUnit) * Count;
                        }
                        else
                        {
                            ib.InterimBill_Price = 0;
                        }
                        if (Count > cologne.Cologne_AllCount)
                        {
                            string message = "تعداد انتخاب شده بیشتر از موجودی می باشد، لطفا تماس بگیرید.(09128774252)";
                            return Json(message, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else if (Category_Title == "سمپل شرکتی")
                    {
                        var sample = db.Tbl_CompanySample.Where(a => a.CompanySample_Perfume_ID == PerfumeID && (float)a.CompanySample_Weight == (float)Weight).SingleOrDefault();
                        if (sample != null)
                        {
                            var pricePerUnit = Convert.ToInt32(sample.CompanySample_Price);
                            ib.InterimBill_Price = pricePerUnit * Count;
                        }
                        else
                        {
                            ib.InterimBill_Price = 0;
                        }
                        if (Count > sample.CompanySample_AllCount)
                        {
                            string message = "تعداد انتخاب شده بیشتر از موجودی می باشد، لطفا تماس بگیرید.(09128774252)";
                            return Json(message, JsonRequestBehavior.AllowGet);
                        }
                    }
                    else
                    {
                        var handy_sample = db.Tbl_HandySample.Where(a => a.HandySample_Perfume_ID == PerfumeID).SingleOrDefault();
                        if (handy_sample != null)
                        {
                            var pricePerMil = Convert.ToInt32(handy_sample.HandySample_PricePerMil * Weight);
                            ib.InterimBill_Price = pricePerMil * Count;
                        }
                        else
                        {
                            ib.InterimBill_Price = 0;
                        }
                        if (Count*Weight > handy_sample.HandySample_AllWeight)
                        {
                            string message = "تعداد انتخاب شده بیشتر از موجودی می باشد، لطفا تماس بگیرید.(09128774252)";
                            return Json(message, JsonRequestBehavior.AllowGet);
                        }
                    }
                    string username = Session["UserName"].ToString();
                    var userID = db.Tbl_User.Where(a => a.User_Username == username).SingleOrDefault().User_ID;
                    ib.InterimBill_User_ID = userID;
                    ib.InterimBill_Status = false;
                    db.Tbl_InterimBill.Add(ib);
                    if (Convert.ToBoolean(db.SaveChanges() > 0))
                    {
                        Message = "perfume with perfume ID " + PerfumeID + " added to cart successfully.";
                        log.addLog(Message, "AddCart", "Cart", logStatus.EventLog);
                        string message = "OK";
                        return Json(message, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        Message = "added perfume with perfume ID " + PerfumeID + " failed.";
                        log.addLog(Message, "AddCart", "Cart", logStatus.ErrorLog);
                        string message = "محصول به سبد اضافه نشد، لطفا دوباره تلاش کنید.";
                        return Json(message, JsonRequestBehavior.AllowGet);
                    }
                }
                catch
                {
                    Message = "added perfume with perfume ID " + PerfumeID + " failed.";
                    log.addLog(Message, "AddCart", "Cart", logStatus.ErrorLog);
                    string message = "محصول به سبد اضافه نشد، لطفا دوباره تلاش کنید.";
                    return Json(message, JsonRequestBehavior.AllowGet);

                }
            }
        }

        public ActionResult DeleteCart(int id = 0)
        {
                String Message = "";
                if (Session["UserName"] == null)
                {
                    Message = "Access denied. need login.";
                    log.addLog(Message, "DeleteCart", "Cart", logStatus.EventLog);
                    return RedirectToAction("Index", "Account");
                }
                else
                {
                    try
                    {
                        var ib = db.Tbl_InterimBill.Where(a => a.InterimBill_ID == id).SingleOrDefault();
                        if (ib != null)
                        {
                            db.Tbl_InterimBill.Remove(ib);
                            if (Convert.ToBoolean(db.SaveChanges() > 0))
                            {
                                Message = "cart with id " + id + " deleted successfully.";
                                log.addLog(Message, "DeleteCart", "Cart", logStatus.EventLog);
                                ViewBag.Success = "محصول شما با موفقیت حذف شد.";
                                return RedirectToAction("Index", "Cart");
                            }
                            else
                            {
                                Message = "delete cart with ID " + id + " failed.";
                                log.addLog(Message, "DeleteCart", "Cart", logStatus.ErrorLog);
                                ViewBag.Error = "محصول حذف نشد، لطفا دوباره تلاش کنید.";
                                return RedirectToAction("Index", "Cart");
                            }
                        }
                        else
                        {
                            Message = "cart with ID" + id + "not found.";
                            log.addLog(Message, "DeleteCart", "Cart", logStatus.EventLog);
                            ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                            return RedirectToAction("Index", "Cart");
                        }
                    }
                    catch
                    {
                        Message = "delete cart with ID " + id + " failed.";
                        log.addLog(Message, "DeleteCart", "Cart", logStatus.ErrorLog);
                        ViewBag.Error = "محصول حذف نشد، لطفا دوباره تلاش کنید.";
                        return RedirectToAction("Index", "Cart");
                    }
                }              
        }

        //public ActionResult CartDetails(int id = 0)
        //{
        //    String Message = "";
        //    if (Session["UserName"] == null)
        //    {
        //        Message = "Access denied. need login.";
        //        log.addLog(Message, "CartDetails", "Cart", logStatus.EventLog);
        //        return RedirectToAction("Index", "Account");
        //    }
        //    else
        //    {
        //        try
        //        {
        //            var ib = db.Tbl_InterimBill.Where(a=>a.InterimBill_ID == id).SingleOrDefault();
        //            if(ib != null)
        //            {
        //                return View(ib);
        //            }
        //            else
        //            {
        //                Message = "cart with ID" + id + "not found.";
        //                log.addLog(Message, "CartDetails", "Cart", logStatus.EventLog);
        //                ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
        //                return RedirectToAction("Index","Cart");
        //            }
        //        }
        //        catch
        //        {
        //            Message = "cart with ID" + id + "not found.";
        //            log.addLog(Message, "CartDetails", "Cart", logStatus.EventLog);
        //            ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
        //            return RedirectToAction("Index", "Cart");
        //        }                
        //    }
        //}

        public void InitDropdownLists(int perfumeID)
        {
            var categoryList = new List<SelectListItem>();
            var cologne = db.Tbl_Cologne.Where(a => a.Cologne_Perfume_ID == perfumeID).FirstOrDefault();
            if (cologne != null)
            {
                categoryList.Add(new SelectListItem { Text = "ادکلن", Value = "ادکلن" });
            }

            var handySample = db.Tbl_HandySample.Where(a => a.HandySample_Perfume_ID == perfumeID).FirstOrDefault();
            if (handySample != null)
            {
                categoryList.Add(new SelectListItem { Text = "سمپل دست ریز", Value = "سمپل دست ریز" });
            }

            var companySample = db.Tbl_CompanySample.Where(a => a.CompanySample_Perfume_ID == perfumeID).FirstOrDefault();
            if (companySample != null)
            {
                categoryList.Add(new SelectListItem { Text = "سمپل شرکتی", Value = "سمپل شرکتی" });
            }
            //var categoryList = db.Tbl_Category.OrderBy(r => r.Category_Title).ToList().Select(rr =>
            //new SelectListItem { Value = rr.Category_Title.ToString(), Text = rr.Category_Title }).ToList();
            ViewBag.categoryList = categoryList;

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

        public JsonResult GetWeights(string category, int perfumeID)
        {
            List<SelectListItem> weights = new List<SelectListItem>();
            switch (category)
            {
                case "ادکلن":
                    break;

                case "سمپل شرکتی":
                    weights = db.Tbl_CompanySample.Where(a => a.CompanySample_Perfume_ID == perfumeID).ToList().Select(rr =>
                  new SelectListItem { Value = rr.CompanySample_Weight.ToString(), Text = rr.CompanySample_Weight.ToString() }).ToList();
                    break;

                case "سمپل دست ریز":
                    weights = new List<SelectListItem>
                    {
                        new SelectListItem{ Text="3", Value = "3" , Selected = true},
                        new SelectListItem{ Text="5", Value = "5" },
                        new SelectListItem{ Text="10", Value = "10"},
                        new SelectListItem{ Text="15", Value = "15" },
                        new SelectListItem{ Text="20", Value = "20" },
                        new SelectListItem{ Text="25", Value = "25" },
                    };
                    break;
            }
            return Json(new SelectList(weights, "Value", "Text"));
        }

        //[HttpPost]
        //public ActionResult ChangeCartCount(int interimBill_id , int count)
        //{
        //    if (interimBill_id != 0)
        //    {
        //        var cart = db.Tbl_InterimBill.Find(interimBill_id);
        //        int PerfumeID = (int)cart.InterimBill_Perfume_ID;
        //        float weight =(float) cart.InterimBill_Weight;

        //        cart.InterimBill_Count = count;
        //        if (cart.Tbl_Category.Category_Title == "ادکلن")
        //        {
        //            var cologne = db.Tbl_Cologne.Where(a => a.Cologne_Perfume_ID == PerfumeID).SingleOrDefault();
        //            cart.InterimBill_Price = (cologne.Cologne_PricePerUnit) * count;
        //            if (count > cologne.Cologne_AllCount)
        //            {
        //                TempData["Stok"] = "تعداد انتخاب شده بیشتر از موجودی می باشد، لطفا تماس بگیرید.";
        //                return RedirectToAction("Index", "Cart");
        //            }
        //            else
        //            {
        //                cart.InterimBill_Count = count;
        //                cart.InterimBill_Weight = 0;
        //            }
        //        }
        //        else if (cart.Tbl_Category.Category_Title == "سمپل شرکتی")
        //        {
        //            var sample = db.Tbl_CompanySample.Where(a => a.CompanySample_Perfume_ID == PerfumeID && (float)a.CompanySample_Weight == weight).SingleOrDefault();
        //            if (sample != null)
        //            {
        //                var pricePerUnit = Convert.ToInt32(sample.CompanySample_Price);
        //                cart.InterimBill_Price = pricePerUnit * count;
        //            }
        //            else
        //            {
        //                cart.InterimBill_Price = 0;
        //            }
        //        }
        //        else
        //        {
        //            int pricePerMil = db.Tbl_HandySample.Where(a => a.HandySample_Perfume_ID == PerfumeID).SingleOrDefault().HandySample_PricePerMil;
        //            cart.InterimBill_Price = Convert.ToInt32(weight * pricePerMil * count);
        //        }
        //        db.Tbl_InterimBill.Attach(cart);
        //        db.Entry(cart).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //    return Json(true, JsonRequestBehavior.AllowGet);
        //}
    }
}