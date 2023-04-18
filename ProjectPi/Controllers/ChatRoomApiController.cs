using NSwag.Annotations;
using ProjectPi.Models;
using ProjectPi.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Http;

namespace ProjectPi.Controllers
{
    [OpenApiTag("ChatRoom", Description = "聊天室")]
    public class ChatRoomApiController : ApiController
    {
        PiDbContext _db = new PiDbContext();
        /// <summary>
        /// 儲存聊天內容
        /// </summary>
        /// <param name="ChatRoom"></param>
        /// <response code="200">儲存成功</response>
        /// <returns></returns>
        [HttpPost]
        [Route("api/chatroom/chatlogs")]
        //[JwtAuthFilter]
        [SwaggerResponse(typeof(ApiResponse))]
        public IHttpActionResult PostChatlogs(ChatRoom ChatRoom)
        {
        
            ApiResponse result = new ApiResponse();

            ChatRoom _chatroom = new ChatRoom();
            try
            {
                _chatroom.CounselorId = ChatRoom.CounselorId;
                _chatroom.UserId = ChatRoom.UserId;
                _chatroom.Content = ChatRoom.Content;
                _chatroom.Type = ChatRoom.Type;
                _chatroom.InitDate = DateTime.Now;
                
                if (_chatroom.Type == "send")
                {
                    _chatroom.UserRead = true;
                    _chatroom.CounselorRead = false;
                }
                else if (_chatroom.Type == "accept")
                {
                    _chatroom.UserRead = false;
                    _chatroom.CounselorRead = true;
                }
                else
                {
                    _chatroom.UserRead = false;
                    _chatroom.CounselorRead = false;
                }
                result.Success = true;
                result.Message = "聊天訊息存入成功";
                _db.ChatRooms.Add(_chatroom);
                _db.SaveChanges();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("ERROR :" + ex);
            }

        }

        /// <summary>
        /// 取得聊天內容
        /// </summary>
        /// <response code="200">取得全部聊天內容</response>
        /// <returns></returns>
        [HttpGet]
        //[JwtAuthFilter]
        [Route("api/chatroom/GetChatlogs")]
        [SwaggerResponse(typeof(ApiResponse))]
        public IHttpActionResult GetChatlogs(int CounselorId, int UserId, string UserType)
        {
            
            ApiResponse result = new ApiResponse();

            if (!_db.ChatRooms.Where(x => x.CounselorId == CounselorId).Any())
            {
                result.Success = true;
                result.Message = "沒有聊天訊息";
                result.Data = new { };
                return Ok(result);
            }
            if (!_db.ChatRooms.Where(x => x.UserId == UserId).Any())
            {
                result.Success = true;
                result.Message = "沒有聊天訊息";
                return Ok(result);
            }
            //修改已讀
            if (UserType.ToLower() == "user")
            {
                _db.ChatRooms
                    .Where(c => c.UserId == UserId && c.CounselorId == CounselorId)
                    .ToList()
                    .ForEach(c => c.UserRead = true);
            }
            else if (UserType.ToLower() == "counselor")
            {
                _db.ChatRooms
                    .Where(c => c.UserId == UserId && c.CounselorId == CounselorId)
                    .ToList()
                    .ForEach(c => c.CounselorRead = true);
            }
            _db.SaveChanges();

            var chatlogList = _db.ChatRooms.Where(x => (x.CounselorId == CounselorId && x.UserId == UserId)).OrderBy(x => x.InitDate).Select(x => new { x.CounselorId, x.UserId, x.Content, x.Type, x.InitDate });

            if (!chatlogList.Any())
            {
                result.Success = true;
                result.Message = "沒有聊天訊息";
                return Ok(result);
            }
            try
            {
                result.Success = true;
                result.Message = "聊天訊息取得成功";
                result.Data = new { Photo = _db.Counselors.Where(x=>x.Id == CounselorId).Select(x=>new { x.Photo}).FirstOrDefault(), ChatlogList = chatlogList };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("ERROR :" + ex);
            }

        }

        /// <summary>
        /// 取得聊天對象以及最後一句話
        /// </summary>
        /// <response code="200">取得所有人聊天紀錄</response>
        /// <returns></returns>
        [HttpGet]
        //[JwtAuthFilter]
        [Route("api/chatroom/lastMsgTarget")]
        [SwaggerResponse(typeof(ApiResponse))]
        public IHttpActionResult LastMsgTarget(int Id, string Type)
        {
           
            ApiResponse result = new ApiResponse();
            var targetList = _db.ChatRooms.Select(x => new {
                x.Id,
                x.CounselorId,
                x.UserId,
                x.Type,
                x.Content,
                x.InitDate,
                x.UserRead,
                x.CounselorRead
            });

            if (!_db.ChatRooms.Where(x => x.CounselorId == Id).Any() && Type.ToLower() == "counselor")
            {
                {
                    result.Success = true;
                    result.Message = "沒有聊天訊息";
                    return Ok(result);
                }
            }
            if (!_db.ChatRooms.Where(x => x.UserId == Id).Any() && Type.ToLower() == "user")
            {
                {
                    result.Success = true;
                    result.Message = "沒有聊天訊息";
                    return Ok(result);
                }
            }

            if (Type.ToLower() == "user")
            {
                targetList = _db.ChatRooms
                   .GroupBy(c => c.UserId)
                   .Select(grp => new
                   {
                       UserId = grp.Key,
                       CounselorIds = grp.Select(x => x.CounselorId).Distinct(),
                       MaxDates = grp.GroupBy(x => x.CounselorId).Select(x => new
                       {
                           CounselorId = x.Key,
                           MaxDate = x.Max(y => y.InitDate)
                       })
                   })
                   .SelectMany(x => x.CounselorIds.Select(cid => new
                   {
                       x.UserId,
                       CounselorId = cid,
                       ChatRoom = _db.ChatRooms.FirstOrDefault(c => c.UserId == x.UserId && c.CounselorId == cid && x.MaxDates.FirstOrDefault(md => md.CounselorId == cid).MaxDate == c.InitDate)
                   }))
                   .Where(x => x.UserId == Id)
                   .Select(x => new
                   {
                       x.ChatRoom.Id,
                       x.ChatRoom.CounselorId,
                       x.ChatRoom.UserId,
                       x.ChatRoom.Type,
                       x.ChatRoom.Content,
                       x.ChatRoom.InitDate,
                       x.ChatRoom.UserRead,
                       x.ChatRoom.CounselorRead

                   }).OrderByDescending(x => x.InitDate);
            }
            else if (Type.ToLower() == "counselor")
            {
                targetList = _db.ChatRooms
                    .GroupBy(c => c.CounselorId)
                    .Select(grp => new
                    {
                        CounselorId = grp.Key,
                        UserIds = grp.Select(x => x.UserId).Distinct(),
                        MaxDates = grp.GroupBy(x => x.UserId).Select(x => new
                        {
                            UserId = x.Key,
                            MaxDate = x.Max(y => y.InitDate)
                        })
                    })
                    .SelectMany(x => x.UserIds.Select(uid => new
                    {
                        x.CounselorId,
                        UserId = uid,
                        ChatRoom = _db.ChatRooms.FirstOrDefault(c => c.UserId == uid && c.CounselorId == x.CounselorId && x.MaxDates.FirstOrDefault(md => md.UserId == uid).MaxDate == c.InitDate)
                    }))
                    .Where(x => x.CounselorId == Id)
                    .Select(x => new
                    {
                        x.ChatRoom.Id,
                        x.ChatRoom.CounselorId,
                        x.ChatRoom.UserId,
                        x.ChatRoom.Type,
                        x.ChatRoom.Content,
                        x.ChatRoom.InitDate,
                        x.ChatRoom.UserRead,
                        x.ChatRoom.CounselorRead
                    }).OrderByDescending(x => x.InitDate);
            }
            else return BadRequest("Type類型輸入錯誤");


            var lista = targetList.ToList();
            string msgg = "";
            List<UserChatTarget> userChatTargetList = new List<UserChatTarget>();
            List<string> targetName = new List<string>();
            foreach (var item in lista)
            {
                UserChatTarget userChatTarget = new UserChatTarget();

                if (Type.ToLower() == "user")
                {
                    Counselor counselor = _db.Counselors.Where(x => x.Id == item.CounselorId).FirstOrDefault();
                    if (counselor != null)
                    {
                        userChatTarget.OutName = counselor.Name;
                        userChatTarget.Id = item.Id;
                        userChatTarget.InitDate = item.InitDate;
                        userChatTarget.UserId = item.UserId;
                        userChatTarget.Content = item.Content;
                        userChatTarget.CounselorId = item.CounselorId;
                        userChatTarget.UserRead = item.UserRead;
                        userChatTarget.CounselorRead = item.CounselorRead;
                        userChatTarget.Type = item.Type;
                        userChatTarget.Photo = counselor.Photo;
                      
                        userChatTargetList.Add(userChatTarget);
                    }
                }
                else if (Type.ToLower() == "counselor")
                {
                    User user = _db.Users.Where(x => x.Id == item.UserId).FirstOrDefault();
                    if (user != null)
                    {
                        userChatTarget.OutName = user.Name;
                        userChatTarget.Id = item.Id;
                        userChatTarget.InitDate = item.InitDate;
                        userChatTarget.UserId = item.UserId;
                        userChatTarget.Content = item.Content;
                        userChatTarget.CounselorId = item.CounselorId;
                        userChatTarget.Type = item.Type;
                        userChatTarget.UserRead = item.UserRead;
                        userChatTarget.CounselorRead = item.CounselorRead;
                        userChatTargetList.Add(userChatTarget);
                    }
                }
            }

            try
            {
                result.Success = true;
                result.Message = "得到所有人最後一則訊息";
                result.Data = new { userChatTargetList };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest("ERROR :" + ex);
            }

        }

        /// <summary>
        /// 取得所有諮商師目標
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        //[JwtAuthFilter]
        [Route("api/chatroom/getTatgetCounselor")]
        [SwaggerResponse(typeof(ApiResponse))]
        public async Task<IHttpActionResult> GetTatgetCounselor()
        {
            ApiResponse result = new ApiResponse();
          

            var counselorList = await Task.Run(() => _db.Counselors.Select(x => new { x.Id, x.Name }));
            result.Success = true;
            result.Message = "取得諮商師目標";
            result.Data = new { counselorList };
            return Ok(result);
        }


        /// <summary>
        /// 已讀雙方的所有訊息
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="CounselorId"></param>
        /// <returns></returns>
        [HttpPost]
        //[JwtAuthFilter]
        [Route("api/chatroom/PostReadChatRoom")]
        [SwaggerResponse(typeof(ApiResponse))]
        public async Task<IHttpActionResult> PostReadChatRoom(int UserId , int CounselorId)
        {
            ApiResponse result = new ApiResponse();
            if(_db.ChatRooms.Where(c => c.UserId == UserId && c.CounselorId == CounselorId).Any())
            {
               
                return BadRequest("沒有聊天紀錄");
            }
            _db.ChatRooms
                    .Where(c => c.UserId == UserId && c.CounselorId == CounselorId)
                    .ToList()
                    .ForEach(c => { c.UserRead = true; c.CounselorRead = true; });
            await _db.SaveChangesAsync();
            result.Success = true;
            result.Message = "已讀對方訊息";
            return Ok(result);
        }
        //**

    }

}
