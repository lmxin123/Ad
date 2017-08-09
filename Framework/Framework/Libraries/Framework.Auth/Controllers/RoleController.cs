using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

using Framework.Common.Mvc;
using Framework.Common.Json;

namespace Framework.Auth.Test.Controllers
{
    public class RoleController : BaseController
    {
        private readonly ApplicationRoleManager _roleManager;

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Create(ApplicationRole model)
        {
            return await CreateOrUpdate(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Update(ApplicationRole model)
        {
            return await CreateOrUpdate(model);
        }

        private async Task<JsonResult> CreateOrUpdate(ApplicationRole model)
        {
            try
            {
                model.Operator = User.Identity.Name;

                var result = new IdentityResult();
                model.Operator = User.Identity.Name;
                if (string.IsNullOrEmpty(model.Id))
                {
                    model.Id = Guid.NewGuid().ToString();
                    result = await RoleManager.CreateAsync(model);
                }
                else
                {
                    var updateModel = await RoleManager.FindByIdAsync(model.Id);
                    updateModel.Name = model.Name;
                    updateModel.Operator = model.Operator;
                    updateModel.Remark = model.Remark;
                    result = await RoleManager.UpdateAsync(updateModel);
                }
                if (result.Succeeded)
                {
                    return Success(true);
                }
                else
                {
                    return Fail(ErrorCode.ModelValidateError, string.Join(",", result.Errors.Select(item => item)));
                }
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.DataError, e.Message);
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetList()
        {
            try
            {
                var items = await RoleManager.Roles.ToListAsync();
                return Success(items);
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
                    throw new ArgumentNullException("参数有误");

                var role = await RoleManager.FindByIdAsync(id);

                if (role.Users.Count > 0)
                    throw new Exception("角色包含用户，不允许删除！");

                var result = await RoleManager.DeleteAsync(role);

                if (!result.Succeeded)
                    throw new Exception(string.Join(",", result.Errors));

                return Success(true);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.DataError, e.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_roleManager != null)
                {
                    _roleManager.Dispose();
                }
            }

            base.Dispose(disposing);
        }
    }
}