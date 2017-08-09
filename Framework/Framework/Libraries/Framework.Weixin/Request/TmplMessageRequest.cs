using Newtonsoft.Json;

using Framework.Common.Http;
using Framework.Weixin.Response;

namespace Framework.Weixin.Request
{
    public class TmplMessageRequest : RequestObject<TmplMessageResponse>
    {
        public TmplMessageRequest() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="toUser"></param>
        /// <param name="tmpl_Id">请使用TmplMessagesIds类型赋值</param>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="topColor"></param>
        public TmplMessageRequest(string token, string toUser, string tmpl_Id, string url, object data, string topColor = "")
        {
            access_token = token;
            touser = toUser;
            template_id = tmpl_Id;
            this.url = url;
            this.data = data;
            this.topColor = topColor;
        }

        public override string RequestUrl
        {
            get
            {
                return string.Format("https://api.weixin.qq.com/cgi-bin/message/template/send?access_token={0}", this.access_token);
            }
        }

        [JsonIgnore]
        public string access_token { get; set; }

        /// <summary>
        /// OpenId
        /// </summary>
        public string touser { get; set; }
        /// <summary>
        /// 模版ID，请使用TmplMessagesIds类型赋值
        /// </summary>
        public string template_id { get; set; }

        /// <summary>
        /// 点击信息跳转到的地址
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 模版数据
        /// </summary>
        public object data { get; set; }
        /// <summary>
        /// 信息头部颜色
        /// </summary>
        public string topColor { get; set; }
    }

    /// <summary>
    /// 目前支持的模板消息ID值
    /// </summary>
    public static class TmplMessagesIds
    {
        /// <summary>
        /// 推荐会员成功提醒
        /// {{first.DATA}}
        ///会员账号：{{keyword1.DATA }}
        ///时间：{{keyword2.DATA}}
        ///{{remark.DATA}}
        ///在发送时，需要将内容中的参数（{{.DATA}}内为参数）赋值替换为需要的信息
        /// </summary>
        public const string BecomeMember = "7-kFJTx2IAS5ZG6gR2kgsE3UTD_5RMX8GzFnrUnsmiA";
        /// <summary>
        /// 帐户资金变动提醒
        /// {{first.DATA}}
        /// 变动时间：{{date.DATA }}
        ///变动金额：{{adCharge.DATA}}
        ///{{type.DATA}}帐户余额：{{cashBalance.DATA}}
        ///{{remark.DATA}}
        ///在发送时，需要将内容中的参数（{{.DATA}}内为参数）赋值替换为需要的信息
        /// </summary>
        public const string AccountChangeReminder = "m_YuB3vpCNTEYXJsAF8Ua1cvMjTHNqjy1lMfJe7jdJc";
        /// <summary>
        /// 充值通知
        /// {{first.DATA}}
        ///{{accountType.DATA}}：{{account.DATA}}
        ///充值金额：{{amount.DATA}}
        ///充值状态：{{result.DATA}}
        ///{{remark.DATA}}
        ///在发送时，需要将内容中的参数（{{.DATA}}内为参数）赋值替换为需要的信息
        /// </summary>
        public const string RechargeNotice = "ArtgiAxQ63dYrbFCqZhaPGLlCtkbFpQatLP7495vca0";
        /// <summary>
        /// 充值失败通知
        /// {{first.DATA}}
        ///充值号码：{{number.DATA}}
        ///退款金额：{{value.DATA}}
        ///{{remark.DATA}}
        ///在发送时，需要将内容中的参数（{{.DATA}}内为参数）赋值替换为需要的信息
        /// </summary>
        public const string RechargeFailureNotice = "b-IIVVW4doxBuLC1DhnZOipW9xs6rVdBk7y0meVXAx4";
        /// <summary>
        /// 推荐成交通知
        /// {{first.DATA}}
        /// 推荐客户：{{keyword1.DATA }}
        ///成交时间：{{keyword2.DATA}}
        ///成交佣金：{{keyword3.DATA}}
        ///成交项目：{{keyword4.DATA}}
        ///{{remark.DATA}}
        ///在发送时，需要将内容中的参数（{{.DATA}}内为参数）赋值替换为需要的信息
        /// </summary>
        public const string RecommendedTransactionNotice = "oPhmW3uizSv0kuM7V9ztfqGutTdBUSTRC0J-2HlkXv4";
        /// <summary>
        /// 提交提现申请通知
        /// {{first.DATA}}
        /// 提现金额：{{keyword1.DATA }}
        ///提现方式：{{keyword2.DATA}}
        ///状态：{{keyword3.DATA}}
        ///流水号：{{keyword4.DATA}}
        ///{{remark.DATA}}
        ///在发送时，需要将内容中的参数（{{.DATA}}内为参数）赋值替换为需要的信息
        /// </summary>
        public const string SubmitWithdrawalsNotice = "zbngexiwazhgc5klwTy7tKRhdFyw7x7-25lsmDa9su0";
        /// <summary>
        /// 付款完成通知
        /// {{first.DATA}}
        ///单据号：{{keyword1.DATA }}
        ///单据类型：{{keyword2.DATA}}
        ///金额：{{keyword3.DATA}}
        ///日期：{{keyword4.DATA}}
        ///项目名称：{{keyword5.DATA}}
        ///{{remark.DATA}}
        /// </summary>
        public const string PaymentCompletionNotice = "MHDairySl8UVpDchoyod81BKBz7fk-1Y_eDELnmKX1k";
        /// <summary>
        /// 审核驳回通知
        /// {{first.DATA}}
        ///审核信息：{{keyword1.DATA }}
        ///审核人：{{keyword2.DATA}}
        ///驳回原因：{{keyword3.DATA}}
        ///驳回日期：{{keyword4.DATA}}
        ///{{remark.DATA}}
        /// </summary>
        public const string ReviewAndDismissTheNotice = "Mk3Tt7ECMise9fzZoZgShVSW6i7wX6RVsLelY_uKzgQ";
    }
}