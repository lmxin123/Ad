using System;
using System.Net;
using System.Web.Mvc;
using System.Data.Entity;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Framework.Weixin.Response;
using Framework.Weixin.Model;


namespace Framework.Weixin
{
    /// <summary>
    /// 
    /// </summary>
    public class WeixinGetOpenIDAttribute : ActionFilterAttribute
    {
        private readonly ILogger _logger;
        private readonly DbContext _db;
        private readonly IWeixinApiService _weixinService;
        private readonly WeixinSettings _weixinSetting;
        private readonly IMemberManager<MemberModel> _memberManager;
     //   private readonly IPasswordHasher<MemberModel> _pwdHasher;

        public WeixinGetOpenIDAttribute(
            DbContext db,
         //   IOptions<WeixinSettings> weixinSetting,
            ILoggerFactory loggerFactory,
            IWeixinApiService weixinService,
            IMemberManager<MemberModel> memberManager)
        {
            _db = db;
            _weixinService = weixinService;
           // _weixinSetting = weixinSetting.Value;
            _logger = loggerFactory.CreateLogger<WeixinGetUserInfoAttribute>();
         //   _pwdHasher = new PasswordHasher<MemberModel>();
            _memberManager = memberManager;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var querys = filterContext.HttpContext.Request.QueryString;
            string code = querys["code"],
                     retUrl = querys["retUrl"],
                     openId = querys["OPENID"],
                     requestUrl = filterContext.HttpContext.Request.Url.AbsoluteUri;

            string callbackUrl = Regex.Replace(requestUrl,
              "(?<=[?&]{1})code=[^&]*&?", "", RegexOptions.IgnoreCase);
            string authorizeUrl = string.Format(_weixinSetting.AuthorizeUrl, WebUtility.UrlEncode(requestUrl), "snsapi_base");

            var tokenResp = new Oauth2TokenResponse();

            if (!string.IsNullOrEmpty(openId))
            {
                filterContext.RouteData.Values.Add("OPENDID", openId);
            }
            //如果Code不等于空，则通过Code获取OpenId
            else if (!string.IsNullOrEmpty(code))
            {
                _logger.LogInformation("开始请求Access_Token，Code={0}", code);
                tokenResp = _weixinService.GetOauth2TokenAsync(code).Result;
                if (tokenResp.ErrCode != "40029")
                {
                    _logger.LogInformation("请求成功:Access_Token={0}，openid={1}", tokenResp.Access_Token, tokenResp.OpenId);

                    filterContext.RouteData.Values.Add("OPENDID", tokenResp.OpenId);
                    int inviteCode = 0;
                    int.TryParse(querys["inviteCode"], out inviteCode);
                    var result = Register(tokenResp.OpenId, inviteCode).Result;

                    retUrl = string.Format("{0}&OPENID={1}", callbackUrl, tokenResp.OpenId);
                    filterContext.HttpContext.Response.Redirect(retUrl, false);
                    filterContext.Result = new HttpStatusCodeResult((int)HttpStatusCode.Redirect);
                }
                else
                {
                    _logger.LogInformation("Access_Token请求失败:ErrCode={0}，ErrMsg={1}：", tokenResp.ErrCode, tokenResp.ErrMsg);
                    var codeIndex = requestUrl.IndexOf("code");
                    if (codeIndex > 0)
                        requestUrl = requestUrl.Substring(0, codeIndex - 1);
                    authorizeUrl = string.Format(_weixinSetting.AuthorizeUrl,
                        WebUtility.UrlEncode(requestUrl), "snsapi_base");
                    filterContext.Result = new HttpStatusCodeResult((int)HttpStatusCode.Redirect);
                    filterContext.HttpContext.Response.Redirect(authorizeUrl, false);
                }
            }
            else
            {
                filterContext.Result = new HttpStatusCodeResult((int)HttpStatusCode.Redirect);
                filterContext.HttpContext.Response.Redirect(authorizeUrl, false);
            }
        }

        public virtual async Task<bool> Register(string openId, int inviteCode)
        {
            bool registerResult = true;
            var member = await _memberManager.GetMemberByIdAsync(openId);
            if (member == null)
            {
                using (var trans = _db.Database.BeginTransaction())
                {
                    try
                    {
                        member = new MemberModel
                        {
                            OpenID = openId,
                            LastLoginDate = System.DateTime.Now
                        };
                      //  member.Password = _pwdHasher.HashPassword(member, "123456");
                        int result = _memberManager.CreateMemberAsync(member, inviteCode);
                        trans.Commit();
                        _logger.LogInformation("用户注册成功：" + member.WeixinName);
                    }
                    catch (System.Exception e)
                    {
                        registerResult = false;
                        trans.Rollback();
                        _logger.LogInformation("用户注册失败原因是：" + e.StackTrace);
                    }
                }
            }

            return registerResult;
        }
    }
}
