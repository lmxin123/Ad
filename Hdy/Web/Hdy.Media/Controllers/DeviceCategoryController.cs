using Framework.Common.Json;
using Hdy.Media.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Hdy.Media.Controllers
{
    public class DeviceCategoryController : MediaBaseController
    {
        private readonly DeviceModelManager _DeviceModelManager;
        private readonly DeviceCategoryModelManager _DeviceCategoryManager;

        public DeviceCategoryController()
        {
            _DeviceCategoryManager = _DeviceCategoryManager ?? new DeviceCategoryModelManager();
            _DeviceModelManager = _DeviceModelManager ?? new DeviceModelManager();
        }

        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public async Task<JsonResult> ValidateNameExist(string id, string name)
        {
            if (string.IsNullOrEmpty(name))
                return Json(false, JsonRequestBehavior.AllowGet);

            bool isExists = await _DeviceCategoryManager.GetByExpreAsync(item =>
             item.Name == name.Trim().Replace("\t", "") && item.ID != id) == null;

            return Json(isExists, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<JsonResult> GetList(int pageIndex, int pageSize)
        {
            try
            {
                var result = await _DeviceCategoryManager.QueryAsync(pageIndex, pageSize);

                return Success(result);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.ProcessError, e.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Create(DeviceCategoryModel model)
        {
            return await CreateOrUpdate(model);
        }

        [HttpPost]
        public async Task<JsonResult> Update(DeviceCategoryModel model)
        {
            return await CreateOrUpdate(model);
        }

        private async Task<JsonResult> CreateOrUpdate(DeviceCategoryModel model)
        {
            try
            {
                var validateResul = await ValidateNameExist(model.ID, model.Name);
                if (validateResul.Data.ToString() == bool.FalseString)
                    throw new Exception("保存失败：分类名称己存在！");

                model.Operator = User.Identity.Name;

                if (string.IsNullOrEmpty(model.ID))
                    await _DeviceCategoryManager.SaveAsync(model);
                else
                {
                    await _DeviceCategoryManager.SaveAsync(model);
                }

                return Success(true);
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
                var device =await _DeviceModelManager.GetByExpreAsync(item => item.CategoryID == id);
                if (device != null)
                    throw new Exception("此分类下包含终端，不允许删除！");

                var model = await _DeviceCategoryManager.GetByIdAsync(id);
                model.Operator = User.Identity.Name;
                await _DeviceCategoryManager.SaveAsync(model);

                await _DeviceCategoryManager.DeleteAsync(id);
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
                if (_DeviceCategoryManager != null)
                {
                    _DeviceCategoryManager.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}