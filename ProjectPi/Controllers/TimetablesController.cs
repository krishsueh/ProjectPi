using NSwag.Annotations;
using ProjectPi.Models;
using ProjectPi.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ProjectPi.Controllers
{
    [OpenApiTag("Timetables", Description = "諮商師預約時段")]
    public class TimetablesController : ApiController
    {
        PiDbContext _db = new PiDbContext();

        /// <summary>
        /// 諮商師編輯預約時段
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [Route("api/timetables")]
        [JwtAuthFilter]
        [HttpPost]
        public IHttpActionResult PostTimetable(ViewModel_C.Timetable view)
        {
            var counselorToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int counselorId = (int)counselorToken["Id"];

            //有新增過預約時段 -> 先刪掉舊資料
            var timetableExist = _db.Timetables
                .Where(c => c.CounselorId == counselorId).ToList();
            if (timetableExist != null)
            {
                _db.Timetables.RemoveRange(timetableExist);
                _db.SaveChanges();
            }

            //新增預約時段
            DateTime startDate = view.StartDate;
            DateTime endDate = view.EndDate;
            TimeSpan interval = endDate - startDate;
            int timeSpan = interval.Days + 1;

            Timetable timetable = new Timetable();
            for (int i = 0; i < timeSpan; i++)
            {
                for (int j = 0; j < 24; j++)
                {
                    timetable.CounselorId = counselorId;
                    DateTime nextDate = startDate.AddDays(i);
                    DayOfWeek dayOfWeek = nextDate.DayOfWeek;

                    switch (dayOfWeek)
                    {
                        case DayOfWeek.Sunday:
                            timetable.WeekDay = "日";
                            timetable.Date = nextDate;
                            timetable.Time = view.WeekData[0].Hours[j].Time;
                            timetable.Availability = view.WeekData[0].Hours[j].Available;
                            break;
                        case DayOfWeek.Monday:
                            timetable.WeekDay = "一";
                            timetable.Date = nextDate;
                            timetable.Time = view.WeekData[1].Hours[j].Time;
                            timetable.Availability = view.WeekData[1].Hours[j].Available;
                            break;
                        case DayOfWeek.Tuesday:
                            timetable.WeekDay = "二";
                            timetable.Date = nextDate;
                            timetable.Time = view.WeekData[2].Hours[j].Time;
                            timetable.Availability = view.WeekData[2].Hours[j].Available;
                            break;
                        case DayOfWeek.Wednesday:
                            timetable.WeekDay = "三";
                            timetable.Date = nextDate;
                            timetable.Time = view.WeekData[3].Hours[j].Time;
                            timetable.Availability = view.WeekData[3].Hours[j].Available;
                            break;
                        case DayOfWeek.Thursday:
                            timetable.WeekDay = "四";
                            timetable.Date = nextDate;
                            timetable.Time = view.WeekData[4].Hours[j].Time;
                            timetable.Availability = view.WeekData[4].Hours[j].Available;
                            break;
                        case DayOfWeek.Friday:
                            timetable.WeekDay = "五";
                            timetable.Date = nextDate;
                            timetable.Time = view.WeekData[5].Hours[j].Time;
                            timetable.Availability = view.WeekData[5].Hours[j].Available;
                            break;
                        case DayOfWeek.Saturday:
                            timetable.WeekDay = "六";
                            timetable.Date = nextDate;
                            timetable.Time = view.WeekData[6].Hours[j].Time;
                            timetable.Availability = view.WeekData[6].Hours[j].Available;
                            break;
                    }

                    _db.Timetables.Add(timetable);
                    _db.SaveChanges();
                }
            }
            ApiResponse result = new ApiResponse { };
            result.Success = true;
            result.Message = "成功編輯預約時段";
            result.Data = null;
            return Ok(result);
        }

        /// <summary>
        /// 諮商師取得預約時段
        /// </summary>
        /// <returns></returns>
        [Route("api/timetables")]
        [JwtAuthFilter]
        [HttpGet]
        public IHttpActionResult GetTimetable()
        {
            var counselorToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int counselorId = (int)counselorToken["Id"];

            var earliestDate = _db.Timetables
                .Where(x => x.CounselorId == counselorId).Min(x => x.Date).ToShortDateString();
            var latestDate = _db.Timetables
                .Where(x => x.CounselorId == counselorId).Max(x => x.Date).ToShortDateString();
            object[] weekData = new object[7];
            weekData[0] = new { WeekDay = "日", Hours = HoursData(counselorId, "日") };
            weekData[1] = new { WeekDay = "一", Hours = HoursData(counselorId, "一") };
            weekData[2] = new { WeekDay = "二", Hours = HoursData(counselorId, "二") };
            weekData[3] = new { WeekDay = "三", Hours = HoursData(counselorId, "三") };
            weekData[4] = new { WeekDay = "四", Hours = HoursData(counselorId, "四") };
            weekData[5] = new { WeekDay = "五", Hours = HoursData(counselorId, "五") };
            weekData[6] = new { WeekDay = "六", Hours = HoursData(counselorId, "六") };

            ApiResponse result = new ApiResponse { };
            result.Success = true;
            result.Message = "成功取得預約時段";
            result.Data = new
            {
                StartDate = earliestDate,
                EndDate = latestDate,
                WeekData = weekData
            };
            return Ok(result);
        }

        /// <summary>
        /// 根據星期找到對應的可預約時段
        /// </summary>
        /// <param name="weekDay">星期</param>
        /// /// <param name="counselorIdInToken">token中諮商師的Id</param>
        /// <returns></returns>
        public static object HoursData(int counselorIdInToken, string weekDay)
        {
            PiDbContext _db = new PiDbContext();

            var findTime = _db.Timetables
              .Where(x => x.CounselorId == counselorIdInToken && x.WeekDay == weekDay)
              .Select(t => new { Time = t.Time, t.Availability })
              .Distinct();

            return findTime;
        }

    }

}
