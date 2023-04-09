using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using ProjectPi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace ProjectPi.SignalRHub
{
    [HubName("chartHubb")]
    public class ChartHubb : Hub
    {
        private static List<UserInfo> USERLIST = new List<UserInfo>();

        //连接
        public override Task OnConnected()
        {
            var currentUser = USERLIST.Where(x => x.ConnectionID == Context.ConnectionId).FirstOrDefault();
            if (currentUser == null)
            {
                UserInfo newUser = new UserInfo(Context.ConnectionId, "");
                USERLIST.Add(newUser);
            }

            return base.OnConnected();
        }

        //断开
        public override Task OnDisconnected(bool stopCalled)
        {
            var currentUser = USERLIST.Where(x => x.ConnectionID == Context.ConnectionId).FirstOrDefault();
            if (currentUser != null)
            {
                USERLIST.Remove(currentUser);
            }

            ShowAllUser();
            return base.OnDisconnected(stopCalled);
        }


        /// <summary>
        /// 广播登陆用户列表到全体连接客户端
        /// </summary>
        [HubMethodName("showAllUser")]
        public void ShowAllUser()
        {
            string userJson = Newtonsoft.Json.JsonConvert.SerializeObject(USERLIST);
            Clients.All.broadcastUserList(userJson);
            //前端js定义 function broadcastUserList(userList)
        }

        /// <summary>
        /// 登陆时设置名字
        /// </summary>
        /// <param name="inputName"></param>
        [HubMethodName("setUserName")]
        public void SetUserName(string inputName)
        {
            var currentUser = USERLIST.Where(x => x.ConnectionID == Context.ConnectionId).FirstOrDefault();
            if (currentUser != null)
            {
                currentUser.UserName = inputName;
            }
            //广播给全体客户端
            this.ShowAllUser();
        }

        /// <summary>
        /// 发送信息给某人
        /// </summary>
        /// <param name="outsideID"></param>
        /// <param name="message"></param>
        [HubMethodName("sendTo")]
        public void SendTo(string outsideID, string message)
        {
            var myUser = USERLIST.Where(y => y.ConnectionID == Context.ConnectionId).FirstOrDefault();
            var outsideUser = USERLIST.Where(x => x.ConnectionID == outsideID).FirstOrDefault();

            //前端js定义function showMessage(speakerName , message)
            if (outsideUser != null)
            {
                Clients.Client(outsideUser.ConnectionID).showMessage(myUser.UserName, message);
                Clients.Client(myUser.ConnectionID).showMessage(myUser.UserName, message);
                //对方  和  我方 的界面都要显示语录
            }
            else
            {
                Clients.Client(myUser.ConnectionID).showMessage(outsideUser.UserName + outsideUser.ConnectionID, "离线");
            }
        }


    }
}