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
    }
}
