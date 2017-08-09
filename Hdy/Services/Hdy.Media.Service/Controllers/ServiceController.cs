using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Framework.Common.IO;
using Framework.Common.Json;
using Framework.Common.Mvc;
using Framework.Data;

using Hdy.Media.Models;
using Hdy.Media.Service.ViewModels;
using System.Threading.Tasks;
using Hdy.Media.ViewModels;
using Framework.Common;
using System.IO;

namespace Hdy.Media.Service.Controllers
{
    public class ServiceController : BaseController
    {
        private readonly DeviceModelManager _DeviceManager;
        private readonly CorpModelManager _MerchantManager;
        private readonly SchedulingModelManager _SchedulingManager;
        private readonly SchedulingDetailModelManager _SchedulingDetailManager;
        private readonly AdvertisementModelManager _AdvertisementManager;
        private readonly AdDowloadRecordModelManager _AdDowloadRecordManager;
        private readonly AdvertisementDetailModelManager _AdvertisementDetailManager;
        private readonly AdPackageModelManager _AdPackageManager;

        public ServiceController()
        {
            _AdPackageManager = _AdPackageManager ?? new AdPackageModelManager();
            _DeviceManager = _DeviceManager ?? new DeviceModelManager();
            _MerchantManager = _MerchantManager ?? new CorpModelManager();
            _SchedulingManager = _SchedulingManager ?? new SchedulingModelManager();
            _SchedulingDetailManager = _SchedulingDetailManager ?? new SchedulingDetailModelManager();
            _AdvertisementManager = _AdvertisementManager ?? new AdvertisementModelManager();
            _AdDowloadRecordManager = _AdDowloadRecordManager ?? new AdDowloadRecordModelManager();
            _AdvertisementDetailManager = _AdvertisementDetailManager ?? new AdvertisementDetailModelManager();
        }

        [HttpGet]
        public async Task<JsonResult> GetScheduling(SchedulingQueryModel model)
        {
            try
            {
                await ValidateParams(model);

                var result = new List<SchedulingViewModel>();

                using (var db = _SchedulingManager.Db)
                {
                    #region 查找排期
                    var quary = from a in db.SchedulingModels
                                join b in db.DeviceModels on a.DeviceID equals b.ID
                                where b.DeviceCode == model.DeviceCode
                                select new
                                {
                                    SchedulingId = a.ID,
                                    DeviceId = a.DeviceID,
                                    DeviceCode = b.DeviceCode,
                                    SchedulingName = a.Name,
                                    DeviceName = b.Name,
                                    b.Province,
                                    b.City,
                                    b.Region,
                                    a.BeginPlayDate,
                                    a.EndPlayDate,
                                    a.PlayCount,
                                    a.Loop,
                                    b.OpenArea,
                                    a.PlayTimes
                                };

                    if (!string.IsNullOrEmpty(model.BeginDate))
                        quary = quary.Where(item => string.Compare(item.EndPlayDate, model.BeginDate, StringComparison.Ordinal) >= 0);
                    if (!string.IsNullOrEmpty(model.EndDate))
                        quary = quary.Where(item => string.Compare(item.BeginPlayDate, model.EndDate, StringComparison.Ordinal) <= 0);

                    var SchedulingList = quary.ToList();
                    if (SchedulingList.Count == 0)
                        throw new Exception("未找到相应排期！");
                    #endregion

                    SchedulingList.ForEach(scheduling =>
                    {
                        var resultItem = new SchedulingViewModel { AdList = new List<AdViewModel> { } };

                        var schedulingDetailList = db.SchedulingDetailModels.Where(s => s.SchedulingID == scheduling.SchedulingId).ToList();

                        schedulingDetailList.ForEach(sDetail =>
                        {
                            var ad = db.AdvertisementModels.FirstOrDefault(a => a.ID == sDetail.AdvertisementID);
                            var pg = db.AdPackageModels.FirstOrDefault(p => p.AdvertisementID == ad.ID);

                            string pgName = ad.Name + ".zip";
                            string pgPathName = AppSetting.ZipFileStoragePath + pgName;
                            FileInfo file = new FileInfo(pgPathName);
                            var files = new List<RedirectUrlViewModel>();

                            var adDetails = (from a in db.AdvertisementDetailModels
                                             join b in db.SchedulingDetailModels on a.AdvertisementID equals b.AdvertisementID
                                             where b.SchedulingID == scheduling.SchedulingId && a.AdvertisementID == sDetail.AdvertisementID
                                             select new { a.FileName, a.RedirectUrl, a.StoragePath, a.ContentType, a.AdvertisementID })
                                          .ToList();

                            adDetails.ForEach(detail =>
                            {
                                if (detail.ContentType.Contains("image"))
                                {
                                    files.Add(new RedirectUrlViewModel
                                    {
                                        Name = string.Concat(AppSetting.AdImagePath, ad.MerchantID, "\\", detail.FileName),
                                        RedirectUrl = detail.RedirectUrl
                                    });
                                }
                                else
                                {
                                    files.Add(new RedirectUrlViewModel
                                    {
                                        Name = string.Concat(AppSetting.AdVideoPath, ad.MerchantID, "\\", detail.FileName),
                                        RedirectUrl = detail.RedirectUrl
                                    });
                                }
                            });

                            #region 查找内容文件,生成压缩包
                            if (!System.IO.File.Exists(pgPathName))//如果内容己经打包过，不再重复打包
                            {
                                ZipFile.CreateZipFile(files.Select(f => f.Name).ToArray(), AppSetting.ZipFileStoragePath, pgName);

                                if (pg == null)
                                {
                                    pg = new AdPackageModel
                                    {
                                        ID = Guid.NewGuid().ToString(),
                                        AdvertisementID = ad.ID
                                    };
                                    db.AdPackageModels.Add(pg);
                                }
                                pg.Count = files.Count;
                                pg.Name = pgName;
                                pg.Operator = User.Identity.Name;
                                pg.Path = AppSetting.ZipFileStoragePath;
                                pg.SchedulingID = scheduling.SchedulingId;
                                pg.Size = file.Length;
                                //创建内容包记录
                                db.SaveChanges();
                            }
                            #endregion

                            resultItem.AdList.Add(new AdViewModel
                            {
                                Files = files.Select(f => new RedirectUrlViewModel { Name = f.Name.Substring(f.Name.LastIndexOf("\\") + 1), RedirectUrl = f.RedirectUrl }),
                                AdName = pgName,
                                Size = file.Length,
                                DowloadUrl = string.Format(AppSetting.ZipFileRequestPath, model.DeviceCode, pg.ID)
                            });
                        });

                        var playTimes = JsonConvert.DeserializeObject<List<PlayTimesSelectorViewModel>>(scheduling.PlayTimes).Select(times => times.Name);

                        resultItem.BeginPlayDate = scheduling.BeginPlayDate;
                        resultItem.EndPlayDate = scheduling.EndPlayDate;
                        resultItem.SchedulingName = scheduling.SchedulingName;
                        resultItem.DeviceCode = scheduling.DeviceCode;
                        resultItem.DeviceName = scheduling.DeviceName;
                        resultItem.Loop = scheduling.Loop;
                        resultItem.PlayCount = scheduling.PlayCount;
                        resultItem.PlayTimes = playTimes.ToList();

                        result.Add(resultItem);
                    });

                    return Success(result);
                }
            }
            catch (ArgumentException ex)
            {
                return Fail(ErrorCode.ModelValidateError, ex.Message);
            }
            catch (Exception ex)
            {
                return Fail(ErrorCode.DataError, ex.Message);
            }

        }

        private async Task ValidateParams(SchedulingQueryModel model)
        {
            if (model == null)
                throw new ArgumentException("参数有误！");

            if (string.IsNullOrEmpty(model.DeviceCode))
                throw new ArgumentException("缺少终端编号参数！");

            if (!string.IsNullOrEmpty(model.BeginDate))
            {
                DateTime date;
                DateTime.TryParse(model.BeginDate, out date);
                if (date == default(DateTime))
                    throw new ArgumentException("BeginDate 参数无效！");
            }

            if (!string.IsNullOrEmpty(model.EndDate))
            {
                DateTime date;
                DateTime.TryParse(model.EndDate, out date);
                if (date == default(DateTime))
                    throw new ArgumentException("EndDate 参数无效！");
            }

            var device = await _DeviceManager.GetByExpreAsync(item => item.DeviceCode == model.DeviceCode);
            if (device == null)
                throw new ArgumentException("终端编号无效！");
        }

        [HttpGet]
        public async Task Dowload(string deviceCode, string id)
        {
            Stream oStream = null;

            try
            {
                AdPackageModel package = await CreateDowloadLog(deviceCode, id);

                oStream =
                    new FileStream
                        (path: package.PathName,
                        mode: FileMode.Open,
                        share: FileShare.Read,
                        access: FileAccess.Read);

                long lngFileLength = oStream.Length;
                int offset = 0;

                string range = Request.Params["HTTP_RANGE"];

                Response.Buffer = false;
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + package.Name);
                if (!string.IsNullOrEmpty(range))
                {
                    var arr_split = range.Split(new char[] { Convert.ToChar("=") });
                    int.TryParse(arr_split[1].Split('-')[0], out offset);
                }
                Response.AddHeader("Content-Range", $"bytes={offset}-{lngFileLength}");
                Response.AddHeader("Content-Length", lngFileLength.ToString());
                long lngDataToRead = lngFileLength - offset;
                while (lngDataToRead > 0)
                {
                    if (Response.IsClientConnected)
                    {
                        int intBufferSize = 8 * 1024;

                        byte[] bytBuffers = new System.Byte[intBufferSize];

                        oStream.Seek(offset, SeekOrigin.Begin);

                        int startPosition = oStream.Read(buffer: bytBuffers, offset: 0, count: intBufferSize);

                        Response.OutputStream.Write(buffer: bytBuffers, offset: 0, count: startPosition);

                        Response.Flush();

                        offset += startPosition;
                        lngDataToRead -= startPosition;
                    }
                    else
                    {
                        lngDataToRead = -1;
                    }
                }
            }
            catch (Exception e)
            {
                Response.Write(e.Message);
            }
            finally
            {
                if (oStream != null)
                {
                    oStream.Close();
                    oStream.Dispose();
                    oStream = null;
                }
                Response.Close();
            }
        }

        /// <summary>
        /// 生成下载日志
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task<AdPackageModel> CreateDowloadLog(string deviceCode, string id)
        {
            var pg = await _AdPackageManager.GetByIdAsync(id);
            if (pg == null)
                throw new ArgumentNullException($"id值为{id}的内容包不存在！");

            var device = await _DeviceManager.GetByExpreAsync(item => item.DeviceCode == deviceCode);
            if (device == null)
                throw new ArgumentNullException($"deviceCode值为{deviceCode}的终端不存在！");

            var dl = await _AdDowloadRecordManager.GetByExpreAsync(d => d.AdPackageID == pg.ID);

            if (dl == null)
            {
                dl = new AdDowloadRecordModel();
                dl.DevieID = device.ID;
                dl.Name = device.Name;
                dl.AdPackageID = pg.ID;
                dl.AdPackageName = pg.Name;
                dl.AdPackagePath = pg.Path;
                dl.City = device.City;
                dl.Count = 1;
                dl.FileCount = pg.Count;
                dl.IP = Utility.ConvertDefaultIPAddress(Request.UserHostAddress);
                dl.OpenArea = device.OpenArea;
                dl.Operator = User.Identity.Name;
                dl.Province = device.Province;
                dl.Region = device.Region;
                dl.OpenArea = device.OpenArea;
                dl.Size = pg.Size;
                dl.Url = Request.Url.ToString();
                dl.LastDowloadDate = DateTime.Now;
            }
            else
            {
                dl.LastDowloadDate = DateTime.Now;
                dl.IP = Utility.ConvertDefaultIPAddress(Request.UserHostAddress);
                dl.Url = Request.Url.ToString();
                dl.Count += 1;
            }

            await _AdDowloadRecordManager.SaveAsync(dl);
            return pg;
        }
    }
}
