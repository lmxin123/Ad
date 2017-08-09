using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Hdy.Media.Models;
using Hdy.Media.ViewModels;
using Framework.Common.Json;

namespace Hdy.Media.Controllers
{
    public class OpenAreaController : MediaBaseController
    {
        private readonly OpenAreaModelManager _OpenAreaManager;
        private readonly DeviceModelManager _DeviceModelManager;

        public OpenAreaController()
        {
            _OpenAreaManager = _OpenAreaManager ?? new OpenAreaModelManager();
            _DeviceModelManager = _DeviceModelManager ?? new DeviceModelManager();
        }

        public ActionResult Index()
        {
            return View(new OpenAreaModel());
        }

        [HttpPost]
        public async Task<JsonResult> GetList(OpenAreaModel model, int pageIndex, int pageSize)
        {
            try
            {
                var result = await _OpenAreaManager.QueryAsync(model, pageIndex, pageSize);

                return Success(result);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.ProcessError, e.Message);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create(OpenAreaModel model)
        {
            return await CreateOrUpdate(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(OpenAreaModel model)
        {
            return await CreateOrUpdate(model);
        }

        private async Task<JsonResult> CreateOrUpdate(OpenAreaModel model)
        {
            try
            {
                model.Operator = User.Identity.Name;

                if (string.IsNullOrEmpty(model.ID))
                    await _OpenAreaManager.SaveAsync(model);
                else
                {
                    await _OpenAreaManager.SaveAsync(model);
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
                var device = await _DeviceModelManager.GetByExpreAsync(item => item.OpenArea == id);
                if (device != null)
                    throw new Exception("此区域下有终端信息，不允许删除！");

                var model = await _OpenAreaManager.GetByIdAsync(id);
                model.Operator = User.Identity.Name;
                await _OpenAreaManager.SaveAsync(model);

                await _OpenAreaManager.DeleteAsync(id);
                return Success(true);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.ProcessError, e.Message);
            }
        }

        [AllowAnonymous]
        public async Task<PartialViewResult> GetAreaSelectorPartial(string province, string city, string region)
        {
            var model = await _OpenAreaManager.GetSelectorListAsync(province, city, region);
            return PartialView("_AreaSelectorPartial", model);
        }

        [HttpPost]
        public async Task<JsonResult> GetOpenAreaList(string province, string city, string region)
        {
            try
            {
                var items = await _OpenAreaManager.GetSelectorListAsync(province, city, region);
                var result = items.Select(item => new
                {
                    Text = item.Name,
                    Value = item.Value
                });
                return Success(result);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.DataError, e.Message);
            }

        }
        [AllowAnonymous]
        public async Task<JsonResult> ValidateNameExist(string id, string name, string province, string city, string region)
        {
            if (string.IsNullOrEmpty(name))
                return Json(false, JsonRequestBehavior.AllowGet);

            bool isExists = await _OpenAreaManager.GetByExpreAsync(item =>
             item.Name == name.Trim().Replace("\t", "") &&
             item.Province == province &&
             item.City == city &&
             item.Region == region &&
             item.ID != id) == null;

            return Json(isExists, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_OpenAreaManager != null)
                {
                    _OpenAreaManager.Dispose();
                }
                if (_DeviceModelManager != null)
                {
                    _DeviceModelManager.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}