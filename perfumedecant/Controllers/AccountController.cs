using perfumedecant.Models;
using perfumedecant.Models.Domains;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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

    }
}