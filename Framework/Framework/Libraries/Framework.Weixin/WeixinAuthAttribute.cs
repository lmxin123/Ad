//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Text.RegularExpressions;
//using Microsoft.Data.Entity;
//using Microsoft.AspNet.Mvc.Filters;
//using Microsoft.AspNet.Identity;
//using Microsoft.AspNet.Http;
//using Microsoft.AspNet.Http.Extensions;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.OptionsModel;
//using Microsoft.Extensions.WebEncoders;

//using Lmxin.Common;
//using Framework.Weixin.Model;
//using Framework.Weixin.Response;
//using Microsoft.AspNet.Mvc;
//using System.Net;
//using Microsoft.AspNet.Authorization;

//namespace Framework.Weixin
//{
//    public class WeixinAuthRequirement : AuthorizationHandler<WeixinAuthRequirement>, IAuthorizationRequirement
//    {
//        private readonly DbContext _db;
//        private readonly WeixinSettings _weixinSetting;
//        private readonly CommonContext _commonContext;
//        private readonly ILogger _logger;
//        private readonly IWeixinApiService _weixinService;
//        private readonly IMemberManager _memberManager;
//        private readonly IPasswordHasher<MemberModel> _pwdHasher;

//        public WeixinAuthRequirement(
//            DbContext db,
//            IHttpContextAccessor contextAccessor,
//            ILoggerFactory loggerFactory,
//            IMemberManager memberManager)
//        {
//            _db = db;
//            _commonContext = new CommonContext(contextAccessor);
//            _weixinService = _commonContext.GetService<IWeixinApiService>();
//            _weixinSetting = _commonContext.GetService<IOptions<WeixinSettings>>().Value;
//            _logger = _commonContext.CreateLogger<WeixinGetUserInfoAttribute>();
//            _pwdHasher = new PasswordHasher<MemberModel>();
//            _memberManager = memberManager;
//        }

//        public bool IsGetOpenID { get; set; } = true;

//        protected virtual void Authorization(AuthorizationContext context)
//        {
//            var querys = context.HttpContext.Request.Query;
//            string code = querys["code"],
//             retUrl = querys["retUrl"],
//             openid = querys["OPENID"],
//             requestUrl = context.HttpContext.Request.GetDisplayUrl();

//            MemberModel member;
//            int inviteCode = 0;

//            string callbackUrl = Regex.Replace(requestUrl,
//              "(?<=[?&]{1})code=[^&]*&?", "", RegexOptions.IgnoreCase);
//            string authorizeUrl = string.Format(_weixinSetting.AuthorizeUrl, UrlEncoder.Default.UrlEncode(requestUrl), "snsapi_base");

//            if (!string.IsNullOrEmpty(openid))
//            {
//                _logger.LogInformation("带有openId，直接跳过授权：{0}", openid);
//                context.RouteData.Values.Add("OPENDID", openid);
//            }
//            else if (!string.IsNullOrEmpty(code))//如果Code不等于空，则通过Code获取OpenId
//            {
//                #region code授权处理 
//                _logger.LogInformation("开始请求Access_Token，Code：{0}", code);
//                var tokenResp = _weixinService.GetOauth2TokenAsync(code).Result;
//                if (tokenResp.ErrCode != "40029")
//                {
//                    _logger.LogInformation("请求成功:Access_Token={0}，openid={1}", tokenResp.Access_Token, tokenResp.OpenId);
//                    context.RouteData.Values.Add("OPENDID", tokenResp.OpenId);
//                    member = _memberManager.GetByIdAsync(tokenResp.OpenId).Result;
//                    if (member == null)
//                    {
//                        #region 新用户注册
//                        _logger.LogInformation("新用户开始注册");
//                        var wxInfo = _weixinService.GetWeixinInfoAsync(tokenResp.Access_Token, tokenResp.OpenId).Result;
//                        if (!string.IsNullOrEmpty(wxInfo.OpenId))
//                        {
//                            int.TryParse(querys["invitecode"], out inviteCode);
//                            if (!Register(wxInfo, inviteCode))
//                            {
//                                throw new Exception("会员注册过程中发生异常！");
//                            }
//                        }
//                        else
//                        {
//                            _logger.LogInformation("拉取微信用户信息失败，重新进行授权！{0}");
//                            Redirect(context, authorizeUrl);
//                        }
//                        #endregion
//                    }
//                    else
//                    {
//                        #region 旧用户更新
//                        if (string.IsNullOrEmpty(member.WeixinName) || member.LastLoginDate.AddHours(2) <= System.DateTime.Now)
//                        {
//                            var wxInfo = _weixinService.GetWeixinInfoAsync(tokenResp.Access_Token, tokenResp.OpenId).Result;
//                            _logger.LogInformation("更新微信会员{0}资料信息", wxInfo.NickName);
//                            member.WeixinName = wxInfo.NickName;
//                            member.HeadImgUrl = wxInfo.HeadImgUrl;
//                            member.Sex = wxInfo.Sex;
//                            member.Province = wxInfo.Province;
//                            member.City = wxInfo.City;
//                            member.LastLoginDate = DateTime.Now;
//                            var result = _memberManager.ModifyAsync(member).Result;
//                        }
//                        #endregion
//                    }
//                    //如果有带跳转地址，则跳转
//                    if (string.IsNullOrEmpty(retUrl))
//                        retUrl = $"{callbackUrl}&OPENID={tokenResp.OpenId}";
//                    Redirect(context, retUrl);
//                }
//                else
//                {
//                    _logger.LogInformation("Access_Token请求失败:ErrCode={0}，ErrMsg={1}：", tokenResp.ErrCode, tokenResp.ErrMsg);
//                    var codeIndex = requestUrl.IndexOf("code");
//                    if (codeIndex > 0)
//                        requestUrl = requestUrl.Substring(0, codeIndex - 1);
//                    authorizeUrl = string.Format(_weixinSetting.AuthorizeUrl, UrlEncoder.Default.UrlEncode(requestUrl), "snsapi_userinfo");
//                    Redirect(context, authorizeUrl);
//                }
//                #endregion
//            }
//            else
//            {
//                Redirect(context, authorizeUrl);
//            }
//        }

//        protected virtual void AuthorizationFail(AuthorizationContext context)
//        {
//            if (context == null)
//            {
//                throw new ArgumentNullException(nameof(context));
//            }
//            context.Result = new UnauthorizedResult();
//        }

//        protected virtual void GetOpenID()
//        {

//        }

//        protected virtual void GetUserInfo()
//        {

//        }

//        protected virtual void Redirect(AuthorizationContext context, string redirectUrl)
//        {
//            _logger.LogInformation("跳转地址：{0}", redirectUrl);
//            context.HttpContext.Response.Redirect(redirectUrl, true);
//            context.Result = new HttpStatusCodeResult((int)HttpStatusCode.Redirect);
//        }

//        protected virtual bool Register(Oauth2MemberfoResponse wxInfo, int inviteCode)
//        {
//            bool registerResult = false;
//            var member = new MemberModel
//            {
//                WeixinName = wxInfo.NickName,
//                HeadImgUrl = wxInfo.HeadImgUrl,
//                Sex = wxInfo.Sex,
//                OpenID = wxInfo.OpenId,
//                UnionID = wxInfo.UnionID ?? wxInfo.OpenId,
//                Province = wxInfo.Province,
//                City = wxInfo.City,
//                LastLoginDate = System.DateTime.Now
//            };
//            using (var trans = _db.Database.BeginTransaction())
//            {
//                try
//                {
//                    member.Password = _pwdHasher.HashPassword(member, "123456");
//                    int result = _memberManager.CreateAsync(member, inviteCode).Result;
//                    if (result > 0)
//                    {
//                        var sign = _commonContext.SignInAsync(new MemberModel
//                        {
//                            OpenID = member.OpenID,
//                            WeixinName = member.WeixinName,
//                            HeadImgUrl = member.HeadImgUrl,
//                            Sex = member.Sex
//                        }, 24);
//                    }
//                    trans.Commit();
//                    _logger.LogInformation("用户注册成功：{0}", member.WeixinName);
//                    registerResult = true;
//                }
//                catch (Exception e)
//                {
//                    trans.Rollback();
//                    _logger.LogInformation("用户注册失败：{0}，{1}", member.WeixinName, e.StackTrace);
//                }
//            }
//            return registerResult;
//        }

//        protected override void Handle(Microsoft.AspNet.Authorization.AuthorizationContext context, WeixinAuthRequirement requirement)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
