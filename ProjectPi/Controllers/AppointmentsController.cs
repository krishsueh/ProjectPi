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
                .Select(x => new
                {
                    x.Photo,
                    x.Name,
                    x.SelfIntroduction,
                    x.CertNumber,
                    x.VideoLink
                })
                .FirstOrDefault();

            if (counselorData == null)
            {
                return BadRequest("不存在此諮商師");
            }
            else
            {
                // 諮商師專業領域
                var counselorFields = _db.Features
                    .Where(x => x.CounselorId == id)
                    .Select(x => new
                    {
                        x.FieldId,
                        x.MyField.Field,
                        x.Feature1,
                        x.Feature2,
                        x.Feature3,
                        x.Feature4,
                        x.Feature5,
                    })
                    .ToList();

                //照片存取位置
                string path = "https://pi.rocket-coding.com/upload/headshot/";

                ViewModel.counselorProfile data = new ViewModel.counselorProfile();
                data.Photo = path + counselorData.Photo;
                data.Name = counselorData.Name;
                data.FieldTags = counselorFields.Select(x => x.Field).ToArray();
                data.SelfIntroduction = counselorData.SelfIntroduction;
                data.CertNumber = counselorData.CertNumber;
                data.VideoLink = counselorData.VideoLink;
                data.Fields = new List<ViewModel.Fields>();

                foreach (var fieldItem in counselorFields)
                {
                    ViewModel.Fields fields = new ViewModel.Fields();
                    fields.Field = fieldItem.Field;
                    fields.Features = new ViewModel.Features
                    {
                        Feature1 = fieldItem.Feature1,
                        Feature2 = fieldItem.Feature2,
                        Feature3 = fieldItem.Feature3,
                        Feature4 = fieldItem.Feature4,
                        Feature5 = fieldItem.Feature5
                    };
                    fields.Courses = new List<ViewModel.Courses>();

                    // 諮商師課程資訊
                    var counselorCourses = _db.Products
                        .Where(x => x.CounselorId == id && x.FieldId == fieldItem.FieldId && x.Availability == true)
                        .Select(x => new
                        {
                            x.Item,
                            x.Price
                        })
                        .ToList();

                    foreach (var courseItem in counselorCourses)
                    {
                        ViewModel.Courses courses = new ViewModel.Courses();
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
}
