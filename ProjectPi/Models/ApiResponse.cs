using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectPi.Models
{
    public class ApiResponse
    {
        /// <summary>
        /// 回傳內容
        /// </summary>
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}