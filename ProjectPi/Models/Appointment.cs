using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectPi.Models
{
    public class Appointment
    {
        /// <summary>
        /// 編號
        /// </summary>
        [Key]
        [Display(Name = "編號")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 訂單編號
        /// </summary>
        [Display(Name = "訂單")]
        public int OrderId { get; set; }
        [ForeignKey("OrderId")]
        public virtual OrderRecord MyOrder { get; set; }

        /// <summary>
        /// 預約狀態
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "預約狀態")]
        public string ReserveStatus { get; set; }

        /// <summary>
        /// 預約時間編號
        /// </summary>
        [Display(Name = "預約時間編號")]
        public int AppointmentTimeId { get; set; }

        /// <summary>
        /// 預約時間
        /// </summary>
        [Display(Name = "預約時間")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh:mm}")]
        [DataType(DataType.DateTime)]
        public DateTime? AppointmentTime { get; set; }

        /// <summary>
        /// Zoom 連結
        /// </summary>
        [Display(Name = "Zoom 連結")]
        public string ZoomLink { get; set; }

        /// <summary>
        /// 個案紀錄
        /// </summary>
        [Display(Name = "個案紀錄")]
        public string CounsellingRecord { get; set; }

        /// <summary>
        /// 紀錄日期
        /// </summary>
        [Display(Name = "紀錄日期")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime? RecordDate { get; set; }

        /// <summary>
        /// 評分星等
        /// </summary>
        [Display(Name = "評分")]
        public int? Star { get; set; }

        /// <summary>
        /// 評價
        /// </summary>
        [Display(Name = "評價")]
        public string Comment { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Display(Name = "建立時間")]
        public DateTime InitDate { get; set; }

        /// <summary>
        /// 將 InitDate 設為自動生成
        /// </summary>
        public Appointment()
        {
            InitDate = DateTime.UtcNow;
        }
    }
}