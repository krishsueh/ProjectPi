using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectPi.Models
{
    /// <summary>
    /// 諮商師/個案 共用的ViewModel
    /// </summary>
    public class ViewModel
    {
        /// <summary>
        /// 諮商師/個案帳號
        /// </summary>
        [Required]
        [Display(Name = "諮商師/個案帳號")]
        [EmailAddress(ErrorMessage = "格式不符")]
        [MaxLength(50)]
        [DataType(DataType.EmailAddress)]
        public string Account { get; set; }

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
            public List<Courses> Courses { get; set; }
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

        /// <summary>
        /// 取得諮商師總覽
        /// </summary>
        public class SearchingCounselors
        {
            [Display(Name = "頁數")]
            public int TotalPageNum { get; set; }

            [Display(Name = "諮商師列表")]
            public List<CounselorsData> CounselorsData { get; set; }
        }
        public class CounselorsData
        {
            [Display(Name = "個人頭像")]
            public string Photo { get; set; }

            [Display(Name = "姓名")]
            public string Name { get; set; }

            [Display(Name = "賣點")]
            public string SellingPoint { get; set; }

            [Display(Name = "自我介紹")]
            public string SelfIntroduction { get; set; }
        }
    }
}