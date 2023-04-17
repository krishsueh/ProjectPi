using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectPi.Models
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string ConnectionID { get; set; }
        public string UserName { get; set; }
        
        public string UserType { get; set; }

        //构造函数
        public UserInfo(string cid, string unm, string UserType)
        {
            this.ConnectionID = cid;
            this.UserName = unm;
            this.UserType = UserType;
        }
    }

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