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
    }
}