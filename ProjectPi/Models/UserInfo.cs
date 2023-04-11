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

        //构造函数
        public UserInfo(string cid, string unm)
        {
            this.ConnectionID = cid;
            this.UserName = unm;
        }
    }

    public class UserChatTarget
    {

        public int Id { get; set; }
        public int CounselorId { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public string Name { get; set; }
        public DateTime InitDate { get; set; }
    }
}