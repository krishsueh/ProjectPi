using Jose;
using NSwag.Annotations;
using ProjectPi.Models;
using ProjectPi.Security;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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
                    // 官網首頁
                    string indexPath = Url.Content("https://pi-rocket-coding.vercel.app");

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
                    string sendTo = view.Account.Trim().ToLower();
                    string subject = "【拍拍】重設密碼連結";
                    string mailBody = @"<div class='container' style='width: 560px; margin: auto; border: 1px gray solid;'><div class='header'><h2 style = 'color: #424242; margin-left: 10px;'>拍拍</h2></div><div class='main' style='color: #424242; padding: 30px 30px;'><p>親愛的用戶您好：<br><br>請點選下列連結進入重設密碼頁面。<br><br>提醒您，若您未提出重設密碼的需求，請忽略此封信件。</p><div class='btn' style='color: #424242; margin: 40px 0; border-radius: 53px; display: inline-block; background-color: #FFF6E2;'><a href = '" + path + "?guid=" + guid + "' style='text-decoration: none; display: inline-block; padding: 10px 20px; color: black'>重設密碼</a></div></div><div class='footer' style='color: #424242; background-color: #FFF6E2; padding: 20px 10px;'><p> 若您需要聯繫您的諮商師／個案用戶，請直接登入平台與您的諮商師／個案用戶聯繫。若需要客服人員協助，歡迎回覆此信件。</p><ul style = 'list-style: none; display: flex;' ><li><a href='" + indexPath + "' style='text-decoration: none; color: black;'>官方網站</a></li><li><span style = 'margin: 0 5px;' >|</ span ></li><li><a href='#' style='text-decoration: none; color: black;'>常見問題</a></li></ul><p>© 2023 Pi Life Limited.</p></div>";

                    SendGmailMail(sendFrom, sendTo, subject, mailBody, password);

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
        public IHttpActionResult PostResetPassword(ViewModel.AccountReset _Password)
        {
            PiDbContext _db = new PiDbContext();
            ApiResponse result = new ApiResponse();
            var token = Request.Headers.Authorization.Parameter;
            string secretKey = WebConfigurationManager.AppSettings["TokenKey"];
            if (_Password.Password.Length < 8)
            {
                return BadRequest("密碼不能少於8位數");
            }
            if (_Password.Password != _Password.ConfirmPassword)
            {
                return BadRequest("二次密碼輸入不符");
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
                          .ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(_Password.Password)))
                          .Replace("-", null);
                    if (user.Password == newPassword)
                    {
                        return BadRequest("不能與舊密碼相同");
                    }
                    user.Password = newPassword;
                    result.Success = true;
                    result.Message = "密碼重設成功";
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
                           .ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(_Password.Password)))
                           .Replace("-", null);
                    if (counselor.Password == newPassword)
                    {
                        return BadRequest("不能與舊密碼相同");
                    }

                    counselor.Password = newPassword;
                    result.Success = true;
                    result.Message = "密碼重設成功";
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
            else return BadRequest("沒有登入");
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
        public IHttpActionResult PostGuidSetPassword(ViewModel.AccountResetGuid _Password)
        {
            PiDbContext _db = new PiDbContext();
            ApiResponse result = new ApiResponse();
            //判斷用哪一種方式重設密碼guid / token
            if (_Password.Password.Length < 8)
            {
                return BadRequest("密碼不能少於8位數");
            }
            if (_Password.Password != _Password.ConfirmPassword)
            {
                return BadRequest("二次密碼輸入不符");
            }

            User user = _db.Users.Where(x => x.Guid == _Password.Guid).FirstOrDefault();
            Counselor counselor = _db.Counselors.Where(x => x.Guid == _Password.Guid).FirstOrDefault();
            if (user != null)
            {
                var newPassword = BitConverter
                           .ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(_Password.Password)))
                           .Replace("-", null);
                if (user.Password == newPassword)
                {
                    return BadRequest("不能與舊密碼相同");
                }
                user.Password = newPassword;
                if (ModelState.IsValid)
                {
                    _db.Entry(user).State = EntityState.Modified;
                    _db.SaveChanges();
                    result.Success = true;
                    result.Message = "密碼重設成功";
                }
                return Ok(result);
            }
            else if (counselor != null)
            {
                var newPassword = BitConverter
                          .ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(_Password.Password)))
                          .Replace("-", null);
                if (counselor.Password == newPassword)
                {
                    return BadRequest("不能與舊密碼相同");
                }
                counselor.Password = newPassword;
                if (ModelState.IsValid)
                {
                    _db.Entry(counselor).State = EntityState.Modified;
                    _db.SaveChanges();
                    result.Success = true;
                    result.Message = "密碼重設成功";
                }
                return Ok(result);

            }
            else return BadRequest("參數錯誤，請重新發送驗證信");
        }

        /// <summary>
        /// 儲存諮商師執照
        /// </summary>
        /// <returns></returns>
        [Route("api/uploadLicense")]
        [HttpPost]
        public async Task<IHttpActionResult> UploadLicense()
        {
            PiDbContext _db = new PiDbContext();

            // 檢查請求是否包含 multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // 使用 HttpContext.Current.Server.MapPath 方法來獲取指定路徑的物理路徑
            string root = HttpContext.Current.Server.MapPath(@"~/upload/license");

            try
            {
                // 讀取 MIME 資料
                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                // 取得檔案副檔名，單檔用.FirstOrDefault()直接取出，多檔需用迴圈
                string fileNameData = provider.Contents.FirstOrDefault().Headers.ContentDisposition.FileName.Trim('\"');
                string fileType = fileNameData.Remove(0, fileNameData.LastIndexOf('.')); // .jpg

                // 定義檔案名稱
                string fileName = "License" + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + fileType;

                // 儲存圖片，單檔用.FirstOrDefault()直接取出，多檔需用迴圈
                var fileBytes = await provider.Contents.FirstOrDefault().ReadAsByteArrayAsync();
                var filePath = Path.Combine(root, fileName);

                // 創建文件流
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    // 寫入文件內容
                    await fileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                }

                // 從form-data撈出Account，再將檔名寫入資料庫
                string account = "";
                foreach (var content in provider.Contents)
                {
                    var name = content.Headers.ContentDisposition.Name.Trim('"');
                    if (name == "Account")
                    {
                        account = await content.ReadAsStringAsync();
                        Counselor haveCounselor = _db.Counselors
                                .Where(x => x.Account == account).FirstOrDefault();
                        haveCounselor.LicenseImg = fileName;
                        _db.SaveChanges();
                        break;
                    }
                }

                ApiResponse result = new ApiResponse { };
                result.Success = true;
                result.Message = "成功上傳諮商師執照";
                result.Data = null;
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("執照上傳失敗或未上傳");
            }
        }

        /// <summary>
        /// 得到ZoomUrl
        /// </summary>
        /// <returns></returns>
        [Route("api/getZoomUrl")]
        [JwtAuthFilter]
        [HttpGet]
        public async Task<IHttpActionResult> GetZoomUrl()
        {
            PiDbContext _db = new PiDbContext();
            ApiResponse result = new ApiResponse();
            var userToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            string userAccount = userToken["Account"].ToString();
            Counselor counselor = _db.Counselors.Where(x => x.Account == userAccount).FirstOrDefault();
            User user = _db.Users.Where(x => x.Account == userAccount).FirstOrDefault();
            string url = "";
            bool isHaveUrl = false;
            if (counselor != null)
            {
                var appointmentsWithOrder = _db.Appointments
                .Join(
                    _db.OrderRecords,
                    appointment => appointment.OrderId,
                    order => order.Id,
                    (appointment, order) => new { Appointment = appointment, Order = order }
                )
                .Where(joined =>  joined.Order.CounselorId == counselor.Id && joined.Appointment.ReserveStatus == "已成立" && joined.Appointment.AppointmentTime != null)
                .Select(joined => new
                {
                    AppointmentId = joined.Appointment.Id,
                    OrderNum = joined.Order.OrderNum,
                    UserName = joined.Order.UserName,
                    CounselorName = joined.Order.CounselorName,
                    AppointmentTime = joined.Appointment.AppointmentTime,
                    ReserveStatus = joined.Appointment.ReserveStatus,
                    ZoomLink = joined.Appointment.ZoomLink,
                    CounsellingRecord = joined.Appointment.CounsellingRecord,
                    RecordDate = joined.Appointment.RecordDate,
                    Star = joined.Appointment.Star,
                    Comment = joined.Appointment.Comment,
                    InitDate = joined.Appointment.InitDate
                })
                .OrderBy(joined => joined.AppointmentTime)
                .ToList();

                
                if (appointmentsWithOrder.Count != 0)
                {
                    string msg = "";
                    TimeSpan spanTime = TimeSpan.FromMinutes(0);
                    try
                    {
                        foreach(var item in appointmentsWithOrder)
                        {
                            spanTime = (TimeSpan)(item.AppointmentTime - DateTime.Now);
                            msg += " spanTime = " + spanTime.ToString();
                            if(spanTime.TotalMinutes > -30 && spanTime.TotalMinutes < 60)
                            {
                                url = item.ZoomLink;
                                break;
                            }
                        }
                        if(string.IsNullOrEmpty(url))
                        {
                           
                            result.Success = true;
                            result.Message = "課程時間還沒到";
                            result.Data = new {isHaveUrl , appointmentsWithOrder };
                            return Ok(result);
                        }
                        isHaveUrl = true;
                        result.Success = true;
                        result.Message = "時間快到囉~ " ;
                        result.Data = new { isHaveUrl,url };
                        return Ok(result);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("error: " + ex);
                    }
            
              
                }
                else
                {
                    result.Success = true;
                    result.Message = "沒有成立的訂單";
                  
                    return Ok(result);
                }
            }
            else if (user != null)
            {
                var appointmentsWithOrder = _db.Appointments
                .Join(
                    _db.OrderRecords,
                    appointment => appointment.OrderId,
                    order => order.Id,
                    (appointment, order) => new { Appointment = appointment, Order = order }
                )
                .Where(joined => joined.Order.UserId == user.Id && joined.Appointment.ReserveStatus == "已成立" && joined.Appointment.AppointmentTime != null)
                .Select(joined => new
                {
                    AppointmentId = joined.Appointment.Id,
                    OrderNum = joined.Order.OrderNum,
                    UserName = joined.Order.UserName,
                    CounselorName = joined.Order.CounselorName,
                    AppointmentTime = joined.Appointment.AppointmentTime,
                    ReserveStatus = joined.Appointment.ReserveStatus,
                    ZoomLink = joined.Appointment.ZoomLink,
                    CounsellingRecord = joined.Appointment.CounsellingRecord,
                    RecordDate = joined.Appointment.RecordDate,
                    Star = joined.Appointment.Star,
                    Comment = joined.Appointment.Comment,
                    InitDate = joined.Appointment.InitDate
                })
                .OrderBy(joined => joined.AppointmentTime)
                .ToList();


                if (appointmentsWithOrder.Count != 0)
                {
                    string msg = "";
                    TimeSpan spanTime = TimeSpan.FromMinutes(0);
                    try
                    {
                        foreach (var item in appointmentsWithOrder)
                        {
                            spanTime = (TimeSpan)(item.AppointmentTime - DateTime.Now);
                            msg += " spanTime = " + spanTime.ToString();
                            if (spanTime.TotalMinutes > -30 && spanTime.TotalMinutes < 60)
                            {
                                url = item.ZoomLink;
                                break;
                            }
                        }
                        if (string.IsNullOrEmpty(url))
                        {
                            isHaveUrl = false;
                            result.Success = true;
                            result.Message = "課程時間還沒到";
                            result.Data = new { isHaveUrl , appointmentsWithOrder };
                            return Ok(result);
                        }
                        isHaveUrl = true;
                        result.Success = true;
                        result.Message = "時間快到囉~ ";
                        result.Data = new { isHaveUrl, url };
                        return Ok(result);
                    }
                    catch (Exception ex)
                    {
                        return BadRequest("error: " + ex);
                    }


                }
                else
                {
                    isHaveUrl = false;
                    result.Success = true;
                    result.Message = "沒有成立的訂單";
                    result.Data = new { isHaveUrl};
                    return Ok(result);
                }

            }
            else return BadRequest("Token錯誤");

        }

       

    }
}


