using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using Framework.Data;
using Framework.Common;
using Framework.Common.Json;

using Hdy.Media.Models;
using Hdy.Media.ViewModels;
using Framework.Auth;
using Framework.Common.IO;

namespace Hdy.Media.Controllers
{
    public class CorpListController : MediaBaseController
    {
        protected readonly CorpModelManager _MerchantManager;
        protected readonly AdvertisementModelManager _AdvertisementManager;

        public CorpListController()
        {
            _MerchantManager = _MerchantManager ?? new CorpModelManager();
            _AdvertisementManager = _AdvertisementManager ?? new AdvertisementModelManager();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetList(MerchantQueryViewModel model, int pageIndex, int pageSize)
        {
            try
            {
                string userId = User.Identity.GetUserId();
                model.UserID = userId.ToLower() != AuthSetting.Administrator ? string.Empty : User.Identity.GetUserId();
                var result = await _MerchantManager.QueryAsync(model, pageIndex, pageSize);

                return Success(result);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.DataError, e.Message);
            }
        }

        [HttpGet]
        public JsonResult GetBusinessLicense(string filePath)
        {
            string data = string.Empty;

            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
            {
                data = Utility.ImageToBase64(filePath);
                return Success(data);
            }
            return Fail(ErrorCode.DataError, "营业执照不存在！");
        }

        public async Task<ActionResult> Create()
        {
            CorpModel model = await CreateOrUpdate(string.Empty);

            return View(model);
        }

        public async Task<ActionResult> Update(string id)
        {
            CorpModel model = await CreateOrUpdate(id);

            return View("Create",model);
        }

        private async Task<CorpModel> CreateOrUpdate(string id)
        {
            var model = new CorpModel();
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.Title = "添加商户";
            }
            else
            {
                ViewBag.Title = "修改商户资料";
                model = await _MerchantManager.GetByIdAsync(id);

                if (!string.IsNullOrEmpty(model.BusinessLicensePath))
                {
                    ViewBag.BusinessLicense = Utility.ImageToBase64(model.BusinessLicensePath);
                }
            }

            return model;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create(CorpModel model)
        {
            return await CreateOrUpdate(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(CorpModel model)
        {
            return await CreateOrUpdate(model);
        }

        private async Task<JsonResult> CreateOrUpdate(CorpModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Fail(ErrorCode.ModelValidateError, ModelState.Values.FirstOrDefault(item => item.Errors.Count > 0).Errors.FirstOrDefault().ErrorMessage);
                }

                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase BusinessImg = Request.Files[0];
                    if (BusinessImg != null && BusinessImg.ContentLength > 0)
                    {
                        if (!Directory.Exists(AppSetting.BusinessLicensePath))
                            Directory.CreateDirectory(AppSetting.BusinessLicensePath);

                        string fileName = string.Concat(AppSetting.BusinessLicensePath, model.Name, "_", BusinessImg.FileName);
                        BusinessImg.SaveAs(fileName);
                        //如果原来有文件，则要先删除
                        if (!string.IsNullOrEmpty(model.BusinessLicensePath) && model.BusinessLicensePath != fileName)
                            FileHelper.DeleteFile(model.BusinessLicensePath);
                        model.BusinessLicensePath = fileName;
                    }
                }

                model.Operator = User.Identity.Name;
                if (string.IsNullOrEmpty(model.UserID))
                    model.UserID = User.Identity.GetUserId();

                if (!string.IsNullOrEmpty(model.RefusalReasons))
                {
                    model.RefusalReasons = null;
                    model.RecordState = RecordStates.PendingAudit;
                }

                await _MerchantManager.SaveAsync(model);

                return Success(true);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.ProcessError, e.Message);
            }
        }

        /// <summary>
        /// 验证商户名称是否重复
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<JsonResult> ValidateNameExist(string id, string name)
        {
            if (string.IsNullOrEmpty(name))
                return Json(false, JsonRequestBehavior.AllowGet);

            bool isExists = await _MerchantManager.GetByExpreAsync(item =>
             item.Name == name.Trim().Replace("\t", "") && item.ID != id) == null;

            return Json(isExists, JsonRequestBehavior.AllowGet);
        }
        public async Task<JsonResult> ValidateAdNameExist(string id, string name)
        {
            if (string.IsNullOrEmpty(name))
                return Json(false, JsonRequestBehavior.AllowGet);

            bool isExists = await _AdvertisementManager.GetByExpreAsync(item =>
             item.Name == name.Trim().Replace("\t", "") && item.ID != id) == null;

            return Json(isExists, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 商户审核
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> Check(CorpModel model)
        {
            try
            {
                var item = await _MerchantManager.GetByIdAsync(model.ID);
                item.RecordState = model.RecordState;
                if (model.RecordState == RecordStates.AuditFailure)
                    item.RefusalReasons = model.RefusalReasons;
                item.Operator = User.Identity.Name;

                await _MerchantManager.SaveAsync(item);

                return Success(true);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.ProcessError, e.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_AdvertisementManager != null)
                {
                    _AdvertisementManager.Dispose();
                }

                if (_MerchantManager != null)
                {
                    _MerchantManager.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}