using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectPi.Models
{
    /// <summary>
    /// 個案的ViewModel
    /// </summary>
    public class ViewModel_U
    {
        /// <summary>
        /// 個案註冊
        /// </summary>
        public class Register
        {
            /// <summary>
            /// 個案姓名
            /// </summary>
            [Required]
            [MaxLength(50)]
            [Display(Name = "姓名")]
            public string Name { get; set; }

            /// <summary>
            /// 性別
            /// </summary>
            [Required]
            [MaxLength(50)]
            [Display(Name = "性別")]
            public string Sex { get; set; }

            /// <summary>
            /// 生日
            /// </summary>
            [Display(Name = "生日")]
            [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
            [DataType(DataType.Date)]
            public DateTime BirthDate { get; set; }

            /// <summary>
            /// 個案帳號
            /// </summary>
            [Required]
            [Display(Name = "個案帳號")]
            [EmailAddress(ErrorMessage = "格式不符")]
            [MaxLength(50)]
            [DataType(DataType.EmailAddress)]
            public string Account { get; set; }

            /// <summary>
            /// 個案密碼
            /// </summary>
            [Required]
            [Display(Name = "個案密碼")]
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
        /// 個案登入
        /// </summary>
        public class Login
        {
            /// <summary>
            /// 個案帳號
            /// </summary>
            [Required]
            [Display(Name = "個案帳號")]
            [EmailAddress(ErrorMessage = "格式不符")]
            [MaxLength(50)]
            [DataType(DataType.EmailAddress)]
            public string Account { get; set; }

            /// <summary>
            /// 個案密碼
            /// </summary>
            [Required]
            [Display(Name = "個案密碼")]
            [StringLength(100, ErrorMessage = "{0} 長度至少必須為 {2} 個字元。", MinimumLength = 8)]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        /// <summary>
        /// 修改基本資料
        /// </summary>
        public class Profile
        {
            /// <summary>
            /// 個案姓名
            /// </summary>
            [Required]
            [MaxLength(50)]
            [Display(Name = "姓名")]
            public string Name { get; set; }
        }

        /// <summary>
        /// 取得特定諮商師頁面
        /// </summary>
        public class counselorProfile
        {
            [Display(Name = "個人頭像")]
            public string Photo { get; set; }

            [Display(Name = "姓名")]
            public string Name { get; set; }

            [Display(Name = "自我介紹")]
            public string SelfIntroduction { get; set; }

            [Display(Name = "證號")]
            public string CertNumber { get; set; }

            [Display(Name = "影片")]
            public string VideoLink { get; set; }

            [Display(Name = "專長領域")]
            public List<Fields> Fields { get; set; }
        }
        public class Fields
        {
            [Display(Name = "專長領域")]
            public string Field { get; set; }

            [Display(Name = "課程特色")]
            public Features Features { get; set; }

            [Display(Name = "課程方案")]
            public List<Courses>  Courses { get; set; }
        }
        public class Features
        {
            [Display(Name = "特色1")]
            public string Feature1 { get; set; }

            [Display(Name = "特色2")]
            public string Feature2 { get; set; }

            [Display(Name = "特色3")]
            public string Feature3 { get; set; }

            [Display(Name = "特色4")]
            public string Feature4 { get; set; }

            [Display(Name = "特色5")]
            public string Feature5 { get; set; }
        }
        public class Courses
        {
            [Display(Name = "方案")]
            public string Item { get; set; }

            [Display(Name = "價格")]
            public int Price { get; set; }
        }



    }
}