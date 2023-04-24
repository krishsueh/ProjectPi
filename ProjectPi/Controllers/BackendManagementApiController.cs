using Newtonsoft.Json;
using NSwag.Annotations;
using ProjectPi.Models;
using ProjectPi.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
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
        /// 得到諮商師證書申請列表
        /// </summary>
        /// <returns></returns>
        [Route("api/CounselorLicense")]
        [HttpGet]
        public IHttpActionResult GetCounselorLicense()
        {
            ApiResponse result = new ApiResponse();
            var LicenseList = _db.Counselors.Where(x => x.LicenseImg != null && x.Validation == false)
                .Select(x => new { x.Id,x.Name,x.LicenseImg,x.CertNumber}).ToList();
            result.Success = true;
            result.Message = "取得成功";
            if(!LicenseList.Any())
            {
                result.Message = "沒有諮商師申請";
                return Ok(result);
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
                counselor.LicenseImg = "null";
                result.Success = true;
                result.Message = "審核不通過";
            }
            else
            {
                result.Success = true;
                result.Message = "審核通過";
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
        public IHttpActionResult GetNewebPayOrder(bool isPay = true, int PageNumber =1 , int PageSize = 10)
        {
            ApiResponse result = new ApiResponse();
            var orderRecordsList = _db.OrderRecords.Where(x => x.OrderStatus == "已成立").OrderBy(x => x.Id).Skip((PageNumber - 1) * PageSize).Take(PageSize).Select(x => new { x.CounselorName,x.UserName,x.OrderNum,x.OrderDate,x.Price,x.Field}).ToList();
            if (isPay == false) orderRecordsList = _db.OrderRecords.Where(x => x.OrderStatus != "已成立").OrderBy(x => x.Id).Skip((PageNumber - 1) * PageSize).Take(PageSize).Select(x => new { x.CounselorName, x.UserName, x.OrderNum, x.OrderDate, x.Price, x.Field }).ToList();
            
            if(orderRecordsList == null)
            {
                return BadRequest("沒有任何金流資料");
            }
            
            result.Success = true;
            result.Message = "取得成功";
            result.Data = new { orderRecordsList };
            return Ok(result);
        }

        /// <summary>
        /// 取得治療紀錄
        /// </summary>
        /// <returns></returns>
        [Route("api/getAppTList")]
        [HttpGet]
        public IHttpActionResult GetAppTList()
        {
            ApiResponse result = new ApiResponse();
            var appointmentsList = _db.Appointments.Select(x => new { x.Id , x.MyOrder.Field ,x.MyOrder.CounselorName,x.MyOrder.UserName , x.ReserveStatus ,Time = x.AppointmentTime!=null?x.AppointmentTime:null}).ToList();
            if (appointmentsList == null) return BadRequest("沒有任何資料");
            result.Success = true;
            result.Message = "成功取得訊息";
            result.Data = new { appointmentsList };
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


    }
}
