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
        PiDbContext _db = new PiDbContext();
        //連接
        public override Task OnConnected()
        {

            var currentUser = USERLIST.Where(x => x.ConnectionID == Context.ConnectionId).FirstOrDefault();
            if (currentUser == null)
            {
                UserInfo newUser = new UserInfo(Context.ConnectionId, "" , "nohaveType");
                USERLIST.Add(newUser);
            }
            else
            {
                //// 如果使用者已經存在，則關閉現有的連接
                //Clients.Client(currentUser.ConnectionID).closeConnection();
                //USERLIST.Remove(currentUser);
                //// 新增新的連接
                //UserInfo newUser = new UserInfo(Context.ConnectionId, "", "user");
                //USERLIST.Add(newUser);
            }


            return base.OnConnected();
        }

        //斷開
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
        /// 廣播登陸用戶列表到全體連接客戶端
        /// </summary>
        [HubMethodName("showAllUser")]
        public void ShowAllUser()
        {
            string userJson = Newtonsoft.Json.JsonConvert.SerializeObject(USERLIST);
            Clients.All.broadcastUserList(userJson);
            //前端js定義 function broadcastUserList(userList)
        }

        /// <summary>
        /// 登陸時設置名字
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
            //廣播給全部客戶端
            this.ShowAllUser();
        }

        /// <summary>
        /// 儲存訊息
        /// </summary>

        [HubMethodName("chatLog")]
        public void ChatLog(int CounselorId, int UserId, string Content, string Type)
        {
            ChatRoom _chatroom = new ChatRoom();
            _chatroom.CounselorId = CounselorId;
            _chatroom.UserId = UserId;
            _chatroom.Content = Content;
            _chatroom.Type = Type;
            _chatroom.InitDate = DateTime.Now;
            _db.ChatRooms.Add(_chatroom);
            _db.SaveChanges();

        }

        /// <summary>
        /// 連線完要註冊
        /// </summary>
        /// <param name="outsideID"></param>
        /// <param name="message"></param>
        [HubMethodName("setUserId")]
        public void SetUserId(int id , string userType)
        {
            var currentUser = USERLIST.Where(x => x.ConnectionID == Context.ConnectionId).FirstOrDefault();
            var isCheckList = USERLIST.Where(x => x.Id == id && x.UserType == userType).FirstOrDefault();
            //if (isCheckList != null)
            //{
            //    var connectionId = currentUser.ConnectionID;
            //    USERLIST.Remove(currentUser);
            //    Clients.Client(connectionId).forceDisconnect();
            //}
            if (currentUser != null)
            {
                currentUser.Id = id;
                currentUser.UserType = userType;
            }
           
            this.ShowAllUser();
            //this.CheckUserId(id, userType);
        }

        /// <summary>
        /// 
        /// </summary>
        [HubMethodName("loginOut")]
        public void LoginOut()
        {
            var myUser = USERLIST.Where(y => y.ConnectionID == Context.ConnectionId).FirstOrDefault();
            Clients.All.broadcastUserList("有人想要登出");
            Clients.All.broadcastUserList(Context.ConnectionId);

            Clients.Client(myUser.ConnectionID).stopConnect();
        }

        /// <summary>
        /// 確認是否重複連線
        /// </summary>

        [HubMethodName("checkUserId")]
        public void CheckUserId(int id, string userType)
        {
            var isCheckList = USERLIST.Where(x => x.Id == id && x.UserType == userType).FirstOrDefault();
            if (isCheckList != null)
            {
                // 通知現有連線被斷開
                Clients.Client(isCheckList.ConnectionID).IsLogin(true);
                // 斷開現有連線
            }
            else
            {
                Clients.Client(Context.ConnectionId).IsLogin(false);
            }


        }
        /// <summary>
        /// 指定人發送信息
        /// </summary>
        /// <param name="outsideID"></param>
        /// <param name="message"></param>
        [HubMethodName("sendTo")]
        public void SendTo(int outsideID, string message , string myType)
        {
            var myUser = USERLIST.Where(y => y.ConnectionID == Context.ConnectionId).FirstOrDefault();
            var outsideUser = USERLIST.FirstOrDefault(x => x.UserType != myType && x.Id == outsideID);
            var chatMsg = new { CounselorId = outsideID, UserId = myUser.Id, Content = message, Type = "send" , InitDate = DateTime.Now };
            Clients.All.broadcastUserList("有人傳送訊息哦");
            int userId, counselorId;
            if (myType == "user")
            {
                 chatMsg = new { CounselorId = outsideID , UserId = myUser.Id , Content = message , Type = "send", InitDate = DateTime.Now };
                userId = myUser.Id;
                counselorId = outsideID;
            }
            else
            {
                 chatMsg = new { CounselorId = myUser.Id, UserId = outsideID, Content = message, Type = "accept", InitDate = DateTime.Now };
                userId = outsideID;
                counselorId = myUser.Id;
            }
            var UList = USERLIST.Where(x => x.UserType == "user" && x.Id == userId).ToList();
            var CList = USERLIST.Where(x => x.UserType == "counselor" && x.Id == counselorId).ToList();
            foreach(var item in UList)
            {
                Clients.Client(item.ConnectionID).showMessage(myUser.Id, message, myType, chatMsg);
                Clients.Client(item.ConnectionID).showLastMsg();
                //var UserList = new { Myid = myUser.Id, MyType = myUser.UserType };
                Clients.Client(item.ConnectionID).showIconUnRead(USERLIST.Count.ToString());
            }
            foreach (var item in CList)
            {
                Clients.Client(item.ConnectionID).showMessage(myUser.Id, message, myType, chatMsg);
                Clients.Client(item.ConnectionID).showLastMsg();
                //var UserList = new { Myid = myUser.Id, MyType = myUser.UserType };
                Clients.Client(item.ConnectionID).showIconUnRead(USERLIST.Count.ToString());
            }

            //var outsideUser = USERLIST.Where(x => x.ConnectionID != Context.ConnectionId).Where(x => x.Id == outsideID).FirstOrDefault();
            //Clients.Client(myUser.ConnectionID).showMessage(myUser.Id,message , myType , chatMsg);
            //Clients.Client(myUser.ConnectionID).showLastMsg();
            //var UserList = new { Myid = myUser.Id, MyType = myUser.UserType };
            //Clients.Client(myUser.ConnectionID).showIconUnRead(USERLIST.Count.ToString() , UserList );

            ////前端js定义function showMessage(speakerName , message)
            //if (outsideUser != null)
            //{
            //    var ConList = new { Outid = outsideUser.Id, OutType = outsideUser.UserType };
            //    Clients.Client(outsideUser.ConnectionID).showMessage(myUser.Id,message, myType , chatMsg);
            //    Clients.Client(outsideUser.ConnectionID).showLastMsg();
            //    Clients.Client(outsideUser.ConnectionID).showIconUnRead(USERLIST.Count.ToString(), UserList, ConList);
            //    //雙方的界面都要渲染，透過伺服器呼叫雙方的JS的Method重新渲染畫面
            //}
            /*
             離線訊息不用渲染OR 提示
            else
            {
                Clients.Client(myUser.ConnectionID).showMessage(outsideUser.UserName + outsideUser.ConnectionID, "離線");
            }
            */
        }



    }
}