using System;
using System.Net;
using System.Web.Mvc;
using System.Data.Entity;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

using Framework.Weixin.Response;
using Framework.Weixin.Model;

namespace Framework.Weixin
{
    /// <summary>
    /// 微信授权，获取头像，用户注册等
    /// </summary>
    public class WeixinGetUserInfoAttribute : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly DbContext _db;
        private readonly IWeixinApiService _weixinService;
        private readonly WeixinSettings _weixinSetting;
        private readonly IMemberManager<MemberModel> _memberManager;
      //  private readonly IPasswordHasher<MemberModel> _pwdHasher;

        public WeixinGetUserInfoAttribute(
            DbContext db, 
          //  IHttpContextAccessor contextAccessor, 
            ILoggerFactory loggerFactory,
            IMemberManager<MemberModel> memberManager)
        {
            _db = db;
       //     _commonContext = new CommonContext(contextAccessor);
            //_weixinService = _commonContext.GetService<IWeixinApiService>();
            //_weixinSetting = _commonContext.GetService<IOptions<WeixinSettings>>().Value;
            //_logger = _commonContext.CreateLogger<WeixinGetUserInfoAttribute>();
            //_pwdHasher = new PasswordHasher<MemberModel>();
            //_memberManager = memberManager;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException(nameof(filterContext));
            }

            ActionExecuting(filterContext);
        }

        protected virtual void ActionExecuting(ActionExecutingContext filterContext)
        {
            var querys = filterContext.HttpContext.Request.QueryString;
            string code = querys["code"],
             retUrl = querys["retUrl"],
             openid = querys["OPENID"],
             requestUrl = filterContext.HttpContext.Request.Url.AbsoluteUri;

            MemberModel member;
            int inviteCode = 0;

            string callbackUrl = Regex.Replace(requestUrl,
              "(?<=[?&]{1})code=[^&]*&?", "", RegexOptions.IgnoreCase);
            string authorizeUrl = string.Format(_weixinSetting.AuthorizeUrl, WebUtility.UrlEncode(requestUrl), "snsapi_base");

            if (!string.IsNullOrEmpty(openid))
            {
                _logger.LogInformation("带有openId，直接跳过授权：{0}", openid);
                filterContext.RouteData.Values.Add("OPENDID", openid);
            }
            else if (!string.IsNullOrEmpty(code))//如果Code不等于空，则通过Code获取OpenId
            {
                #region code授权处理 
                _logger.LogInformation("开始请求Access_Token，Code：{0}", code);
                var tokenResp = _weixinService.GetOauth2TokenAsync(code).Result;
                if (tokenResp.ErrCode != "40029")
                {
                    _logger.LogInformation("请求成功:Access_Token={0}，openid={1}", tokenResp.Access_Token, tokenResp.OpenId);
                    filterContext.RouteData.Values.Add("OPENDID", tokenResp.OpenId);
                    member = _memberManager.GetMemberByIdAsync(tokenResp.OpenId).Result;
                    if (member == null)
                    {
                        #region 新用户注册
                        _logger.LogInformation("新用户开始注册");
                        var wxInfo = _weixinService.GetWeixinInfoAsync(tokenResp.Access_Token, tokenResp.OpenId).Result;
                        if (!string.IsNullOrEmpty(wxInfo.OpenId))
                        {
                            int.TryParse(querys["invitecode"], out inviteCode);
                            if (!Register(wxInfo, inviteCode))
                            {
                                string errUrl = "/weixin/member/createError";
                                filterContext.HttpContext.Response.Redirect(errUrl, true);
                                filterContext.Result = new EmptyResult();
                                return;
                            }
                        }
                        else
                        {
                            _logger.LogInformation("拉取微信用户信息失败，重新进行授权！{0}", authorizeUrl);
                            filterContext.HttpContext.Response.Redirect(authorizeUrl, true);
                            filterContext.Result = new EmptyResult();
                            return;
                        }
                        #endregion
                    }
                    else
                    {
                        #region 旧用户更新
                        if (string.IsNullOrEmpty(member.WeixinName) || member.LastLoginDate.AddHours(2) <= System.DateTime.Now)
                        {
                            var wxInfo = _weixinService.GetWeixinInfoAsync(tokenResp.Access_Token, tokenResp.OpenId).Result;
                            _logger.LogInformation("更新微信会员{0}资料信息", wxInfo.NickName);
                            member.WeixinName = wxInfo.NickName;
                            member.HeadImgUrl = wxInfo.HeadImgUrl;
                            member.Sex = wxInfo.Sex;
                            member.Province = wxInfo.Province;
                            member.City = wxInfo.City;
                            member.LastLoginDate = System.DateTime.Now;
                            var result = _memberManager.ModifyMemberAsync(member).Result;
                        }
                        #endregion
                    }
                    //如果有带跳转地址，则跳转
                    if (!string.IsNullOrEmpty(retUrl))
                    {
                        _logger.LogInformation("带有跳转地址：{0}", retUrl);
                        // filterContext.HttpContext.Response.Redirect(retUrl, true);
                        // filterContext.Result = new RedirectResult(retUrl, true);
                        filterContext.Result = new HttpStatusCodeResult((int)HttpStatusCode.Redirect);
                        filterContext.HttpContext.Response.Redirect(retUrl, false);
                    }
                    else
                    {
                        retUrl = string.Format("{0}&OPENID={1}", callbackUrl, tokenResp.OpenId);
                        filterContext.HttpContext.Response.Redirect(retUrl, false);
                        filterContext.Result = new HttpStatusCodeResult((int)HttpStatusCode.Redirect);
                    }
                }
                else
                {
                    _logger.LogInformation("Access_Token请求失败:ErrCode={0}，ErrMsg={1}：", tokenResp.ErrCode, tokenResp.ErrMsg);
                    var codeIndex = requestUrl.IndexOf("code");
                    if (codeIndex > 0)
                        requestUrl = requestUrl.Substring(0, codeIndex - 1);
                    authorizeUrl = string.Format(_weixinSetting.AuthorizeUrl,
                             WebUtility.UrlEncode(requestUrl), "snsapi_userinfo");

                    filterContext.Result = new HttpStatusCodeResult((int)HttpStatusCode.Redirect);
                    filterContext.HttpContext.Response.Redirect(authorizeUrl, false);
                }
                #endregion
            }
            else
            {
                _logger.LogInformation("开始授权：{0}", authorizeUrl);
                filterContext.Result = new HttpStatusCodeResult((int)HttpStatusCode.Redirect);
                filterContext.HttpContext.Response.Redirect(authorizeUrl, false);
            }
        }

        protected virtual bool Register(Oauth2MemberfoResponse wxInfo, int inviteCode)
        {
            bool registerResult = false;
            var member = new MemberModel
            {
                WeixinName = wxInfo.NickName,
                HeadImgUrl = wxInfo.HeadImgUrl,
                Sex = wxInfo.Sex,
                OpenID = wxInfo.OpenId,
                UnionID = wxInfo.UnionID ?? wxInfo.OpenId,
                Province = wxInfo.Province,
                City = wxInfo.City,
                LastLoginDate = System.DateTime.Now
            };
            using (var trans = _db.Database.BeginTransaction())
            {
                try
                {
                   // member.Password = _pwdHasher.HashPassword(member, "123456");
                    int result=  _memberManager.CreateMemberAsync(member, inviteCode);
                    if (result > 0)
                    {
                        //var sign = _commonContext.SignInAsync(new MemberModel
                        //{
                        //    OpenID = member.OpenID,
                        //    WeixinName = member.WeixinName,
                        //    HeadImgUrl = member.HeadImgUrl,
                        //    Sex = member.Sex
                        //}, 24);
                    }
                    trans.Commit();
                    _logger.LogInformation("用户注册成功：{0}", member.WeixinName);
                    registerResult = true;
                }
                catch (System.Exception e)
                {
                    trans.Rollback();
                    _logger.LogInformation("用户注册失败：{0},原因是：{1}", member.WeixinName, e.StackTrace);
                }
            }
            return registerResult;
        }
    }
}

