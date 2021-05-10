using perfumedecant.Models;
using perfumedecant.Models.Domains;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace perfumedecant.Controllers
{
    public class AccountController : Controller
    {
        PD_DB db = new PD_DB();
        Log log = new Log();
        LogStatus logStatus = new LogStatus();
      
        
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserName"] != null)
                    return RedirectToAction("Index", "Home");

                var existUser = db.Tbl_User.Where(a => a.User_Username == userViewModel.User_Username && a.User_Email == userViewModel.User_Email).SingleOrDefault();
                if (existUser == null)
                {
                    Tbl_User user = new Tbl_User();
                    user.User_Username = userViewModel.User_Username;
                    user.User_Password = userViewModel.User_Password;
                    user.User_Email = userViewModel.User_Email;

                    user.User_Date = DateTime.Now;
                    user.User_Active = true;
                    user.User_Role_ID = 2;

                    user.User_Password = Crypto.Hash(user.User_Password);
                    db.Tbl_User.Add(user);
                    if (Convert.ToBoolean(db.SaveChanges() > 0))
                    {
                        Session["UserName"] = user.User_Username;
                        Session["RoleName"] = "User";
                        String Message = "User with username " + user.User_Username + "registered successfully.";
                        log.addLog(Message, "Register", "Account", logStatus.EventLog);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        String Message = "register User with username " + user.User_Username + "failed.";
                        log.addLog(Message, "Register", "Account", logStatus.ErrorLog);
                        return View("Index", userViewModel);
                    }

                }
                else
                {
                    String Message = "register User with username " + userViewModel.User_Username + "failed.";
                    log.addLog(Message, "Register", "Account", logStatus.EventLog);
                    ViewBag.result = "قبلا با این نام کاربری و ایمیل ثبت نام شده است.";
                    return View("Index", userViewModel);
                }
            }
            else
                return View("Index");

        }


        [HttpPost]
        public ActionResult Login(UserViewModel user)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserName"] != null)
                {
                    string Message = "Index page loaded, Because user already logined.";
                    log.addLog(Message, "Login", "User", logStatus.EventLog);
                    return RedirectToAction("Index", "Home");
                }
                var hashedPassword = Crypto.Hash(user.User_Password);
                var user1 = db.Tbl_User.Where(a => a.User_Username == user.User_Username && a.User_Password == hashedPassword).SingleOrDefault();
                if (user1 != null)
                {
                    Session["UserName"] = user1.User_Username;
                    Session["RoleName"] = user1.Tbl_Role.Role_Name;
                    string Message = "User with username " + user1.User_Username + " login successfully.";
                    log.addLog(Message, "Login", "User", logStatus.EventLog);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //نام کاربری اشتباه است
                    string Message = "User with username " + user.User_Username + " login failed. Username or password worng.";
                    log.addLog(Message, "Login", "User", logStatus.EventLog);
                    ViewBag.result = "نام کاربری و یا کلمه عبور اشتباه است! ";
                    return View("Index",user);
                }
            }
            else
                return View("Index");
        }

        public ActionResult SignOut()
        {
            string username = Session["UserName"].ToString();
            Session["UserName"] = null;
            Session["RoleName"] = null;
            String Message = "User with username " + username + "Logout successfully.";
            log.addLog(Message, "LogOut", "Account", logStatus.EventLog);


            Response.AddHeader("Cache-control", "no-store, must-revalidate, private,no-cache");
            Response.AddHeader("Pragma", "no-cache");
            Response.AddHeader("Expires", "0");


            return RedirectToAction("Index", "Home");
        }


        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string email)
        {
            //String Message = "";
            string CS = ConfigurationManager.ConnectionStrings["PD_DB1"].ConnectionString;

            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand("spResetPassword", con);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter paramEmail = new SqlParameter("@Email", email);

                cmd.Parameters.Add(paramEmail);

                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (Convert.ToBoolean(rdr["ReturnCode"]))
                    {
                        SendPasswordResetEmail(email, rdr["UserName"].ToString(), rdr["UniqueId"].ToString());
                        //"An email with instructions to reset your password is sent to your registered email";
                    }
                    else
                    {
                        ViewBag.Error = "ارسال لینک بازیابی با خطا مواجه شده است. لطفا از صحت ایمیل وارد شده اطمینان حاصل کنید.";
                        return View("ForgotPassword");
                        //"Username not found!";
                    }
                }
            }
            //Message = "Redirect the user with username " + rdr["UserName"].ToString() + " to forgot view.";
            //log.addLog(Message, "Forgot", "User", logStatus.EventLog);
            ViewBag.Error = TempData["Error"];
            ViewBag.Success = TempData["Success"];
            return View("ForgotPassword");
        }

        private void SendPasswordResetEmail(string ToEmail, string UserName, string UniqueId)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();

                StringBuilder sbEmailBody = new StringBuilder();
                sbEmailBody.Append("<body style='direction:rtl'>");
                sbEmailBody.Append("کاربر گرامی  " + UserName + ",<br/><br/>");
                sbEmailBody.Append("لطفاً جهت تغییر رمز عبور روی لینک زیر کلیک کنید:");
                sbEmailBody.Append("<br/>");
                sbEmailBody.Append("<a href=\"http://http://www.perfumedecant.ir/Account/ChangePassword?uid=");
                sbEmailBody.Append(UniqueId);
                sbEmailBody.Append("\">");
                sbEmailBody.Append("http://www.perfumedecant.ir/Account/ChangePassword?uid=" + UniqueId);
                sbEmailBody.Append("</a>");
                sbEmailBody.Append("<br/><br/>");
                sbEmailBody.Append("<b>پرفیوم دکانت</b></body>");

                mailMessage.IsBodyHtml = true;

                mailMessage.To.Add(ToEmail);

                mailMessage.From = new MailAddress("info@perfumedecant.ir");

                mailMessage.Subject = "بازگردانی رمز عبور";

                mailMessage.Body = sbEmailBody.ToString();

                SmtpClient smtpClient = new SmtpClient("webmail.perfumedecant.ir");

                smtpClient.Credentials = new NetworkCredential("info@perfumedecant.ir", "ekjhupyqi2tYwlozgm/r");
                smtpClient.Port = 25;
                //smtpClient.EnableSsl = false;

                smtpClient.Send(mailMessage);

                TempData["Success"] = "لینک بازیابی به ایمیلتان ارسال شد.";
            }
            catch
            {
                TempData["Error"] = "ارسال لینک بازیابی با خطا مواجه شده است. لطفا از صحت ایمیل وارد شده اطمینان حاصل کنید.";
            }
        }

        private bool IsPasswordResetLinkValid(String guid)
        {
            String Message = "";
            List<SqlParameter> paramList = new List<SqlParameter>()
            {
                new SqlParameter()
                {
                    ParameterName = "@GUID",
                    Value = guid
                }
            };

            Message = "IsPasswordResetLinkValid function Done successfully.";
            log.addLog(Message, "IsPasswordResetLinkValid", "Account", logStatus.EventLog);
            return ExecuteSP("spIsPasswordResetLinkValid", paramList);
        }

        private bool ExecuteSP(string SPName, List<SqlParameter> SPParameters)
        {
            String Message = "";
            string CS = ConfigurationManager.ConnectionStrings["PD_DB1"].ConnectionString;
            using (SqlConnection con = new SqlConnection(CS))
            {
                SqlCommand cmd = new SqlCommand(SPName, con);
                cmd.CommandType = CommandType.StoredProcedure;

                foreach (SqlParameter parameter in SPParameters)
                {
                    cmd.Parameters.Add(parameter);
                }

                con.Open();
                Message = "ExecuteSP function Done successfully.";
                log.addLog(Message, "ExecuteSP", "Account", logStatus.EventLog);
                return Convert.ToBoolean(cmd.ExecuteScalar());
            }
        }

        public ActionResult ChangePassword()
        {
            String Message = "";
            string guid = this.Request.QueryString["uid"];

            if (!IsPasswordResetLinkValid(guid))
            {
                //"Password Reset link has expired or is invalid";
                Message = "ChangePassword page loaded failed. PasswordResetLink is invalid.";
                log.addLog(Message, "ChangePassword", "Account", logStatus.EventLog);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Message = "ChangePassword page loaded successfully.";
                log.addLog(Message, "ChangePassword", "Account", logStatus.EventLog);
                return View("ChangePassword");
            }
        }

        [HttpPost]
        public ActionResult ChangePassword(string password, string guid)
        {
            String Message = "";
            if (ChangeUserPassword(password, guid))
            {
                Message = "Password Change successfully.";
                log.addLog(Message, "ChangePassword", "Account", logStatus.EventLog);
                ViewBag.result = "پسورد با موفقیت تغییر یافت.";
                // "Password Changed Successfully!";
            }
            else
            {
                ViewBag.result = "تغییر پسورد با خطا مواجه شده است. لطفاً دوباره تلاش کنید.";
                //"Password Reset link has expired or is invalid";
            }
            return View("ChangePassword");
        }


        private bool ChangeUserPassword(String password, String guid)
        {
            String Message = "";
            List<SqlParameter> paramList = new List<SqlParameter>()
            {
                new SqlParameter()
                {
                    ParameterName = "@GUID",
                    Value = guid
                },
                new SqlParameter()
                {
                    ParameterName = "@Password",
                    Value = Crypto.Hash(password)
                }
            };

            Message = "ChangeUserPassword function Done successfully.";
            log.addLog(Message, "ChangeUserPassword", "Account", logStatus.EventLog);
            return ExecuteSP("spChangePassword", paramList);
        }


        public ActionResult EditProfile()
        {
            string Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "AddBrand", "DashboardBrand", logStatus.EventLog);
                return RedirectToAction("Index", "Account");
            }
            try
            {
                InitDropdownLists();

                var username = Session["UserName"].ToString();
                var user = db.Tbl_User.Where(a => a.User_Username == username).SingleOrDefault();
                if (user != null)
                {
                    //Tbl_User vm_user = new Tbl_User();
                    //vm_user.User_Active = user.User_Active;
                    //vm_user.User_Address = user.User_Address;
                    //vm_user.User_Birthday = user.User_Birthday;
                    //vm_user.User_City_ID = user.User_City_ID;
                    //vm_user.User_Date = user.User_Date;
                    //vm_user.User_Email = user.User_Email;
                    //vm_user.User_Gender = user.User_Gender;
                    //vm_user.User_ID = user.User_ID;
                    //vm_user.User_ImageIndex = user.User_ImageIndex;
                    //vm_user.User_Mobile = user.User_Mobile;
                    //vm_user.User_NameFamily = user.User_NameFamily;
                    //vm_user.User_NationalCode = user.User_NationalCode;
                    //vm_user.User_Password = user.User_Password;
                    //vm_user.User_PostalCode = user.User_PostalCode;
                    //vm_user.User_Role_ID = user.User_Role_ID;
                    //vm_user.User_Tel = user.User_Tel;
                    //vm_user.User_Username = user.User_Username;

                    return View(user);
                }
                else
                {
                    Message = "User with username" + user.User_Username + "not found.";
                    log.addLog(Message, "EditProfile", "Account", logStatus.EventLog);
                    ViewBag.result = "کاربر پیدا نشد، لطفا دوباره تلاش کنید.";
                    return RedirectToAction("Index", "Cart");
                }
            }
            catch
            {
                Message = "User with username" + "" + "not found.";
                log.addLog(Message, "EditProfile", "Account", logStatus.EventLog);
                ViewBag.result = "کاربر پیدا نشد، لطفا دوباره تلاش کنید.";
                return RedirectToAction("Index", "Cart");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditProfile(Tbl_User user)
        {
            InitDropdownLists();

            if (!ModelState.IsValid)
            {
                return View(user);
            }
            String Message = "";
            if (Session["UserName"] == null)
            {
                Message = "Access denied. need login.";
                log.addLog(Message, "EditProfile", "Account", logStatus.EventLog);
                return RedirectToAction("Index", "Account");
            }

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

                oldUser = db.Tbl_User.Where(a => a.User_ID == user.User_ID).SingleOrDefault();

                if (oldUser == null)
                {
                    Message = "User with ID" + user.User_ID + "not found.";
                    log.addLog(Message, "EditProfile", "Account", logStatus.EventLog);
                    ViewBag.result = "کاربر یافت نشد. لطفاً دوباره تلاش کنید.";
                    return RedirectToAction("Index", "Cart");
                }
                string oldImageName = oldUser.User_ImageIndex;

                oldUser.User_Address = user.User_Address;
                oldUser.User_Birthday = user.User_Birthday;
                oldUser.User_Email = user.User_Email;
                oldUser.User_Gender = user.User_Gender;
                oldUser.User_Mobile = user.User_Mobile;
                oldUser.User_NameFamily = user.User_NameFamily;
                oldUser.User_Tel = user.User_Tel;
                oldUser.User_Username = user.User_Username;
                oldUser.User_Password = Crypto.Hash(user.User_Password);
                oldUser.User_City_ID = user.User_City_ID;
                oldUser.User_PostalCode = user.User_PostalCode;

                var uploadFiles = Request.Files[0];
                Random rnd = new Random();

                if (uploadFiles != null && uploadFiles.ContentLength > 0)
                {
                    if (!validImageTypes.Contains(uploadFiles.ContentType))
                    {
                        Message = "invalid image format.";
                        log.addLog(Message, "EditProfile", "Account", logStatus.EventLog);
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
                    log.addLog(Message, "EditProfile", "Account", logStatus.EventLog);

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
                    //var oldFile = Path.Combine(Server.MapPath("/Uploads/UserImages/"), oldImageName);
                    if (user.User_ImageIndex != oldImageName)
                    {
                        var oldFile = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("/Uploads/UserImages/"), oldImageName);

                        if (System.IO.File.Exists(oldFile))
                        {
                            System.IO.File.Delete(oldFile);
                            Message = "Delete image for Users with ID " + oldUser.User_ID + " Done successfully.";
                            log.addLog(Message, "EditProfile", "Account", logStatus.EventLog);
                            ViewBag.success = "ویرایش با موفقیت انجام شد.";
                        }
                    }
                    return RedirectToAction("Index", "Cart");
                }
                else
                {
                    Message = "edited User with userName " + user.User_Username + " failed.";
                    log.addLog(Message, "EditProfile", "Account", logStatus.ErrorLog);
                    ViewBag.result = "ویرایش با موفقیت انجام نشد، لطفا دوباره تلاش کنید.";
                    return View(user);
                }

            }
            catch
            {
                Message = "edited User with userName " + user.User_Username + " failed.";
                log.addLog(Message, "EditProfile", "Account", logStatus.ErrorLog);
                ViewBag.result = "ویرایش با موفقیت انجام نشد، لطفا دوباره تلاش کنید.";
                return View(user);
            }
            return View(user);
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

    }
}