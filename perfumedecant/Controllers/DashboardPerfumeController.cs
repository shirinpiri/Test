using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using perfumedecant.Models;
using perfumedecant.Models.Domains;
using System.IO;

namespace perfumedecant.Controllers
{
    public class DashboardPerfumeController : Controller
    {
        PD_DB db = new PD_DB();
        Log log = new Log();
        LogStatus logStatus = new LogStatus();
        String Message = "";

        string[] validImageTypes = new string[]
{
                    "image/gif",
                    "image/jpg",
                    "image/jpeg",
                    "image/pjpeg",
                    "image/png"
};
        //
        // GET: /DashboardPerfume/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddPerfume()
        {
            String Message = "";

            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddPerfume", "DashboardPerfume", logStatus.EventLog);
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
                log.addLog(Message, "AddUser", "DashboardUser", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddPerfume(Tbl_Perfume perfume, int? spring, int? summer, int? autumn, int? winter,
            IEnumerable<HttpPostedFileBase> files)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddPerfume", "DashboardPerfume", logStatus.EventLog);
                return RedirectToAction("Index", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    var validImageTypes = new string[]
                  {
                    "image/gif",
                    "image/jpg",
                    "image/jpeg",
                    "image/pjpeg",
                    "image/png"
                  };

                    InitDropdownLists();
                    Tbl_Perfume p = new Tbl_Perfume();

                    p.Perfume_Brand_ID = perfume.Perfume_Brand_ID;
                    p.Perfume_Gender = perfume.Perfume_Gender;
                    p.Perfume_Country = perfume.Perfume_Country;
                    p.Perfume_Name = perfume.Perfume_Name;
                    p.Perfume_Notes = perfume.Perfume_Notes;
                    p.Perfume_OlfactionGroups = perfume.Perfume_OlfactionGroups;
                    p.Perfume_Perfumer = perfume.Perfume_Perfumer;
                    p.Perfume_Description = perfume.Perfume_Description;

                    p.Perfume_SpecialOffer = perfume.Perfume_SpecialOffer;
                    p.Perfume_TemperOfPerfume = perfume.Perfume_TemperOfPerfume;
                    p.Perfume_Type_ID = perfume.Perfume_Type_ID;

                    if (perfume.Perfume_ImageIndex != null && perfume.Perfume_ImageIndex != "")
                    {
                        var uploadFiles = Request.Files[0];
                        Random rnd = new Random();

                        if (!validImageTypes.Contains(uploadFiles.ContentType))
                        {
                            Message = "invalid image format.";
                            log.addLog(Message, "AddPerfume", "DashboardPerfume", logStatus.EventLog);
                            ViewBag.Error = "عکس آپلود شده باید از نوع jpg ویا png باشد.";
                            return View();
                        }
                        else if (uploadFiles != null && uploadFiles.ContentLength > 0)
                        {
                            var fileName = rnd.Next().ToString() + ".jpg";
                            p.Perfume_ImageIndex = fileName;

                            var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/Uploads/PerfumeImages/"), fileName);
                            uploadFiles.SaveAs(path);
                            Message = "Image save successfully for Perfumes with title " + p.Perfume_Name + ".";
                            log.addLog(Message, "AddPerfume", "DashboardPerfume", logStatus.EventLog);
                            //return View();
                            db.Tbl_Perfume.Add(p);
                            if (Convert.ToBoolean(db.SaveChanges() > 0))
                            {
                                //////add season////////
                                List<Tbl_PerfumeSeason> seasons = new List<Tbl_PerfumeSeason>();

                                if (spring == 1)
                                {
                                    Tbl_PerfumeSeason ps = new Tbl_PerfumeSeason();
                                    int springID = db.Tbl_Season.Where(a => a.Season_Title == "مناسب فصل بهار").SingleOrDefault().Season_ID;
                                    ps.PerfumeSeason_Season_ID = springID;
                                    ps.PerfumeSeason_Perfume_ID = p.Perfume_ID;
                                    seasons.Add(ps);
                                }
                                if (summer == 1)
                                {
                                    Tbl_PerfumeSeason ps = new Tbl_PerfumeSeason();
                                    int summerID = db.Tbl_Season.Where(a => a.Season_Title == "مناسب فصل تابستان").SingleOrDefault().Season_ID;
                                    ps.PerfumeSeason_Season_ID = summerID;
                                    ps.PerfumeSeason_Perfume_ID = p.Perfume_ID;
                                    seasons.Add(ps);
                                }
                                if (autumn == 1)
                                {
                                    Tbl_PerfumeSeason ps = new Tbl_PerfumeSeason();
                                    int autumnID = db.Tbl_Season.Where(a => a.Season_Title == "مناسب فصل پاییز").SingleOrDefault().Season_ID;
                                    ps.PerfumeSeason_Season_ID = autumnID;
                                    ps.PerfumeSeason_Perfume_ID = p.Perfume_ID;
                                    seasons.Add(ps);
                                }
                                if (winter == 1)
                                {
                                    Tbl_PerfumeSeason ps = new Tbl_PerfumeSeason();
                                    int winterID = db.Tbl_Season.Where(a => a.Season_Title == "مناسب فصل زمستان").SingleOrDefault().Season_ID;
                                    ps.PerfumeSeason_Season_ID = winterID;
                                    ps.PerfumeSeason_Perfume_ID = p.Perfume_ID;
                                    seasons.Add(ps);
                                }
                                db.Tbl_PerfumeSeason.AddRange(seasons);
                                if (Convert.ToBoolean(db.SaveChanges() > 0))
                                {
                                    List<Tbl_PerfumeImages> images = new List<Tbl_PerfumeImages>();
                                    Tbl_PerfumeImages imageIndexes = new Tbl_PerfumeImages();

                                    foreach (var file in files)
                                    {
                                        if (file.ContentLength > 0)
                                        {
                                            var fileName1 = rnd.Next().ToString() + ".jpg";
                                            var path1 = Path.Combine(Server.MapPath("/Uploads/PerfumeImages/"), fileName1);
                                            file.SaveAs(path1);

                                            imageIndexes = new Tbl_PerfumeImages();
                                            imageIndexes.PerfumeImages_Perfume_ID = p.Perfume_ID;
                                            imageIndexes.PerfumeImages_ImageIndex = fileName1;
                                            images.Add(imageIndexes);
                                        }
                                    }
                                    db.Tbl_PerfumeImages.AddRange(images);
                                    if (Convert.ToBoolean(db.SaveChanges() > 0))
                                    {
                                        Message = "Perfume with title " + perfume.Perfume_Name + " added successfully.";
                                        log.addLog(Message, "AddPerfume", "DashboardPerfume", logStatus.EventLog);
                                        return RedirectToAction("Index", "DashboardPerfume");
                                    }
                                    else
                                    {
                                        db.Tbl_Perfume.Remove(p);
                                        db.SaveChanges();
                                        Message = "added Perfume with title " + perfume.Perfume_Name + " failed.";
                                        log.addLog(Message, "AddPerfume", "DashboardPerfume", logStatus.ErrorLog);
                                        ViewBag.Error = "محصول جدید ثبت نشد، لطفا دوباره تلاش کنید.";
                                        return View();
                                    }
                                }
                                else
                                {
                                    db.Tbl_Perfume.Remove(p);
                                    db.SaveChanges();
                                    Message = "added Perfume with title " + perfume.Perfume_Name + " failed.";
                                    log.addLog(Message, "AddPerfume", "DashboardPerfume", logStatus.ErrorLog);
                                    ViewBag.Error = "محصول جدید ثبت نشد، لطفا دوباره تلاش کنید.";
                                    return View();
                                }
                            }
                            else
                            {
                                Message = "added Perfume with title " + perfume.Perfume_Name + " failed.";
                                log.addLog(Message, "AddPerfume", "DashboardPerfume", logStatus.EventLog);
                                ViewBag.Error = "محصول جدید ثبت نشد، لطفا دوباره تلاش کنید.";
                                return View();
                            }
                        }
                        else
                        {
                            Message = "Image save for Perfuem with title " + p.Perfume_Name + " failed.";
                            log.addLog(Message, "AddPerfume", "DashboardPerfume", logStatus.EventLog);
                            ViewBag.EmptyImage = "آپلود عکس با خطا مواجه شده است.";
                            return View();
                        }
                    }
                    else
                    {
                        Message = "added Perfume with title " + perfume.Perfume_Name + " failed.";
                        log.addLog(Message, "AddPerfume", "DashboardPerfume", logStatus.EventLog);
                        ViewBag.Error = "محصول جدید ثبت نشد، لطفا دوباره تلاش کنید.";
                        return View();
                    }


                }
                catch
                {
                    Message = "added Perfume with title " + perfume.Perfume_Name + " failed.";
                    log.addLog(Message, "AddPerfume", "DashboardPerfume", logStatus.ErrorLog);
                    ViewBag.Error = "محصول جدید ثبت نشد، لطفا دوباره تلاش کنید.";
                    return View();
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "AddUser", "DashboardUser", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }
        }

        public ActionResult EditPerfume(int perfumeID)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "EditPerfume", "DashboardPerfume", logStatus.EventLog);
                return RedirectToAction("Index", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    InitDropdownLists();
                    var perfume = db.Tbl_Perfume.Where(a => a.Perfume_ID == perfumeID).SingleOrDefault();
                    if (perfume != null)
                    {
                        return View(perfume);
                    }
                    else
                    {
                        Message = "perfume with ID" + perfumeID + "not found.";
                        log.addLog(Message, "EditPerfume", "DashboardPerfume", logStatus.EventLog);
                        ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                        return RedirectToAction("Index", "DashboardPerfume");
                    }

                }
                catch
                {
                    Message = "edit Perfume with ID " + perfumeID + " failed.";
                    log.addLog(Message, "EditPerfume", "DashboardPerfume", logStatus.ErrorLog);
                    ViewBag.Error = "محصول ویرایش نشد، لطفا دوباره تلاش کنید.";
                    return View();
                }
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
        public ActionResult EditPerfume(Tbl_Perfume perfume, int? spring, int? summer, int? autumn, int? winter,
            IEnumerable<HttpPostedFileBase> files)
        {
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "EditPerfume", "DashboardPerfume", logStatus.EventLog);
                return RedirectToAction("Index", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                Tbl_Perfume p = new Tbl_Perfume();
                try
                {
                    InitDropdownLists();
                    p = db.Tbl_Perfume.Where(a => a.Perfume_ID == perfume.Perfume_ID).SingleOrDefault();
                    string oldImageName = p.Perfume_ImageIndex;
                    if (p == null)
                    {
                        Message = "perfume with ID" + perfume.Perfume_ID + "not found.";
                        log.addLog(Message, "EditPerfume", "DashboardPerfume", logStatus.EventLog);
                        ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                        return RedirectToAction("Index", "DashboardPerfume");
                    }

                    p.Perfume_Brand_ID = perfume.Perfume_Brand_ID;
                    p.Perfume_Gender = perfume.Perfume_Gender;
                    p.Perfume_Country = perfume.Perfume_Country;
                    p.Perfume_Name = perfume.Perfume_Name;
                    p.Perfume_Notes = perfume.Perfume_Notes;
                    p.Perfume_OlfactionGroups = perfume.Perfume_OlfactionGroups;
                    p.Perfume_Perfumer = perfume.Perfume_Perfumer;
                    p.Perfume_Description = perfume.Perfume_Description;
                    p.Perfume_SpecialOffer = perfume.Perfume_SpecialOffer;
                    p.Perfume_TemperOfPerfume = perfume.Perfume_TemperOfPerfume;
                    p.Perfume_Type_ID = perfume.Perfume_Type_ID;

                    if (Request.Files.Count > 0)
                    {
                        var uploadFiles = Request.Files[0];
                        Random rnd = new Random();
                        if (uploadFiles != null && uploadFiles.ContentLength > 0)
                        {
                            if (!validImageTypes.Contains(uploadFiles.ContentType))
                            {
                                Message = "invalid image format.";
                                log.addLog(Message, "EditPerfume", "DashboardPerfume", logStatus.EventLog);
                                ViewBag.Error = "عکس آپلود شده باید از نوع jpg ویا png باشد.";
                                return View(p);
                            }

                            //add new image
                            var fileName = rnd.Next().ToString() + ".jpg";
                            p.Perfume_ImageIndex = fileName;
                            var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/Uploads/PerfumeImages/"), fileName);
                            //var path = Path.Combine(Server.MapPath("/Uploads/PerfumeImages/"), fileName);
                            uploadFiles.SaveAs(path);
                            Message = "Image save successfully for Perfumes with title " + p.Perfume_Name + ".";
                            log.addLog(Message, "EditPerfume", "DashboardPerfume", logStatus.EventLog);
                        }
                        else
                        {
                            p.Perfume_ImageIndex = perfume.Perfume_ImageIndex;
                        }
                    }
                    db.Tbl_Perfume.Attach(p);
                    db.Entry(p).State = System.Data.Entity.EntityState.Modified;
                    if (Convert.ToBoolean(db.SaveChanges() > 0))
                    {
                        /////////////addnewimages/////////////////////
                        bool isAdd = AddNewImages(files, p.Perfume_ID);
                        if (!isAdd)
                        {
                            return View(p.Perfume_ID);
                        }

                        if (Request.Files[0].FileName != "")
                        {
                            //delete old image
                            var oldFile = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/Uploads/PerfumeImages/"), oldImageName);
                            if (System.IO.File.Exists(oldFile))
                            {
                                System.IO.File.Delete(oldFile);
                                Message = "Delete image for perfumes with ID " + p.Perfume_ID + " Done successfully.";
                                log.addLog(Message, "EditPerfume", "DashboardPerfume", logStatus.EventLog);
                            }
                        }
                        //edit seasons//
                        bool IsEditSeasons = EditSeasons(spring, summer, autumn, winter, p.Perfume_ID);
                        if (IsEditSeasons)
                        {
                            return RedirectToAction("Index", "DashboardPerfume");
                        }
                        else
                        {
                            return View(p.Perfume_ID);
                        }
                    }
                    else
                    {
                        Message = "edited Perfume with title " + perfume.Perfume_Name + " failed.";
                        log.addLog(Message, "EditPerfume", "DashboardPerfume", logStatus.EventLog);
                        ViewBag.Error = "محصول ویرایش نشد، لطفا دوباره تلاش کنید.";
                        return View(p);
                    }
                }
                catch
                {
                    Message = "edited Perfume with title " + perfume.Perfume_Name + " failed.";
                    log.addLog(Message, "محصول ویرایش نشد", "DashboardPerfume", logStatus.ErrorLog);
                    ViewBag.Error = "محصول ویرایش نشد، لطفا دوباره تلاش کنید.";
                    return View(p);
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "AddUser", "DashboardUser", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }
        }

        public ActionResult PerfumeDetails(int perfumeID)
        {
            string Message = "";
            var perfume = db.Tbl_Perfume.Where(a => a.Perfume_ID == perfumeID).SingleOrDefault();
            if (perfume != null)
            {
                return View(perfume);
            }
            else
            {
                Message = "perfume with ID" + perfumeID + "not found.";
                log.addLog(Message, "PerfumeDetails", "DashboardPerfume", logStatus.EventLog);
                ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                return RedirectToAction("Index", "DashboardPerfume");
            }
        }

        public ActionResult DeletePerfume(int perfumeID)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddPerfume", "DashboardPerfume", logStatus.EventLog);
                return RedirectToAction("Index", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    var perfume = db.Tbl_Perfume.Where(a => a.Perfume_ID == perfumeID).SingleOrDefault();
                    if (perfume != null)
                    {
                        var images = db.Tbl_PerfumeImages.Where(a => a.PerfumeImages_Perfume_ID == perfume.Perfume_ID).ToList();
                        List<Tbl_PerfumeImages> img = new List<Tbl_PerfumeImages>();
                        if (images.Count() > 0)
                        {
                            foreach (var image in images)
                            {
                                if (image.PerfumeImages_ImageIndex != null)
                                {
                                    var oldImage = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/Uploads/PerfumeImages/"), image.PerfumeImages_ImageIndex);
                                    if (System.IO.File.Exists(oldImage))
                                    {
                                        System.IO.File.Delete(oldImage);
                                        Message = "Delete image for perfume with Id " + perfumeID + " Done successfully.";
                                        log.addLog(Message, "DeletePerfume", "DashboardPerfume", logStatus.EventLog);
                                    }
                                }
                                img.Add(image);
                            }
                            db.Tbl_PerfumeImages.RemoveRange(img);
                            if (Convert.ToBoolean(db.SaveChanges() > 0))
                            {
                                Message = "Delete image for perfume with Id " + perfumeID + " Done successfully.";
                                log.addLog(Message, "DeletePerfume", "DashboardPerfume", logStatus.EventLog);
                                ViewBag.Success = "محصول شما با موفقیت حذف شد.";
                            }
                            else
                            {
                                Message = "delete Perfume with ID " + perfumeID + " failed.";
                                log.addLog(Message, "DeletePerfume", "DashboardPerfume", logStatus.ErrorLog);
                                ViewBag.Error = "محصول حذف نشد، لطفا دوباره تلاش کنید.";
                                return RedirectToAction("Index", "DashboardPerfume");
                            }

                        }
                        if (perfume.Perfume_ImageIndex != null)
                        {
                            var oldFile = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/Uploads/PerfumeImages/"), perfume.Perfume_ImageIndex);
                            if (System.IO.File.Exists(oldFile))
                            {
                                System.IO.File.Delete(oldFile);
                                Message = "Delete image for perfume with Id " + perfumeID + " Done successfully.";
                                log.addLog(Message, "DeletePerfume", "DashboardPerfume", logStatus.EventLog);
                            }
                        }
                        db.Tbl_Perfume.Remove(perfume);
                        if (Convert.ToBoolean(db.SaveChanges() > 0))
                        {
                            Message = "Perfume with title " + perfume.Perfume_Name + " deleted successfully.";
                            log.addLog(Message, "Index", "DashboardPerfume", logStatus.EventLog);
                            ViewBag.Success = "محصول شما با موفقیت حذف شد.";
                            return RedirectToAction("Index", "DashboardPerfume");
                        }
                        else
                        {
                            Message = "delete Perfume with ID " + perfumeID + " failed.";
                            log.addLog(Message, "DeletePerfume", "DashboardPerfume", logStatus.ErrorLog);
                            ViewBag.Error = "محصول حذف نشد، لطفا دوباره تلاش کنید.";
                            return RedirectToAction("Index", "DashboardPerfume");
                        }
                    }
                    else
                    {
                        Message = "perfume with ID" + perfumeID + "not found.";
                        log.addLog(Message, "DeletePerfume", "DashboardPerfume", logStatus.EventLog);
                        ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                        return RedirectToAction("Index", "DashboardPerfume");
                    }
                }
                catch
                {
                    Message = "delete Perfume with ID " + perfumeID + " failed.";
                    log.addLog(Message, "DeletePerfume", "DashboardPerfume", logStatus.ErrorLog);
                    ViewBag.Error = "محصول حذف نشد، لطفا دوباره تلاش کنید.";
                    return RedirectToAction("Index", "DashboardPerfume");
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "DeletePerfume", "DashboardPerfume", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }


        }

        public bool AddNewImages(IEnumerable<HttpPostedFileBase> files, int perfumeID)
        {
            List<Tbl_PerfumeImages> p_images = new List<Tbl_PerfumeImages>();

            if (files.Count() > 0)
            {
                foreach (var file in files)
                {
                    var uploadFiles = file;
                    Random rnd = new Random();
                    if (uploadFiles != null && uploadFiles.ContentLength > 0)
                    {
                        if (!validImageTypes.Contains(uploadFiles.ContentType))
                        {
                            Message = "invalid image format.";
                            log.addLog(Message, "EditPerfume", "DashboardPerfume", logStatus.EventLog);
                            ViewBag.Error = "عکس آپلود شده باید از نوع jpg ویا png باشد.";
                            return false;
                        }

                        //add new image
                        Tbl_PerfumeImages images = new Tbl_PerfumeImages();
                        var fileName = rnd.Next().ToString() + ".jpg";
                        images.PerfumeImages_ImageIndex = fileName;
                        images.PerfumeImages_Perfume_ID = perfumeID;
                        var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/Uploads/PerfumeImages/"), fileName);
                        //var path = Path.Combine(Server.MapPath("/Uploads/PerfumeImages/"), fileName);
                        uploadFiles.SaveAs(path);
                        p_images.Add(images);
                        Message = "Image save successfully for Perfumes with id " + perfumeID + ".";
                        log.addLog(Message, "EditPerfume", "DashboardPerfume", logStatus.EventLog);
                    }
                }
            }

            db.Tbl_PerfumeImages.AddRange(p_images);
            db.SaveChanges();
            return true;
        }

        public bool EditSeasons(int? spring, int? summer, int? autumn, int? winter, int perfumeID)
        {
            List<Tbl_PerfumeSeason> seasons = new List<Tbl_PerfumeSeason>();
            int springID = db.Tbl_Season.Where(a => a.Season_Title == "مناسب فصل بهار").SingleOrDefault().Season_ID;
            int summerID = db.Tbl_Season.Where(a => a.Season_Title == "مناسب فصل تابستان").SingleOrDefault().Season_ID;
            int autumnID = db.Tbl_Season.Where(a => a.Season_Title == "مناسب فصل پاییز").SingleOrDefault().Season_ID;
            int winterID = db.Tbl_Season.Where(a => a.Season_Title == "مناسب فصل زمستان").SingleOrDefault().Season_ID;

            var ps_spring = db.Tbl_PerfumeSeason.Where(a => a.PerfumeSeason_Perfume_ID == perfumeID && a.PerfumeSeason_Season_ID == springID).SingleOrDefault();
            var ps_summer = db.Tbl_PerfumeSeason.Where(a => a.PerfumeSeason_Perfume_ID == perfumeID && a.PerfumeSeason_Season_ID == summerID).SingleOrDefault();
            var ps_autumn = db.Tbl_PerfumeSeason.Where(a => a.PerfumeSeason_Perfume_ID == perfumeID && a.PerfumeSeason_Season_ID == autumnID).SingleOrDefault();
            var ps_winter = db.Tbl_PerfumeSeason.Where(a => a.PerfumeSeason_Perfume_ID == perfumeID && a.PerfumeSeason_Season_ID == winterID).SingleOrDefault();

            if (spring == null)
            {
                if (ps_spring != null)
                {
                    db.Tbl_PerfumeSeason.Remove(ps_spring);
                    db.SaveChanges();
                }
            }
            if (spring == 1)
            {
                if (ps_spring == null)
                {
                    Tbl_PerfumeSeason ps = new Tbl_PerfumeSeason();
                    ps.PerfumeSeason_Season_ID = springID;
                    ps.PerfumeSeason_Perfume_ID = perfumeID;
                    seasons.Add(ps);
                }
            }
            if (summer == null)
            {
                if (ps_summer != null)
                {
                    db.Tbl_PerfumeSeason.Remove(ps_summer);
                    db.SaveChanges();
                }
            }
            if (summer == 1)
            {
                if (ps_summer == null)
                {
                    Tbl_PerfumeSeason ps = new Tbl_PerfumeSeason();
                    ps.PerfumeSeason_Season_ID = summerID;
                    ps.PerfumeSeason_Perfume_ID = perfumeID;
                    seasons.Add(ps);
                }
            }
            if (autumn == null)
            {
                if (ps_autumn != null)
                {
                    db.Tbl_PerfumeSeason.Remove(ps_autumn);
                    db.SaveChanges();
                }
            }
            if (autumn == 1)
            {
                if (ps_autumn == null)
                {
                    Tbl_PerfumeSeason ps = new Tbl_PerfumeSeason();
                    ps.PerfumeSeason_Season_ID = autumnID;
                    ps.PerfumeSeason_Perfume_ID = perfumeID;
                    seasons.Add(ps);
                }
            }
            if (winter == null)
            {
                if (ps_winter != null)
                {
                    db.Tbl_PerfumeSeason.Remove(ps_winter);
                    db.SaveChanges();
                }
            }
            if (winter == 1)
            {
                if (ps_winter == null)
                {
                    Tbl_PerfumeSeason ps = new Tbl_PerfumeSeason();
                    ps.PerfumeSeason_Season_ID = winterID;
                    ps.PerfumeSeason_Perfume_ID = perfumeID;
                    seasons.Add(ps);
                }
            }

            if (seasons.Count() > 0)
            {
                db.Tbl_PerfumeSeason.AddRange(seasons);
                if (Convert.ToBoolean(db.SaveChanges() > 0))
                {
                    Message = "Perfume with id " + perfumeID + " edited successfully.";
                    log.addLog(Message, "EditPerfume", "DashboardPerfume", logStatus.EventLog);
                    return true;
                }
                else
                {
                    Message = "edited Perfume seasons with id " + perfumeID + " failed.";
                    log.addLog(Message, "EditPerfume", "DashboardPerfume", logStatus.ErrorLog);
                    return false;
                }
            }
            return true;
        }

        //public bool EditImages(IEnumerable<HttpPostedFileBase> files1, int perfumeID)
        //{
        //    if (files1.Count() > 0)
        //    {
        //        List<Tbl_PerfumeImages> p_images = new List<Tbl_PerfumeImages>();
        //        foreach (var file1 in files1)
        //        {
        //            var uploadFiles = file1;
        //            Random rnd = new Random();
        //            if (uploadFiles != null && uploadFiles.ContentLength > 0)
        //            {
        //                if (!validImageTypes.Contains(uploadFiles.ContentType))
        //                {
        //                    Message = "invalid image format.";
        //                    log.addLog(Message, "EditPerfume", "DashboardPerfume", logStatus.EventLog);
        //                    ViewBag.Error = "عکس آپلود شده باید از نوع jpg ویا png باشد.";
        //                    return false;
        //                }

        //                //add new image
        //                Tbl_PerfumeImages images = new Tbl_PerfumeImages();
        //                var fileName = rnd.Next().ToString() + ".jpg";
        //                images.PerfumeImages_ImageIndex = fileName;
        //                images.PerfumeImages_Perfume_ID = perfumeID;
        //                var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/Uploads/PerfumeImages/"), fileName);
        //                //var path = Path.Combine(Server.MapPath("/Uploads/PerfumeImages/"), fileName);
        //                uploadFiles.SaveAs(path);
        //                p_images.Add(images);
        //                Message = "Image save successfully for Perfumes with id " + perfumeID + ".";
        //                log.addLog(Message, "EditPerfume", "DashboardPerfume", logStatus.EventLog);
        //            }
        //        }
        //        db.Tbl_PerfumeImages.AddRange(p_images);
        //        db.SaveChanges();
        //    }
        //    return true;
        //}

        public void InitDropdownLists()
        {
            //brand list
            var brandList = db.Tbl_Brand.OrderBy(r => r.Brand_Title).ToList().Select(rr =>
                new SelectListItem { Value = rr.Brand_ID.ToString(), Text = rr.Brand_Title }).ToList();
            ViewBag.brand = brandList;

            //category list
            var categoryList = db.Tbl_Category.OrderBy(r => r.Category_Title).ToList().Select(rr =>
               new SelectListItem { Value = rr.Category_ID.ToString(), Text = rr.Category_Title }).ToList();
            ViewBag.category = categoryList;

            //type list
            var typeList = db.Tbl_PerfumeType.OrderBy(r => r.PerfumeType_Title).ToList().Select(rr =>
               new SelectListItem { Value = rr.PerfumeType_ID.ToString(), Text = rr.PerfumeType_Title }).ToList();
            ViewBag.types = typeList;

            //gender list
            var genderList = new List<SelectListItem>
                    {
                        new SelectListItem{ Text="مردانه", Value = "مردانه" , Selected = true},
                        new SelectListItem{ Text="زنانه", Value = "زنانه" },
                        new SelectListItem{ Text="یونیسکس", Value = "یونیسکس" }
                    };
            ViewBag.gender = genderList;
        }

        public ActionResult DeleteImage(int id)
        {
            Tbl_PerfumeImages image = db.Tbl_PerfumeImages.Where(a => a.PerfumeImages_ID == id).SingleOrDefault();
            int perfumeID =(int) image.PerfumeImages_Perfume_ID;
            db.Tbl_PerfumeImages.Remove(image);
            db.SaveChanges();
            return RedirectToAction("EditPerfume", "DashboardPerfume", new { perfumeID = perfumeID });
        }

    }
}

