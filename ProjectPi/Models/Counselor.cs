using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectPi.Models
{
    /// <summary>
    /// 諮商師
    /// </summary>
    public class Counselor
    {
        /// <summary>
        /// 諮商師編號
        /// </summary>
        [Key]
        [Display(Name = "編號")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 諮商師姓名
        /// </summary>
        [Required]
        [MaxLength(50)]
        [Display(Name = "姓名")]
        public string Name { get; set; }

        /// <summary>
        /// 諮商師帳號
        /// </summary>
        [Required]
        [Display(Name = "帳號")]
        [EmailAddress(ErrorMessage = "格式不符")]
        [RegularExpression(@"^[A-Za-z0-9_\-\.\+]*\@[a-z]*[.][a-z]*(.[a-z]*)$", ErrorMessage = "請輸入正確的Email")]
        [MaxLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Account { get; set; }

        /// <summary>
        /// 諮商師密碼
        /// </summary>
        [Required]
        [Display(Name = "密碼")]
        [StringLength(100, ErrorMessage = "{0} 長度至少必須為 {2} 個字元。", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// 證書字號
        /// </summary>
        [Required]
        [MaxLength(50)]
        [Display(Name = "證書字號")]
        public string CertNumber { get; set; }

        /// <summary>
        /// 執業證照
        /// </summary>
        [Required]
        [MaxLength(50)]
        [Display(Name = "執業證照")]
        public string LicenseImg { get; set; }

        /// <summary>
        /// 執業證照 Base64
        /// </summary>
        [Display(Name = "執業證照Base64")]
        public string LicenseBase64 { get; set; }

        /// <summary>
        /// 個人照片
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "個人照片")]
        public string Photo { get; set; }

        /// <summary>
        /// 個人照片 Base64
        /// </summary>
        [Display(Name = "個人照片Base64")]
        public string PhotoBase64 { get; set; }

        /// <summary>
        /// 個人賣點
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "個人賣點")]
        public string SellingPoint { get; set; }

        /// <summary>
        /// 自我介紹
        /// </summary>
        [MaxLength(300)]
        [Display(Name = "自我介紹")]
        public string SelfIntroduction { get; set; }

        /// <summary>
        /// 介紹影片
        /// </summary>
        [MaxLength(100)]
        [Display(Name = "介紹影片")]
        public string VideoLink { get; set; }

        /// <summary>
        /// 影片開放
        /// </summary>
        [Display(Name = "影片開放")]
        public bool? IsVideoOpen { get; set; }

        /// <summary>
        /// 諮商師審核狀態
        /// </summary>
        [Display(Name = "審核狀態")]
        public bool Validation { get; set; }

        /// <summary>
        /// 諮商師 Guid
        /// </summary>
        [Display(Name = "Guid")]
        public Guid Guid { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        [Display(Name = "建立時間")]
        public DateTime InitDate { get; set; }

        /// <summary>
        /// 將 Guid 與 InitDate 設為自動生成
        /// </summary>
        public Counselor()
        {
            Guid = Guid.NewGuid();
            InitDate = DateTime.UtcNow;
        }

        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Feature> Features { get; set; }
    }
}