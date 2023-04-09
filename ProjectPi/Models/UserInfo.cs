using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectPi.Models
{
    public class UserInfo
    {
        public string ConnectionID { get; set; }
        public string UserName { get; set; }

        //构造函数
        public UserInfo(string cid, string unm)
        {
            this.ConnectionID = cid;
            this.UserName = unm;
        }
    }
}