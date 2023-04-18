using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectPi.Models
{
    /// <summary>
    /// 購物車
    /// </summary>
    public class Cart
    {
        /// <summary>
        /// 購物車品項編號
        /// </summary>
        [Key]
        [Display(Name = "編號")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 個案編號
        /// </summary>
        [Display(Name = "個案編號")]
        public int UersId { get; set; }
        [ForeignKey("UersId")]
        public virtual User MyUser { get; set; }

        /// <summary>
        /// 商品編號
        /// </summary>
        [Display(Name = "商品編號")]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public virtual Product Products { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Display(Name = "建立時間")]
        public DateTime InitDate { get; set; }

        /// <summary>
        /// 將 InitDate 設為自動生成
        /// </summary>
        public Cart()
        {
            InitDate = DateTime.UtcNow;
        }
    }
}