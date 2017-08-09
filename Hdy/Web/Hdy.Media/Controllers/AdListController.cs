using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Newtonsoft.Json;

using Framework.Common.Json;
using Framework.Auth;
using Framework.Common;
using Framework.Common.IO;
using Framework.Common.Mvc;

using Hdy.Media.Models;
using Hdy.Media.ViewModels;

namespace Hdy.Media.Controllers
{
    public class AdListController : MediaBaseController
    {
        protected readonly AdvertisementModelManager _AdvertisementManager;
        protected readonly AdvertisementDetailModelManager _AdvertisementDetailManager;
        protected readonly SchedulingModelManager _SchedulingManager;

        string now = DateTime.Now.ToString("yyyy-MM-dd");

        static object lockObj = new object();

        public AdListController()
        {
            _AdvertisementManager = _AdvertisementManager ?? new AdvertisementModelManager();
            _AdvertisementDetailManager = _AdvertisementDetailManager ?? new AdvertisementDetailModelManager();
            _SchedulingManager = _SchedulingManager ?? new SchedulingModelManager();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Create(string aId, string mId)
        {
            var model = await CreateOrUpdate(string.Empty, mId);
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Update(string aId, string mId)
        {
            var model = await CreateOrUpdate(aId, mId);

            return View("Create", model);
        }

        private async Task<AdvertisementModel> CreateOrUpdate(string aId, string mId)
        {
            var model = new AdvertisementModel
            {
                MerchantID = mId
            };
            if (string.IsNullOrEmpty(aId))
            {
                ViewBag.Title = "提交内容";
                return model;
            }
            else
            {
                ViewBag.Title = "修改内容";
                return await _AdvertisementManager.GetByIdAsync(aId, true);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create(AdvertisementModel model)
        {
            try
            {
                var validateResult = await ValidateAdNameExist(model.ID, model.Name);
                if (validateResult.Data.ToString() == bool.FalseString)
                {
                    return Fail(ErrorCode.ModelValidateError, "保存失败：内容名称己存在！");
                }

                model.Operator = User.Identity.Name;
                model.UserID = UserId;

                if (!string.IsNullOrEmpty(model.RefusalReasons))
                {
                    model.RefusalReasons = null;
                    model.RecordState = RecordStates.PendingAudit;
                }

                //更新 detail 
                if (!string.IsNullOrEmpty(model.ID))
                {
                    var redirectUrls = GetRedirectUrls();
                    if (redirectUrls.Count > 0)
                    {
                        using (var db = new MediaDbContext())
                        {
                            var fileNames = redirectUrls.Select(m => m.Name);
                            var detailList = db.AdvertisementDetailModels.Where(m => m.AdvertisementID == model.ID && fileNames.Contains(m.FileName)).ToList();
                            redirectUrls.ForEach(url =>
                            {
                                var detail = detailList.FirstOrDefault(m => m.FileName == url.Name);
                                if (detail != null && detail.RedirectUrl != url.RedirectUrl)
                                    detail.RedirectUrl = GetRedirectUrl(url.RedirectUrl);
                            });
                            db.SaveChanges();
                        }
                    }
                }

                await _AdvertisementManager.SaveAsync(model);

                return Success(model.ID);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.DataError, e.Message);
            }
        }

        private List<RedirectUrlViewModel> GetRedirectUrls()
        {
            var urls = Request.Form["RedirectUrls"];
            var redirectUrls = new List<RedirectUrlViewModel>();
            if (!string.IsNullOrEmpty(urls))
            {
                redirectUrls.AddRange(JsonConvert.DeserializeObject<List<RedirectUrlViewModel>>(urls));
            }
            return redirectUrls;
        }

        /// <summary>
        /// 上传内容
        /// </summary>
        /// <param name="merchantId">商户Id</param>
        /// <param name="aId">广告内容Id</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public JsonResult Upload(string mId, string aId)
        {
            try
            {
                lock (lockObj)
                {
                    if (string.IsNullOrEmpty(mId) || string.IsNullOrEmpty(aId))
                        throw new ArgumentNullException("文件上传失败，缺少mId或者aId参数！");

                    string imgPath = string.Concat(AppSetting.AdImagePath, mId, "/"),
                        videoPath = string.Concat(AppSetting.AdVideoPath, mId, "/");

                    FileHelper filesHelper = new FileHelper(imgPath, videoPath, Request.Url.Authority);

                    var resultList = filesHelper.Upload(HttpContext);
                    var redirectUrls = GetRedirectUrls();

                    if (resultList.Count > 0)
                    {
                        var detailList = resultList.Select(item => new AdvertisementDetailModel
                        {
                            ID = Guid.NewGuid().ToString(),
                            AdvertisementID = aId,
                            ContentType = item.type,
                            FileName = item.name.Length > 14 ? item.name.Substring(item.name.Length - 14) : item.name,
                            Operator = User.Identity.Name,
                            Size = item.size,
                            RedirectUrl = GetRedirectUrl(redirectUrls.FirstOrDefault(r => r.Name == item.name)?.RedirectUrl),
                            StoragePath = item.path,
                        }).ToList();

                        using (var db = new MediaDbContext())
                        {
                            db.AdvertisementDetailModels.AddRange(detailList);
                            db.SaveChanges();
                        }

                        var result = new UploadJsonFiles(detailList.Select(item => new UploadFilesResult
                        {
                            deleteUrl = string.Concat("/adList/DeleteFile?detailId=", item.ID),
                            name = item.FileName,
                            size = item.Size,
                            thumbnailUrl = filesHelper.GetThumbUrl(item.ContentType, item.FileName),
                            type = item.ContentType
                            //  url = item.url
                        }).ToList());
                        return Json(result);
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception e)
            {
                return Json(e.Message);
            }
        }

        string GetRedirectUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            if (url.IndexOf("http://") == -1 && url.IndexOf("https://") == -1)
            {
                return "http://" + url;
            }
            return url;
        }

        public async Task<PartialViewResult> GetPlayTimesSelectorPartial(string adId, string schedulingId)
        {
            var model = new List<PlayTimesSelectorViewModel>();
            if (!string.IsNullOrEmpty(adId))
            {
                var ad = await _AdvertisementManager.GetByIdAsync(adId);
                if (!string.IsNullOrEmpty(ad.PlayTimes))
                    model = JsonConvert.DeserializeObject<List<PlayTimesSelectorViewModel>>(ad.PlayTimes);
            }
            if (!string.IsNullOrEmpty(schedulingId))
            {
                var scheduling = await _SchedulingManager.GetByIdAsync(schedulingId);
                if (!string.IsNullOrEmpty(scheduling.PlayTimes))
                    model = JsonConvert.DeserializeObject<List<PlayTimesSelectorViewModel>>(scheduling.PlayTimes);
            }
            if (model.Count == 0)
            {
                model = PlayTimesSelectorViewModel.Items;
            }

            return PartialView("_PlayTimesSelectorPartial", model);
        }

        [HttpGet]
        public JsonResult DeleteFile(string detailId)
        {
            try
            {
                DeleteAd(detailId);

                return Json("OK", JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json("error", JsonRequestBehavior.AllowGet);
            }
        }

        private bool DeleteAd(string detailId)
        {
            var adDetail = _AdvertisementDetailManager.GetById(detailId);
            var fileHelper = GetFileHelper(adDetail.AdvertisementID);
            string file = fileHelper.GetStorageFullName(adDetail.FileName);
            FileHelper.DeleteFile(file);

            return _AdvertisementDetailManager.Delete(detailId, true);
        }

        [HttpPost]
        public JsonResult Delete(string id)
        {
            var details = _AdvertisementDetailManager.GetListByAdId(id);
            foreach (var d in details)
            {
                DeleteAd(d.ID);
            }

            try
            {
                var result = _AdvertisementManager.Delete(id, true);
                return Success(result);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.ProcessError, e.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetList(AdQueryViewModel model, int pageIndex, int pageSize)
        {
            try
            {
                if (!User.IsInRole(AuthSetting.AdminRoleName))
                {
                    model.UserID = UserId;
                }

                var result = await _AdvertisementManager.QueryAsync(model, pageIndex, pageSize).ConfigureAwait(false);

                result.Data.ForEach(item => { item.Merchant = null; });

                return Success(result);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.DataError, e.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetDetailList(string adId)
        {
            try
            {
                var items = await _AdvertisementDetailManager.QueryAsync(item => item.AdvertisementID == adId && item.RecordState == RecordStates.AuditPass);
                var fileHelper = GetFileHelper(adId);
                var result = items.Data.Select(item => new AdvertisementDetailModel
                {
                    ID = item.ID,
                    Remark=$"/AdList/PlayVideo?id={item.ID}",//借用这个字段
                    FileName = item.FileName,
                    ContentType = item.ContentType,
                    Size = item.Size,
                    RedirectUrl = item.RedirectUrl,
                    StoragePath = string.Concat(item.ContentType.Contains("image") ?
                    AppSetting.AdImagePath : AppSetting.AdVideoPath, item.StoragePath),
                    AdvertisementID = item.AdvertisementID,
                    ThumbPath = fileHelper.GetThumbUrl(item.ContentType, item.FileName)
                });

                return Success(result);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.DataError, e.Message);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetFileList(string adId)
        {
            try
            {
                var items = await _AdvertisementDetailManager.QueryAsync(item => item.AdvertisementID == adId && item.RecordState != RecordStates.DeleteTag);
                var result = new List<UploadFilesResult>();
                var fileHelper = GetFileHelper(adId);
                if (items.Data.Count > 0)
                {
                    result.AddRange(items.Data.Select(item => new UploadFilesResult
                    {
                        deleteUrl = string.Concat("/adList/DeleteFile?detailId=", item.ID),
                        name = item.FileName,
                        size = item.Size,
                        redirectUrl = item.RedirectUrl,
                        thumbnailUrl = fileHelper.GetThumbUrl(item.ContentType, item.FileName),
                        type = item.ContentType
                    }));
                }

                return Success(result);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.DataError, e.Message);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult PlayVideo(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return HttpNotFound();
            }

            var file = _AdvertisementDetailManager.GetById(id);

            return new VideoResult(file?.StoragePath);
        }

        /// <summary>
        /// 内容审核
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> Check(string id, string reasons, RecordStates checkState)
        {
            try
            {
                var item = await _AdvertisementManager.GetByIdAsync(id);
                item.RecordState = checkState;
                if (item.RecordState == RecordStates.AuditFailure)
                    item.RefusalReasons = reasons;

                item.Operator = User.Identity.Name;
                await _AdvertisementManager.SaveAsync(item);

                return Success(true);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.ProcessError, e.Message);
            }
        }

        private FileHelper GetFileHelper(string adId)
        {
            var ad = _AdvertisementManager.GetById(adId);
            if (ad == null)
                throw new Exception($"不存在Id为{adId}的Advertisement对象");

            string imgPath = string.Concat(AppSetting.AdImagePath, ad.MerchantID, "\\"),
           videoPath = string.Concat(AppSetting.AdVideoPath, ad.MerchantID, "\\");

            return new FileHelper(imgPath, videoPath, Request.Url.Authority);
        }

        [AllowAnonymous]
        public async Task<JsonResult> ValidateAdNameExist(string id, string name)
        {
            if (string.IsNullOrEmpty(name))
                return Json(false, JsonRequestBehavior.AllowGet);

            bool isExists = await _AdvertisementManager.GetByExpreAsync(item =>
             item.Name == name.Trim().Replace("\t", "") && item.ID != id) == null;

            return Json(isExists, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_SchedulingManager != null)
                {
                    _SchedulingManager.Dispose();
                }

                if (_AdvertisementDetailManager != null)
                {
                    _AdvertisementDetailManager.Dispose();
                }

                if (_AdvertisementManager != null)
                {
                    _AdvertisementManager.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}