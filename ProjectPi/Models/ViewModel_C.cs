using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectPi.Models
{
    /// <summary>
    /// 諮商師的ViewModel
    /// </summary>
    public class ViewModel_C
    {
        /// <summary>
        /// 諮商師註冊
        /// </summary>
        public class Register
        {
            /// <summary>
            /// 諮商師姓名
            /// </summary>
            [Required]
            [MaxLength(50)]
            [Display(Name = "姓名")]
            public string Name { get; set; }

            /// <summary>
            /// 執業證照
            /// </summary>
            [Required]
            [MaxLength(50)]
            [Display(Name = "執業證照")]
            public string License { get; set; }

            /// <summary>
            /// 證書字號
            /// </summary>
            [Required]
            [MaxLength(50)]
            [Display(Name = "證書字號")]
            public string Certification { get; set; }

            /// <summary>
            /// 諮商師帳號
            /// </summary>
            [Required]
            [Display(Name = "諮商師帳號")]
            [EmailAddress(ErrorMessage = "格式不符")]
            [MaxLength(50)]
            [DataType(DataType.EmailAddress)]
            public string Account { get; set; }

            /// <summary>
            /// 諮商師密碼
            /// </summary>
            [Required]
            [Display(Name = "諮商師密碼")]
            [StringLength(100, ErrorMessage = "{0} 長度至少必須為 {2} 個字元。", MinimumLength = 8)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            /// <summary>
            /// 再次輸入密碼
            /// </summary>
            [Required]
            [Display(Name = "驗證密碼")]
            [StringLength(100, ErrorMessage = "{0} 長度至少必須為 {2} 個字元。", MinimumLength = 8)]
            [DataType(DataType.Password)]
            public string ConfirmPassword { get; set; }
        }

        /// <summary>
        /// 諮商師登入
        /// </summary>
        public class Login
        {
            /// <summary>
            /// 諮商師帳號
            /// </summary>
            [Required]
            [Display(Name = "諮商師帳號")]
            [EmailAddress(ErrorMessage = "格式不符")]
            [MaxLength(50)]
            [DataType(DataType.EmailAddress)]
            public string Account { get; set; }

            /// <summary>
            /// 諮商師密碼
            /// </summary>
            [Required]
            [Display(Name = "諮商師密碼")]
            [StringLength(100, ErrorMessage = "{0} 長度至少必須為 {2} 個字元。", MinimumLength = 8)]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }
    }
}