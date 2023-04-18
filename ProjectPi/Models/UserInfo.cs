using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectPi.Models
{
    /// <summary>
    /// 聊天室連線的成員註冊
    /// </summary>
    public class UserInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// SiganlR會給每個人獨立的ConnectionId
        /// </summary>
        public string ConnectionID { get; set; }
        public string UserName { get; set; }
        
        public string UserType { get; set; }

        //建構值
        public UserInfo(string cid, string unm, string UserType)
        {
            this.ConnectionID = cid;
            this.UserName = unm;
            this.UserType = UserType;
        }
    }
    /// <summary>
    /// 回傳給前端聊天室訊息
    /// </summary>
    public class UserChatTarget
    {

        public int Id { get; set; }
        public int CounselorId { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public string OutName { get; set; }
        public string Photo { get; set; }
        public bool UserRead { get; set; }
        public bool CounselorRead { get; set; }
        public DateTime InitDate { get; set; }
    }
}