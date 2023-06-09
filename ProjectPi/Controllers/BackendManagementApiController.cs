﻿using Newtonsoft.Json;
using NSwag.Annotations;
using ProjectPi.Models;
using ProjectPi.Security;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace ProjectPi.Controllers
{
    [OpenApiTag("BackendManagement", Description = "後台管理系統")]
    public class BackendManagementApiController : ApiController
    {
        PiDbContext _db = new PiDbContext();
        /// <summary>
        /// 接收結果
        /// </summary>
        /// <returns></returns>
        [Route("api/getPaymentData")]
        [HttpPost]
        public HttpResponseMessage GetPaymentData(NewebPayReturn data)
        {

            // 付款失敗跳離執行
            var response = Request.CreateResponse(HttpStatusCode.OK);
            if (!data.Status.Equals("SUCCESS")) return response;

            // 加密用金鑰
            string hashKey = "1jUogKMU7sfOyJBtARgJzUfCd3NzFlIS";
            string hashIV = "CUUuIrArgfNETY1P";
            // AES 解密
            string decryptTradeInfo = CryptoUtil.DecryptAESHex(data.TradeInfo, hashKey, hashIV);
            PaymentResult result = JsonConvert.DeserializeObject<PaymentResult>(decryptTradeInfo);
            // 取出交易記錄資料庫的訂單ID
            string orderNo = result.Result.MerchantOrderNo;
            //            int userId = Convert.ToInt32(orderNo);
            // 用取得的"訂單ID"修改資料庫此筆訂單的付款狀態為 true
            List<OrderRecord> orderRecord = _db.OrderRecords.Where(x => x.OrderNum == orderNo).ToList();
            foreach (var item in orderRecord)
            {
                item.OrderStatus = "已成立";
            }
            _db.SaveChanges();
            // 用取得的"訂單ID"寄出付款完成訂單成立，商品準備出貨通知信

            foreach(var item in orderRecord)
            {
                Appointment appointment = new Appointment();
                for(int i=0;i< item.Quantity;i++)
                {
                    appointment.OrderId = item.Id;
                    appointment.ReserveStatus = "待預約";
                    _db.Appointments.Add(appointment);
                    _db.SaveChanges();
                }

            }
            /*
            //建立預約明細
            var findOrders = _db.OrderRecords
                .Where(c => c.UserName == orderRecord[0].UserName)
                .GroupBy(c => c.OrderNum)
                .OrderByDescending(c => c.Key)
                .ToList()
                .FirstOrDefault();

            if (!findOrders.Any())
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "查無訂單");
            else
            {
                Appointment appointment = new Appointment();
                foreach (var orderItem in findOrders)
                {
                    switch (orderItem.Quantity)
                    {
                        case 1:
                            appointment.OrderId = orderItem.Id;
                            appointment.ReserveStatus = "待預約";
                            _db.Appointments.Add(appointment);
                            _db.SaveChanges();
                            break;
                        case 3:
                            for (int i = 0; i < 3; i++)
                            {
                                appointment.OrderId = orderItem.Id;
                                appointment.ReserveStatus = "待預約";
                                _db.Appointments.Add(appointment);
                                _db.SaveChanges();
                            }
                            break;
                        case 5:
                            for (int i = 0; i < 5; i++)
                            {
                                appointment.OrderId = orderItem.Id;
                                appointment.ReserveStatus = "待預約";
                                _db.Appointments.Add(appointment);
                                _db.SaveChanges();
                            }
                            break;
                    }
                }
            }

            */
            return response;
        }

        /// <summary>
        /// 顯示諮商師證書申請列表
        /// </summary>
        /// <returns></returns>
        [Route("api/CounselorLicense")]
        [HttpGet]
        public IHttpActionResult GetCounselorLicense(string CounselorName="")
        {
            ApiResponse result = new ApiResponse();
            var LicenseList = _db.Counselors.Where(x => x.LicenseImg != null && x.Name.Contains(CounselorName))
                .Select(x => new {  x.Id,x.Name, LicenseImg = "https://pi.rocket-coding.com/upload/license/" + x.LicenseImg, x.CertNumber,x.Validation}).OrderByDescending(x=>x.Id).ToList();
            if(string.IsNullOrEmpty(CounselorName))
            {
                LicenseList = _db.Counselors.Where(x => x.LicenseImg != null)
                .Select(x => new { x.Id, x.Name, LicenseImg = "https://pi.rocket-coding.com/upload/license/" + x.LicenseImg, x.CertNumber, x.Validation }).OrderByDescending(x => x.Id).ToList();
            }
            else
            {
                LicenseList = _db.Counselors.Where(x => x.LicenseImg != null && x.Name.Contains(CounselorName))
                .Select(x => new { x.Id, x.Name, LicenseImg = "https://pi.rocket-coding.com/upload/license/" + x.LicenseImg, x.CertNumber, x.Validation }).OrderByDescending(x => x.Id).ToList();
            }
            result.Success = true;
            result.Message = "取得成功";
            if(!LicenseList.Any())
            {
                result.Message = "沒有諮商師";
            }
            result.Data = new { LicenseList };
            return Ok(result);
        }
        /// <summary>
        /// 修改諮商師開通狀態
        /// </summary>
        /// <returns></returns>
        [Route("api/udateValidation")]
        [HttpPut]
        public IHttpActionResult UdateValidation(ViewModel_C.C_Validation view)
        {
            ApiResponse result = new ApiResponse();
            Counselor counselor = _db.Counselors.Where(x => x.Id == view.CounselorId).FirstOrDefault();
            if (counselor == null) return BadRequest("沒有此諮商師");
            counselor.Validation = view.Validation;
            if (counselor.Validation == false)
            {
                //counselor.LicenseImg = "null";
                result.Success = true;
                result.Message = "審核不通過";
            }
            else if(counselor.Validation == true)
            {
                result.Success = true;
                result.Message = "審核通過";
            }
            else
            {
                return BadRequest("參數錯誤");
            }
            _db.SaveChanges();
            return Ok(result);
        }

        /// <summary>
        /// 取得金流
        /// </summary>
        /// <returns></returns>
        [Route("api/getNewebPayOrder")]
        [HttpGet]
        public IHttpActionResult GetNewebPayOrder(string UserName="")
        {
            ApiResponse result = new ApiResponse();
            int sum = 0;
            var orderRecordsList = _db.OrderRecords.Where(x => x.OrderStatus == "已成立").OrderByDescending(x => x.OrderDate).Select(x => new { x.CounselorName, x.UserName, x.OrderNum, x.OrderDate, x.Price, x.Field, x.OrderStatus }).ToList();

            if (string.IsNullOrEmpty(UserName))
            {
                orderRecordsList = _db.OrderRecords.Where(x => x.OrderStatus == "已成立").OrderByDescending(x => x.OrderDate).Select(x => new { x.CounselorName, x.UserName, x.OrderNum, x.OrderDate, x.Price, x.Field, x.OrderStatus }).ToList();
            }
            else
            {
                orderRecordsList = _db.OrderRecords.Where(x => x.OrderStatus == "已成立" && x.UserName.Contains(UserName)).OrderByDescending(x => x.OrderDate).Select(x => new { x.CounselorName, x.UserName, x.OrderNum, x.OrderDate, x.Price, x.Field, x.OrderStatus }).ToList();
            }

            foreach(var item in orderRecordsList)
            {
                sum += item.Price;
            }
            result.Success = true;
            result.Message = "取得成功";
            result.Data = new { PriceTotal=sum,orderRecordsList };
            if (orderRecordsList == null)
            {
                result.Message = "沒有任何金流資料";
            }
            return Ok(result);
        }

        /// <summary>
        /// 取得諮商紀錄
        /// </summary>
        /// <returns></returns>
        [Route("api/getAppTList")]
        [HttpGet]
        public IHttpActionResult GetAppTList(string UserName="")
        {
            ApiResponse result = new ApiResponse();
            var appointmentsList = _db.Appointments.Where(x=>x.MyOrder.UserName.Contains(UserName)).Select(x => new { x.Id , x.MyOrder.Field ,x.MyOrder.CounselorName,x.MyOrder.UserName , x.ReserveStatus ,Time = x.AppointmentTime!=null?x.AppointmentTime:null , x.Star}).OrderByDescending(x=>x.Time).ToList();
            if (string.IsNullOrEmpty(UserName))
            {
                appointmentsList = _db.Appointments.Select(x => new { x.Id, x.MyOrder.Field, x.MyOrder.CounselorName, x.MyOrder.UserName, x.ReserveStatus, Time = x.AppointmentTime != null ? x.AppointmentTime : null, x.Star }).OrderByDescending(x => x.Time).ToList();
            }
            else
            {
                appointmentsList = _db.Appointments.Where(x => x.MyOrder.UserName.Contains(UserName)).Select(x => new { x.Id, x.MyOrder.Field, x.MyOrder.CounselorName, x.MyOrder.UserName, x.ReserveStatus, Time = x.AppointmentTime != null ? x.AppointmentTime : null, x.Star }).OrderByDescending(x => x.Time).ToList();
            }
            result.Success = true;
            result.Message = "成功取得訊息";
            result.Data = new { appointmentsList };
            if (appointmentsList == null)
            {
                result.Message = "沒有任何資料";
            }
          
            return Ok(result);
        }

        /// <summary>
        /// 取得諮商師列表
        /// </summary>
        /// <returns></returns>
        [Route("api/getCounselorList")]
        [HttpGet]
        public IHttpActionResult GetCounselorList(int PageNumber=1 , int PageSize = 10)
        {
            ApiResponse result = new ApiResponse();
         
            var counselorList = _db.Counselors.OrderBy(x => x.Id).Skip((PageNumber - 1) * PageSize).Take(PageSize).Select(x=> new { x.Id,x.Name,x.Validation,x.Photo,x.CertNumber,x.InitDate}).ToList();
            if (counselorList == null) return BadRequest("沒有任何資料");
            result.Success = true;
            result.Message = "成功取得訊息";
            result.Data = new { counselorList };
            return Ok(result);
        }

        /// <summary>
        /// 取得個案列表
        /// </summary>
        /// <returns></returns>
        [Route("api/getUserList")]
        [HttpGet]
        public IHttpActionResult GetUserList(int PageNumber = 1, int PageSize = 10)
        {
            ApiResponse result = new ApiResponse();
         
            var userList = _db.Users.OrderBy(x => x.Id).Skip((PageNumber-1) * PageSize).Take(PageSize).Select(x => new { x.Id, x.Name, x.Account,x.Sex, x.InitDate }).ToList();
            if (userList == null) return BadRequest("沒有任何資料");
            result.Success = true;
            result.Message = "成功取得訊息";
            result.Data = new { userList };
            return Ok(result);
        }

        /// <summary>
        /// 顯示管理員名單
        /// </summary>
        /// <returns></returns>
        [Route("api/backend/getBackender")]
        [HttpGet]
        public IHttpActionResult GetBackender(int PageNumber = 1, int PageSize = 10)
        {
            ApiResponse result = new ApiResponse();
            if (PageNumber <= 0 || PageSize <= 0) return BadRequest("分頁參數錯誤");
            var backEndMangersList = _db.BackEndMangers.OrderBy(x => x.Id).Skip((PageNumber - 1) * PageSize).Take(PageSize)
                .Select(x => new {
                x.Id,
                x.Name,
                x.Sex,
                x.Guid,
                x.InitDate,
                x.AdminAccess,
                }).ToList();
            
            result.Success = true;
            result.Message = "管理員取得成功";
            result.Data = new { backEndMangersList };

            return Ok(result);
        }

        /// <summary>
        /// 編輯管理員
        /// </summary>
        /// <returns></returns>
        [Route("api/backend/updateBackender")]
        [HttpPut]
        public IHttpActionResult PutBackender(ViewModel.backenderUpdate view)
        {
            ApiResponse result = new ApiResponse();
            BackEndManger backEndManger = _db.BackEndMangers.Where(x => x.Guid == view.Guid).FirstOrDefault();
            if (backEndManger == null) return BadRequest("Guid錯誤");
            backEndManger.Name = view.Name;
            backEndManger.AdminAccess = view.AdminAccess;
            _db.SaveChanges();
            result.Success = true;
            result.Message = "管理員修改成功";
            
            return Ok(result);
        }

        /// <summary>
        /// 管理員註冊
        /// </summary>
        /// <param name="view"></param>
        /// <response code="200">註冊成功</response>
        /// <returns></returns>
        [HttpPost]
        [Route("api/backend/register")]
        [SwaggerResponse(typeof(ApiResponse))]
        public IHttpActionResult Register_BackEnd(ViewModel_U.Register view)
        {
            PiDbContext _db = new PiDbContext();

            var hasEmail1 = _db.BackEndMangers
                .Where(c => c.Account == view.Account
                .ToLower()).FirstOrDefault();


            if (hasEmail1 != null)
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
                        BackEndManger backEndManger = new BackEndManger();
                        backEndManger.Account = view.Account;
                        backEndManger.Password = BitConverter
                            .ToString(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(view.Password))).Replace("-", null);
                        backEndManger.Name = view.Name;
                        backEndManger.Sex = view.Sex;
                        backEndManger.BirthDate = view.BirthDate;
                        backEndManger.AdminAccess = 0;

                        _db.BackEndMangers.Add(backEndManger);
                        _db.SaveChanges();

                        ApiResponse result = new ApiResponse { };
                        result.Success = true;
                        result.Message = "管理員註冊成功";
                        result.Data = null;

                        return Ok(result);
                    }
                }
            }
        }

        /// <summary>
        /// 管理員登入
        /// </summary>
        /// <param name="view"></param>
        /// <response code="200">登入驗證成功</response>
        /// <returns></returns>
        [HttpPost]
        [Route("api/backend/login")]
        [SwaggerResponse(typeof(ApiResponse))]
        public IHttpActionResult Login_BackEnd(ViewModel_U.Login view)
        {
            PiDbContext _db = new PiDbContext();

            BackEndManger hasAccount = _db.BackEndMangers
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
                            Identity = "backend",
                            BackendID = hasAccount.Id,
                            Authorization = token
                        };
                        return Ok(result);
                    }
                }
            }
        }

        /// <summary>
        /// 身分審核未通過，寄信給諮商師
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/backend/SendEmailCounselor")]
        [SwaggerResponse(typeof(ApiResponse))]
        public async Task<IHttpActionResult> SendEmailCounselor(ViewModel_C.C_Account view)
        {
            string indexPath = Url.Content("https://pi-rocket-coding.vercel.app");
            string path = "https://pi-rocket-coding.vercel.app/resentlicense";
            Counselor counselor = _db.Counselors.Where(x => x.Id == view.Id).FirstOrDefault();
            if (counselor == null) return BadRequest("沒有此諮商師");
            ApiResponse result = new ApiResponse();
            string guid = counselor.Guid.ToString();

            // Google 發信帳號密碼
            string sendFrom = ConfigurationManager.AppSettings["SendFrom"];
            string password = ConfigurationManager.AppSettings["GmailPassword"];
            string sendTo = counselor.Account.Trim().ToLower();
            string subject = "【拍拍】您的身分審核未通過。";
            string mailBody = @"<div class='container' style='width: 560px; margin: auto; border: 1px gray solid;'><div class='header'><h2 style = 'color: #424242; margin-left: 10px;'>拍拍</h2></div><div class='main' style='color: #424242; padding: 30px 30px;'><p>親愛的用戶您好：<br><br>您的身分審核未通過。<br><br>提醒您，請重新上傳符合資格的執業執照。</p><div class='btn' style='color: #424242; margin: 40px 0; border-radius: 53px; display: inline-block; background-color: #FFF6E2;'><a href = '" + path + "?guid=" + guid + "' style='text-decoration: none; display: inline-block; padding: 10px 20px; color: black'>重新上傳</a></div></div><div class='footer' style='color: #424242; background-color: #FFF6E2; padding: 20px 10px;'><p> 若您需要聯繫您的諮商師／個案用戶，請直接登入平台與您的諮商師／個案用戶聯繫。若需要客服人員協助，歡迎回覆此信件。</p><ul style = 'list-style: none; display: flex;' ><li><a href='" + indexPath + "' style='text-decoration: none; color: black;'>官方網站</a></li><li><span style = 'margin: 0 5px;' >|</span ></li><li><a href='#' style='text-decoration: none; color: black;'>常見問題</a></li></ul><p>© 2023 Pi Life Limited.</p></div>";
            SendGmailMail(sendFrom, sendTo, subject, mailBody, password);
            result.Success = true;
            result.Message = "寄信成功";
            return Ok(result);
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

        ///ModelGuid view
        /// <summary>
        /// 更新/補件諮商師執照
        /// </summary>
        /// <returns></returns>
        [Route("api/updateGuidLicense")]
        //[JwtAuthFilter]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateGuidLicense()
        {

            var counselorToken = Request.Headers.Authorization.Parameter;//JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            Guid guid = Guid.Parse(counselorToken);
            int counselorId = _db.Counselors.Where(x => x.Guid == guid).FirstOrDefault().Id;//(int)counselorToken["Id"];
            string counselorName = _db.Counselors.Where(x => x.Guid == guid).FirstOrDefault().Name;//counselorToken["Name"].ToString();

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
                string fileName = "License_" + DateTime.Now.ToString("yyyyMMddHHmmss") + fileType;

                // 儲存圖片，單檔用.FirstOrDefault()直接取出，多檔需用迴圈
                var fileBytes = await provider.Contents.FirstOrDefault().ReadAsByteArrayAsync();
                var filePath = Path.Combine(root, fileName);

                // 創建文件流
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    // 寫入文件內容
                    await fileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                }

                // 使用 SixLabors.ImageSharp 調整圖片尺寸 (正方形大頭貼)
                var image = SixLabors.ImageSharp.Image.Load<Rgba32>(filePath);
                image.Mutate(x => x.Resize(640, 0));
                image.Save(filePath);

                // 將頭像路徑存入資料庫
                Counselor haveCounselor = _db.Counselors
                .Where(x => x.Id == counselorId).FirstOrDefault();
                haveCounselor.LicenseImg = fileName;
                _db.SaveChanges();

                ApiResponse result = new ApiResponse { };
                result.Success = true;
                result.Message = "成功更新諮商師執照";
                result.Data = null;
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("執照上傳失敗或未上傳:");
            }
        }

    }
}
