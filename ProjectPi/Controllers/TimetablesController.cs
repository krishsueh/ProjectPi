using NSwag.Annotations;
using ProjectPi.Models;
using ProjectPi.Security;
using System;
using System.Collections.Generic;
using System.Globalization;
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

        /// <summary>
        /// 諮商師頁面的預約時段
        /// </summary>
        /// <param name="id">諮商師ID</param>
        /// /// <param name="page">頁碼</param>
        /// <returns></returns>
        [Route("api/timetableBrowser")]
        [HttpGet]
        public IHttpActionResult GetTimetableBrowser(int id, int page)
        {
            CultureInfo taiwanCulture = new CultureInfo("zh-TW");

            var findTimes = _db.Timetables
                .Where(x => x.CounselorId == id)
                .GroupBy(x => x.Date)
                .ToList();

            if (!findTimes.Any())
                return Ok(new
                {
                    Success = true,
                    Message = "尚未新增預約時段",
                    Data = new
                    {
                        TotalPageNum = 0,
                        Pagination = new object[0]
                    }
                });
            else
            {
                var dateList = findTimes
                .Select(x => new
                {
                    Year = x.Key.ToShortDateString().Split('/')[2],
                    Month = x.Key.ToShortDateString().Split('/')[0],
                    Date = x.Key.ToShortDateString().Split('/')[1],
                    WeekDay = DateTimeFormatInfo.GetInstance(taiwanCulture).GetDayName(x.Key.DayOfWeek)[2],
                    Hours = x.Select(y => new
                    {
                        AppointmentTimeId = y.Id,
                        Time = y.Time,
                        Availability = y.Availability,
                    }).ToList()
                })
                .ToList();

                // 諮商師可約的第一天
                DateTime firstDayOfAvailable = _db.Timetables.Where(x => x.CounselorId == id).Min(x => x.Date);

                // 諮商師可約的最後一天
                DateTime lastDayOfAvailable = _db.Timetables.Where(x => x.CounselorId == id).Max(x => x.Date);

                // 日曆顯示的第一天
                DateTime today = DateTime.Today;

                // 計算當天與可約第一天之間的 interval
                int interval = Math.Abs((firstDayOfAvailable - today).Days);

                // 頭部資料處理
                var newDateList = new List<object>();
                if ((firstDayOfAvailable - today).Days >= 0)
                {
                    // 產出開頭需補足資料
                    var frontFalseDates = new List<object>();
                    for (int i = 0; i < interval; i++)
                    {
                        var falseDates = new
                        {
                            Year = today.AddDays(i).ToShortDateString().Split('/')[2],
                            Month = today.AddDays(i).ToShortDateString().Split('/')[0],
                            Date = today.AddDays(i).ToShortDateString().Split('/')[1],
                            WeekDay = DateTimeFormatInfo.GetInstance(taiwanCulture).GetDayName(today.AddDays(i).DayOfWeek)[2],
                            Hours = FalseDate()
                        };

                        frontFalseDates.Add(falseDates);
                    }

                    // 將 falseDates 塞入資料頭部
                    newDateList = frontFalseDates.Take(interval).Concat(dateList).ToList();
                }
                else
                {
                    // 移除開頭以過期的資料
                    // newDateList 型態為 List<object>。dateList 型態為 List<`a>，故加上 .Cast<object>() 轉型為 List<object>
                    newDateList = dateList.Skip(interval).Take(dateList.Count() - interval).Cast<object>().ToList();
                }

                // 尾部資料處理：
                var allDateList = new List<object>();
                if (newDateList.Count() % 7 == 0)
                {
                    // newDataList 總天數若能被 7 整除，則尾部不需補資料
                    allDateList = newDateList;
                }
                else
                {
                    // 若不能被 7 整除，需另外在 dateList 後面再補上剩餘的 falseDates 湊足一周 7 天

                    // 須補足的天數
                    int days = 7 - (newDateList.Count() % 7);

                    // 產出結尾需補足資料
                    var endFalseDates = new List<object>();
                    for (int i = 0; i < days; i++)
                    {
                        var falseDates = new
                        {
                            Year = lastDayOfAvailable.AddDays(i + 1).ToShortDateString().Split('/')[2],
                            Month = lastDayOfAvailable.AddDays(i + 1).ToShortDateString().Split('/')[0],
                            Date = lastDayOfAvailable.AddDays(i + 1).ToShortDateString().Split('/')[1],
                            WeekDay = DateTimeFormatInfo.GetInstance(taiwanCulture).GetDayName(today.AddDays(i + 1).DayOfWeek)[2],
                            Hours = FalseDate()
                        };

                        endFalseDates.Add(falseDates);
                    }
                    // 再將 falseDates 塞入資料尾部
                    allDateList = newDateList.Concat(endFalseDates.Take(days)).ToList();
                }
                ApiResponse result = new ApiResponse { };
                result.Success = true;
                result.Message = "成功取得預約時段";
                result.Data = Pagination(page, allDateList);
                return Ok(result);
            }
        }

        /// <summary>
        /// 分頁功能
        /// </summary>
        /// <param name="page">前端傳入的分頁數</param>
        /// <param name="allDateList">可預約時段加入無用日期後的資料</param>
        /// <returns></returns>
        public static object Pagination(int page, List<object> allDateList)
        {
            int pageSize = 7;
            int pageNum = allDateList.Count() / pageSize;

            var pagination = allDateList
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new { PageNum = pageNum, Pagination = pagination };
        }

        /// <summary>
        /// 取得無效日期的全天時段
        /// </summary>
        /// <returns></returns>
        public static object FalseDate()
        {
            var Hours = new[]
            {
                new { AppointmentTimeId = 0, Time = "00:00", Available = false },
                new { AppointmentTimeId = 0, Time = "01:00", Available = false },
                new { AppointmentTimeId = 0, Time = "02:00", Available = false },
                new { AppointmentTimeId = 0, Time = "03:00", Available = false },
                new { AppointmentTimeId = 0, Time = "04:00", Available = false },
                new { AppointmentTimeId = 0, Time = "05:00", Available = false },
                new { AppointmentTimeId = 0, Time = "06:00", Available = false },
                new { AppointmentTimeId = 0, Time = "07:00", Available = false },
                new { AppointmentTimeId = 0, Time = "08:00", Available = false },
                new { AppointmentTimeId = 0, Time = "09:00", Available = false },
                new { AppointmentTimeId = 0, Time = "10:00", Available = false },
                new { AppointmentTimeId = 0, Time = "11:00", Available = false },
                new { AppointmentTimeId = 0, Time = "12:00", Available = false },
                new { AppointmentTimeId = 0, Time = "13:00", Available = false },
                new { AppointmentTimeId = 0, Time = "14:00", Available = false },
                new { AppointmentTimeId = 0, Time = "15:00", Available = false },
                new { AppointmentTimeId = 0, Time = "16:00", Available = false },
                new { AppointmentTimeId = 0, Time = "17:00", Available = false },
                new { AppointmentTimeId = 0, Time = "18:00", Available = false },
                new { AppointmentTimeId = 0, Time = "19:00", Available = false },
                new { AppointmentTimeId = 0, Time = "20:00", Available = false },
                new { AppointmentTimeId = 0, Time = "21:00", Available = false },
                new { AppointmentTimeId = 0, Time = "22:00", Available = false },
                new { AppointmentTimeId = 0, Time = "23:00", Available = false }
            };
            return Hours;
        }
    }

}
