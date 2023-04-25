using NSwag.Annotations;
using ProjectPi.Models;
using ProjectPi.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Http;
using RestSharp;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;



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
                        .Where(x => x.CounselorId == id).OrderBy(x => x.FieldId).Select(x => x.MyField.Field).ToArray(),
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

            //照片存取位置
            string path = "https://pi.rocket-coding.com/upload/headshot/";

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
                    FieldImg = path + x.Products.MyField.FieldImg,
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
                    order.UserId = userId;
                    order.UserName = userName;
                    order.CounselorId = item.Products.MyCounselor.Id;
                    order.CounselorName = item.Products.MyCounselor.Name;

                    order.Field = item.Products.MyField.Field;
                    order.Item = item.Products.Item;
                    order.Quantity = item.Products.Quantity;
                    order.Price = item.Products.Price;
                    string sPrice = string.Format("{0:#,0}", order.Price);
                    sBuilder.Append($"{order.Field}    {order.Item}｜{sPrice}\n");
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
                List<KeyValuePair<string, string>> tradeData = new List<KeyValuePair<string, string>>()
                {
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

            var findAppointment = _db.Appointments
                    .Where(x => x.MyOrder.UserId == userId && x.ReserveStatus == status)
                    .OrderByDescending(x => x.OrderId)
                    .ToList();

            if (!findAppointment.Any())
                return Ok(new
                {
                    Success = true,
                    Message = "尚無紀錄",
                    Data = new
                    {
                        TotalPageNum = 0,
                        List = new object[0]
                    }
                });
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
                        CounselorId = x.MyOrder.CounselorId,
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
                        CounselorId = x.MyOrder.CounselorId,
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
        /// 選取/修改預約時段
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/apptTime")]
        [JwtAuthFilter]
        public IHttpActionResult PostApptTime(ViewModel_U.AppointmentTime view)
        {
            var userToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int userId = (int)userToken["Id"];

            var findAppointment = _db.Appointments
                .Where(x => x.MyOrder.UserId == userId && x.Id == view.AppointmentId)
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

                findAppointment.AppointmentTimeId = view.AppointmentTimeId;
                findAppointment.AppointmentTime = dateTimeValue;
                findAppointment.ReserveStatus = "待回覆";
                _db.SaveChanges();

                ApiResponse result = new ApiResponse { };
                result.Success = true;
                result.Message = "預約完成，請待諮商師接收預約";
                result.Data = null;
                return Ok(result);
            }
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

            var findAppointment = _db.Appointments
                .Where(x => x.MyOrder.CounselorId == counselorId && x.ReserveStatus == status)
                .OrderByDescending(x => x.OrderId)
                .ToList();

            if (!findAppointment.Any())
                return Ok(new
                {
                    Success = true,
                    Message = "尚無紀錄",
                    Data = new
                    {
                        TotalPageNum = 0,
                        List = new object[0]
                    }
                });
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

            var findAppointment = _db.Appointments
                .Where(x => x.MyOrder.CounselorId == counselorId && x.Id == view.AppointmentId)
                .FirstOrDefault();

            if (findAppointment == null)
                return BadRequest("查無此筆預約紀錄");
            else
            {
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var now = DateTime.Now;
                var apiSecret = "7S2JIaMSBmx32CLpYAVtZ3ThTQ897kplWlIM";
                byte[] symmetricKey = Encoding.ASCII.GetBytes(apiSecret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = "3eA43EJUTa2PXAXf2TjiBg",
                    Expires = now.AddSeconds(300),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256),
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                var client = new RestClient("https://api.zoom.us/v2/users/plowrain1328@gmail.com/meetings");
                var request = new RestRequest(Method.POST);
                request.RequestFormat = DataFormat.Json;
                request.AddJsonBody(new { topic = findAppointment.MyOrder.Field, duration = "40", start_time = findAppointment.AppointmentTime, type = 2 });
                request.AddHeader("authorization", String.Format("Bearer {0}", tokenString));
                IRestResponse restResponse = client.Execute(request);
                HttpStatusCode statusCode = restResponse.StatusCode;
                int numericStatusCode = (int)statusCode;
                var jObject = JObject.Parse(restResponse.Content);
                //result.Data = new { Host = (string)jObject["start_url"], Join = (string)jObject["join_url"], Code = Convert.ToString(numericStatusCode) };
                findAppointment.ZoomLink = (string)jObject["join_url"];


                var selectedTime = _db.Timetables.Where(x => x.Id == findAppointment.AppointmentTimeId).FirstOrDefault();
                selectedTime.Availability = false;
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
        /// 請求變更預約時間
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/reAppt")]
        [JwtAuthFilter]
        public IHttpActionResult PostReAppt(ViewModel_C.ReAppt view)
        {
            var counselorToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int counselorId = (int)counselorToken["Id"];
            string counselorName = (string)counselorToken["Name"];

            var hasAppointment = _db.Appointments
                .Where(c => c.Id == view.AppointmentId)
                .FirstOrDefault();

            if (hasAppointment == null)
                return BadRequest("查無此筆預約紀錄");
            else
            {
                // 官網首頁
                string indexPath = Url.Content("https://pi-rocket-coding.vercel.app");

                //登入頁面
                string loginPath = Url.Content("https://pi-rocket-coding.vercel.app/login");

                // 找出該筆訂單的用戶信箱
                var userEmail = _db.Users
                    .Where(c => c.Id == hasAppointment.MyOrder.UserId)
                    .Select(c => c.Account)
                    .FirstOrDefault();

                // 找出該筆訂單課程方案及預約時段
                var course = hasAppointment.MyOrder.Field;
                var apptDateTime = hasAppointment.AppointmentTime;

                // Google 發信帳號密碼
                string sendFrom = ConfigurationManager.AppSettings["SendFrom"];
                string password = ConfigurationManager.AppSettings["GmailPassword"];
                string sendTo = userEmail;
                string subject = "【拍拍】預約時段變更請求";
                string mailBody = @"<div class='container' style='width: 560px; margin: auto; border: 1px gray solid;'><div class='header'><h2 style = 'color: #424242; margin-left: 10px;'>拍拍</h2></div><div class='main' style='color: #424242; padding: 30px 30px;'><p>親愛的用戶您好：<br><br>您有一則預約時間的變更請求，請立即連繫您的諮商師，重新洽談諮商時間。<br><br>登入平台後，可點擊聊天室尋找您的諮商師。若您未曾與該諮商師建立過聊天視窗，請至該諮商師頁面的【我有問題】創建一個聊天視窗，謝謝您的配合。<br><br><br>諮商師姓名：" + counselorName + "<br><br>課程方案：" + course + "<br><br>預約時間：" + apptDateTime + "<br><br>原因：" + view.Reason + "</p><div class='btn' style='color: #424242; margin: 40px 0; border-radius: 53px; display: inline-block; background-color: #FFF6E2;'><a href = '" + loginPath + "' style='text-decoration: none; display: inline-block; padding: 10px 20px; color: black'>登入</a></div></div><div class='footer' style='color: #424242; background-color: #FFF6E2; padding: 20px 10px;'><p> 若您需要聯繫您的諮商師／個案用戶，請直接登入平台與您的諮商師／個案用戶聯繫。若需要客服人員協助，歡迎回覆此信件。</p><ul style = 'list-style: none; display: flex;' ><li><a href='" + indexPath + "' style='text-decoration: none; color: black;'>官方網站</a></li><li><span style = 'margin: 0 5px;' >|</ span ></li><li><a href='#' style='text-decoration: none; color: black;'>常見問題</a></li></ul><p>© 2023 Pi Life Limited.</p></div>";

                SendGmailMail(sendFrom, sendTo, subject, mailBody, password);

                ApiResponse result = new ApiResponse { };
                result.Success = true;
                result.Message = "Email已發送，請檢查信箱";
                result.Data = null;
                return Ok(result);
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

        /// <summary>
        /// 諮商師的會員中心的個案紀錄
        /// </summary>
        /// <returns></returns>
        [Route("api/AppointmentsLogs")]
        [JwtAuthFilter]
        [HttpGet]
        public IHttpActionResult GetAppointmentsLogs(int Page = 1 , string Name="")
        {
            int pageSize = 15;
            var userToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int myId = (int)userToken["Id"];
         
            Counselor counselor = _db.Counselors.Where(x => x.Id == myId).FirstOrDefault();
            ApiResponse result = new ApiResponse();
            if (counselor == null) return BadRequest("此諮商師不存在");

            var appointmentsWithOrder = _db.Appointments
                .Join(
                    _db.OrderRecords,
                    appointment => appointment.OrderId,
                    order => order.Id,
                    (appointment, order) => new { Appointment = appointment, Order = order }
                )
                .Where(joined => joined.Order.CounselorId == counselor.Id && joined.Appointment.ReserveStatus == "已成立")
                .GroupBy(join => join.Order.UserId)
                .Select(group => new
                {
                    UserId = group.Key,
                    AppointmentCount = group.Count(),
                    Appointments = group.Select(joined => new
                    {
                        //AppointmentId = joined.Appointment.Id,
                        UserName = joined.Order.UserName,


                    }).FirstOrDefault()
                })
                .OrderBy(x => x.UserId)
                .ToList();
            if (string.IsNullOrEmpty(Name))
            {
                appointmentsWithOrder = appointmentsWithOrder.Skip((Page - 1) * pageSize)
              .Take(pageSize)
              .ToList();
            }
            else
            {
                appointmentsWithOrder = appointmentsWithOrder.Where(x=>x.Appointments.UserName == Name).Skip((Page - 1) * pageSize)
              .Take(pageSize)
              .ToList();
            }
            /*
              .Skip((Page - 1) * pageSize)
              .Take(pageSize)
              .ToList();*/
            string photo = "https://pi.rocket-coding.com/upload/headshot/user_profile.svg";
            result.Success = true;
            result.Message = "取得個案紀錄";
            result.Data = new { Photo=photo, appointmentsWithOrder };
            if(appointmentsWithOrder == null)
            {
                result.Message = "無個案紀錄";
                result.Data = new { };
            }
            return Ok(result);
            //下面全部刪掉
            
        }

        /// <summary>
        /// 取得諮商列表紀錄
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [Route("api/AppointmentsRecordLogs")]
        [JwtAuthFilter]
        [HttpPost]
        public IHttpActionResult GetAppointmentsRecordLogs(AppointmentLogs_UserName view)
        {
            var userToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int userId = (int)userToken["Id"];
            Counselor counselor = _db.Counselors.Where(x => x.Id == userId).FirstOrDefault();
            ApiResponse result = new ApiResponse();
            if (!_db.Users.Where(x => x.Name == view.Name).Any()) return BadRequest("user名字錯誤");
            List<AppointmentLogs> appointmentLogsList = new List<AppointmentLogs>();
            List<Appointment> appointmentsList = new List<Appointment>();
            if (counselor == null) return BadRequest("id不存在");

            var appointmentsWithOrder = _db.Appointments
                .Join(
                    _db.OrderRecords,
                    appointment => appointment.OrderId,
                    order => order.Id,
                    (appointment, order) => new { Appointment = appointment, Order = order }
                )
                .Where(joined => joined.Order.CounselorId == counselor.Id && joined.Appointment.AppointmentTime != null && joined.Order.UserName == view.Name)
                .Select(joined => new
                {
                    AppointmentId = joined.Appointment.Id,
                    OrderNum = joined.Order.OrderNum,
                    UserName = joined.Order.UserName,
                    Field = joined.Order.Field,
                    CounselorName = joined.Order.CounselorName,
                    AppointmentTime = joined.Appointment.AppointmentTime,
                    ReserveStatus = joined.Appointment.ReserveStatus,
                    CounsellingRecord = joined.Appointment.CounsellingRecord,
                    RecordDate = joined.Appointment.RecordDate,
                    Star = joined.Appointment.Star,
                    Comment = joined.Appointment.Comment,
                    InitDate = joined.Appointment.InitDate
                })
                .OrderBy(joined => joined.AppointmentTime)
                .ToList();


            DateTime time = DateTime.Now;

            foreach (var item in appointmentsWithOrder)
            {
                AppointmentLogs appointmentLogs = new AppointmentLogs();

                appointmentLogs.AppointmentId = item.AppointmentId;
                appointmentLogs.Name = item.UserName;
                appointmentLogs.Field = item.Field;
                if (item.AppointmentTime != null)
                {
                    time = (DateTime)item.AppointmentTime;
                }
                appointmentLogs.AppointmentDate = time.ToString("yyyy/M/d");
                appointmentLogs.AppointmentTime = time.ToString("HH:mm");
                appointmentLogsList.Add(appointmentLogs);
            }
            //判斷有沒有已成立的課程
            if (!appointmentLogsList.Any())
            {
                result.Success = true;
                result.Message = "尚無任何紀錄 ";
                result.Data = new { appointmentLogsList };
                return Ok(result);
            }

            result.Success = true;
            result.Message = "取得諮商列表紀錄 ";
            result.Data = new { appointmentLogsList };
            return Ok(result);


        }

        /// <summary>
        /// 取得諮商列表紀錄
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [Route("api/AppointmentsRecord")]
        [JwtAuthFilter]
        [HttpPost]
        public IHttpActionResult GetAppointmentsRecord(AppointmentLogs_Id view)
        {
            ApiResponse result = new ApiResponse();
            Appointment appointment = _db.Appointments.Where(x => x.Id == view.AppointmentId).FirstOrDefault();
            AppointmentLogs_Record record = new AppointmentLogs_Record();

            //判斷有沒有已成立的課程
            if (appointment == null)
            {
                return BadRequest("參數錯誤，無此課程");
            }
            record.Name = appointment.MyOrder.UserName;
            record.Field = appointment.MyOrder.Field;
            record.AppointmentDate = ((DateTime)appointment.AppointmentTime).ToString("yyyy/M/d");
            if (appointment.RecordDate != null) record.LastRecordDate = ((DateTime)appointment.RecordDate).ToString("yyyy/M/d");
            else record.LastRecordDate = DateTime.Now.ToString("yyyy/M/d");
            record.RecordDate = DateTime.Now.ToString("yyyy/M/d");
            record.CounsellingRecord = appointment.CounsellingRecord;
            record.AppointmentId = appointment.Id;
            result.Success = true;
            result.Message = "取得諮商列表紀錄 ";
            result.Data = new { record };
            return Ok(result);
        }
        /// <summary>
        /// 修改諮商紀錄
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [Route("api/UpdateAppointmentsRecord")]
        [JwtAuthFilter]
        [HttpPut]
        public IHttpActionResult UpdateAppointmentsRecord(AppointmentLogs_UpdateRecod view)
        {
            ApiResponse result = new ApiResponse();
            Appointment appointment = _db.Appointments.Where(x => x.Id == view.AppointmentId).FirstOrDefault();
            //判斷有沒有已成立的課程
            if (appointment == null)
            {
                return BadRequest("參數錯誤，無此諮商");
            }
            appointment.RecordDate = DateTime.Now;
            appointment.CounsellingRecord = view.CounsellingRecord;
            _db.SaveChanges();

            result.Success = true;
            result.Message = "修改成功";
            return Ok(result);
        }

        /// <summary>
        /// 顯示評價
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [Route("api/GetAppointmentsCommit")]
        [JwtAuthFilter]
        [HttpGet]
        public IHttpActionResult GetAppointmentsCommit(int AppointmentId)
        {
            ApiResponse result = new ApiResponse();
            Appointment appointment = _db.Appointments.Where(x => x.Id == AppointmentId).FirstOrDefault();
            //判斷有沒有已成立的課程
            if (appointment == null)
            {
                return BadRequest("參數錯誤，無此諮商");
            }
            result.Success = true;
            result.Message = "顯示成功";
            result.Data = new { Star = appointment.Star, Comment = appointment.Comment };
            return Ok(result);
        }

        /// <summary>
        /// 儲存評價
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [Route("api/PostAppointmentsComment")]
        [JwtAuthFilter]
        [HttpPost]
        public IHttpActionResult GetAppointmentsCommit(AppointmentLogs_Comment view)
        {
            ApiResponse result = new ApiResponse();
            Appointment appointment = _db.Appointments.Where(x => x.Id == view.AppointmentId).FirstOrDefault();
            //判斷有沒有已成立的課程
            if (appointment == null)
            {
                return BadRequest("參數錯誤，無此諮商");
            }
            appointment.Star = view.Star;
            appointment.Comment = view.Comment;
            _db.SaveChanges();
            result.Success = true;
            result.Message = "修改成功";
            return Ok(result);
        }

    }
}
