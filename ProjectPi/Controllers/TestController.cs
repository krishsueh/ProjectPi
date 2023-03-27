using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProjectPi.Controllers
{
    public class TestController : ApiController
    {
        /// <summary>
        /// 跨域測試用
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/user/test")]
        public IHttpActionResult Test()
        {

            return Ok(new { Message = "OK" });
        }
    }
}
