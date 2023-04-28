using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectPi.Models
{
    /// <summary>
    /// 諮商師/個案 共用的ViewModel
    /// </summary>
    public class ViewModel
    {
        /// <summary>
        /// 諮商師/個案帳號
        /// </summary>
        [Required]
        [Display(Name = "諮商師/個案帳號")]
        [EmailAddress(ErrorMessage = "格式不符")]
        [MaxLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Account { get; set; }

        /// <summary>
        /// 預約管理分頁數
        /// </summary>
        [Display(Name = "頁數")]
        public int PageNum { get; set; }

        /// <summary>
        /// 取得諮商師總覽
        /// </summary>
        public class SearchingCounselors
        {
            [Display(Name = "頁數")]
            public int TotalPageNum { get; set; }

            [Display(Name = "諮商師列表")]
            public List<CounselorsData> CounselorsData { get; set; }
        }
        public class CounselorsData
        {
            [Display(Name = "諮商師編號")]
            public int Id { get; set; }

            [Display(Name = "姓名")]
            public string Name { get; set; }

            [Display(Name = "賣點")]
            public string SellingPoint { get; set; }

            [Display(Name = "自我介紹")]
            public string SelfIntroduction { get; set; }

            [Display(Name = "個人頭像")]
            public string Photo { get; set; }
        }

        /// <summary>
        /// 重設密碼
        /// </summary>
        public class AccountResetGuid
        {
            [Display(Name = "新密碼")]
            public string Password { get; set; }
            [Display(Name = "二次確認")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "Guid")]
            public Guid Guid { get; set; }

        }
        /// <summary>
        /// 重設密碼
        /// </summary>
        public class AccountReset
        {
            [Display(Name = "新密碼")]
            public string Password { get; set; }
            [Display(Name = "二次確認")]
            public string ConfirmPassword { get; set; }

       

        }

        /// <summary>
        /// 聊天室存訊息VIEW
        /// </summary>
        public class PostChatRoomLog
        {

            /// <summary>
            /// 諮商師編號
            /// </summary>
            [Display(Name = "諮商師編號")]
            public int CounselorId { get; set; }

            /// <summary>
            /// 個案編號
            /// </summary>
            [Display(Name = "個案編號")]
            public int UserId { get; set; }

            /// <summary>
            /// 傳輸類型
            /// </summary>
            [MaxLength(10)]
            [Display(Name = "傳輸類型")]
            public string Type { get; set; }

            /// <summary>
            /// 訊息內容
            /// </summary>
            [Display(Name = "訊息內容")]
            public string Content { get; set; }






        }

        /// <summary>
        /// 已讀對方訊息的VIEW
        /// </summary>
        public class PostReadChatRooms
        {
            /// <summary>
            /// 個案編號
            /// </summary>
            [Display(Name = "個案編號")]
            public int UserId { get; set; }
            /// <summary>
            /// 諮商師編號
            /// </summary>
            [Display(Name = "諮商師編號")]
            public int CounselorId { get; set; }
            /// <summary>
            /// 用戶類別
            /// </summary>
            [Display(Name = "用戶類別")]
            public string MyType { get; set; }
        }

        /// <summary>
        /// 編輯管理員
        /// </summary>
        public class backenderUpdate
        {
            /// <summary>
            /// Guid
            /// </summary>
            [Display(Name = "Guid")]
            public Guid Guid { get; set; }

            /// <summary>
            /// 姓名
            /// </summary>
            [Display(Name = "姓名")]
            public string Name { get; set; }

            /// <summary>
            /// 後端管理權限
            /// </summary>
            [Display(Name = "權限")]
            public int AdminAccess { get; set; }
        }

        /// <summary>
        /// 後台諮商師補件郵件取得Guid
        /// </summary>
        public class ModelGuid
        {
            public Guid guid { get; set; }
        }


    }


}