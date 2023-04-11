using System;
using System.Collections.Generic;

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using ProjectPi.Models;
using RestSharp;

namespace ProjectPi.Controllers
{
    public class ZoomApiController : ApiController
    {
        /// <summary>
        /// 登入API
        /// </summary>
        /// <param name="Title">標題</param>
        /// <param name="startDateTime">開始時間</param>
        /// 時間格式範例: 2023-03-27T13:15:00
        /// <returns></returns>
        /// System.IdentityModel.Tokens.Jwt 套件 6.27.0
        /// RestSharp 106.11.7  記得要用舊版本!!!!!!!!
        [HttpPost]
        [Route("api/Zoom")]
        public IHttpActionResult PostLogin(string Title, DateTime startDateTime)
        {

            ApiResponse result = new ApiResponse();


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
            request.AddJsonBody(new { topic = Title, duration = "40", start_time = startDateTime, type = 2 });
            request.AddHeader("authorization", String.Format("Bearer {0}", tokenString));
            IRestResponse restResponse = client.Execute(request);
            HttpStatusCode statusCode = restResponse.StatusCode;
            int numericStatusCode = (int)statusCode;
            var jObject = JObject.Parse(restResponse.Content);

            result.Success = true;
            result.Message = "ZoomApi建立成功";
            result.Data = new { Host = (string)jObject["start_url"], Join = (string)jObject["join_url"], Code = Convert.ToString(numericStatusCode) };



            return Ok(result);

        }
    }

}
