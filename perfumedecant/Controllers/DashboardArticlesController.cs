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
    public class DashboardArticlesController : Controller
    {
        PD_DB db = new PD_DB();
        Log log = new Log();
        LogStatus logStatus = new LogStatus();
        // GET: DashboardArticles
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddArticle()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AddArticle(Tbl_Article article)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddBrand", "DashboardBrand", logStatus.EventLog);
                return RedirectToAction("Login", "Account");
            }
            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    Tbl_Article a = new Tbl_Article();
                    a.Article_Title = article.Article_Title;
                    if (article.Article_Path != null)
                    {
                        var uploadFiles = Request.Files[0];

                        if (uploadFiles.ContentLength > 0)
                        {
                            var fileName = uploadFiles.FileName;
                            a.Article_Path = fileName;
                            var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/Uploads/ArticleFiles/"), fileName);
                            uploadFiles.SaveAs(path);
                            Message = "file save successfully for article with title " + article.Article_Title + ".";
                            log.addLog(Message, "AddArticle", "DashboardArticles", logStatus.EventLog);
                            db.Tbl_Article.Add(a);
                            if (Convert.ToBoolean(db.SaveChanges() > 0))
                            {
                                Message = "Article with title " + article.Article_Title + " added successfully.";
                                log.addLog(Message, "AddArticle", "DashboardArticles", logStatus.EventLog);
                                return RedirectToAction("Index", "DashboardArticles");
                            }
                            else
                            {
                                Message = "added article with title " + article.Article_Title + " failed.";
                                log.addLog(Message, "AddArticle", "DashboardArticles", logStatus.EventLog);
                                ViewBag.Error = "مقاله جدید ثبت نشد، لطفا دوباره تلاش کنید.";
                                return View();
                            }

                        }
                        else
                        {
                            Message = "file save for article with title " + article.Article_Title + " failed.";
                            log.addLog(Message, "AddArticle", "DashboardArticles", logStatus.EventLog);
                            ViewBag.EmptyImage = "آپلود فایل با خطا مواجه شده است.";
                            return View();
                        }
                    }
                    else
                    {
                        Message = "file save for article with title " + article.Article_Title + " failed.";
                        log.addLog(Message, "AddArticle", "DashboardArticles", logStatus.EventLog);
                        ViewBag.EmptyImage = "آپلود فایل با خطا مواجه شده است.";
                        return View();
                    }
                }
                catch
                {
                    Message = "file save for article with title " + article.Article_Title + " failed.";
                    log.addLog(Message, "AddArticle", "DashboardArticles", logStatus.EventLog);
                    ViewBag.EmptyImage = "آپلود فایل با خطا مواجه شده است.";
                    return View();
                }
            }
            else
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddArticle", "DashboardArticles", logStatus.EventLog);
                return RedirectToAction("Login", "Account");
            }
        }

        public FileResult ArticleDetails(string fileName = "")
        {
            var pathToTheFile = Server.MapPath("~/Uploads/ArticleFiles/" + fileName);
            var fileStream = new FileStream(pathToTheFile,
                                                FileMode.Open,
                                                FileAccess.Read
                                            );
            var fsr = new FileStreamResult(fileStream, "application/pdf");
            return fsr;

        }

        public ActionResult DeleteArticle(int ArticleID = 0)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "DeleteArticle", "DashboardArticles", logStatus.EventLog);
                return RedirectToAction("Login", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    var article = db.Tbl_Article.Where(a => a.Article_ID == ArticleID).SingleOrDefault();
                    if (article != null)
                    {
                        if (article.Article_Path != null)
                        {
                            var oldFile = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/Uploads/ArticleFiles/"), article.Article_Path);
                            if (System.IO.File.Exists(oldFile))
                            {
                                System.IO.File.Delete(oldFile);
                                Message = "Delete file for article with Id " + ArticleID + " Done successfully.";
                                log.addLog(Message, "DeleteArticle", "DashboardArticles", logStatus.EventLog);
                            }
                        }
                        db.Tbl_Article.Remove(article);
                        if (Convert.ToBoolean(db.SaveChanges() > 0))
                        {
                            Message = "Article with title " + article.Article_Title + " deleted successfully.";
                            log.addLog(Message, "DeleteArticle", "DashboardArticles", logStatus.EventLog);
                            ViewBag.Success = "برند شما با موفقیت حذف شد.";
                            return RedirectToAction("Index", "DashboardArticles");
                        }
                        else
                        {
                            Message = "delete Article with ID " + ArticleID + " failed.";
                            log.addLog(Message, "DeleteArticle", "DashboardArticles", logStatus.ErrorLog);
                            ViewBag.Error = "مقاله حذف نشد، لطفا دوباره تلاش کنید.";
                            return RedirectToAction("Index", "DashboardArticles");
                        }
                    }
                    else
                    {
                        Message = "Article with ID" + ArticleID + "not found.";
                        log.addLog(Message, "DeleteArticle", "DashboardArticles", logStatus.ErrorLog);
                        ViewBag.Error = "مقاله پیدا نشد، لطفا دوباره تلاش کنید.";
                        return RedirectToAction("Index", "DashboardArticles");
                    }
                }
                catch
                {
                    Message = "delete Article with ID " + ArticleID + " failed.";
                    log.addLog(Message, "DeleteArticle", "DashboardArticles", logStatus.ErrorLog);
                    ViewBag.Error = "مقاله حذف نشد، لطفا دوباره تلاش کنید.";
                    return RedirectToAction("Index", "DashboardArticles");
                }
            }
            else
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "DeleteArticle", "DashboardArticles", logStatus.EventLog);
                return RedirectToAction("Login", "Account");
            }
        }
    }
}