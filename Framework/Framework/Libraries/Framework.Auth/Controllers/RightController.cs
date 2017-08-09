using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Framework.Common.Mvc;
using Framework.Common.Json;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Framework.Auth.Controllers
{
    public class RightController : BaseController
    {
        private ApplicationRoleManager _roleManager;

        public RightController() { }

        public RightController(
            ApplicationRoleManager roleManager)
        {
            RoleManager = roleManager;
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }

        public async Task<ActionResult> Index()
        {
            var roles = await RoleManager.Roles.Where(item => item.Name != "Admin").ToListAsync();
            ViewBag.Roles = roles.Select(item => new SelectListItem { Text = item.Name, Value = item.Id }).ToList();
            var menus = MenuManager.GetAllMenus();
            return View(menus);
        }

        [HttpPost]
        public async Task<JsonResult> Get(string roleId)
        {
            try
            {
                if (string.IsNullOrEmpty(roleId))
                    throw new ArgumentNullException("参数有误");

                var roleRight = await RoleManager.FindByIdAsync(roleId) ?? new ApplicationRole();

                return Success(roleRight.RoleRights);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.DataError, e.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Create(string roleId, string roleJsonStr)
        {
            try
            {
                if (string.IsNullOrEmpty(roleJsonStr) || string.IsNullOrEmpty(roleId))
                    throw new ArgumentNullException("参数有误");

                var roleRight = await RoleManager.FindByIdAsync(roleId) ?? new ApplicationRole();
                roleRight.Operator = User.Identity.Name;
                roleRight.Rights = roleJsonStr;

                var result = await RoleManager.CreateOrUpdateAsync(roleRight);

                if (!result.Succeeded)
                    throw new Exception("权限信息保存失败，请稍后再试！");

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
                    _roleManager = null;
                }
            }

            base.Dispose(disposing);
        }

    }
}