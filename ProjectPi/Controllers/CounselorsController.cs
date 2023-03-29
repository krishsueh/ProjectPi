using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web.Http;
using NSwag.Annotations;
using ProjectPi.Models;
using ProjectPi.Security;

namespace ProjectPi.Controllers
{
    [OpenApiTag("Counselors", Description = "諮商師操作功能")]
    public class CounselorsController : ApiController
    {
        PiDbContext _db = new PiDbContext();

        /// <summary>
        /// 取得諮商師基本資料
        /// </summary>
        /// <returns></returns>
        [Route("api/counselors")]
        [JwtAuthFilter]
        [HttpGet]
        public IHttpActionResult GetCounselors()
        {
            var counselorToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int counselorId = (int)counselorToken["Id"];
            var data = _db.Counselors
                .Where(x => x.Id == counselorId)
                .Select(x => new
                {
                    Account = x.Account,
                    CounselorName = x.Name,
                    LicenseImg = x.LicenseImg,
                    CertNumber = x.CertNumber,
                    Photo = x.Photo,
                    SellingPoint = x.SellingPoint,
                    SelfIntroduction = x.SelfIntroduction,
                    VideoLink = x.VideoLink,
                    IsVideoOpen = x.IsVideoOpen,
                    AccountStatus = x.Validation
                });

            ApiResponse result = new ApiResponse { };
            result.Success = true;
            result.Message = "成功取得諮商師基本資料";
            result.Data = data;
            return Ok(result);
        }

        /// <summary>
        /// 修改諮商師基本資料
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [Route("api/counselors")]
        [JwtAuthFilter]
        [HttpPut]
        public IHttpActionResult PutCounselors(ViewModel_C.Profile view)
        {
            if (view.Name == null)
                return BadRequest("姓名欄必填");
            else if (view.LicenseImg == null)
                return BadRequest("請上傳執照");
            else
            {
                var counselorToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
                int counselorId = (int)counselorToken["Id"];
                var haveCounselor = _db.Counselors
                .Where(x => x.Id == counselorId).FirstOrDefault();

                if (haveCounselor != null)
                {
                    haveCounselor.Name = view.Name;
                    haveCounselor.LicenseImg = view.LicenseImg;
                    haveCounselor.Photo = view.Photo;
                    haveCounselor.SellingPoint = view.SellingPoint;
                    haveCounselor.SelfIntroduction = view.SelfIntroduction;
                    haveCounselor.VideoLink = view.VideoLink;
                    haveCounselor.IsVideoOpen = view.IsVideoOpen;

                    _db.SaveChanges();

                    ApiResponse result = new ApiResponse { };
                    result.Success = true;
                    result.Message = "成功修改諮商師基本資料";
                    result.Data = null;
                    return Ok(result);
                }
                else
                    return BadRequest("無此帳號");
            }
        }
    }
}
