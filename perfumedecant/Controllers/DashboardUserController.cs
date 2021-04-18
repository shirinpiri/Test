using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using perfumedecant.Models.Domains;
using perfumedecant.Models;
using perfumedecant.Models.Repository;
using System.Web.Helpers;

namespace UserDecant.Controllers
{
    public class DashboardUserController : Controller
    {
        PD_DB db = new PD_DB();
        Log log = new Log();
        LogStatus logStatus = new LogStatus();

        //
        // GET: /DashboardUser/

        public ActionResult Index()
        {
            return View(); 
        }

        public ActionResult AddUser()
        {
            String Message = "";

            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddUser", "DashboardUser", logStatus.EventLog);
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
        public ActionResult AddUser(Tbl_User user, int State)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddUser", "DashboardUser", logStatus.EventLog);
                return RedirectToAction("Login", "Account");
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
                  user.User_Date = DateTime.Now;

                  if (user.User_ImageIndex != null)
                  {
                      var uploadFiles = Request.Files[0];
                      Random rnd = new Random();

                      if (!validImageTypes.Contains(uploadFiles.ContentType))
                      {
                          Message = "invalid image format.";
                          log.addLog(Message, "AddUser", "DashboardUser", logStatus.EventLog);
                          ViewBag.result = "عکس آپلود شده باید از نوع jpg ویا png باشد.";
                          return View();
                      }
                      else if (uploadFiles != null && uploadFiles.ContentLength > 0)
                      {
                          var fileName = rnd.Next().ToString() + ".jpg";
                          user.User_ImageIndex = fileName;
                          var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/Uploads/UserImages/"), fileName);
                          //var path = Path.Combine(Server.MapPath("/Uploads/UserImages/"), fileName);
                          uploadFiles.SaveAs(path);
                          Message = "Image save successfully for Users with userName " + user.User_Username + ".";
                          log.addLog(Message, "AddUser", "DashboardUser", logStatus.EventLog);
                      }
                      else
                      {
                          Message = "Image save for Perfuem with userName " + user.User_Username + " failed.";
                          log.addLog(Message, "AddUser", "DashboardUser", logStatus.EventLog);
                          ViewBag.EmptyImage = "آپلود عکس با خطا مواجه شده است.";
                          return View();
                      }
                  }

                  user.User_Password = Crypto.Hash(user.User_Password);

                  db.Tbl_User.Add(user);

                  if (Convert.ToBoolean(db.SaveChanges() > 0))
                  {
                      Message = "User with userName " + user.User_Username + " added successfully.";
                      log.addLog(Message, "AddUser", "DashboardUser", logStatus.EventLog);
                      ViewBag.success = "کاربر با موفقیت اضافه شد.";
                      return RedirectToAction("Index", "DashboardUser");

                  }
                  else
                  {
                      Message = "added User with userName " + user.User_Username + " failed.";
                      log.addLog(Message, "AddUser", "DashboardUser", logStatus.EventLog);
                      ViewBag.result = "کاربر جدید اضافه نشد. لطفاً دوباره تلاش کنید.";
                      return View();
                  }

                }catch
                {
                    Message = "added User with userName " + user.User_Username + " failed.";
                    log.addLog(Message, "AddUser", "DashboardUser", logStatus.ErrorLog);
                    ViewBag.result = "کاربر جدید اضافه نشد. لطفاً دوباره تلاش کنید.";
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

        public ActionResult EditUser(int id = 0)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "EditUser", "DashboardUser", logStatus.EventLog);
                return RedirectToAction("Login", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    InitDropdownLists();
                    var qUserDetails = db.Tbl_User.Where(a => a.User_ID == id).SingleOrDefault();
                    if (qUserDetails != null)
                    {
                        return View(qUserDetails);
                    }
                    else
                    {
                        Message = "User with ID" + id + "not found.";
                        log.addLog(Message, "EditUser", "DashboardUser", logStatus.EventLog);
                        ViewBag.result = "کاربر پیدا نشد، لطفا دوباره تلاش کنید.";
                        return RedirectToAction("Index", "DashboardUser");
                    }
                }
                catch
                {
                    Message = "edit User with ID " + id + " failed.";
                    log.addLog(Message, "EditUser", "DashboardUser", logStatus.ErrorLog);
                    ViewBag.result = "کاربر ویرایش نشد. لطفاً دوباره تلاش کنید.";
                    return View();
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "EditUser", "DashboardUser", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditUser(Tbl_User user , HttpPostedFileBase UserImage)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "EditUser", "DashboardUser", logStatus.EventLog);
                return RedirectToAction("Login", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                Tbl_User oldUser = new Tbl_User();
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
                  oldUser = db.Tbl_User.Where(a => a.User_ID == user.User_ID).SingleOrDefault();
                  string oldImageName = oldUser.User_ImageIndex;

                  if (oldUser == null)
                  {
                      Message = "User with ID" + user.User_ID + "not found.";
                      log.addLog(Message, "EditUser", "DashboardUser", logStatus.EventLog);
                      ViewBag.result = "کاربر یافت نشد. لطفاً دوباره تلاش کنید.";
                      return RedirectToAction("Index", "DashboardUser");
                  }

                  oldUser.User_Address = user.User_Address;
                  oldUser.User_Birthday = user.User_Birthday;
                  oldUser.User_Email = user.User_Email;
                  oldUser.User_Gender = user.User_Gender;
                  oldUser.User_Mobile = user.User_Mobile;
                  oldUser.User_NameFamily = user.User_NameFamily;
                  oldUser.User_NationalCode = user.User_NationalCode;
                  oldUser.User_PostalCode = user.User_PostalCode;
                  oldUser.User_Tel = user.User_Tel;

                  var uploadFiles = Request.Files[0];
                  Random rnd = new Random();

                  if (uploadFiles != null && uploadFiles.ContentLength > 0)
                  {
                      if (!validImageTypes.Contains(uploadFiles.ContentType))
                      {
                          Message = "invalid image format.";
                          log.addLog(Message, "EditUser", "DashboardUser", logStatus.EventLog);
                          ViewBag.result = "عکس آپلود شده باید از نوع jpg ویا png باشد.";
                          return View(oldUser);
                      }

                      //add new image
                      var fileName = rnd.Next().ToString() + ".jpg";
                      oldUser.User_ImageIndex = fileName;

                      var path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/Uploads/UserImages/"), fileName);
                      //var path = Path.Combine(Server.MapPath("/Uploads/UserImages/"), fileName);
                      uploadFiles.SaveAs(path);
                      Message = "Image save successfully for Users with userName " + oldUser.User_Username + ".";
                      log.addLog(Message, "EditUser", "DashboardUser", logStatus.EventLog);

                  }
                  else
                  {
                      oldUser.User_ImageIndex = user.User_ImageIndex;
                  }

                  db.Tbl_User.Attach(oldUser);
                  db.Entry(oldUser).State = System.Data.Entity.EntityState.Modified;
                  if (Convert.ToBoolean(db.SaveChanges() > 0))
                    {
                        //delete old image
                        var oldFile = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/Uploads/UserImages/"), oldImageName);
                        //var oldFile = Path.Combine(Server.MapPath("/Uploads/UserImages/"), oldImageName);
                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                            Message = "Delete image for Users with ID " + oldUser.User_ID + " Done successfully.";
                            log.addLog(Message, "EditUser", "DashboardUser", logStatus.EventLog);
                            ViewBag.success = "کاربر با موفقیت ویرایش شد.";
                            return RedirectToAction("Index" , "DashboardUser");
                        }
                    }
                  else
                  {
                      Message = "edited User with userName " + user.User_Username + " failed.";
                      log.addLog(Message, "EditUser", "DashboardUser", logStatus.ErrorLog);
                      ViewBag.result = "کاربر ویرایش نشد، لطفا دوباره تلاش کنید.";
                      return View(user);
                  }

                }
                catch
                {
                    Message = "edited User with userName " + user.User_Username + " failed.";
                    log.addLog(Message, "EditUser", "DashboardUser", logStatus.ErrorLog);
                    ViewBag.result = "کاربر ویرایش نشد، لطفا دوباره تلاش کنید.";
                    return View(user);
                }
                return View(user);
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "EditUser", "DashboardUser", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }

        }

        public ActionResult UserDetails(int id = 0)
        {
            string Message = "";
            var qUserDetails = db.Tbl_User.Where(a => a.User_ID == id).SingleOrDefault();
            if (qUserDetails != null)
            {
                return View(qUserDetails);
            }
            else
            {
                Message = "User with ID" + id + "not found.";
                log.addLog(Message, "UserDetails", "DashboardUser", logStatus.EventLog);
                ViewBag.result = "کاربر پیدا نشد، لطفا دوباره تلاش کنید.";
                return RedirectToAction("Index", "DashboardUser");
            }
           
        }

        public ActionResult DeleteUser(int id = 0)
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "DeleteUser", "DashboardUser", logStatus.EventLog);
                return RedirectToAction("Login", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    var user = db.Tbl_User.Where(a => a.User_ID == id).SingleOrDefault();
                    if (user != null)
                    {
                        if (user.User_ImageIndex != null)
                        {
                            var oldFile = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/Uploads/UserImages/"), user.User_ImageIndex);
                            //var oldFile = Path.Combine(Server.MapPath("/Uploads/UserImages/"), user.User_ImageIndex);
                            if (System.IO.File.Exists(oldFile))
                            {
                                System.IO.File.Delete(oldFile);
                                Message = "Delete image for User with Id " + id + " Done successfully.";
                                log.addLog(Message, "DeleteUser", "DashboardUser", logStatus.EventLog);
                            }
                        }
                        db.Tbl_User.Remove(user);
                        if (Convert.ToBoolean(db.SaveChanges() > 0))
                        {
                            Message = "User with title " + user.User_Username + " deleted successfully.";
                            log.addLog(Message, "Index", "DashboardUser", logStatus.EventLog);
                            ViewBag.Success = "کاربر با موفقیت حذف شد.";
                            return RedirectToAction("Index", "DashboardUser");
                        }
                        else
                        {
                            Message = "delete User with ID " + id + " failed.";
                            log.addLog(Message, "DeleteUser", "DashboardUser", logStatus.ErrorLog);
                            ViewBag.result = "کاربر حذف نشد، لطفا دوباره تلاش کنید.";
                            return RedirectToAction("Index", "DashboardUser");
                        }
                    }
                    else
                    {
                        Message = "User with ID" + id + "not found.";
                        log.addLog(Message, "DeleteUser", "DashboardUser", logStatus.EventLog);
                        ViewBag.result = "کاربر پیدا نشد، لطفا دوباره تلاش کنید.";
                        return RedirectToAction("Index", "DashboardUser");
                    }
                }
                catch
                {
                    Message = "delete User with ID " + id + " failed.";
                    log.addLog(Message, "DeleteUser", "DashboardUser", logStatus.ErrorLog);
                    ViewBag.result = "کاربر حذف نشد، لطفا دوباره تلاش کنید.";
                    return RedirectToAction("Index", "DashboardUser");
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "DeleteUser", "DashboardUser", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult ChangePassword(int id = 0,String confirm_password = "")
        {
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "ChangePassword", "DashboardUser", logStatus.EventLog);
                return RedirectToAction("Login", "Account");
            }

            else if (Session["RoleName"].ToString() == "Admin")
            {
                try
                {
                    var qUser = db.Tbl_User.Where(a => a.User_ID == id).SingleOrDefault();
                    qUser.User_Password = confirm_password;
                    db.Tbl_User.Attach(qUser);
                    db.Entry(qUser).State = System.Data.Entity.EntityState.Modified;

                    if (Convert.ToBoolean(db.SaveChanges() > 0))
                    {
                        
                        Message = "user password with user ID " + id + " changed successfully";
                        log.addLog(Message, "changePassword", "DashboardUser", logStatus.EventLog);
                        ViewBag.result = "پسورد با موفقیت تغییر یافت";
                        return RedirectToAction("Index", "DashboardUser");
                    }
                    else
                    {
                        Message = "change user password with user ID " + id + " failed";
                        log.addLog(Message, "changePassword", "DashboardUser", logStatus.ErrorLog);
                        ViewBag.result = "تغییر پسورد با خطا مواجه شده است.";
                        return RedirectToAction("Index", "DashboardUser");
                    }
                }
                catch
                {
                    Message = "change user password with user ID " + id + " failed";
                    log.addLog(Message, "changePassword", "DashboardUser", logStatus.ErrorLog);
                    ViewBag.result = "تغییر پسورد با خطا مواجه شده است.";
                    return RedirectToAction("Index", "DashboardUser");
                }
            }
            else
            {
                Message = "You do not have access to this page.";
                log.addLog(Message, "ChangePassword", "DashboardUser", logStatus.EventLog);
                return RedirectToAction("Error404", "Home");
            }
        }
        public void InitDropdownLists()
        {
            //state list
            var stateList = db.Tbl_State.OrderBy(r => r.State_Title).ToList().Select(rr =>
                new SelectListItem { Value = rr.State_ID.ToString(), Text = rr.State_Title }).ToList();
            ViewBag.states = stateList;
            
            //gender list
            var genderList = new List<SelectListItem>
                    {
                        new SelectListItem{ Text="مرد", Value = "مرد" , Selected = true},
                        new SelectListItem{ Text="زن", Value = "زن" }
                    };
            ViewBag.Gender = genderList;

            //activation list
            var activationList = new List<SelectListItem>
                    {
                        new SelectListItem{ Text="فعال", Value = "true" , Selected = true},
                        new SelectListItem{ Text="غیر فعال", Value = "false" }
                    };
            ViewBag.activation = activationList;

            //roles list
            var rolesList = db.Tbl_Role.OrderBy(r => r.Role_Title).ToList().Select(rr =>
                new SelectListItem { Value = rr.Role_ID.ToString(), Text = rr.Role_Title }).ToList();
            ViewBag.roles = rolesList;
        }

        public JsonResult GetCities(int id)
        {
            var cityList = db.Tbl_City.Where(r => r.City_State_ID == id).OrderBy(r => r.City_Title);
            List<SelectListItem> cities = new List<SelectListItem>();

            foreach (var item in cityList)
            {
                cities.Add(new SelectListItem { Text = item.City_Title, Value = item.City_ID.ToString()});
            }

            return Json(new SelectList(cities, "Value", "Text"));
        }
	}
}