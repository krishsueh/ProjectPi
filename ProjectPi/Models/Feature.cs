using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectPi.Models
{
    public class Feature
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
        /// 特色1
        /// </summary>
        [Required]
        [MaxLength(25)]
        [Display(Name = "特色1")]
        public string Feature1 { get; set; }

        /// <summary>
        /// 特色2
        /// </summary>
        [Required]
        [MaxLength(25)]
        [Display(Name = "特色2")]
        public string Feature2 { get; set; }

        /// <summary>
        /// 特色3
        /// </summary>
        [Required]
        [MaxLength(25)]
        [Display(Name = "特色3")]
        public string Feature3 { get; set; }

        /// <summary>
        /// 特色4
        /// </summary>
        [MaxLength(25)]
        [Display(Name = "特色4")]
        public string Feature4 { get; set; }

        /// <summary>
        /// 特色5
        /// </summary>
        [MaxLength(25)]
        [Display(Name = "特色5")]
        public string Feature5 { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Display(Name = "建立時間")]
        public DateTime InitDate { get; set; }

        /// <summary>
        /// 將 InitDate 設為自動生成
        /// </summary>
        public Feature()
        {
            InitDate = DateTime.UtcNow;
        }
    }
}