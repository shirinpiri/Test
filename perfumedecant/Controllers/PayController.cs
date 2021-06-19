using perfumedecant.Models;
using perfumedecant.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using perfumedecant.Models.Repository;
using System.Threading.Tasks;
using System.Collections.Specialized;
using ZarinPal;

namespace PerfumeDecant.Controllers
{
    public class PayController : Controller
    {
        Rep_Cart rep_cart = new Rep_Cart();
        PD_DB db = new PD_DB();
        Log log = new Log();
        LogStatus logStatus = new LogStatus();
        string Message = "";
        // GET: Pay

        [HttpPost]
        public ActionResult PayPrice(int post_type=0 , string final_price="")
        {
            try
            {
                var postType = db.Tbl_PostType.Where(a => a.PostType_Price==post_type).FirstOrDefault();
                if (Session["UserName"] == null)
                {
                    Message = "Access denied. need login.";
                    log.addLog(Message, "Index", "Cart", logStatus.EventLog);
                    return RedirectToAction("Index", "Account", new { returnUrl = "/Cart/Index" });
                }
                else
                {
                    var username = Session["UserName"].ToString();

                    var user = db.Tbl_User.Where(a => a.User_Username == username).SingleOrDefault();
                    if (user.User_Address == null || user.User_Address == "")
                    {
                        return RedirectToAction("EditProfile", "Account");
                    }
                    //if (PostagePrice == 1)
                    //{
                    //    TempData["PostageType"] = "لطفا نحوه ارسال را انتخاب نمایید.";
                    //    return RedirectToAction("Index", "Cart");
                    //}

                    long allPrice = 0;
                    var carts = rep_cart.Get_UserCart(username);
                    foreach (var cart in carts)
                    {
                        allPrice += cart.InterimBill_Price;
                    }
                    //if (allPrice > 300000)
                    //    PostagePrice = 0;

                    Session["AllPrice"] = allPrice + postType.PostType_Price;
                    Tbl_Bill bill = new Tbl_Bill();
                    bill.Bill_Date = DateTime.Now;
                    Random rnd1 = new Random();
                    int InvoiceNumber = (rnd1.Next(1, 1000) * DateTime.Now.Year) + DateTime.Now.Second;
                    bill.Bill_InvoiceNumber = InvoiceNumber.ToString();
                    bill.Bill_Off = 0;
                    bill.Bill_OtherPrice = 0;
                    bill.Bill_PostStatus_ID = 1;
                    //if (allPrice > 300000)
                    //    bill.Bill_PostType_ID = 2;
                    //else
                        bill.Bill_PostType_ID = postType.PostType_ID;
                    bill.Bill_Status = false;
                    bill.Bill_PayPrice = Convert.ToInt32(allPrice + postType.PostType_Price);
                    bill.Bill_UserID = user.User_ID;
                    db.Tbl_Bill.Add(bill);
                    if (Convert.ToBoolean(db.SaveChanges() > 0))
                    {
                        Tbl_UserBills ub = new Tbl_UserBills();
                        foreach (var item in carts)
                        {
                            ub.UserBills_Bill_ID = bill.Bill_ID;
                            ub.UserBills_InterimBill_ID = item.InterimBill_ID;
                            db.Tbl_UserBills.Add(ub);
                            db.SaveChanges();
                        }

                        try
                        {
                            Session["Bill_ID"] = bill.Bill_ID;
                            //اتصال به درگاه
                            ZarinPal.ZarinPal zarinpal = ZarinPal.ZarinPal.Get();

                            String MerchantID = "91afdd4e-006f-11ea-bad7-000c295eb8fc";
                            String CallbackURL = "http://www.perfumedecant.ir/Pay/PaymentVerification?billID=" + bill.Bill_ID;
                            //String CallbackURL = "https://localhost:44309/Pay/PaymentVerification?billID=" + bill.Bill_ID;
                            long Amount =(long) (allPrice + postType.PostType_Price);
                            String Description = "پرداخت";

                            ZarinPal.PaymentRequest pr = new ZarinPal.PaymentRequest(MerchantID, Amount, CallbackURL, Description);

                            zarinpal.DisableSandboxMode();
                            //zarinpal.EnableSandboxMode();
                            var res = zarinpal.InvokePaymentRequest(pr);


                            if (res.Status == 100)
                            {
                                //Response.Redirect("https://www.zarinpal.com/pg/StartPay/" + Authority + "/Asan");
                                //Response.Redirect("https://www.zarinpal.com/pg/StartPay/" + Authority + "/ZarinGate");
                                Response.Redirect(res.PaymentURL);

                            }
                            else
                            {
                                TempData["PayError"] = "تراکنش با موفقیت انجام نشد، لطفا دوباره تلاش کنید.";
                                RedirectToAction("Index", "Cart");
                            }
                        }
                        catch
                        {
                            TempData["PayError"] = "تراکنش با موفقیت انجام نشد، لطفا دوباره تلاش کنید.";
                            return RedirectToAction("Index", "Cart");
                        }
                    }
                    TempData["PayError"] = "تراکنش با موفقیت انجام نشد، لطفا دوباره تلاش کنید.";
                    return RedirectToAction("Index", "Cart");
                }
            }
            catch(Exception ex)
            {
                var e = ex.ToString();
                TempData["PayError"] = "تراکنش با موفقیت انجام نشد، لطفا دوباره تلاش کنید.";
                return RedirectToAction("Index", "Cart");
            }
        }

        public ActionResult PaymentVerification(int billID = 0)
        {
            NameValueCollection nvc = Request.Params;
            String Status = nvc["Status"];

            var bill = db.Tbl_Bill.Find(billID);
            var user = db.Tbl_User.Where(a => a.User_ID == bill.Bill_UserID).SingleOrDefault();
            var username = user.User_Username;
            Session["UserName"] = username;

            if (Status != "OK")
            {
                Response.Write("<script>alert('Purchase unsuccessfully')</script>");
                TempData["PayError"] = "متاسفانه پرداخت با موفقیت انجام نشد، لطفا دوباره تلاش کنید.";
                return RedirectToAction("Index", "Cart");
            }

          
            int Amount = bill.Bill_PayPrice;
            var zarinpal = ZarinPal.ZarinPal.Get();
            zarinpal.DisableSandboxMode();
            //zarinpal.EnableSandboxMode();
            String Authority = nvc["Authority"];
            String MerchantID = "91afdd4e-006f-11ea-bad7-000c295eb8fc";

            var verificationRequest = new ZarinPal.PaymentVerification(MerchantID, Amount, Authority);
            var verificationResponse = zarinpal.InvokePaymentVerification(verificationRequest);
            if (verificationResponse.Status == 100)
            {
                //clear carts
                var carts = db.Tbl_UserBills.Where(a => a.UserBills_Bill_ID == bill.Bill_ID).ToList();
                List<Tbl_InterimBill> ib_list = new List<Tbl_InterimBill>();
                foreach (var cart in carts)
                {
                    Tbl_InterimBill ib = new Tbl_InterimBill();
                    ib = db.Tbl_InterimBill.Where(a => a.InterimBill_ID == cart.UserBills_InterimBill_ID).SingleOrDefault();
                    ib.InterimBill_Status = true;
                    db.SaveChanges();
                }
                // db.Tbl_InterimBill.RemoveRange(ib_list);
                //db.SaveChanges();

                //update bill table
                bill.Bill_Status = true;
                //bill.Bill_RefID = RefID;
                db.SaveChanges();

                ViewBag.NameFamily = user.User_NameFamily;
                ViewBag.InvoiceNum = bill.Bill_InvoiceNumber;

                return View();
            }
            else
            {
                TempData["PayError"] = "تراکنش با موفقیت انجام نشد، لطفا دوباره تلاش کنید.";
                return RedirectToAction("Index", "Cart");
            }

        }


        public ActionResult test()
        {
            return View();
        }
    }//end
}