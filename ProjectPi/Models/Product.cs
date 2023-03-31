using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectPi.Models
{
    public class Product
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
        /// 專業領域編號
        /// </summary>
        [Display(Name = "專業領域編號")]
        public int FieldId { get; set; }
        [ForeignKey("FieldId")]
        public virtual MainField MyField { get; set; }

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
        /// 課程是否開放
        /// </summary>
        [Display(Name = "課程是否開放")]
        public bool Availability { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Display(Name = "建立時間")]
        public DateTime InitDate { get; set; }

        /// <summary>
        /// 將 InitDate 設為自動生成
        /// </summary>
        public Product()
        {
            InitDate = DateTime.UtcNow;
        }

        public virtual ICollection<Cart> Carts { get; set; }
    }
}