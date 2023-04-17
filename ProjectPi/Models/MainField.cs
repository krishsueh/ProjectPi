using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectPi.Models
{
    public class MainField
    {
        /// <summary>
        /// 編號
        /// </summary>
        [Key]
        [Display(Name = "編號")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 專業領域
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "專業領域")]
        public string Field { get; set; }

        /// <summary>
        /// 圖片
        /// </summary>
        [Display(Name = "圖片")]
        public string FieldImg { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Display(Name = "建立時間")]
        public DateTime InitDate { get; set; }

        /// <summary>
        /// 將 InitDate 設為自動生成
        /// </summary>
        public MainField()
        {
            InitDate = DateTime.UtcNow;
        }

        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Feature> Features { get; set; }
    }
}