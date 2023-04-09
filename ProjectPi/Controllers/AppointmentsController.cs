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
                .Select(x => new
                {
                    x.Photo, x.Name, x.SelfIntroduction, x.CertNumber, x.VideoLink
                })
                .FirstOrDefault() ;

            // 諮商師專業領域
            var counselorFields = _db.Features
                .Where(x => x.CounselorId == id)
                .Select(x => new
                {
                    x.FieldId, x.MyField.Field, x.Feature1, x.Feature2, x.Feature3, x.Feature4, x.Feature5,
                })
                .ToList();

            //照片存取位置
            string path = "https://pi.rocket-coding.com/upload/headshot/";

            ViewModel_U.counselorProfile data = new ViewModel_U.counselorProfile();
            data.Photo = path + counselorData.Photo;
            data.Name = counselorData.Name;
            data.SelfIntroduction = counselorData.SelfIntroduction;
            data.CertNumber = counselorData.CertNumber;
            data.VideoLink = counselorData.VideoLink;
            data.Fields = new List<ViewModel_U.Fields>();

            foreach (var fieldItem in counselorFields)
            {
                ViewModel_U.Fields fields = new ViewModel_U.Fields();
                fields.Field = fieldItem.Field;
                fields.Features = new ViewModel_U.Features
                {
                    Feature1 = fieldItem.Feature1,
                    Feature2 = fieldItem.Feature2,
                    Feature3 = fieldItem.Feature3,
                    Feature4 = fieldItem.Feature4,
                    Feature5 = fieldItem.Feature5
                };
                fields.Courses = new List<ViewModel_U.Courses>();

                // 諮商師課程資訊
                var counselorCourses = _db.Products
                    .Where(x => x.CounselorId == id && x.FieldId == fieldItem.FieldId && x.Availability == true)
                    .Select(x => new
                    {
                        x.Item, x.Price
                    })
                    .ToList();

                foreach (var courseItem in counselorCourses)
                {
                    ViewModel_U.Courses courses = new ViewModel_U.Courses();
                    courses.Item = courseItem.Item;
                    courses.Price = courseItem.Price;

                    fields.Courses.Add(courses);
                }

                data.Fields.Add(fields);
            }

            ApiResponse result = new ApiResponse { };
            result.Success = true;
            result.Message = "成功取得諮商師頁面";
            result.Data = data;
            return Ok(result);
        }
    }
}
