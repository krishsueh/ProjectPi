using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectPi.Models
{
    public class OrderRecord
    {
        /// <summary>
        /// 編號
        /// </summary>
        [Key]
        [Display(Name = "編號")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 訂單號碼
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "訂單號碼")]
        public string OrderNum { get; set; }

        /// <summary>
        /// 訂單日期
        /// </summary>
        [Display(Name = "訂單日期")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// 個案編號
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "個案編號")]
        public int UserId { get; set; }

        /// <summary>
        /// 個案姓名
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "個案姓名")]
        public string UserName { get; set; }

        /// <summary>
        /// 諮商師編號
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "諮商師編號")]
        public int CounselorId { get; set; }

        /// <summary>
        /// 諮商師姓名
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "諮商師姓名")]
        public string CounselorName { get; set; }

        /// <summary>
        /// 專業領域
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "專業領域")]
        public string Field { get; set; }

        /// <summary>
        /// 課程方案
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "課程方案")]
        public string Item { get; set; }

        /// <summary>
        /// 數量
        /// </summary>
        [Display(Name = "數量")]
        public int Quantity { get; set; }

        /// <summary>
        /// 價格
        /// </summary>
        [Display(Name = "價格")]
        public int Price { get; set; }

        /// <summary>
        /// 訂單狀態
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "訂單狀態")]
        public string OrderStatus { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Display(Name = "建立時間")]
        public DateTime InitDate { get; set; }

        /// <summary>
        /// 將 InitDate 設為自動生成
        /// </summary>
        public OrderRecord()
        {
            InitDate = DateTime.UtcNow;
        }

        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}