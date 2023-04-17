using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectPi.Models
{
    /// <summary>
    /// 聊天室
    /// </summary>
    public class ChatRoom
    {
        /// <summary>
        /// 編號
        /// </summary>
        [Key]
        [Display(Name = "編號")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

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

        /// <summary>
        /// 建立時間
        /// </summary>
        [Display(Name = "建立時間")]
        public DateTime InitDate { get; set; }
        /*
        /// <summary>
        /// 用戶已讀
        /// </summary>
        [Display(Name = "用戶已讀")]
        public bool UserRead { get; set; }

        /// <summary>
        /// 用戶已讀
        /// </summary>
        [Display(Name = "諮商師已讀")]
        public bool CounselorRead { get; set; }
        */
        /// <summary>
        /// 將 InitDate 設為自動生成
        /// </summary>
        public ChatRoom()
        {
            InitDate = DateTime.UtcNow;
        }
    }
}