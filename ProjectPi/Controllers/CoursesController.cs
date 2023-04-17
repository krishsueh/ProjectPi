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
    [OpenApiTag("Courses", Description = "諮商師課程資訊")]
    public class CoursesController : ApiController
    {
        PiDbContext _db = new PiDbContext();

        /// <summary>
        /// 新增/修改課程資訊
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [Route("api/courses")]
        [JwtAuthFilter]
        [HttpPost]
        public IHttpActionResult PostCourses(ViewModel_C.Course view)
        {
            if (!ModelState.IsValid)
                return BadRequest("欄位驗證有誤");

            var counselorToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int counselorId = (int)counselorToken["Id"];

            var hasProduct = _db.Products
                .Where(c => c.CounselorId == counselorId && c.FieldId == view.FieldId)
                .FirstOrDefault();
            var hasFeature = _db.Features
                .Where(c => c.CounselorId == counselorId && c.FieldId == view.FieldId)
                .FirstOrDefault();

            if (hasProduct != null) //已新增過該專業領域的課程資訊 -> 修改
            {
                for (int i = 0; i < view.Courses.Length; i++)
                {
                    var item = view.Courses[i];
                    var findItem = _db.Products
                        .Where(c => c.CounselorId == counselorId && c.FieldId == view.FieldId && c.Item == item.Item)
                        .FirstOrDefault();

                    findItem.Price = view.Courses[i].Price;
                    findItem.Availability = view.Courses[i].Availability;
                    _db.SaveChanges();
                }

                hasFeature.Feature1 = view.Features.Feature1;
                hasFeature.Feature2 = view.Features.Feature2;
                hasFeature.Feature3 = view.Features.Feature3;
                hasFeature.Feature4 = view.Features.Feature4;
                hasFeature.Feature5 = view.Features.Feature5;
                _db.SaveChanges();

                ApiResponse result = new ApiResponse { };
                result.Success = true;
                result.Message = "成功修改課程資訊";
                result.Data = null;
                return Ok(result);
            }
            else //從來沒有新增過該專業領域的課程資訊 -> 新增
            {
                for (int i = 0; i < view.Courses.Length; i++)
                {
                    var item = view.Courses[i];
                    Product product = new Product();
                    product.CounselorId = counselorId;
                    product.FieldId = view.FieldId;
                    product.Item = item.Item;
                    product.Quantity = item.Quantity;
                    product.Price = item.Price;
                    product.Availability = item.Availability;

                    _db.Products.Add(product);
                    _db.SaveChanges();
                }

                Feature feature = new Feature();
                feature.CounselorId = counselorId;
                feature.FieldId = view.FieldId;
                feature.Feature1 = view.Features.Feature1;
                feature.Feature2 = view.Features.Feature2;
                feature.Feature3 = view.Features.Feature3;
                feature.Feature4 = view.Features.Feature4;
                feature.Feature5 = view.Features.Feature5;

                _db.Features.Add(feature);
                _db.SaveChanges();

                ApiResponse result = new ApiResponse { };
                result.Success = true;
                result.Message = "成功新增課程資訊";
                result.Data = null;
                return Ok(result);
            }
        }

        /// <summary>
        /// 刪除課程資訊
        /// </summary>
        /// <param name="id">專長領域的Id</param>
        /// <returns></returns>
        [Route("api/courses")]
        [JwtAuthFilter]
        [HttpDelete]
        public IHttpActionResult DeleteCourses(int id)
        {
            var counselorToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int counselorId = (int)counselorToken["Id"];

            var hasProduct = _db.Products
                .Where(c => c.CounselorId == counselorId && c.FieldId == id)
                .ToList();
            var hasFeature = _db.Features
                .Where(c => c.CounselorId == counselorId && c.FieldId == id)
                .FirstOrDefault();

            if (!hasProduct.Any() || hasFeature == null)
            {
                return BadRequest("找不到課程資訊");
            }
            else
            {
                _db.Products.RemoveRange(hasProduct);
                _db.Features.Remove(hasFeature);
                _db.SaveChanges();
            }

            ApiResponse result = new ApiResponse { };
            result.Success = true;
            result.Message = "成功刪除課程資訊";
            result.Data = null;
            return Ok(result);
        }

        /// <summary>
        /// 取得課程資訊
        /// </summary>
        /// <returns></returns>
        [Route("api/courses")]
        [JwtAuthFilter]
        [HttpGet]
        public IHttpActionResult GetCourses()
        {
            var counselorToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int counselorId = (int)counselorToken["Id"];

            var hasProduct = _db.Products
                .Where(c => c.CounselorId == counselorId)
                .ToList();

            //使用 if (hasProduct == null) 來判斷該 List 是否為空是不正確的
            //因為即使查詢沒有回傳任何資料，hasProduct 仍然是一個空的 List 物件，而不是 null
            if (!hasProduct.Any())
            {
                return BadRequest("尚未新增課程資訊");
            }
            else
            {
                //"FieldIds": [value1, value2,...]
                var fieldIds = _db.Products
                    .Where(x => x.CounselorId == counselorId)
                    .Select(x => x.FieldId)
                    .Distinct()
                    .ToArray();

                //GroupBy 寫法
                var data = new
                {
                    FieldIds = fieldIds,
                    Courses = _db.Products
                    .Where(x => x.CounselorId == counselorId)
                    .GroupBy(x => x.FieldId)
                    .Select(x => new
                    {
                        FieldId = x.Key,
                        
                        Course = x.Select(y => new
                        {
                            Item = y.Item,
                            Quantity = y.Quantity,
                            Price = y.Price,
                            Availability = y.Availability
                        }).ToList(),

                        Feature = _db.Features
                            .Where(y => y.CounselorId == counselorId && y.FieldId == x.Key)
                            .Select(y => new List<string> { y.Feature1, y.Feature2, y.Feature3, y.Feature4, y.Feature5 })
                            .FirstOrDefault()
                    })
                    .ToList()
                };

                ////另一種寫法：
                //var data = new
                //{
                //    FieldIds = fieldIds,
                //    Courses = fieldIds.Select(fieldId => new
                //    {
                //        FieldId = fieldId,
                //        Course = _db.Products
                //        .Where(x => x.CounselorId == counselorId && x.FieldId == fieldId)
                //        .Select(x => new
                //        {
                //            Item = x.Item,
                //            Quantity = x.Quantity,
                //            Price = x.Price,
                //            Availability = x.Availability
                //        }).ToList()
                //    }).ToList()
                //};

                ApiResponse result = new ApiResponse { };
                result.Success = true;
                result.Message = "成功取得課程資訊";
                result.Data = data;
                return Ok(result);
            }
        }
    }
}
