using Jose;
using NSwag.Annotations;
using ProjectPi.Models;
using ProjectPi.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace ProjectPi.Controllers
{
    [OpenApiTag("Accounts", Description = "帳號註冊/登入功能")]
    public class AccountsController : ApiController
    {
        /// <summary>
        /// 諮商師註冊
        /// </summary>
        /// <param name="view"></param>
        /// <response code="200">註冊成功</response>
        /// <returns></returns>
        [HttpPost]
        [Route("api/counselor/register")]
        [SwaggerResponse(typeof(ApiResponse))]
        public IHttpActionResult Register_C(ViewModel_C.Register view)
        {
            PiDbContext _db = new PiDbContext();

            var hasEmail1 = _db.Counselors
                .Where(c => c.Account == view.Account
                .ToLower()).FirstOrDefault();

            var hasEmail2 = _db.Users
                .Where(c => c.Account == view.Account
                .ToLower()).FirstOrDefault();

            if (hasEmail1 != null || hasEmail2 != null)
                return BadRequest("此帳號已註冊過");
            else
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("資料格式有誤");
                }
                else
                {
                    if (view.Password != view.ConfirmPassword)
                        return BadRequest("密碼不一致");
                    else
                    {
                        Counselor counselor = new Counselor();
                        counselor.Name = view.Name;
                        counselor.Account = view.Account.ToLower();
                        counselor.Password = BitConverter
                            .ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(view.Password))).Replace("-", null);
                        counselor.CertNumber = view.Certification;
                        counselor.LicenseImg = view.License;
                        counselor.Validation = false;

                        _db.Counselors.Add(counselor);
                        _db.SaveChanges();

                        ApiResponse result = new ApiResponse { };
                        result.Success = true;
                        result.Message = "諮商師註冊成功";
                        result.Data = null;

                        return Ok(result);
                    }
                }
            }
        }

        /// <summary>
        /// 諮商師登入
        /// </summary>
        /// <param name="view"></param>
        /// <response code="200">登入驗證成功</response>
        /// <returns></returns>
        [HttpPost]
        [Route("api/counselor/login")]
        [SwaggerResponse(typeof(ApiResponse))]
        public IHttpActionResult Login_C(ViewModel_C.Login view)
        {
            PiDbContext _db = new PiDbContext();

            Counselor hasAccount = _db.Counselors
                .Where(c => c.Account == view.Account.ToLower())
                .FirstOrDefault();

            if (hasAccount == null)
                return BadRequest("無此帳號");
            else
            {
                if (!ModelState.IsValid)
                    return BadRequest("Email格式不符");
                else
                {
                    view.Password = BitConverter
                        .ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(view.Password)))
                        .Replace("-", null);

                    if (hasAccount.Password != view.Password)
                        return BadRequest("密碼有誤");
                    else
                    {
                        JwtAuthUtil jwtAuthUtil = new JwtAuthUtil();
                        string token = jwtAuthUtil.GenerateToken(hasAccount.Id, hasAccount.Account, hasAccount.Name, hasAccount.Guid);

                        ApiResponse result = new ApiResponse { };
                        result.Success = true;
                        result.Message = "登入成功";
                        result.Data = new
                        {
                            Username = hasAccount.Name,
                            Identity = "counselor",
                            Validation = hasAccount.Validation,
                            UserID = hasAccount.Id,
                            Authorization = token
                        };
                        return Ok(result);
                    }
                }
            }
        }

        /// <summary>
        /// 個案註冊
        /// </summary>
        /// <param name="view"></param>
        /// <response code="200">註冊成功</response>
        /// <returns></returns>
        [HttpPost]
        [Route("api/user/register")]
        [SwaggerResponse(typeof(ApiResponse))]
        public IHttpActionResult Register_U(ViewModel_U.Register view)
        {
            PiDbContext _db = new PiDbContext();

            var hasEmail1 = _db.Counselors
                .Where(c => c.Account == view.Account
                .ToLower()).FirstOrDefault();

            var hasEmail2 = _db.Users
                .Where(c => c.Account == view.Account
                .ToLower()).FirstOrDefault();

            if (hasEmail1 != null || hasEmail2 != null)
                return BadRequest("此帳號已註冊過");
            else
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("資料格式有誤");
                }
                else
                {
                    if (view.Password != view.ConfirmPassword)
                        return BadRequest("密碼不一致");
                    else
                    {
                        User user = new User();
                        user.Name = view.Name;
                        user.Sex = view.Sex;
                        user.BirthDate = view.BirthDate;
                        user.Account = view.Account.ToLower();
                        user.Password = BitConverter
                            .ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(view.Password))).Replace("-", null);

                        _db.Users.Add(user);
                        _db.SaveChanges();

                        ApiResponse result = new ApiResponse { };
                        result.Success = true;
                        result.Message = "個案註冊成功";
                        result.Data = null;

                        return Ok(result);
                    }
                }
            }
        }

        /// <summary>
        /// 個案登入
        /// </summary>
        /// <param name="view"></param>
        /// <response code="200">登入驗證成功</response>
        /// <returns></returns>
        [HttpPost]
        [Route("api/user/login")]
        [SwaggerResponse(typeof(ApiResponse))]
        public IHttpActionResult Login_U(ViewModel_U.Login view)
        {
            PiDbContext _db = new PiDbContext();

            User hasAccount = _db.Users
                .Where(c => c.Account == view.Account.ToLower())
                .FirstOrDefault();

            if (hasAccount == null)
                return BadRequest("無此帳號");
            else
            {
                if (!ModelState.IsValid)
                    return BadRequest("Email格式不符");
                else
                {
                    view.Password = BitConverter
                        .ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(view.Password)))
                        .Replace("-", null);

                    if (hasAccount.Password != view.Password)
                        return BadRequest("密碼有誤");
                    else
                    {
                        JwtAuthUtil jwtAuthUtil = new JwtAuthUtil();
                        string token = jwtAuthUtil.GenerateToken(hasAccount.Id, hasAccount.Account, hasAccount.Name, hasAccount.Guid);

                        ApiResponse result = new ApiResponse { };
                        result.Success = true;
                        result.Message = "登入成功";
                        result.Data = new
                        {
                            Username = hasAccount.Name,
                            Identity = "user",
                            UserID = hasAccount.Id,
                            Authorization = token
                        };
                        return Ok(result);
                    }
                }
            }
        }

        /// <summary>
        /// 忘記密碼發信
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/forgotPassword")]
        [SwaggerResponse(typeof(ApiResponse))]
        public IHttpActionResult ForgotPassword(ViewModel view)
        {
            PiDbContext _db = new PiDbContext();

            var hasCounselor = _db.Counselors
                .Where(c => c.Account == view.Account
                .ToLower()).FirstOrDefault();

            var hasUser = _db.Users
                .Where(c => c.Account == view.Account
                .ToLower()).FirstOrDefault();

            if (hasCounselor == null && hasUser == null)
                return BadRequest("無此帳號");
            else
            {
                if (!ModelState.IsValid)
                    return BadRequest("Email格式不符");
                else
                {
                    // 重設密碼頁面
                    string path = Url.Content("https://pi-rocket-coding.vercel.app/resetpassword");

                    // 找出 Guid
                    string guid = "";
                    if (hasCounselor != null && hasCounselor.Account == view.Account.Trim().ToLower())
                    {
                        guid = hasCounselor.Guid.ToString();
                    }
                    if (hasUser != null && hasUser.Account == view.Account.Trim().ToLower())
                    {
                        guid = hasUser.Guid.ToString();
                    }
                        

                    // Google 發信帳號密碼
                    string sendFrom = ConfigurationManager.AppSettings["SendFrom"];
                    string password = ConfigurationManager.AppSettings["GmailPassword"];
                    //string sendTo = view.Account.Trim().ToLower();
                    string sendTo = "hbmanikin@gmail.com";//收件測試
                    string subject = "【拍拍】重設密碼連結";
                    string mailBody = @"<div class='container' style='width: 560px; margin: auto; border: 1px gray solid;'><div class='header'><h2 style = 'color: #424242; margin-left: 10px;'>拍拍</h2></div><div class='main' style='color: #424242; padding: 30px 30px;'><p>親愛的用戶您好：<br><br>請點選下列連結進入重設密碼頁面。<br><br>提醒您，若您未提出重設密碼的需求，請忽略此封信件。</p><div class='btn' style='color: #424242; margin: 40px 0; border-radius: 53px; display: inline-block; background-color: #FFF6E2;'><a href = '" + path + "?guid=" + guid + "' style='text-decoration: none; display: inline-block; padding: 10px 20px; color: black'>重設密碼</a></div></div><div class='footer' style='color: #424242; background-color: #FFF6E2; padding: 20px 10px;'><p> 若您需要聯繫您的諮商師／個案用戶，請直接登入平台與您的諮商師／個案用戶聯繫。若需要客服人員協助，歡迎回覆此信件。</p><ul style = 'list-style: none; display: flex;' ><li><a href='#' style='text-decoration: none; color: black;'>官方網站</a></li><li><span style = 'margin: 0 5px;' >|</ span ></li><li><a href='#' style='text-decoration: none; color: black;'>常見問題</a></li></ul><p>© 2023 Pi Life Limited.</p></div>";
                    string mailBodyEnd = "<p style='color: #424242; text-align: center;'>-----此為系統發出信件，請勿直接回覆，感謝您的配合。-----</p>";

                    SendGmailMail(sendFrom, sendTo, subject, mailBody + mailBodyEnd, password);

                    ApiResponse result = new ApiResponse { };
                    result.Success = true;
                    result.Message = "Email已發送，請檢查信箱";
                    result.Data = null;
                    return Ok(result);
                }
            }
        }

        /// <summary>
        /// 發送系統通知信
        /// </summary>
        public static void SendGmailMail(string sendFrom, string sendTo, string Subject, string mailBody, string password)
        {
            MailMessage mailMessage = new MailMessage(sendFrom, sendTo);
            mailMessage.Subject = Subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = mailBody;
            // SMTP Server
            SmtpClient mailSender = new SmtpClient("smtp.gmail.com");
            NetworkCredential basicAuthenticationInfo = new NetworkCredential(sendFrom, password);
            mailSender.Credentials = basicAuthenticationInfo;
            mailSender.Port = 587;
            mailSender.EnableSsl = true;

            try
            {

                mailSender.Send(mailMessage);
                mailMessage.Dispose();
            }
            catch
            {
                return;
            }
            mailSender = null;
        }


        /// <summary>
        /// 登入後，重新設置密碼
        /// </summary>
        /// <param name="Password">密碼</param>
        /// <param name="ConfirmPassword">二次確認密碼</param>
        /// <returns></returns>
        [HttpPost]
        [JwtAuthFilter]
        [Route("api/resetPassword")]
        public IHttpActionResult PostResetPassword(string Password, string ConfirmPassword)
        {
            PiDbContext _db = new PiDbContext();
            ApiResponse result = new ApiResponse();
            var token = Request.Headers.Authorization.Parameter;
            string secretKey = WebConfigurationManager.AppSettings["TokenKey"];
            if (Password != ConfirmPassword)
            {
                return BadRequest("二次密碼輸入不一樣");
            }


            if (secretKey != null)
            {
                var payload = Jose.JWT.Decode<Dictionary<string, object>>(token, Encoding.UTF8.GetBytes(secretKey), JwsAlgorithm.HS512);
                string _account = payload["Account"].ToString();
                User user = _db.Users.Where(x => x.Account == _account).FirstOrDefault();
                Counselor counselor = _db.Counselors.Where(x => x.Account == _account).FirstOrDefault();
                if (user != null)
                {
                    string newPassword = BitConverter
                          .ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(Password)))
                          .Replace("-", null);
                    if (user.Password == newPassword)
                    {
                        return BadRequest("密碼不能跟原本的相同");
                    }
                    user.Password = newPassword;
                    result.Success = true;
                    result.Message = "個案修改成功";
                    if (ModelState.IsValid)
                    {
                        _db.Entry(user).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                    return Ok(result);
                }
                else if (counselor != null)
                {
                    string newPassword = BitConverter
                           .ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(Password)))
                           .Replace("-", null);
                    if (counselor.Password == newPassword)
                    {
                        return BadRequest("密碼不能跟原本的相同");
                    }

                    counselor.Password = newPassword;
                    result.Success = true;
                    result.Message = "諮商師修改成功";
                    if (ModelState.IsValid)
                    {
                        _db.Entry(counselor).State = EntityState.Modified;
                        _db.SaveChanges();
                    }
                    return Ok(result);
                }
                else
                {
                    return BadRequest("帳號不存在");
                }
            }
            else return BadRequest("沒有token");
        }


        /// <summary>
        /// 信箱重新設置密碼
        /// </summary>
        /// <param name="Password">密碼</param>
        /// <param name="ConfirmPassword">二次確認密碼</param>
        /// <param name="Guid">Guid</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/resetPassword/guid")]
        public IHttpActionResult PostResetPassword(string Password, string ConfirmPassword, Guid Guid)
        {
            PiDbContext _db = new PiDbContext();
            ApiResponse result = new ApiResponse();
            //判斷用哪一種方式重設密碼guid / token

            if (Password != ConfirmPassword)
            {
                return BadRequest("二次密碼輸入不一樣");
            }

            User user = _db.Users.Where(x => x.Guid == Guid).FirstOrDefault();
            Counselor counselor = _db.Counselors.Where(x => x.Guid == Guid).FirstOrDefault();
            if (user != null)
            {
                user.Password = BitConverter
                           .ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(Password)))
                           .Replace("-", null);
                if (ModelState.IsValid)
                {
                    _db.Entry(user).State = EntityState.Modified;
                    _db.SaveChanges();
                    result.Success = true;
                    result.Message = "個案修改成功";
                }
                return Ok(result);
            }
            else if (counselor != null)
            {
                counselor.Password = BitConverter
                           .ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(Password)))
                           .Replace("-", null);
                if (ModelState.IsValid)
                {
                    _db.Entry(counselor).State = EntityState.Modified;
                    _db.SaveChanges();
                    result.Success = true;
                    result.Message = "諮商師修改成功";
                }
                return Ok(result);

            }
            else return BadRequest("Guid錯誤");
        }

        //**
    }
}


