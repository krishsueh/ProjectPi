﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectPi.Models
{
    public class AppointmentLogs_UpdateRecod
    {
        [Display(Name = "課程編號")]
        public int AppointmentId { get; set; }
        [Display(Name = "紀錄內容")]
        public string CounsellingRecord { get; set; }
    }
    public class AppointmentLogs_Record
    {
        [Display(Name = "個案姓名")]
        public string Name { get; set; }
        [Display(Name = "課程編號")]
        public int AppointmentId { get; set; }
        [Display(Name = "紀錄內容")]
        public string CounsellingRecord { get; set; }
        [Display(Name = "諮商日期")]
        public string AppointmentDate { get; set; }
        [Display(Name = "紀錄日期")]
        public string RecordDate { get; set; }
        [Display(Name = "諮商議題")]
        public string Field { get; set; }
        [Display(Name = "上次諮商日期")]
        public string LastRecordDate { get; set; }
    }
    public class AppointmentLogs_Id
    {
       
        [Display(Name = "課程編號")]
        public int AppointmentId { get; set; }
        
    }
    public class AppointmentLogs_UserName
    {
        [Display(Name = "個案姓名")]
        public string Name { get; set; }
    }
    public class AppointmentLogs_Comment
    {
        [Display(Name = "課程編號")]
        public int AppointmentId { get; set; }
        [Display(Name = "個案評價")]
        public string Comment { get; set; }
        [Display(Name = "個案評分")]
        public int Star { get; set; }
    }
    public class AppointmentLogs
    {
        [Display(Name = "個案姓名")]
        public string Name { get; set; }

        [Display(Name = "專業領域")]
        public string Field { get; set; }

        [Display(Name = "預約日期")]
        public string AppointmentDate { get; set; }

        [Display(Name = "預約時間")]
        public string AppointmentTime { get; set; }

        [Display(Name = "預約課程的ID")]
        public int AppointmentId { get; set; }
    }
}