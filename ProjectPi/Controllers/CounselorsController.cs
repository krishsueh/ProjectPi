using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using NSwag.Annotations;
using ProjectPi.Models;
using ProjectPi.Security;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ProjectPi.Controllers
{
    [OpenApiTag("Counselors", Description = "諮商師操作功能")]
    public class CounselorsController : ApiController
    {
        PiDbContext _db = new PiDbContext();

        /// <summary>
        /// 取得諮商師基本資料
        /// </summary>
        /// <returns></returns>
        [Route("api/counselors")]
        [JwtAuthFilter]
        [HttpGet]
        public IHttpActionResult GetCounselors()
        {
            var counselorToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int counselorId = (int)counselorToken["Id"];
            var data = _db.Counselors
                .Where(x => x.Id == counselorId)
                .Select(x => new
                {
                    Account = x.Account,
                    CounselorName = x.Name,
                    LicenseImg = x.LicenseImg,
                    CertNumber = x.CertNumber,
                    Photo = x.Photo,
                    SellingPoint = x.SellingPoint,
                    SelfIntroduction = x.SelfIntroduction,
                    VideoLink = x.VideoLink,
                    IsVideoOpen = x.IsVideoOpen,
                    AccountStatus = x.Validation
                });

            ApiResponse result = new ApiResponse { };
            result.Success = true;
            result.Message = "成功取得諮商師基本資料";
            result.Data = data;
            return Ok(result);
        }

        /// <summary>
        /// 修改諮商師基本資料
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [Route("api/counselors")]
        [JwtAuthFilter]
        [HttpPut]
        public IHttpActionResult PutCounselors(ViewModel_C.Profile view)
        {
            if (view.Name == null)
                return BadRequest("姓名欄必填");
            else if (view.LicenseImg == null)
                return BadRequest("請上傳執照");
            else
            {
                var counselorToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
                int counselorId = (int)counselorToken["Id"];
                var haveCounselor = _db.Counselors
                .Where(x => x.Id == counselorId).FirstOrDefault();

                if (haveCounselor != null)
                {
                    haveCounselor.Name = view.Name;
                    //haveCounselor.LicenseImg = view.LicenseImg;
                    //haveCounselor.Photo = view.Photo;
                    haveCounselor.SellingPoint = view.SellingPoint;
                    haveCounselor.SelfIntroduction = view.SelfIntroduction;
                    haveCounselor.VideoLink = view.VideoLink;
                    haveCounselor.IsVideoOpen = view.IsVideoOpen;

                    _db.SaveChanges();

                    ApiResponse result = new ApiResponse { };
                    result.Success = true;
                    result.Message = "成功修改基本資料";
                    result.Data = null;
                    return Ok(result);
                }
                else
                    return BadRequest("無此帳號");
            }
        }

        /// <summary>
        /// 儲存諮商師個人頭像
        /// </summary>
        /// <returns></returns>
        [Route("api/uploadHeadshot")]
        [JwtAuthFilter]
        [HttpPost]
        public async Task<IHttpActionResult> UploadHeadshot()
        {
            var counselorToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int counselorId = (int)counselorToken["Id"];
            string counselorName = counselorToken["Name"].ToString();

            // 檢查請求是否包含 multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // 使用 HttpContext.Current.Server.MapPath 方法來獲取指定路徑的物理路徑
            string root = HttpContext.Current.Server.MapPath(@"~/upload/headshot");

            try
            {
                // 讀取 MIME 資料
                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                // 取得檔案副檔名，單檔用.FirstOrDefault()直接取出，多檔需用迴圈
                string fileNameData = provider.Contents.FirstOrDefault().Headers.ContentDisposition.FileName.Trim('\"');
                string fileType = fileNameData.Remove(0, fileNameData.LastIndexOf('.')); // .jpg

                // 定義檔案名稱
                string fileName = counselorId + "-" + counselorName + "-" + DateTime.Now.ToString("yyyyMMddHHmmss") + fileType;

                // 儲存圖片，單檔用.FirstOrDefault()直接取出，多檔需用迴圈
                var fileBytes = await provider.Contents.FirstOrDefault().ReadAsByteArrayAsync();
                var filePath = Path.Combine(root, fileName);

                // 創建文件流
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    // 寫入文件內容
                    await fileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                }

                // 使用 SixLabors.ImageSharp 調整圖片尺寸 (正方形大頭貼)
                var image = SixLabors.ImageSharp.Image.Load<Rgba32>(filePath);
                image.Mutate(x => x.Resize(640, 640));
                image.Save(filePath);

                // 將頭像路徑存入資料庫
                Counselor haveCounselor = _db.Counselors
                .Where(x => x.Id == counselorId).FirstOrDefault();
                haveCounselor.Photo = fileName;
                _db.SaveChanges();

                ApiResponse result = new ApiResponse { };
                result.Success = true;
                result.Message = "成功上傳諮商師個人頭像";
                result.Data = null;
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("個人頭像上傳失敗或未上傳");
            }
        }

        /// <summary>
        /// 更新/補件諮商師執照
        /// </summary>
        /// <returns></returns>
        [Route("api/updateLicense")]
        [JwtAuthFilter]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateLicense()
        {
            var counselorToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int counselorId = (int)counselorToken["Id"];
            string counselorName = counselorToken["Name"].ToString();

            // 檢查請求是否包含 multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // 使用 HttpContext.Current.Server.MapPath 方法來獲取指定路徑的物理路徑
            string root = HttpContext.Current.Server.MapPath(@"~/upload/license");

            try
            {
                // 讀取 MIME 資料
                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);

                // 取得檔案副檔名，單檔用.FirstOrDefault()直接取出，多檔需用迴圈
                string fileNameData = provider.Contents.FirstOrDefault().Headers.ContentDisposition.FileName.Trim('\"');
                string fileType = fileNameData.Remove(0, fileNameData.LastIndexOf('.')); // .jpg

                // 定義檔案名稱
                string fileName = "License_" + DateTime.Now.ToString("yyyyMMddHHmmss") + fileType;

                // 儲存圖片，單檔用.FirstOrDefault()直接取出，多檔需用迴圈
                var fileBytes = await provider.Contents.FirstOrDefault().ReadAsByteArrayAsync();
                var filePath = Path.Combine(root, fileName);

                // 創建文件流
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    // 寫入文件內容
                    await fileStream.WriteAsync(fileBytes, 0, fileBytes.Length);
                }

                // 使用 SixLabors.ImageSharp 調整圖片尺寸 (正方形大頭貼)
                var image = SixLabors.ImageSharp.Image.Load<Rgba32>(filePath);
                image.Mutate(x => x.Resize(640, 0));
                image.Save(filePath);

                // 將頭像路徑存入資料庫
                Counselor haveCounselor = _db.Counselors
                .Where(x => x.Id == counselorId).FirstOrDefault();
                haveCounselor.LicenseImg = fileName;
                _db.SaveChanges();

                ApiResponse result = new ApiResponse { };
                result.Success = true;
                result.Message = "成功更新諮商師執照";
                result.Data = null;
                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest("執照上傳失敗或未上傳");
            }
        }

        /// <summary>
        /// 新增/修改課程資訊
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        [Route("api/courses")]
        [JwtAuthFilter]
        [HttpPost]
        public IHttpActionResult PostCourses(ViewModel_C.Course view)
        {
            if (!ModelState.IsValid)
                return BadRequest("欄位驗證有誤");

            var counselorToken = JwtAuthFilter.GetToken(Request.Headers.Authorization.Parameter);
            int counselorId = (int)counselorToken["Id"];

            var productExist = _db.Products
                .Where(c => c.CounselorId == counselorId && c.FieldId == view.FieldId)
                .FirstOrDefault();
            var hasFeature = _db.Features
                .Where(c => c.CounselorId == counselorId && c.FieldId == view.FieldId)
                .FirstOrDefault();

            if (productExist != null) //已新增過該專業領域的課程資訊 -> 修改
            {
                for (int i = 0; i < view.Courses.Length; i++)
                {
                    var item = view.Courses[i];
                    var findItem = _db.Products
                        .Where(c => c.CounselorId == counselorId && c.FieldId == view.FieldId && c.Item == item.Item)
                        .FirstOrDefault();

                    findItem.Price = view.Courses[i].Price;
                    findItem.Availability = view.Courses[i].Availability;
                    _db.SaveChanges();
                }

                hasFeature.Feature1 = view.Features.Feature1;
                hasFeature.Feature2 = view.Features.Feature2;
                hasFeature.Feature3 = view.Features.Feature3;
                hasFeature.Feature4 = view.Features.Feature4;
                hasFeature.Feature5 = view.Features.Feature5;
                _db.SaveChanges();

                ApiResponse result = new ApiResponse { };
                result.Success = true;
                result.Message = "成功修改課程資訊";
                result.Data = null;
                return Ok(result);
            }
            else //從來沒有新增過該專業領域的課程資訊 -> 新增
            {
                for (int i = 0; i < view.Courses.Length; i++)
                {
                    var item = view.Courses[i];
                    Product product = new Product();
                    product.CounselorId = counselorId;
                    product.FieldId = view.FieldId;
                    product.Item = item.Item;
                    product.Quantity = item.Quantity;
                    product.Price = item.Price;
                    product.Availability = item.Availability;

                    _db.Products.Add(product);
                    _db.SaveChanges();
                }

                Feature feature = new Feature();
                feature.CounselorId = counselorId;
                feature.FieldId = view.FieldId;
                feature.Feature1 = view.Features.Feature1;
                feature.Feature2 = view.Features.Feature2;
                feature.Feature3 = view.Features.Feature3;
                feature.Feature4 = view.Features.Feature4;
                feature.Feature5 = view.Features.Feature5;

                _db.Features.Add(feature);
                _db.SaveChanges();

                ApiResponse result = new ApiResponse { };
                result.Success = true;
                result.Message = "成功新增課程資訊";
                result.Data = null;
                return Ok(result);
            }
        }
    }
}
