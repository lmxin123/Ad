using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using Newtonsoft.Json;

using Framework.Common.Json;
using Framework.Auth;
using Framework.Common;
using Framework.Common.IO;

using Hdy.Media.Models;
using Hdy.Media.ViewModels;
using System.Data.Entity.Core;
using System.Web.Routing;

namespace Hdy.Media.Controllers
{
    public class AdSchedulingController : MediaBaseController
    {
        protected readonly DeviceModelManager _DeviceManager;
        protected readonly CorpModelManager _MerchantManager;
        protected readonly SchedulingModelManager _SchedulingManager;
        protected readonly SchedulingDetailModelManager _SchedulingDetailManager;
        protected readonly AdvertisementModelManager _AdvertisementManager;
        protected readonly AdvertisementDetailModelManager _AdvertisementDetailManager;
        protected readonly AdPackageModelManager _AdPackageModelManager;
        protected readonly AdDowloadRecordModelManager _AdDowloadRecordModelManager;

        private AdListController _AdListController;

        string now = DateTime.Now.ToString("yyyy-MM-dd");

        public AdSchedulingController()
        {
            _DeviceManager = _DeviceManager ?? new DeviceModelManager();
            _MerchantManager = _MerchantManager ?? new CorpModelManager();
            _SchedulingManager = _SchedulingManager ?? new SchedulingModelManager(_DeviceManager.Db);
            _SchedulingDetailManager = _SchedulingDetailManager ?? new SchedulingDetailModelManager(_DeviceManager.Db);
            _AdvertisementManager = _AdvertisementManager ?? new AdvertisementModelManager();
            _AdvertisementDetailManager = _AdvertisementDetailManager ?? new AdvertisementDetailModelManager();
            _AdPackageModelManager = _AdPackageModelManager ?? new AdPackageModelManager();
            _AdDowloadRecordModelManager = _AdDowloadRecordModelManager ?? new AdDowloadRecordModelManager();
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            _AdListController = new AdListController
            {
                ControllerContext = ControllerContext
            };
        }

        public ActionResult Index()
        {
            return View();
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

        [HttpPost]
        public async Task<JsonResult> GetDetailList(string adId)
        {
            try
            {
                var items = await _AdvertisementDetailManager.QueryAsync(item => item.AdvertisementID == adId && item.RecordState == RecordStates.AuditPass);
                var fileHelper = await GetFileHelper(adId);
                var result = items.Data.Select(item => new AdvertisementDetailModel
                {
                    ID = item.ID,
                    FileName = item.FileName,
                    ContentType = item.ContentType,
                    Size = item.Size,
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

        [HttpPost]
        public async Task<JsonResult> GetHasSchedulingDevices(string queryText, string deviceId)
        {
            var result = new GeneralResponseModel<List<object>>();
            string now = DateTime.Now.ToString("yyyy-MM-dd");

            try
            {
                using (var db = _DeviceManager.Db)
                {
                    if (!string.IsNullOrEmpty(deviceId))
                    {
                        var data = await db.DeviceModels
                                     .Join(db.SchedulingModels, d => d.ID, s => s.DeviceID, (d, s) => new { d.ID, d.Name, s.EndPlayDate })
                                     .Where(item => item.ID == deviceId && string.Compare(item.EndPlayDate, now, StringComparison.Ordinal) > 0)
                                     .Select(item => new { ID = item.ID, Name = item.Name })
                                     .Distinct()
                                     .ToListAsync<object>()
                                     .ConfigureAwait(false);

                        result.Data = data;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(queryText))
                        {
                            var data = await db.DeviceModels
                                 .Join(db.SchedulingModels, d => d.ID, s => s.DeviceID, (d, s) => new { d.ID, d.Name, s.EndPlayDate })
                                 .Where(item => string.Compare(item.EndPlayDate, now, StringComparison.Ordinal) > 0)
                                 .Select(item => new { ID = item.ID, Name = item.Name })
                                 .Distinct()
                                 .ToListAsync<object>()
                                 .ConfigureAwait(false);

                            result.Data = data;
                        }
                        else
                        {
                            var data = await db.DeviceModels
                                 .Join(db.SchedulingModels, d => d.ID, s => s.DeviceID, (d, s) => new { d, s.EndPlayDate })
                                 .Where(item =>
                                 string.Compare(item.EndPlayDate, now, StringComparison.Ordinal) > 0 &&
                                               item.d.Name.Contains(queryText) ||
                                               item.d.Email.Contains(queryText) ||
                                               item.d.DeviceCode.Contains(queryText) ||
                                               item.d.Contact.Contains(queryText) ||
                                               item.d.Address.Contains(queryText) ||
                                               item.d.Operator.Contains(queryText) ||
                                               item.d.Phone.Contains(queryText))
                                 .Select(item => new { ID = item.d.Name, Name = item.d.Name })
                                 .Distinct()
                                 .ToListAsync<object>()
                                 .ConfigureAwait(false);

                            result.Data = data;
                        }
                    }
                }

                if (result.Data.Count == 0)
                    throw new Exception("未找到有效排期的终端！");

                return Success(result);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.DataError, e.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetDeviceList(string queryText, string filterDeviceId)
        {
            var result = new GeneralResponseModel<List<DeviceModel>>();
            try
            {
                if (string.IsNullOrEmpty(queryText))
                {
                    result.Data = await _DeviceManager
                        .Model
                        .OrderByDescending(item => item.CreateDate)
                        .ToListAsync()
                        .ConfigureAwait(false);
                }
                else
                {
                    result.Data = (await _DeviceManager.QueryAsync(item =>
                                           item.Name.Contains(queryText) ||
                                            item.Email.Contains(queryText) ||
                                          item.DeviceCode.Contains(queryText) ||
                                            item.Contact.Contains(queryText) ||
                                          item.Address.Contains(queryText) ||
                                           item.Operator.Contains(queryText) ||
                                           item.Phone.Contains(queryText)).ConfigureAwait(false))
                                           .Data
                                           .OrderByDescending(item => item.CreateDate)
                                           .ToList();
                }

                var filterDevice = result.Data.Find(item => item.ID == filterDeviceId);
                if (!string.IsNullOrEmpty(filterDeviceId) && filterDevice != null)
                    result.Data.Remove(filterDevice);

                return Success(result);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.DataError, e.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetListByDeviceId(string deviceId)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceId))
                    throw new ArgumentNullException("deviceId参数不能为空");

                var data = (await _SchedulingManager
                    .QueryAsync(item => item.DeviceID == deviceId && string.Compare(item.EndPlayDate, now, StringComparison.Ordinal) > 0).ConfigureAwait(false))
                 .Data.Select(item => new
                 {
                     item.ID,
                     item.Name,
                     item.BeginPlayDate,
                     item.EndPlayDate,
                     item.PlayCount,
                     PlayTimeDisplay = item.PlayTimeDisplay
                 });

                return Success(data);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.DataError, e.Message);
            }
        }

        public async Task<JsonResult> GetAdList(AdQueryViewModel model, int pageIndex, int pageSize)
        {
            return await _AdListController.GetList(model, pageIndex, pageSize);
        }
        public async Task<JsonResult> Create(AddSchedulingViewModel model)
        {
            using (var db = _SchedulingManager.Db)
            {
                using (var trans = db.Database.BeginTransaction())
                {
                    try
                    {
                        if (string.IsNullOrEmpty(model.AdIDs))
                            throw new ArgumentException("请选择播放内容！");

                        var validateResult = await ValidateNameExist(model.Name, model.ID);
                        if (validateResult.Data.ToString() == bool.FalseString)
                            throw new ArgumentException("排期名称！");

                        var scheduling = new SchedulingModel
                        {
                            BeginPlayDate = DateTime.Parse(model.BeginPlayDate).ToString("yyyy-MM-dd"),
                            EndPlayDate = DateTime.Parse(model.EndPlayDate).ToString("yyyy-MM-dd"),
                            DeviceID = model.DeviceID,
                            ID = model.ID,
                            Loop = model.Loop,
                            Name = model.Name,
                            Operator = User.Identity.Name,
                            Order = model.Order,
                            PlayCount = model.PlayCount,
                            PlayTimes = model.PlayTimes,
                            Remark = model.Remark
                        };

                        await _SchedulingManager.SaveAsync(scheduling);
                        string[] adids = model.AdIDs.Split('|');
                        for (int i = 0; i < adids.Length; i++)
                        {
                            string aid = adids[i];
                            var detail = (await _SchedulingDetailManager
                                .QueryAsync(s => s.SchedulingID == scheduling.ID && s.AdvertisementID == aid)
                                .ConfigureAwait(false))
                                .Data
                                .FirstOrDefault();

                            if (detail == null)
                            {
                                detail = new SchedulingDetailModel
                                {
                                    ID = Guid.NewGuid().ToString(),
                                    AdvertisementID = adids[i],
                                    Operator = User.Identity.Name,
                                    SchedulingID = scheduling.ID
                                };
                                db.SchedulingDetailModels.Add(detail);
                                db.SaveChanges();
                            }
                        }

                        if (!string.IsNullOrEmpty(model.DelAdIDs))
                        {
                            string[] dAdIds = model.DelAdIDs.Split('|');
                            for (int i = 0; i < dAdIds.Length; i++)
                            {
                                string aid = dAdIds[i];
                                var deleteItem = (await _SchedulingDetailManager.QueryAsync(s => s.SchedulingID == scheduling.ID && s.AdvertisementID == aid)).Data;
                                if (deleteItem.Count > 0)
                                {
                                    await _SchedulingDetailManager.DeleteAsync(deleteItem[0], true).ConfigureAwait(false);
                                }
                            }
                        }

                        trans.Commit();
                        return Success(true);
                    }
                    catch (Exception e)
                    {
                        trans.Rollback();
                        return Fail(ErrorCode.DataError, e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 把未过期的所有排期复制到指定终端
        /// </summary>
        /// <param name="schedulingIds"></param>
        /// <param name="deviceIds"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionAuthorize(ActionType = ActionTypes.Create)]
        public async Task<JsonResult> CopyTo(string deviceIdLeft, List<string> deviceIds, string[] schedulingIds)
        {
            try
            {
                if (string.IsNullOrEmpty(deviceIdLeft) || deviceIds.Count == 0)
                    throw new ArgumentNullException("参数有误！");

                var schedulings = new List<SchedulingModel>();
                if (schedulingIds != null && schedulingIds.Length > 0)
                {
                    schedulings.AddRange(await _SchedulingManager.GetByIdsAsync(schedulingIds).ConfigureAwait(false));
                }
                else
                {
                    schedulings.AddRange((await _SchedulingManager.QueryAsync(item =>
                    item.DeviceID == deviceIdLeft &&
                    string.Compare(item.EndPlayDate, now, StringComparison.Ordinal) > 0)
                    .ConfigureAwait(false)).Data);
                }

                if (schedulings.Count == 0)
                    throw new ArgumentNullException("未找到可以被复制的排期！");

                List<string> newSchedulingIds = new List<string>();//用于返回到前端，撤销用
                using (var db = _SchedulingManager.Db)
                {
                    using (var trans = db.Database.BeginTransaction())
                    {
                        try
                        {
                            schedulings.ForEach(scheduling =>
                            {
                                deviceIds.ForEach(deviceId =>
                                {
                                    if (ValidateScheduledDateIsOverlapping(deviceId, scheduling))
                                        throw new Exception("复制失败，被复制终端排期与目标终端排期的日期有重叠，不允许复制！");

                                    var newScheduling = new SchedulingModel
                                    {
                                        ID = Guid.NewGuid().ToString(),
                                        BeginPlayDate = scheduling.BeginPlayDate,
                                        DeviceID = deviceId,
                                        EndPlayDate = scheduling.EndPlayDate,
                                        Loop = scheduling.Loop,
                                        Name = GenerateScheduledNameWhenCopy(scheduling.Name),
                                        Operator = User.Identity.Name,
                                        Order = scheduling.Order,
                                        PlayCount = scheduling.PlayCount,
                                        PlayTimes = scheduling.PlayTimes,
                                        RecordState = scheduling.RecordState,
                                        Remark = $"{scheduling.Remark}【此排期从{scheduling.Name}复制而来】",
                                        SchedulingStatu = scheduling.SchedulingStatu
                                    };

                                    db.SchedulingModels.Add(newScheduling);
                                    db.SaveChanges();
                                    newSchedulingIds.Add(newScheduling.ID);

                                    var details = _SchedulingDetailManager.QueryAsync(detail => detail.SchedulingID == scheduling.ID).Result.Data;

                                    var newDetails = details.Select(a => new SchedulingDetailModel
                                    {
                                        ID = Guid.NewGuid().ToString(),
                                        SchedulingID = newScheduling.ID,
                                        AdvertisementID = a.AdvertisementID,
                                        Operator = User.Identity.Name,
                                        RecordState = a.RecordState,
                                        Remark = a.Remark,
                                    }).ToList();

                                    db.SchedulingDetailModels.AddRange(newDetails);
                                    db.SaveChanges();
                                });
                            });

                            trans.Commit();
                        }
                        catch (Exception e)
                        {
                            trans.Rollback();
                            throw new Exception(e.Message);
                        }
                    }
                }

                return Success(newSchedulingIds);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.DataError, e.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">name 不能为空</exception>
        private string GenerateScheduledNameWhenCopy(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name 不能为空");

            int start = name.IndexOf("["), end = name.LastIndexOf("]");
            string newName = string.Empty;
            if (start > 0 && end > 0)
                newName = name.Substring(start + 1, end - 2);
            else
                newName = name;

            Random rand = new Random();
            string randNum = rand.Next(1000).ToString("d4");

            string scheduledName = $"从[{newName}]复制{randNum}";
            return scheduledName;
        }

        private bool ValidateScheduledDateIsOverlapping(string deviceId, SchedulingModel scheduling)
        {
            var existItem = _SchedulingManager.QueryAsync(item => item.DeviceID == deviceId &&
               ((string.Compare(item.BeginPlayDate, scheduling.BeginPlayDate, StringComparison.Ordinal) <= 0 &&
                 string.Compare(item.EndPlayDate, scheduling.BeginPlayDate, StringComparison.Ordinal) >= 0) ||
                     (string.Compare(item.BeginPlayDate, scheduling.EndPlayDate, StringComparison.Ordinal) <= 0 &&
                 string.Compare(item.EndPlayDate, scheduling.EndPlayDate, StringComparison.Ordinal) >= 0))).Result;

            return existItem.Data.Count > 0;
        }
        /// <summary>
        /// 取消排期复制的数据
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ActionAuthorize(ActionType = ActionTypes.Create)]
        public JsonResult CancelCopy(List<string> schedulingIds)
        {
            try
            {
                if (schedulingIds.Count == 0)
                    throw new ArgumentNullException("参数有误！");
                using (var db = _SchedulingManager.Db)
                {
                    schedulingIds.ForEach(id =>
                {
                    var items = db.SchedulingModels.Where(s => schedulingIds.Contains(s.ID));
                    if (items.Count() > 0)
                    {
                        db.SchedulingModels.RemoveRange(items);
                        db.SaveChanges();
                    }
                });
                }

                return Success();
            }
            catch (EntityException)
            {
                return Fail(ErrorCode.DataError, "排期己撤销，请不要撤销！");
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.DataError, e.Message);
            }
        }

        public async Task<JsonResult> GetList(SchedulingQueryViewModel model, int pageIndex, int pageSize = 20)
        {
            try
            {
                var queryDeivce = new DeviceModel
                {
                    Province = model.Province,
                    City = model.City,
                    Region = model.Region,
                    OpenArea = model.OpenArea
                };
                var devices = await _DeviceManager.QueryAsync(queryDeivce, pageIndex, pageSize).ConfigureAwait(false);
                model.DevicesIds = devices.Data.Select(item => item.ID).ToArray();
                var schedulings = await _SchedulingManager.QueryAsync(model, pageIndex, pageSize).ConfigureAwait(false);

                var items = new List<object>();

                devices.Data.ForEach(item =>
                {
                    var devicesSchedulings = schedulings
                    .Where(m => m.DeviceID == item.ID)
                    .Select(scheduling => new
                    {
                        scheduling.BeginPlayDate,
                        scheduling.EndPlayDate,
                        scheduling.ID,
                        scheduling.Loop,
                        scheduling.Name,
                        scheduling.Order,
                        scheduling.PlayCount,
                        scheduling.PlayTimes,
                        scheduling.Remark,
                        Details = scheduling.Details?.Select(d => new { d.AdvertisementID }).ToList()
                    }).ToList();

                    items.Add(new
                    {
                        DevieName = item.Name,
                        DeviceId = item.ID,
                        Schedulings = devicesSchedulings
                    });
                });

                return Success(items, devices.TotalCount);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.DataError, e.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException("缺少参数！");

                var item = _SchedulingManager.GetById(id);
                var pg = await _AdPackageModelManager.GetByExpreAsync(a => a.SchedulingID == id);
                if (DateTime.Parse(item.EndPlayDate) > DateTime.Now)
                {
                    if (pg != null)
                    {
                        throw new Exception("该排期己被打包且未过期，不允许删除！");
                    }
                }

                var details = await _SchedulingDetailManager.GetBySIdAsync(id).ConfigureAwait(false);
                foreach (var d in details)
                {
                    _SchedulingDetailManager.Delete(d.ID, true);
                }
                if (pg != null)
                {
                    var dl = await _AdDowloadRecordModelManager.GetByExpreAsync(d => d.AdPackageID == pg.ID);
                    if (dl != null)   _AdDowloadRecordModelManager.Delete(dl.ID, true);

                    _AdPackageModelManager.Delete(pg.ID, true);

                    FileHelper.DeleteFile(pg.PathName);
                }
                _SchedulingManager.Delete(id, true);

                return Success(true);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.DataError, e.Message);
            }
        }

        public async Task<JsonResult> ValidateNameExist(string name, string id)
        {
            if (string.IsNullOrEmpty(name))
                return Json(false, JsonRequestBehavior.AllowGet);

            bool isExists = await _SchedulingManager.GetByExpreAsync(item =>
             item.Name == name.Trim().Replace("\t", "") && item.ID != id) == null;

            return Json(isExists, JsonRequestBehavior.AllowGet);
        }

        private async Task<FileHelper> GetFileHelper(string adId)
        {
            var ad = await _AdvertisementManager.GetByIdAsync(adId);
            if (ad == null)
                throw new Exception($"不存在Id为{adId}的Advertisement对象");

            string imgPath = string.Concat(AppSetting.AdImagePath, ad.MerchantID, "\\"),
           videoPath = string.Concat(AppSetting.AdVideoPath, ad.MerchantID, "\\");

            return new FileHelper(imgPath, videoPath, Request.Url.Authority);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_AdvertisementDetailManager != null)
                {
                    _AdvertisementDetailManager.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}