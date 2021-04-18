using perfumedecant.Models;
using perfumedecant.Models.Domains;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace perfumedecant.Controllers
{
    public class DashboardBrandController : Controller
    {
        PD_DB db = new PD_DB();
        Log log = new Log();
        LogStatus logStatus = new LogStatus();
        //
        // GET: /DashboardBrand/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddBrand()
        {
            String Message = "";

            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddBrand", "DashboardBrand", logStatus.EventLog);
                return RedirectToAction("Index", "Account");
            }
            else if (Session["RoleName"].ToString() == "Admin")
            {
                return View();
            }
            else
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddBrand", "DashboardBrand", logStatus.EventLog);
                return RedirectToAction("Index", "Account");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddBrand(Tbl_Brand brand)
        {
            var validImageTypes = new string[]
                  {
                    "image/gif",
                    "image/jpg",
                    "image/jpeg",
                    "image/pjpeg",
                    "image/png"
                  };
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddBrand", "DashboardBrand", logStatus.EventLog);
                return RedirectToAction("Index", "Account");
            }
            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    Tbl_Brand b = new Tbl_Brand();
                    b.Brand_Title = brand.Brand_Title;
                    if (brand.Brand_ImageIndex != null)
                    {
                        var uploadFiles = Request.Files[0];
                        Random rnd = new Random();

                        if (!validImageTypes.Contains(uploadFiles.ContentType))
                        {
                            Message = "invalid image format.";
                            log.addLog(Message, "AddBrand", "DashboardBrand", logStatus.EventLog);
                            ViewBag.Error = "عکس آپلود شده باید از نوع jpg ویا png باشد.";
                            return View();
                        }
                        else if (uploadFiles != null && uploadFiles.ContentLength > 0)
                        {
                            var fileName = rnd.Next().ToString() + ".jpg";
                            b.Brand_ImageIndex = fileName;
                            var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/Uploads/BrandImages/"), fileName);
                            uploadFiles.SaveAs(path);
                            Message = "Image save successfully for brand with title " + brand.Brand_Title + ".";
                            log.addLog(Message, "AddBrand", "DashboardBrand", logStatus.EventLog);
                            db.Tbl_Brand.Add(b);
                            if (Convert.ToBoolean(db.SaveChanges() > 0))
                            {
                                Message = "Brand with title " + brand.Brand_Title + " added successfully.";
                                log.addLog(Message, "AddBrand", "DashboardBrand", logStatus.EventLog);
                                return RedirectToAction("Index", "DashboardBrand");
                            }
                            else
                            {
                                Message = "added Brand with title " + brand.Brand_Title + " failed.";
                                log.addLog(Message, "AddBrand", "DashboardBrand", logStatus.EventLog);
                                ViewBag.Error = "برند جدید ثبت نشد، لطفا دوباره تلاش کنید.";
                                return View();
                            }

                        }
                        else
                        {
                            Message = "Image save for brand with title " + b.Brand_Title + " failed.";
                            log.addLog(Message, "AddBrand", "DashboardBrand", logStatus.EventLog);
                            ViewBag.EmptyImage = "آپلود عکس با خطا مواجه شده است.";
                            return View();
                        }
                    }
                    else
                    {
                        Message = "Image save for brand with title " + b.Brand_Title + " failed.";
                        log.addLog(Message, "AddBrand", "DashboardBrand", logStatus.EventLog);
                        ViewBag.EmptyImage = "آپلود عکس با خطا مواجه شده است.";
                        return View();
                    }       
                }
                catch
                {
                    Message = "added brand with title " + brand.Brand_Title + " failed.";
                    log.addLog(Message, "AddBrand", "DashboardBrand", logStatus.ErrorLog);
                    ViewBag.Error = "برند جدید ثبت نشد، لطفا دوباره تلاش کنید.";
                    return View();
                }
            }
            else
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddBrand", "DashboardBrand", logStatus.EventLog);
                return RedirectToAction("Index", "Account");
            }
        }

        public ActionResult DeleteBrand(int brandID)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "DeleteBrand", "DashboardBrand", logStatus.EventLog);
                return RedirectToAction("Index", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    var brand = db.Tbl_Brand.Where(a => a.Brand_ID == brandID).SingleOrDefault();
                    if (brand != null)
                    {
                        if (brand.Brand_ImageIndex != null)
                        {
                            var oldFile = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/Uploads/BrandImages/"), brand.Brand_ImageIndex);
                            //var oldFile = Path.Combine(Server.MapPath("/Uploads/BrandImages/"), brand.Brand_ImageIndex);
                            if (System.IO.File.Exists(oldFile))
                            {
                                System.IO.File.Delete(oldFile);
                                Message = "Delete image for brand with Id " + brandID + " Done successfully.";
                                log.addLog(Message, "DeleteBrand", "DashboardBrand", logStatus.EventLog);
                            }
                        }
                        db.Tbl_Brand.Remove(brand);
                        if (Convert.ToBoolean(db.SaveChanges() > 0))
                        {
                            Message = "Brand with title " + brand.Brand_Title + " deleted successfully.";
                            log.addLog(Message, "DeleteBrand", "DashboardBrand", logStatus.EventLog);
                            ViewBag.Success = "برند شما با موفقیت حذف شد.";
                            return RedirectToAction("Index", "DashboardBrand");
                        }
                        else
                        {
                            Message = "delete brand with ID " + brandID + " failed.";
                            log.addLog(Message, "DeleteBrand", "DashboardBrand", logStatus.ErrorLog);
                            ViewBag.Error = "برند حذف نشد، لطفا دوباره تلاش کنید.";
                            return RedirectToAction("Index", "DashboardBrand");
                        }
                    }
                    else
                    {
                        Message = "brand with ID" + brandID + "not found.";
                        log.addLog(Message, "DeleteBrand", "DashboardBrand", logStatus.ErrorLog);
                        ViewBag.Error = "برند پیدا نشد، لطفا دوباره تلاش کنید.";
                        return RedirectToAction("Index", "DashboardBrand");
                    }
                }
                catch
                {
                    Message = "delete brand with ID " + brandID + " failed.";
                    log.addLog(Message, "DeleteBrand", "DashboardBrand", logStatus.ErrorLog);
                    ViewBag.Error = "برند حذف نشد، لطفا دوباره تلاش کنید.";
                    return RedirectToAction("Index", "DashboardBrand");
                }
            }
            else
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "DeleteBrand", "DashboardBrand", logStatus.EventLog);
                return RedirectToAction("Index", "Account");
            }
        }
    }



}