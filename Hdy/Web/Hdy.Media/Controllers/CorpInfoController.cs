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
    public class CorpInfoController : MediaBaseController
    {
        protected readonly CorpModelManager _MerchantManager;

        public CorpInfoController()
        {
            _MerchantManager = _MerchantManager ?? new CorpModelManager();
        }

        public async Task<ActionResult> Index(string id)
        {
            var model = new CorpModel();
            if (string.IsNullOrEmpty(id))
            {
                model = await _MerchantManager.GetByUserIdAsync(User.Identity.GetUserId());
            }
            else
            {
                model = await _MerchantManager.GetByIdAsync(id);
            }
            model = model ?? new CorpModel();

            if (!string.IsNullOrEmpty(model.BusinessLicensePath))
            {
                ViewBag.BusinessLicense = Utility.ImageToBase64(model.BusinessLicensePath);
            }

            return View(model);
        }
        [HttpGet]
        [ActionAuthorize(ActionType =ActionTypes.Select)]
        public async Task<ActionResult> Create(string id)
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

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create(CorpModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelState.Values.FirstOrDefault(item => item.Errors.Count > 0).Errors.FirstOrDefault().ErrorMessage);
                }

                var validateResult = await ValidateNameExist(model.ID, model.Name);
                if (bool.FalseString == validateResult.Data.ToString())
                {
                    throw new Exception("商户名称己存在！");
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
                return Fail(ErrorCode.ProcessError, "保存失败：" + e.Message);
            }
        }

        [HttpGet]
        [ActionAuthorize(ActionType = ActionTypes.Select)]
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
        /// <summary>
        /// 验证商户名称是否重复
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<JsonResult> ValidateNameExist(string id, string name)
        {
            if (string.IsNullOrEmpty(name))
                return Json(false, JsonRequestBehavior.AllowGet);

            bool isExists = true;
            if (string.IsNullOrEmpty(id))
            {
                isExists = await _MerchantManager.GetByExpreAsync(item =>
              item.Name == name.Trim().Replace("\t", "")) == null;
            }
            else
            {
                isExists = await _MerchantManager.GetByExpreAsync(item =>
                item.Name == name.Trim().Replace("\t", "") && item.ID != id) == null;
            }
            return Json(isExists, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_MerchantManager != null)
                {
                    _MerchantManager.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}