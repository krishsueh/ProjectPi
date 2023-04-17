using Newtonsoft.Json;
using NSwag.Annotations;
using ProjectPi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProjectPi.Controllers
{
    [OpenApiTag("BackendManagement", Description = "後台管理")]
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

    }
}
