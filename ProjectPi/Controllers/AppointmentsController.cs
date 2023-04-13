using NSwag.Annotations;
using ProjectPi.Models;
using ProjectPi.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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

            if (counsleorList.Any())
            {
                ApiResponse result = new ApiResponse { };
                result.Success = true;
                result.Message = "成功取得諮商師總覽";
                result.Data = data;
                return Ok(result);

            }
            else
            {
                ApiResponse result = new ApiResponse { };
                result.Success = true;
                result.Message = "找不到符合篩選條件的諮商師";
                result.Data = null;
                return Ok(result);
            }

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

            if (counsleorList.Any())
            {
                ApiResponse result = new ApiResponse { };
                result.Success = true;
                result.Message = "成功取得諮商師總覽";
                result.Data = data;
                return Ok(result);

            }
            else
            {
                ApiResponse result = new ApiResponse { };
                result.Success = true;
                result.Message = "找不到符合篩選條件的諮商師";
                result.Data = null;
                return Ok(result);
            }

        }

        /// <summary>
        /// 取得特定諮商師頁面
        /// </summary>
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
            {
                return BadRequest("不存在此諮商師");
            }
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
            else if (findProduct.Price != view.Price)
                return BadRequest("此課程方案價格錯誤");
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

            var totalAmount = _db.Carts
                .Where(c => c.UersId == userId)
                .Sum(x => x.Products.Price);

            if (!findCart.Any())
                return BadRequest("購物車是空的，趕緊手刀預約吧!");
            else
            {
                var data = findCart.Select(x => new
                {
                    ProductId = x.ProductId,
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
               .Where(c => c.UersId == userId && c.ProductId == view.ProductId)
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
                foreach (var item in cartItems)
                {
                    //建立訂單
                    OrderRecord order = new OrderRecord();
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
                    order.OrderStatus = "已成立";

                    _db.OrderRecords.Add(order);
                    _db.SaveChanges();

                    //建立預約明細
                    var findOrders = _db.OrderRecords
                        .Where(c => c.UserName == userName)
                        .ToArray();

                    foreach (var orderItem in findOrders)
                    {
                        if (orderItem.Quantity == 1)
                        {
                            Appointment appointment1 = new Appointment();
                            appointment1.OrderId = orderItem.Id;
                            appointment1.ReserveStatus = "待預約";
                            _db.Appointments.Add(appointment1);
                            _db.SaveChanges();
                        }
                        else if (orderItem.Quantity == 3)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Appointment appointment3 = new Appointment();
                                appointment3.OrderId = orderItem.Id;
                                appointment3.ReserveStatus = "待預約";
                                _db.Appointments.Add(appointment3);
                                _db.SaveChanges();
                            }
                        }
                        else if (orderItem.Quantity == 5)
                        {
                            for (int i = 0; i < 5; i++)
                            {
                                Appointment appointment5 = new Appointment();
                                appointment5.OrderId = orderItem.Id;
                                appointment5.ReserveStatus = "待預約";
                                _db.Appointments.Add(appointment5);
                                _db.SaveChanges();
                            }
                        }



                        //switch (orderItem.Quantity)
                        //{
                        //    case 1:
                        //        appointment.OrderId = orderItem.Id;
                        //        appointment.ReserveStatus = "待預約";
                        //        _db.Appointments.Add(appointment);
                        //        _db.SaveChanges();
                        //        break;
                        //    case 3:
                        //        for (int i = 0; i < 3; i++)
                        //        {
                        //            appointment.OrderId = orderItem.Id;
                        //            appointment.ReserveStatus = "待預約";
                        //            _db.Appointments.Add(appointment);
                        //            _db.SaveChanges();
                        //        }
                        //        break;
                        //    case 5:
                        //        for (int i = 0; i < 5; i++)
                        //        {
                        //            appointment.OrderId = orderItem.Id;
                        //            appointment.ReserveStatus = "待預約";
                        //            _db.Appointments.Add(appointment);
                        //            _db.SaveChanges();
                        //        }
                        //        break;
                        //}
                    }
                }

                ////刪除已付款購物車商品
                //_db.Carts.RemoveRange(cartItems);
                //_db.SaveChanges();
            }

            ApiResponse result = new ApiResponse { };
            result.Success = true;
            result.Message = "訂單已成立，請至會員中心選擇預約時段";
            result.Data = null;
            return Ok(result);
        }

        ///// <summary>
        ///// 取得待預約紀錄
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //[Route("api/appointment")]
        //[JwtAuthFilter]
        //public IHttpActionResult GetWaitAppointment()
        //{
        //    var userToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
        //    int userId = (int)userToken["Id"];
        //    string userName = (string)userToken["Name"];

        //    var findAppointment = _db.OrderRecords
        //        .Where(c => c.UserName == userName)
        //        .GroupBy(c => c.OrderDate)
        //        .ToList();


        //    if (!findAppointment.Any())
        //        return BadRequest("尚無訂單紀錄");
        //    else
        //    {
        //        var data = findAppointment.Select(x => new
        //        {
        //            Counselor = x.Select(y => y.CounselorName).FirstOrDefault(),
        //            Field = x.Select(y => y.Field).FirstOrDefault(),
        //            Quantity = x.Select(y => y.Quantity).FirstOrDefault(),
        //        }).ToList();

        //        ApiResponse result = new ApiResponse { };
        //        result.Success = true;
        //        result.Message = "成功取得待預約紀錄";
        //        result.Data = data;
        //        return Ok(result);
        //    }
        //}


    }
}
