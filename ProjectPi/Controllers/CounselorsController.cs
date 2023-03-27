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
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("api/counselors/{id}")]
        [HttpGet]
        public IHttpActionResult GetCounselors(int id)
        {
            var data = _db.Counselors
                .Where(x => x.Id == id)
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

    }
}
