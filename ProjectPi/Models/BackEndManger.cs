using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectPi.Models
{
    /// <summary>
    /// 後端管理Model
    /// </summary>
    public class BackEndManger
    {
        /// <summary>
        /// 編號
        /// </summary>
        [Key]
        [Display(Name = "編號")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [Required]
        [MaxLength(50)]
        [Display(Name = "姓名")]
        public string Name { get; set; }

        /// <summary>
        /// 後端管理性別
        /// </summary>
        [Required]
        [MaxLength(50)]
        [Display(Name = "性別")]
        public string Sex { get; set; }

        /// <summary>
        /// 後端管理生日
        /// </summary>
        [Display(Name = "生日")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        /// <summary>
        /// 帳號
        /// </summary>
        [Required]
        [Display(Name = "帳號")]
        [EmailAddress(ErrorMessage = "格式不符")]
        [RegularExpression(@"^[A-Za-z0-9_\-\.\+]*\@[a-z]*[.][a-z]*(.[a-z]*)$", ErrorMessage = "請輸入正確的Email")]
        [MaxLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Account { get; set; }

        /// <summary>
        /// 密碼
        /// </summary>
        [Required]
        [Display(Name = "密碼")]
        [StringLength(100, ErrorMessage = "{0} 長度至少必須為 {2} 個字元。", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary>
        /// 後端管理權限
        /// </summary>
        [Required]
        [Display(Name = "權限")]
        public int AdminAccess { get; set; }

        /// <summary>
        /// Guid
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
        public BackEndManger()
        {
            Guid = Guid.NewGuid();
            InitDate = DateTime.UtcNow;
        }
    }
}