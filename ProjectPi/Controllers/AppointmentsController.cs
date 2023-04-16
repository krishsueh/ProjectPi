﻿using NSwag.Annotations;
using ProjectPi.Models;
using ProjectPi.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;

namespace ProjectPi.Controllers
{
    [OpenApiTag("Appointments", Description = "個案預約課程流程")]
    public class AppointmentsController : ApiController
    {
        PiDbContext _db = new PiDbContext();

        /// <summary>
        /// 取得諮商師總覽
        /// </summary>
        /// <returns></returns>
        [Route("api/profiles")]
        [HttpGet]
        public IHttpActionResult GetProfiles(int page = 1, string keyword = "", string tag = "")
        {
            var Counselors = _db.Features.AsQueryable();

            //搜尋姓名
            if (!string.IsNullOrEmpty(keyword))
            {
                page = 1;
                Counselors = Counselors.Where(x => x.MyCounselor.Name.Contains(keyword));
            }

            //篩選諮商主題 (前端傳入 ?tag=126)
            if (!string.IsNullOrEmpty(tag))
            {
                int[] fieldIds = new int[tag.Length];
                try
                {
                    for (int i = 0; i < tag.Length; i++)
                    {
                        fieldIds[i] = int.Parse(tag[i].ToString());
                    }
                    page = 1;
                    Counselors = Counselors.Where(x => fieldIds.Contains(x.FieldId));
                }
                catch
                {
                    return BadRequest("數值錯誤");
                }
            }

            //使用字串篩選主題
            //因為 tags 是一個 List<string>，如果沒有加上 tags.Any() 的判斷，即使 tags 是 null，程式也會執行下去，並嘗試在 tags 上呼叫 Contains() 方法，導致發生空值異常。
            //[FromUri] List<string> tags = null
            //if (tags != null && tags.Any())
            //    Counselors = Counselors.Where(x => tags.Contains(x.MyField.Field));

            //沒有使用篩選功能的總筆數
            var CounselorNum = Counselors
                .Select(x => x.CounselorId)
                .Distinct()
                .Count();

            //總頁數
            int pageNum = 0;
            int pageSize = 10;
            if (CounselorNum % pageSize == 0)
                pageNum = CounselorNum / pageSize;
            else
                pageNum = CounselorNum / pageSize + 1;

            ViewModel.SearchingCounselors data = new ViewModel.SearchingCounselors();
            data.TotalPageNum = pageNum;
            data.CounselorsData = new List<ViewModel.CounselorsData>();

            var counsleorList = Counselors
                .Select(x => new
                {
                    x.MyCounselor.Id,
                    x.MyCounselor.Photo,
                    x.MyCounselor.Name,
                    x.MyCounselor.SellingPoint,
                    x.MyCounselor.SelfIntroduction
                })
                .Distinct()
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();


            //照片存取位置
            string path = "https://pi.rocket-coding.com/upload/headshot/";
            foreach (var item in counsleorList)
            {
                ViewModel.CounselorsData counselorsData = new ViewModel.CounselorsData();
                counselorsData.Id = item.Id;
                counselorsData.Name = item.Name;
                counselorsData.SellingPoint = item.SellingPoint;
                counselorsData.SelfIntroduction = item.SelfIntroduction;
                counselorsData.Photo = path + item.Photo;

                data.CounselorsData.Add(counselorsData);
            }

            ApiResponse result = new ApiResponse { };
            result.Success = true;
            if (counsleorList.Any())
            {
                result.Message = "成功取得諮商師總覽";
                result.Data = data;
            }
            else
            {
                result.Message = "找不到符合篩選條件的諮商師";
                result.Data = null;
            }
            return Ok(result);
        }

        /// <summary>
        /// 取得諮商師總覽 (手機版)
        /// </summary>
        /// <returns></returns>
        [Route("api/profilesRWD")]
        [HttpGet]
        public IHttpActionResult GetProfilesRWD(int page = 1, string keyword = "", string tag = "")
        {
            var Counselors = _db.Features.AsQueryable();

            //搜尋姓名
            if (!string.IsNullOrEmpty(keyword))
            {
                page = 1;
                Counselors = Counselors.Where(x => x.MyCounselor.Name.Contains(keyword));
            }

            //篩選諮商主題 (前端傳入 ?tag=126)
            if (!string.IsNullOrEmpty(tag))
            {
                int[] fieldIds = new int[tag.Length];
                try
                {
                    for (int i = 0; i < tag.Length; i++)
                    {
                        fieldIds[i] = int.Parse(tag[i].ToString());
                    }
                    page = 1;
                    Counselors = Counselors.Where(x => fieldIds.Contains(x.FieldId));
                }
                catch
                {
                    return BadRequest("數值錯誤");
                }
            }

            //沒有使用篩選功能的總筆數
            var CounselorNum = Counselors
                .Select(x => x.CounselorId)
                .Distinct()
                .Count();

            //總頁數
            int pageNum = 0;
            int pageSize = 5; //手機版一頁只有5筆資料
            if (CounselorNum % pageSize == 0)
                pageNum = CounselorNum / pageSize;
            else
                pageNum = CounselorNum / pageSize + 1;

            ViewModel.SearchingCounselors data = new ViewModel.SearchingCounselors();
            data.TotalPageNum = pageNum;
            data.CounselorsData = new List<ViewModel.CounselorsData>();

            var counsleorList = Counselors
                .Select(x => new
                {
                    x.MyCounselor.Id,
                    x.MyCounselor.Photo,
                    x.MyCounselor.Name,
                    x.MyCounselor.SellingPoint,
                    x.MyCounselor.SelfIntroduction
                })
                .Distinct()
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();


            //照片存取位置
            string path = "https://pi.rocket-coding.com/upload/headshot/";
            foreach (var item in counsleorList)
            {
                ViewModel.CounselorsData counselorsData = new ViewModel.CounselorsData();
                counselorsData.Id = item.Id;
                counselorsData.Name = item.Name;
                counselorsData.SellingPoint = item.SellingPoint;
                counselorsData.SelfIntroduction = item.SelfIntroduction;
                counselorsData.Photo = path + item.Photo;

                data.CounselorsData.Add(counselorsData);
            }

            ApiResponse result = new ApiResponse { };
            result.Success = true;
            if (counsleorList.Any())
            {
                result.Message = "成功取得諮商師總覽";
                result.Data = data;
            }
            else
            {
                result.Message = "找不到符合篩選條件的諮商師";
                result.Data = null;
            }
            return Ok(result);
        }

        /// <summary>
        /// 取得特定諮商師頁面
        /// </summary>
        /// <param name="id">諮商師編號</param>
        /// <returns></returns>
        [Route("api/profile")]
        [HttpGet]
        public IHttpActionResult GetProfile(int id)
        {
            // 諮商師基本資料
            var counselorData = _db.Counselors
                .Where(x => x.Id == id)
                .FirstOrDefault();

            //照片存取位置
            string path = "https://pi.rocket-coding.com/upload/headshot/";

            if (counselorData == null)
                return NotFound();
            else
            {
                var data = new
                {
                    Photo = path + counselorData.Photo,
                    Name = counselorData.Name,
                    FieldTags = _db.Features
                        .Where(x => x.CounselorId == id).Select(x => x.MyField.Field).ToArray(),
                    SelfIntroduction = counselorData.SelfIntroduction,
                    CertNumber = counselorData.CertNumber,
                    VideoLink = counselorData.VideoLink,
                    Fields = _db.Features
                    .Where(x => x.CounselorId == id).GroupBy(x => x.FieldId).ToList()
                    .Select(x => new
                    {
                        Field = _db.Features.Where(y => y.CounselorId == id && y.FieldId == x.Key).Select(y => y.MyField.Field).FirstOrDefault(),

                        Features = _db.Features.Where(y => y.CounselorId == id && y.FieldId == x.Key).ToList().Select(y => new string[5] { y.Feature1, y.Feature2, y.Feature3, y.Feature4, y.Feature5 }).FirstOrDefault(),

                        Courses = _db.Products
                        .Where(y => y.CounselorId == id && y.FieldId == x.Key && y.Availability == true).Select(y => new { y.Item, y.Price }).ToList()
                    })
                };

                ApiResponse result = new ApiResponse { };
                result.Success = true;
                result.Message = "成功取得諮商師頁面";
                result.Data = data;
                return Ok(result);
            }
        }

        /// <summary>
        /// 加入購物車(手刀預約)
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/cart")]
        [JwtAuthFilter]
        public IHttpActionResult PostCart(ViewModel_U.Product view)
        {
            var userToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int userId = (int)userToken["Id"];

            var findProduct = _db.Products
                .Where(c => c.CounselorId == view.CounselorId && c.FieldId == view.FieldId && c.Item == view.Item)
                .FirstOrDefault();

            if (findProduct == null)
                return BadRequest("查無此課程");
            else
            {
                Cart cart = new Cart();
                cart.UersId = userId;
                cart.ProductId = findProduct.Id;

                _db.Carts.Add(cart);
                _db.SaveChanges();

                ApiResponse result = new ApiResponse { };
                result.Success = true;
                result.Message = "成功加入購物車";
                result.Data = null;
                return Ok(result);
            }
        }

        /// <summary>
        /// 取得購物車
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/cart")]
        [JwtAuthFilter]
        public IHttpActionResult GetCart()
        {
            var userToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int userId = (int)userToken["Id"];

            var findCart = _db.Carts
                .Where(c => c.UersId == userId)
                .ToList();

            if (!findCart.Any())
                return Ok("購物車是空的，趕緊手刀預約吧!");
            else
            {
                var totalAmount = _db.Carts
                    .Where(c => c.UersId == userId)
                    .Sum(x => x.Products.Price);

                var data = findCart.Select(x => new
                {
                    CartId = x.Id,
                    Counselor = x.Products.MyCounselor.Name,
                    Field = x.Products.MyField.Field,
                    Item = x.Products.Item,
                    Price = x.Products.Price
                }).ToList();

                ApiResponse result = new ApiResponse { };
                result.Success = true;
                result.Message = "成功取得購物車";
                result.Data = new { TotalAmount = totalAmount, CartList = data };
                return Ok(result);
            }
        }

        /// <summary>
        /// 刪除購物車特定商品
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [Route("api/cart")]
        [JwtAuthFilter]
        [HttpDelete]
        public IHttpActionResult DeleteCart(ViewModel_U.DeleteProduct view)
        {
            var userToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int userId = (int)userToken["Id"];

            var cartItem = _db.Carts
               .Where(c => c.UersId == userId && c.Id == view.CartId)
               .FirstOrDefault();

            if (cartItem == null)
                return BadRequest("購物車中無此課程商品");
            else
            {
                _db.Carts.Remove(cartItem);
                _db.SaveChanges();
            }

            ApiResponse result = new ApiResponse { };
            result.Success = true;
            result.Message = "成功刪除此課程商品";
            result.Data = null;
            return Ok(result);
        }

        /// <summary>
        /// 結帳成立訂單
        /// </summary>
        /// <returns></returns>
        [Route("api/order")]
        [JwtAuthFilter]
        [HttpPost]
        public IHttpActionResult PostOrder()
        {
            int total = 0;
            StringBuilder sBuilder = new StringBuilder();
            var userToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int userId = (int)userToken["Id"];
            string userName = (string)userToken["Name"];

            var cartItems = _db.Carts
               .Where(c => c.UersId == userId)
               .ToArray();

            if (!cartItems.Any())
                return BadRequest("購物車無任何課程商品");
            else
            {
                //建立訂單
                OrderRecord order = new OrderRecord();
                foreach (var item in cartItems)
                {


                    switch (userId.ToString().Length)
                    {
                        case 1:
                            order.OrderNum = $"0000{userId}{DateTime.Now.ToString("yyyyMMddHHmm")}";
                            break;
                        case 2:
                            order.OrderNum = $"000{userId}{DateTime.Now.ToString("yyyyMMddHHmm")}";
                            break;
                        case 3:
                            order.OrderNum = $"00{userId}{DateTime.Now.ToString("yyyyMMddHHmm")}";
                            break;
                    }
                    order.OrderDate = DateTime.Now;
                    order.UserName = userName;
                    order.CounselorName = item.Products.MyCounselor.Name;

                    order.Field = item.Products.MyField.Field;
                    order.Item = item.Products.Item;
                    order.Quantity = item.Products.Quantity;
                    order.Price = item.Products.Price;
                    sBuilder.Append($"{order.CounselorName}    {order.Price}\n");
                    total += order.Price;
                    order.OrderStatus = "未付款";

                    _db.OrderRecords.Add(order);
                    _db.SaveChanges();
                }

                // 整理金流串接資料
                // 加密用金鑰
                string hashKey = "1jUogKMU7sfOyJBtARgJzUfCd3NzFlIS";
                string hashIV = "CUUuIrArgfNETY1P";

                // 金流接收必填資料
                string merchantID = "MS148623457";
                string tradeInfo = "";
                string tradeSha = "";
                string version = "2.0"; // 參考文件串接程式版本

                // tradeInfo 內容，導回的網址都需為 https 
                string respondType = "JSON"; // 回傳格式
                string timeStamp = ((int)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds).ToString();
                string merchantOrderNo = order.OrderNum; // 底線後方為訂單ID，解密比對用，不可重覆(規則參考文件)
                string amt = "訂單金額";
                amt = total.ToString();
                string itemDesc = "商品資訊";
                itemDesc = sBuilder.ToString();
                string tradeLimit = "600"; // 交易限制秒數
                string notifyURL = @"http://pi.rocket-coding.com/api/getPaymentData"; // NotifyURL 填後端接收藍新付款結果的 API 位置，如 : /api/users/getpaymentdata
                string returnURL = "https://pi-rocket-coding.vercel.app/api/bluepay/return";  // 前端可用 Status: SUCCESS 來判斷付款成功，網址夾帶可拿來取得活動內容
                User user = _db.Users.Where(x => x.Id == userId).FirstOrDefault();
                string email = user.Account; // 通知付款完成用
                string loginType = "0"; // 0不須登入藍新金流會員

                // 將 model 轉換為List<KeyValuePair<string, string>>
                List<KeyValuePair<string, string>> tradeData = new List<KeyValuePair<string, string>>() {
        new KeyValuePair<string, string>("MerchantID", merchantID),
        new KeyValuePair<string, string>("RespondType", respondType),
        new KeyValuePair<string, string>("TimeStamp", timeStamp),
        new KeyValuePair<string, string>("Version", version),
        new KeyValuePair<string, string>("MerchantOrderNo", merchantOrderNo),
        new KeyValuePair<string, string>("Amt", amt),
        new KeyValuePair<string, string>("ItemDesc", itemDesc),
        new KeyValuePair<string, string>("TradeLimit", tradeLimit),
        new KeyValuePair<string, string>("NotifyURL", notifyURL),
        new KeyValuePair<string, string>("ReturnURL", returnURL),
        new KeyValuePair<string, string>("Email", email),
        new KeyValuePair<string, string>("LoginType", loginType)
    };

                // 將 List<KeyValuePair<string, string>> 轉換為 key1=Value1&key2=Value2&key3=Value3...
                var tradeQueryPara = string.Join("&", tradeData.Select(x => $"{x.Key}={x.Value}"));
                // AES 加密
                tradeInfo = CryptoUtil.EncryptAESHex(tradeQueryPara, hashKey, hashIV);
                // SHA256 加密
                tradeSha = CryptoUtil.EncryptSHA256($"HashKey={hashKey}&{tradeInfo}&HashIV={hashIV}");

                //刪除已付款購物車商品
                _db.Carts.RemoveRange(cartItems);
                _db.SaveChanges();
                // 送出金流串接用資料，給前端送藍新用
                return Ok(new
                {
                    Status = true,
                    PaymentData = new
                    {
                        MerchantID = merchantID,
                        TradeInfo = tradeInfo,
                        TradeSha = tradeSha,
                        Version = version
                    }
                });





            }


        }
        /// <summary>
        /// 結帳成立訂單
        /// </summary>
        /// <returns></returns>
        [Route("api/GetOrderSuccess")]
        [HttpGet]
        public IHttpActionResult GetOrderSuccess(string merchantOrderNo= "00009202304161757")
        {
            

            //string paramValue = HttpContext.Current.Request.QueryString["merchantOrderNo"];
            string paramValue = "00009202304161757";
            OrderRecord orderRecord = _db.OrderRecords.Where(x => x.OrderNum == paramValue).FirstOrDefault();
            ApiResponse result = new ApiResponse();
/*
            if (paramValue != null)
            {
                return Redirect("https://pi-rocket-coding-i2eiimv5q-roceil.vercel.app/");
            }
*/
            if(orderRecord == null)
            {
                result.Success = true;
                result.Message = "沒有訂單紀錄";
            }
            else
            {
                result.Success = true;
                result.Message = "有訂單紀錄";
                result.Data = new { OrderStatus=orderRecord.OrderStatus };
            }

            if(orderRecord.OrderStatus == "已成立")
            {
                return Ok(result);
                return Redirect("https://pi-rocket-coding-i2eiimv5q-roceil.vercel.app/usercenter/reservation");
            }
            else if(orderRecord.OrderStatus == "未付款") 
            {
                return Ok(result);
                return Redirect("https://www.google.com/");
            }
            else
            {
                return Ok(result);
                return Redirect("https://pi-rocket-coding-i2eiimv5q-roceil.vercel.app/");
            }

        }
        



    /// <summary>
    /// 取得預約管理明細 (個案)
    /// </summary>
    /// <param name="status">預約單狀態</param>
    /// <param name="page">頁數</param>
    /// <returns></returns>
        [HttpGet]
        [Route("api/apptRecords")]
        [JwtAuthFilter]
        public IHttpActionResult GetApptRecords(string status, int page)
        {
            var userToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int userId = (int)userToken["Id"];
            string userName = (string)userToken["Name"];

            var findAppointment = _db.Appointments
                    .Where(x => x.MyOrder.UserName == userName && x.ReserveStatus == status)
                    .OrderByDescending(x => x.OrderId)
                    .ToList();

            if (!findAppointment.Any())
                return BadRequest("尚無訂單紀錄");
            else
            {
                ApiResponse result = new ApiResponse { };
                result.Success = true;
                if (status == "待預約")
                {
                    var dataList = findAppointment.Select(x => new
                    {
                        OrderId = x.OrderId,
                        AppointmentId = x.Id,
                        Counselor = x.MyOrder.CounselorName,
                        Field = x.MyOrder.Field,
                    }).ToList();

                    //分頁功能
                    int pageNum = 0;
                    int pageSize = 5;
                    if (dataList.Count() % pageSize == 0)
                        pageNum = dataList.Count() / pageSize;
                    else
                        pageNum = dataList.Count() / pageSize + 1;

                    var pagination = dataList
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    result.Message = "成功取得待預約明細";
                    result.Data = new
                    {
                        TotalPageNum = pageNum,
                        List = pagination
                    };
                    return Ok(result);
                }
                else if (status == "待回覆" || status == "已成立" || status == "已完成" || status == "已取消")
                {
                    var dataList = findAppointment.Select(x => new
                    {
                        OrderId = x.OrderId,
                        AppointmentId = x.Id,
                        Counselor = x.MyOrder.CounselorName,
                        Field = x.MyOrder.Field,
                        Time = x.AppointmentTime
                    }).ToList();

                    //分頁功能
                    int pageNum = 0;
                    int pageSize = 5;
                    if (dataList.Count() % pageSize == 0)
                        pageNum = dataList.Count() / pageSize;
                    else
                        pageNum = dataList.Count() / pageSize + 1;

                    var pagination = dataList
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    switch (status)
                    {
                        case "待回覆":
                            result.Message = "成功取得待回覆明細";
                            break;
                        case "已成立":
                            result.Message = "成功取得已成立明細";
                            break;
                        case "已完成":
                            result.Message = "成功取得已完成明細";
                            break;
                        case "已取消":
                            result.Message = "成功取得已取消明細";
                            break;
                    }
                    result.Data = new
                    {
                        TotalPageNum = pageNum,
                        List = pagination
                    };
                    return Ok(result);
                }
                else
                    return BadRequest("無此狀態的明細");
            }
        }

        /// <summary>
        /// 選取預約時段
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/apptTime")]
        [JwtAuthFilter]
        public IHttpActionResult PostApptTime(ViewModel_U.AppointmentTime view)
        {
            var userToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int userId = (int)userToken["Id"];
            string userName = (string)userToken["Name"];

            var findAppointment = _db.Appointments
                .Where(x => x.MyOrder.UserName == userName && x.Id == view.AppointmentId)
                .FirstOrDefault();

            if (findAppointment == null)
                return BadRequest("查無此筆預約紀錄");
            else
            {
                int year = int.Parse(view.DateTimeValue.Year);
                int month = int.Parse(view.DateTimeValue.Month);
                int day = int.Parse(view.DateTimeValue.Day);
                int hour = int.Parse(view.DateTimeValue.Hour.Split(':')[0]);
                DateTime dateTimeValue = new DateTime(year, month, day, hour, 00, 0);

                findAppointment.AppointmentTime = dateTimeValue;
                findAppointment.ReserveStatus = "待回覆";
                _db.SaveChanges();
            }

            ApiResponse result = new ApiResponse { };
            result.Success = true;
            result.Message = "預約完成，請待諮商師接收預約";
            result.Data = null;
            return Ok(result);
        }

        /// <summary>
        /// 修改預約時段
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("api/apptTime")]
        [JwtAuthFilter]
        public IHttpActionResult PutApptTime(ViewModel_U.AppointmentTime view)
        {
            var userToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int userId = (int)userToken["Id"];
            string userName = (string)userToken["Name"];

            var findAppointment = _db.Appointments
                .Where(x => x.MyOrder.UserName == userName && x.Id == view.AppointmentId)
                .FirstOrDefault();

            if (findAppointment == null)
                return BadRequest("查無此筆預約紀錄");
            else
            {
                int year = int.Parse(view.DateTimeValue.Year);
                int month = int.Parse(view.DateTimeValue.Month);
                int day = int.Parse(view.DateTimeValue.Day);
                int hour = int.Parse(view.DateTimeValue.Hour.Split(':')[0]);
                DateTime dateTimeValue = new DateTime(year, month, day, hour, 00, 0);

                findAppointment.AppointmentTime = dateTimeValue;
                findAppointment.ReserveStatus = "待回覆";
                _db.SaveChanges();
            }

            ApiResponse result = new ApiResponse { };
            result.Success = true;
            result.Message = "成功修改預約時段";
            result.Data = null;
            return Ok(result);
        }

        /// <summary>
        /// 取得預約管理明細 (諮商師)
        /// </summary>
        /// <param name="status">預約單狀態</param>
        /// <param name="page">頁數</param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/usersAppts")]
        [JwtAuthFilter]
        public IHttpActionResult GetUsersAppts(string status, int page)
        {
            var counselorToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int counselorId = (int)counselorToken["Id"];
            string counselorName = (string)counselorToken["Name"];

            var findAppointment = _db.Appointments
                .Where(x => x.MyOrder.CounselorName == counselorName && x.ReserveStatus == status)
                .OrderByDescending(x => x.OrderId)
                .ToList();

            if (!findAppointment.Any())
                return BadRequest("尚無訂單紀錄");
            else
            {
                if (status != "待回覆" && status == "已成立" && status == "已取消")
                    return BadRequest("無此狀態的明細");
                else
                {
                    var dataList = findAppointment.Select(x => new
                    {
                        OrderId = x.OrderId,
                        AppointmentId = x.Id,
                        User = x.MyOrder.UserName,
                        Field = x.MyOrder.Field,
                        Time = x.AppointmentTime
                    }).ToList();

                    //分頁功能
                    int pageNum = 0;
                    int pageSize = 5;
                    if (dataList.Count() % pageSize == 0)
                        pageNum = dataList.Count() / pageSize;
                    else
                        pageNum = dataList.Count() / pageSize + 1;

                    var pagination = dataList
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    ApiResponse result = new ApiResponse { };
                    result.Success = true;
                    switch (status)
                    {
                        case "待回覆":
                            result.Message = "成功取得待回覆明細";
                            break;
                        case "已成立":
                            result.Message = "成功取得已成立明細";
                            break;
                        case "已取消":
                            result.Message = "成功取得已取消明細";
                            break;
                    }
                    result.Data = new
                    {
                        TotalPageNum = pageNum,
                        List = pagination
                    };
                    return Ok(result);
                }
            }
        }

        /// <summary>
        /// 接受個案預約
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/acceptAppt")]
        [JwtAuthFilter]
        public IHttpActionResult PostApptStatus(ViewModel_C.Appt view)
        {
            var counselorToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int counselorId = (int)counselorToken["Id"];
            string counselorName = (string)counselorToken["Name"];

            var findAppointment = _db.Appointments
                .Where(x => x.MyOrder.CounselorName == counselorName && x.Id == view.AppointmentId)
                .FirstOrDefault();

            if (findAppointment == null)
                return BadRequest("查無此筆預約紀錄");
            else
            {
                findAppointment.ReserveStatus = "已成立";
                _db.SaveChanges();
            }

            ApiResponse result = new ApiResponse { };
            result.Success = true;
            result.Message = "已成立此筆預約";
            result.Data = null;
            return Ok(result);
        }

        /// <summary>
        /// 完成訂單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/finishAppt")]
        [JwtAuthFilter]
        public IHttpActionResult PostApptStatus(ViewModel_U.AppointmentTime view)
        {
            var userToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int userId = (int)userToken["Id"];
            string userName = (string)userToken["Name"];

            var findAppointment = _db.Appointments
                .Where(x => x.MyOrder.UserName == userName && x.Id == view.AppointmentId)
                .FirstOrDefault();

            if (findAppointment == null)
                return BadRequest("查無此筆預約紀錄");
            else
            {
                findAppointment.ReserveStatus = "已完成";
                _db.SaveChanges();
            }

            ApiResponse result = new ApiResponse { };
            result.Success = true;
            result.Message = "完成訂單";
            result.Data = null;
            return Ok(result);
        }
    }
}
