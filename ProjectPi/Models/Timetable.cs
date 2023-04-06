using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectPi.Models
{
    public class Timetable
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
        [ForeignKey("CounselorId")]
        public virtual Counselor MyCounselor { get; set; }

        /// <summary>
        /// 星期
        /// </summary>
        [Display(Name = "星期")]
        [MaxLength(10)]
        public string WeekDay { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        [Display(Name = "日期")]
        public DateTime Date { get; set; }

        /// <summary>
        /// 預約時段
        /// </summary>
        [Display(Name = "預約時段")]
        [MaxLength(5)]
        public string Time { get; set; }

        /// <summary>
        /// 是否為開放時段
        /// </summary>
        [Display(Name = "是否開放")]
        public bool Availability { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Display(Name = "建立時間")]
        public DateTime InitDate { get; set; }

        /// <summary>
        /// 將 InitDate 設為自動生成
        /// </summary>
        public Timetable()
        {
            InitDate = DateTime.UtcNow;
        }
    }
}