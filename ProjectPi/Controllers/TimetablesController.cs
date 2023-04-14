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
        /// <returns></returns>
        [Route("api/timetableBrowser")]
        [HttpGet]
        public IHttpActionResult GetTimetableBrowser(int id)
        {
            var findTimes = _db.Timetables
                .Where(x => x.CounselorId == id)
                .GroupBy(x => x.Date)
                .ToList();

            var dateList = findTimes
                .Select(x => new
                {
                    Year = x.Key.ToShortDateString().Split('/')[0],
                    Month = x.Key.ToShortDateString().Split('/')[1],
                    Date = x.Key.ToShortDateString().Split('/')[2],
                    WeekDay = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(x.Key.DayOfWeek)[2],
                    //Hours = x.Select(y => new {
                    //    Time = y.Time,
                    //    Availability = y.Availability,
                    //}).ToList()
                })
                .ToList();

            int year, month, day;
            //諮商師可以的第一天
            year = int.Parse(dateList.FirstOrDefault().Year);
            month = int.Parse(dateList.FirstOrDefault().Month);
            day = int.Parse(dateList.FirstOrDefault().Date);
            DateTime firstDayOfAvailable = new DateTime(year, month, day);

            //諮商師可以的最後一天
            year = int.Parse(dateList.LastOrDefault().Year);
            month = int.Parse(dateList.LastOrDefault().Month);
            day = int.Parse(dateList.LastOrDefault().Date);
            DateTime lastDayOfAvailable = new DateTime(year, month, day);

            //日立顯示的第一天
            DateTime today = new DateTime(2023, 5, 30);

            //計算需要補足的天數 2 天
            int interval = (firstDayOfAvailable - today).Days;

            //產出開頭需補足資料
            var frontFalseDates = new List<object>();
            for (int i = 0; i < interval; i++)
            {
                var falseDates = new
                {
                    Year = today.AddDays(i).ToShortDateString().Split('/')[0],
                    Month = today.AddDays(i).ToShortDateString().Split('/')[1],
                    Date = today.AddDays(i).ToShortDateString().Split('/')[2],
                    WeekDay = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(today.AddDays(i).DayOfWeek)[2]
                };

                frontFalseDates.Add(falseDates);
            }

            //將 falseDates 塞入資料裡
            var newDataList = frontFalseDates.Take(interval).Concat(dateList).ToArray();

            //計算 newDataList 總共有幾天
            int newDataListLength = newDataList.Count(); //  17

            //判斷 newDataList 總天數是否能被 7 整除
            //如果不行，則需要在 dateList 後面再補上剩餘的 falseDates 湊足一周 7 天 
            if (newDataListLength % 7 == 0)
                return Ok(newDataList);
            else
            {
                //須補足的天數
                int days = 7 - (newDataListLength % 7);

                var endFalseDates = new List<object>();
                //產出結尾需補足資料
                for (int i = 0; i < days; i++)
                {
                    var falseDates = new
                    {
                        Year = lastDayOfAvailable.AddDays(i + 1).ToShortDateString().Split('/')[0],
                        Month = lastDayOfAvailable.AddDays(i + 1).ToShortDateString().Split('/')[1],
                        Date = lastDayOfAvailable.AddDays(i + 1).ToShortDateString().Split('/')[2],
                        WeekDay = CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(lastDayOfAvailable.AddDays(i + 1).DayOfWeek)[2]
                    };

                    endFalseDates.Add(falseDates);
                }

                //再將 falseDates 塞入資料裡
                var allDataList = newDataList.Concat(endFalseDates.Take(days)).ToArray();

                return Ok(allDataList);
            }




            //如果 Today 5/30 < 可約第一天 6/1，interval = 2
            // 要在 6/1 ~ 6/15 這包資料前面追加 2 天假資料
            // 假資料 + 真資料的陣列長度如果 %7 != 0，則 可約的最後一天後面要再補假假資料

            //如果 Today 6/3 > 可約第一天 6/1，interval = -2
            // 6/1 ~ 6/15 這包資料裡，要跳過前 Skip(interval天)  ( 6/1 & 6/2 ) 後，取 7 筆資料




            ////總天數
            //var howManyDates = dateList.Count();

            ////總周數
            //int weekNum = 0;
            //int pageSize = 5;
            //if (howManyDates % pageSize == 0)
            //    weekNum = howManyDates / pageSize;
            //else
            //    weekNum = howManyDates / pageSize + 1;

            //if (dateList.FirstOrDefault().WeekDay.ToString() == "四")
            //{
            //    var data = dateList.Take(3).ToList();
            //    return Ok(data);
            //}
            //else
            //{
            //    return BadRequest("失敗");
            //}


            //ApiResponse result = new ApiResponse { };
            //result.Success = true;
            //result.Message = "成功取得預約時段";
            //result.Data = dateList;
            //return Ok(weekNum);

            //分頁功能
            //ViewModel.SearchingCounselors data = new ViewModel.SearchingCounselors();
            //data.TotalPageNum = pageNum;
            //data.CounselorsData = new List<ViewModel.CounselorsData>();

            //var counsleorList = Counselors
            //    .Select(x => new
            //    {
            //        x.MyCounselor.Id,
            //        x.MyCounselor.Photo,
            //        x.MyCounselor.Name,
            //        x.MyCounselor.SellingPoint,
            //        x.MyCounselor.SelfIntroduction
            //    })
            //    .Distinct()
            //    .OrderBy(x => x.Id)
            //    .Skip((page - 1) * pageSize)
            //    .Take(pageSize)
            //    .ToList();

        }

    }

}
