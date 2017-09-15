using Framework.Common.Json;
using Hdy.Media.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity.Validation;

namespace Hdy.Media.Controllers
{
    public class DeviceController : MediaBaseController
    {
        private readonly DeviceModelManager _DeviceManager;
        private readonly DeviceCategoryModelManager _DeviceCategoryManager;
        private readonly SchedulingModelManager _SchedulingModelManager;

        public DeviceController()
        {
            _DeviceManager = _DeviceManager ?? new DeviceModelManager();
            _DeviceCategoryManager = _DeviceCategoryManager ?? new DeviceCategoryModelManager();
            _SchedulingModelManager = _SchedulingModelManager ?? new SchedulingModelManager();
        }

        public async Task<ActionResult> Index()
        {
            var items = await _DeviceCategoryManager.QueryAsync(1, 50).ConfigureAwait(false);
            ViewBag.CategoryID = items.Data.Select(item => new SelectListItem { Value = item.ID, Text = item.Name }).ToList();

            return View(new DeviceModel());
        }

        //public JsonResult GetDeviceCode()
        //{
        //    try
        //    {
        //        string deviceCode = _DeviceManager.GeneralDeviceCode(AppSetting.DefaultLocation[0], AppSetting.DefaultLocation[1], AppSetting.DefaultLocation[2]);
        //        return Success(deviceCode);
        //    }
        //    catch (Exception e)
        //    {
        //        return Fail(ErrorCode.DataError, e.Message);
        //    }
        //}

        [HttpPost]
        public async Task<JsonResult> GetList(DeviceModel model, int pageIndex, int pageSize)
        {
            try
            {
                var result = await _DeviceManager.QueryAsync(model, pageIndex, pageSize);

                return Success(result);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.ProcessError, e.Message);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create(DeviceModel model)
        {
            return await CreateOrUpdate(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(DeviceModel model)
        {
            return await CreateOrUpdate(model);
        }

        private async Task<JsonResult> CreateOrUpdate(DeviceModel model)
        {
            try
            {
                var validateResul = await ValidateNameExist(model.ID, model.Name, model.Province, model.City, model.Region);
                if (validateResul.Data.ToString() == bool.FalseString)
                    throw new Exception("保存失败：终端名称己存在！");

                model.Operator = User.Identity.Name;

                if (string.IsNullOrEmpty(model.ID))
                {
                    model.DeviceCode = _DeviceManager.GeneralDeviceCode(model.Province, model.City, model.Region);
                }
                await _DeviceManager.SaveAsync(model);

                return Success(true);
            }
            catch (DbEntityValidationException e)
            {
                string msg = string.Join(",", e.EntityValidationErrors.Select(a => string.Join(",", a.ValidationErrors.Select(b => b.ErrorMessage).ToList())).ToList());
                return Fail(ErrorCode.ProcessError, msg);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.ProcessError, e.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Delete(string id)
        {
            try
            {
                string now = DateTime.Now.ToString("yyyy-MM-dd");
                var scheduling = await _SchedulingModelManager.GetByExpreAsync(item => item.DeviceID == id && string.Compare(item.EndPlayDate, now, StringComparison.Ordinal) > 0);

                if (scheduling != null)
                    throw new Exception("此终端己有未过期的排期，不允许删除！");

                var model = await _DeviceManager.GetByIdAsync(id);

                model.Operator = User.Identity.Name;
                await _DeviceManager.SaveAsync(model);

                await _DeviceManager.DeleteAsync(id);
                return Success(true);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.ProcessError, e.Message);
            }
        }

        public async Task<JsonResult> ValidateNameExist(string id, string name, string province, string city, string region)
        {
            if (string.IsNullOrEmpty(name))
                return Json(false, JsonRequestBehavior.AllowGet);

            bool isExists = await _DeviceManager.GetByExpreAsync(item =>
             item.Name == name.Trim().Replace("\t", "") &&
             item.Province == province &&
             item.City == city &&
             item.Region == region &&
             item.ID != id) == null;

            return Json(isExists, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> ValidateDeviceCodeExist(string id, string deviceCode)
        {
            if (string.IsNullOrEmpty(deviceCode))
                return Json(false, JsonRequestBehavior.AllowGet);

            bool isExists = await _DeviceManager.GetByExpreAsync(item =>
             item.DeviceCode == deviceCode.Trim().Replace("\t", "") && item.ID != id) == null;

            return Json(isExists, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_DeviceCategoryManager != null)
                {
                    _DeviceCategoryManager.Dispose();
                }
                if (_SchedulingModelManager != null)
                {
                    _SchedulingModelManager.Dispose();
                }
                if (_DeviceManager != null)
                {
                    _DeviceManager.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}