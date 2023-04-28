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

        /// <summary>
        /// 修改基本資料
        /// </summary>
        public class Profile
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
            public string LicenseImg { get; set; }

            /// <summary>
            /// 個人頭像
            /// </summary>
            [MaxLength(50)]
            [Display(Name = "個人頭像")]
            public string Photo { get; set; }

            /// <summary>
            /// 個人賣點
            /// </summary>
            [MaxLength(12)]
            [Display(Name = "個人賣點")]
            public string SellingPoint { get; set; }

            /// <summary>
            /// 自我介紹
            /// </summary>
            [MaxLength(100)]
            [Display(Name = "自我介紹")]
            public string SelfIntroduction { get; set; }

            /// <summary>
            /// 介紹影片
            /// </summary>
            [MaxLength(100)]
            [Display(Name = "介紹影片")]
            public string VideoLink { get; set; }

            /// <summary>
            /// 影片是否開放
            /// </summary>
            [Display(Name = "影片開放")]
            public bool IsVideoOpen { get; set; }
        }

        /// <summary>
        /// 新增/修改 課程資訊
        /// </summary>
        public class Course
        {
            /// <summary>
            /// 專業領域編號
            /// </summary>
            [Display(Name = "專業領域編號")]
            public int FieldId { get; set; }

            /// <summary>
            /// 課程方案
            /// </summary>
            [Display(Name = "課程方案")]
            public Courses[] Courses { get; set; }

            /// <summary>
            /// 課程特色
            /// </summary>
            [Display(Name = "課程特色")]
            public Features Features { get; set; }
        }
        public class Features
        {
            /// <summary>
            /// 課程特色1
            /// </summary>
            [Display(Name = "特色1")]
            [Required]
            [MaxLength(25)]
            public string Feature1 { get; set; }

            /// <summary>
            /// 課程特色2
            /// </summary>
            [Display(Name = "特色2")]
            [Required]
            [MaxLength(25)]
            public string Feature2 { get; set; }

            /// <summary>
            /// 課程特色3
            /// </summary>
            [Display(Name = "特色3")]
            [Required]
            [MaxLength(25)]
            public string Feature3 { get; set; }

            /// <summary>
            /// 課程特色4
            /// </summary>
            [Display(Name = "特色4")]
            [MaxLength(25)]
            public string Feature4 { get; set; }

            /// <summary>
            /// 課程特色5
            /// </summary>
            [Display(Name = "特色5")]
            [MaxLength(25)]
            public string Feature5 { get; set; }
        }
        public class Courses
        {
            /// <summary>
            /// 課程方案
            /// </summary>
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
            /// 是否開放
            /// </summary>
            [Display(Name = "是否開放")]
            public bool Availability { get; set; }
        }

        /// <summary>
        /// 新增/修改 預約時段
        /// </summary>
        public class Timetable
        {
            /// <summary>
            /// 開始日期
            /// </summary>
            [Display(Name = "開始日期")]
            public DateTime StartDate { get; set; }

            /// <summary>
            /// 結束日期
            /// </summary>
            [Display(Name = "結束日期")]
            public DateTime EndDate { get; set; }

            /// <summary>
            /// 一週資訊
            /// </summary>
            [Display(Name = "一週資訊")]
            public WeekData[] WeekData { get; set; }
        }
        public class WeekData
        {
            /// <summary>
            /// 星期
            /// </summary>
            [Display(Name = "星期")]
            public string WeekDay { get; set; }

            /// <summary>
            /// 預約時間
            /// </summary>
            [Display(Name = "預約時間")]
            public Hours[] Hours { get; set; }
        }

        public class Hours
        {
            /// <summary>
            /// 時段
            /// </summary>
            [Display(Name = "時段")]
            public string Time { get; set; }

            /// <summary>
            /// 該時段是否開放
            /// </summary>
            [Display(Name = "是否開放")]
            public bool Available { get; set; }
        }

        /// <summary>
        /// 接收預約
        /// </summary>
        public class Appt
        {
            /// <summary>
            /// 預約編號
            /// </summary>
            [Display(Name = "預約編號")]
            public int AppointmentId { get; set; }
        }

        /// <summary>
        /// 請求時段變更
        /// </summary>
        public class ReAppt:Appt
        {
            /// <summary>
            /// 請求變更時段原因
            /// </summary>
            [Display(Name = "原因")]
            public string Reason { get; set; }
        }

        /// <summary>
        /// 修改諮商師開通狀態
        /// </summary>
        public class C_Validation
        {
            [Display(Name="諮商師ID")]
            public int CounselorId { get; set; }
            [Display(Name = "狀態")]
            public bool Validation { get; set; }
        }

        /// <summary>
        /// 寄送諮商師Email
        /// </summary>
        public class C_Account
        {
            [Display(Name = "諮商師ID")]
            public int Id { get; set; }
        }

    }
}