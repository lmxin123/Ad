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
    public class UserController : BaseController
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

        public UserController() { }

        public UserController(
            ApplicationUserManager userManager,
            ApplicationSignInManager signInManager,
            ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            RoleManager = roleManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
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

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        #region user
        public ActionResult Index()
        {
            ViewBag.RoleId = RoleManager.Roles.Select(item => new SelectListItem
            {
                Text = item.Name,
                Value = item.Id
            }).ToList();

            return View();
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (User.Identity.IsAuthenticated)
            {
                return RedirectPermanent(GetDefaultUrl(User.Identity.GetUserId()));
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> Login(LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    throw new ArgumentException("输入无效");

                var loginResult = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, shouldLockout: false);
                var result = new GeneralResponseModel<object>();
                if (loginResult == SignInStatus.Success)
                {
                    var userId = UserManager.FindByName(model.UserName).Id;
                    result.Data = model.ReturnUrl ?? GetDefaultUrl(userId);
                }
                else
                {
                    result.Success = false;
                    result.Message = "用户名或密码错误";
                }

                return Success(result);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.RequestParamError, e.Message);
            }
        }

        private string GetDefaultUrl(string userId)
        {
            //这里需要把用户Id传过去，是因为此时刚刚登录，并没有把cookie信息返回到前端，
            //IsAuthenticated还是为false的，所以后台还是获取不到当前用户的登录信息的
            var rights = User.Identity.GetUserRight(userId, true);
            string url = string.Empty;
            foreach (var right in rights)
            {
                if (right.All)
                {
                    if (!string.IsNullOrEmpty(right.Url))
                    {
                        url = right.Url;
                    }
                    else
                    {
                        url = right.SubMenuList.FirstOrDefault(item => item.All).Url;
                    }
                    break;
                }
            }
            return url;
        }

        public async Task<JsonResult> GetList(UserQueryViewModel model, int pageIndex, int pageSize)
        {
            try
            {
                using (var db = new AuthDbContext())
                {
                    var query = from u in db.Users
                                join ur in db.UserRoles on u.Id equals ur.UserId
                                join r in db.Roles on ur.RoleId equals r.Id
                                orderby u.CreateTime descending
                                where u.UserState != UserStates.Delete
                                select new
                                {
                                    u.Id,
                                    u.UserName,
                                    u.UserState,
                                    u.Email,
                                    u.PhoneNumber,
                                    u.CreateTime,
                                    u.Operator,
                                    u.Remark,
                                    RoleId = r.Id,
                                    RoleName = r.Name
                                };

                    if (!string.IsNullOrEmpty(model.QueryText))
                    {
                        query = query.Where(item =>
                                    item.UserName.Contains(model.QueryText) ||
                                    item.Email.Contains(model.QueryText) ||
                                    item.PhoneNumber.Contains(model.QueryText) ||
                                    item.Remark.Contains(model.QueryText) ||
                                    item.RoleName.Contains(model.QueryText) ||
                                    item.Operator.Contains(model.QueryText));
                    }

                    if (!string.IsNullOrEmpty(model.RoleId))
                    {
                        query = query.Where(item => item.RoleId == model.RoleId);
                    }
                    var list = await query
                       .Skip(pageSize * (pageIndex - 1))
                       .Take(pageSize)
                       .Distinct()
                       .ToListAsync();

                    return Success(list, await UserManager.Users.CountAsync());
                }
            }
            catch (Exception ex)
            {
                return Fail(ErrorCode.DataError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Create(RegisterViewModel model)
        {
            return await CreateOrUpdate(model);
        }

        [HttpPost]
        public async Task<JsonResult> Update(RegisterViewModel model)
        {
            return await CreateOrUpdate(model);
        }

        private async Task<JsonResult> CreateOrUpdate(RegisterViewModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.Id) && ModelState.IsValid)
                    throw new Exception(string.Join(",", ModelState.Values.Select(item => item.Errors.Select(err => err.ErrorMessage))));

                var result = new IdentityResult();
                string userId = string.Empty;

                if (string.IsNullOrEmpty(model.Id))
                {
                    userId = Guid.NewGuid().ToString();
                    var user = new ApplicationUser
                    {
                        Id = userId,
                        UserName = model.UserName,
                        PasswordHash = UserManager.PasswordHasher.HashPassword(model.Password),
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        Gender = 1,
                        Operator = User.Identity.Name,
                        Remark = model.Remark,
                        UserState = UserStates.Normal,
                        CreateTime = DateTime.Now
                    };

                    result = await UserManager.CreateAsync(user, model.Password);
                }
                else
                {
                    var user = await UserManager.FindByIdAsync(model.Id);
                    if (user == null)
                        throw new Exception("用户不存在");
                    userId = user.Id;

                    user.UserName = model.UserName;
                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Operator = User.Identity.Name;
                    user.Remark = model.Remark;

                    result = await UserManager.UpdateAsync(user);

                    var roles = UserManager.GetRoles(userId);
                    foreach (var role in roles)
                    {
                        UserManager.RemoveFromRole(userId, role);
                    }
                }

                if (!result.Succeeded)
                    throw new Exception(string.Join(",", result.Errors));

                var newRole = await RoleManager.FindByIdAsync(model.RoleId);
                await UserManager.AddToRoleAsync(userId, newRole.Name);

                return Success(true);
            }
            catch (Exception ex)
            {
                return Fail(ErrorCode.ModelValidateError, ex.Message);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Fail(ErrorCode.ModelValidateError, string.Join(",", ModelState.Values.Select(item => item.Errors.Select(a => a.ErrorMessage))));
            }
            var user = await UserManager.FindByNameAsync(model.Name);
            if (user == null)
            {
                return Fail(ErrorCode.DataError, "用户不存在！");
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.Password);
            if (result.Succeeded)
            {
                return Success(true);
            }
            return Fail(ErrorCode.UnknownError, string.Join("，", result.Errors.Select(item => item)));
        }

        [AllowAnonymous]
        public ActionResult LogOut()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<JsonResult> UpdateState(string id, UserStates state)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    throw new ArgumentNullException("缺少参数！");

                var user = UserManager.FindById(id);
                user.UserState = state;
                user.Operator = User.Identity.Name;

                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Success(result.Succeeded);
                }
                else
                {
                    throw new Exception(string.Join(",", result.Errors));
                }
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.DataError, e.Message);
            }
        }

        [HttpPost]
        public async Task<JsonResult> Delete(string id, UserStates state)
        {
            return await UpdateState(id, state);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }
    }
}