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
    [OpenApiTag("Users", Description = "個案操作功能")]
    public class UsersController : ApiController
    {
        PiDbContext _db = new PiDbContext();

        /// <summary>
        /// 取得個案基本資料
        /// </summary>
        /// <returns></returns>
        [Route("api/users")]
        [JwtAuthFilter]
        [HttpGet]
        public IHttpActionResult GetUsers()
        {
            var userToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int userId = (int)userToken["Id"];
            var data = _db.Users
                .Where(x => x.Id == userId)
                .Select(x => new
                {
                    Account = x.Account,
                    Name = x.Name,
                    BirthDate = x.BirthDate,
                    Sex = x.Sex,
                });

            ApiResponse result = new ApiResponse { };
            result.Success = true;
            result.Message = "成功取得個案基本資料";
            result.Data = data;
            return Ok(result);
        }

        /// <summary>
        /// 修改個案基本資料
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [Route("api/users")]
        [JwtAuthFilter]
        [HttpPut]
        public IHttpActionResult PutCounselors(ViewModel_U.Profile view)
        {
            if (view.Name == null)
                return BadRequest("姓名欄必填");
            else
            {
                var userToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
                int userId = (int)userToken["Id"];
                var haveUser = _db.Users
                .Where(x => x.Id == userId).FirstOrDefault();

                if (haveUser != null)
                {
                    haveUser.Name = view.Name;

                    _db.SaveChanges();

                    ApiResponse result = new ApiResponse { };
                    result.Success = true;
                    result.Message = "成功修改個案基本資料";
                    result.Data = null;
                    return Ok(result);
                }
                else
                    return BadRequest("無此帳號");
            }
        }
    }

}
