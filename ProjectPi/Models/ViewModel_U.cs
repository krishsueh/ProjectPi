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
        /// 加入購物車
        /// </summary>
        public class Product
        {
            /// <summary>
            /// 諮商師編號
            /// </summary>
            [Display(Name = "諮商師編號")]
            public int CounselorId { get; set; }

            /// <summary>
            /// 專業領域編號
            /// </summary>
            [Display(Name = "專業領域編號")]
            public int FieldId { get; set; }

            /// <summary>
            /// 課程方案
            /// </summary>
            [Display(Name = "課程方案")]
            public string Item { get; set; }

            /// <summary>
            /// 方案價格
            /// </summary>
            [Display(Name = "方案價格")]
            public int Price { get; set; }
        }

        public class DeleteProduct
        {
            /// <summary>
            /// 商品編號
            /// </summary>
            [Display(Name = "商品編號")]
            public int ProductId { get; set; }
        }
    }
}