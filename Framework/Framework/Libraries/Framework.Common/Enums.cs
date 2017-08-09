using System.ComponentModel.DataAnnotations;

namespace Framework.Common
{
    /// <summary>
    /// 数据记录的状态
    /// </summary>
    public enum RecordStates
    {
        /// <summary>
        /// 一般用于查询
        /// </summary>
        [Display(Name = "请选择")]
        All,
        /// <summary>
        /// 待审核
        /// </summary>
        [Display(Name = "待审核")]
        PendingAudit,
        /// <summary>
        /// 审核通过
        /// </summary>
        [Display(Name = "审核通过")]
        AuditPass,
        /// <summary>
        /// 审核失败
        /// </summary>
        [Display(Name = "审核失败")]
        AuditFailure,
        /// <summary>
        /// 下线
        /// </summary>
        [Display(Name = "下线")]
        Offline,
        /// <summary>
        /// 锁定
        /// </summary>
        [Display(Name = "锁定")]
        Lock,
        /// <summary>
        /// 删除
        /// </summary>
        [Display(Name = "删除")]
        DeleteTag
    }
    /// <summary>
    /// 访问设备类型
    /// </summary>
    public enum DeviceTypes
    {
        [Display(Name = "Android终端")]
        Android = 0,
        [Display(Name = "iPhone终端")]
        IPhone = 1,
        [Display(Name = "iPad 终端")]
        IPad = 2,
        [Display(Name = "IMac终端")]
        IMac = 3,
        [Display(Name = "Windows终端")]
        Windows = 4,
        [Display(Name = "WindowPhone终端")]
        WindowPhone = 5,
        [Display(Name = "PC终端")]
        PC = 6,
        [Display(Name = "微信公众号")]
        Wechat = 7,
        [Display(Name = "其它平台终端")]
        Other = 8
    }
    /// <summary>
    /// 社交平台
    /// </summary>
    public enum SocialPlatforms
    {
        [Display(Name = "微信")]
        Wechat,
        [Display(Name = "微博")]
        Weibo,
        [Display(Name = "QQ")]
        QQ
    }
    /// <summary>
    /// 安装包类型
    /// </summary>
    public enum PkgTypes
    {
        [Display(Name = "安卓安装包")]
        APK = 0,
        [Display(Name = "苹果安装包")]
        IPA = 1,
        /// <summary>
        /// 应用资源升级包
        /// </summary>
        [Display(Name = "应用资源升级包")]
        WGT = 2,
        /// <summary>
        /// 应用资源差量升级包
        /// </summary>
        [Display(Name = "应用资源差量升级包")]
        WGTU = 3
    }
    /// <summary>
    /// 资金账户变动类型
    /// </summary>
    public enum AccountChangeTypes
    {
        [Display(Name = "全部")]
        All,
        /// <summary>
        /// 关注公众号奖励
        /// </summary>
        [Display(Name = "关注公众号奖励")]
        Subscribe,
        /// <summary>
        /// 邀请好友返佣
        /// </summary>
        [Display(Name = "邀请好友返佣")]
        Invite,
        /// <summary>
        /// 消费返佣
        /// </summary>
        [Display(Name = "消费返佣")]
        Consume,
        /// <summary>
        /// 好友消费返佣
        /// </summary>
        [Display(Name = "好友消费返佣")]
        FriendConsume,
        /// <summary>
        /// 签到奖励
        /// </summary>
        [Display(Name = "签到奖励")]
        SignIn,
        /// <summary>
        /// 提现扣除
        /// </summary>
        [Display(Name = "提现扣除")]
        Withdrawals,
        /// <summary>
        /// 消费抵现
        /// </summary>
        [Display(Name = "消费抵现")]
        Deduction,
        /// <summary>
        /// 违规操作冻结
        /// </summary>
        [Display(Name = "违规操作冻结")]
        Frozen
    }
    /// <summary>
    /// 提现状态
    /// </summary>
    public enum WithdrawStates
    {
        /// <summary>
        /// 全部，用于查询
        /// </summary>
        [Display(Name = "全部")]
        All,
        /// <summary>
        /// 己提交申请
        /// </summary>
        [Display(Name = "己提交申请，等待处理")]
        HasAlreadyApplied,
        /// <summary>
        /// 申请被驳回
        /// </summary>
        [Display(Name = "申请被驳回")]
        AuditDoesNotPass,
        /// <summary>
        /// 己转账
        /// </summary>
        [Display(Name = "己转账")]
        HasTransfer
    }
    /// <summary>
    /// 数据请求类型
    /// </summary>
    public enum RequestStringDataTypes
    {
        /// <summary>
        ///字符
        /// </summary>
        [Display(Name = "String")]
        String,
        /// <summary>
        /// Json(JavaScript Object Notation)
        /// </summary>
        [Display(Name = "Json")]
        Json,
        /// <summary>
        /// 可扩展标记语言
        /// </summary>
        [Display(Name = "Xml")]
        Xml
    }
    /// <summary>
    /// 支付平台
    /// </summary>
    public enum PayTypes
    {
        /// <summary>
        /// 全部，一般用于查询
        /// </summary>
        [Display(Name = "全部")]
        All,
        [Display(Name = "微信支付")]
        WxPay,
        [Display(Name = "支付宝")]
        AliPay,
        /// <summary>
        /// 其它支付，如果积分等
        /// </summary>
        [Display(Name = "积分支付")]
        Other
    }
    /// <summary>
    /// 银行类型
    /// </summary>
    public enum BankTypes
    {
        /// <summary>
        /// 工商银行（借记卡）
        /// </summary>
        [Display(Name = "工商银行（借记卡）")]
        ICBC_DEBIT,
        /// <summary>
        /// 工商银行（信用卡）
        /// </summary>
        [Display(Name = "工商银行（信用卡）")]
        ICBC_CREDIT,
        /// <summary>
        /// 农业银行（借记卡）
        /// </summary>
        [Display(Name = "农业银行（借记卡）")]
        ABC_DEBIT,
        /// <summary>
        /// 农业银行 （信用卡）
        /// </summary>
        [Display(Name = " 农业银行 （信用卡）")]
        ABC_CREDIT,
        /// <summary>
        /// 邮政储蓄（借记卡）
        /// </summary>
        [Display(Name = "邮政储蓄（借记卡）")]
        PSBC_DEBIT,
        /// <summary>
        /// 邮政储蓄 （信用卡）
        /// </summary>
        [Display(Name = "邮政储蓄 （信用卡）")]
        PSBC_CREDIT,
        /// <summary>
        /// 建设银行（借记卡）
        /// </summary>
        [Display(Name = "建设银行（借记卡）")]
        CCB_DEBIT,
        /// <summary>
        /// 建设银行 （信用卡）  
        /// </summary>
        [Display(Name = "建设银行 （信用卡）  ")]
        CCB_CREDIT,
        /// <summary>
        /// 招商银行（借记卡）
        /// </summary>
        [Display(Name = "招商银行（借记卡）")]
        CMB_DEBIT,
        /// <summary>
        /// 招商银行（信用卡） 
        /// </summary>
        [Display(Name = "招商银行（信用卡） ")]
        CMB_CREDIT,
        /// <summary>
        /// 交通银行（借记卡）
        /// </summary>
        [Display(Name = "交通银行（借记卡）")]
        COMM_DEBIT,
        /// <summary>
        /// 中国银行（信用卡）
        /// </summary>
        [Display(Name = "中国银行（信用卡）")]
        BOC_CREDIT,
        /// <summary>
        /// 浦发银行（借记卡）
        /// </summary>
        [Display(Name = " 浦发银行（借记卡）")]
        SPDB_DEBIT,
        /// <summary>
        /// 浦发银行 （信用卡）
        /// </summary>
        [Display(Name = "浦发银行 （信用卡）")]
        SPDB_CREDIT,
        /// <summary>
        /// 广发银行（借记卡）
        /// </summary>
        [Display(Name = "广发银行（借记卡）")]
        GDB_DEBIT,
        /// <summary>
        /// 广发银行（信用卡）
        /// </summary>
        [Display(Name = "广发银行（信用卡）")]
        GDB_CREDIT,
        /// <summary>
        ///  民生银行（借记卡）
        /// </summary>
        [Display(Name = " 民生银行（借记卡）")]
        CMBC_DEBIT,
        /// <summary>
        /// 民生银行（信用卡）
        /// </summary>
        [Display(Name = "民生银行（信用卡）")]
        CMBC_CREDIT,
        /// <summary>
        /// 平安银行（借记卡）
        /// </summary>
        [Display(Name = "平安银行（借记卡）")]
        PAB_DEBIT,
        /// <summary>
        /// 平安银行（信用卡）
        /// </summary>
        [Display(Name = "平安银行（信用卡）")]
        PAB_CREDIT,
        /// <summary>
        /// 光大银行（借记卡）  
        /// </summary>
        [Display(Name = "光大银行（借记卡）")]
        CEB_DEBIT,
        /// <summary>
        /// 光大银行（信用卡）
        /// </summary>
        [Display(Name = "光大银行（信用卡）")]
        CEB_CREDIT,
        /// <summary>
        /// 兴业银行 （借记卡）
        /// </summary>
        [Display(Name = "兴业银行 （借记卡）")]
        CIB_DEBIT,
        /// <summary>
        /// 兴业银行（信用卡） 
        /// </summary>
        [Display(Name = "兴业银行（信用卡） ")]
        CIB_CREDIT,
        /// <summary>
        /// 中信银行（借记卡）
        /// </summary>
        [Display(Name = "中信银行（借记卡）")]
        CITIC_DEBIT,
        /// <summary>
        /// 中信银行（信用卡） 
        /// </summary>
        [Display(Name = "中信银行（信用卡） ")]
        CITIC_CREDIT,
        /// <summary>
        /// 深发银行（信用卡）
        /// </summary>
        [Display(Name = "深发银行（信用卡）")]
        SDB_CREDIT,
        /// <summary>
        /// 上海银行（借记卡）
        /// </summary>
        [Display(Name = "上海银行（借记卡）")]
        BOSH_DEBIT,
        /// <summary>
        /// 上海银行 （信用卡）
        /// </summary>
        [Display(Name = "上海银行 （信用卡）")]
        BOSH_CREDIT,
        /// <summary>
        /// 华润银行（借记卡）
        /// </summary>
        [Display(Name = "华润银行（借记卡）")]
        CRB_DEBIT,
        /// <summary>
        /// 杭州银行（借记卡） 
        /// </summary>
        [Display(Name = "杭州银行（借记卡） ")]
        HZB_DEBIT,
        /// <summary>
        /// 杭州银行（信用卡）
        /// </summary>
        [Display(Name = "杭州银行（信用卡）")]
        HZB_CREDIT,
        /// <summary>
        /// 包商银行（借记卡）
        /// </summary>
        [Display(Name = "包商银行（借记卡）")]
        BSB_DEBIT,
        /// <summary>
        /// 包商银行 （信用卡）
        /// </summary>
        [Display(Name = "包商银行 （信用卡）")]
        BSB_CREDIT,
        /// <summary>
        /// 重庆银行（借记卡）
        /// </summary>
        [Display(Name = "重庆银行（借记卡）")]
        CQB_DEBIT,
        /// <summary>
        /// 顺德农商行 （借记卡）
        /// </summary>
        [Display(Name = "顺德农商行 （借记卡）")]
        SDEB_DEBIT,
        /// <summary>
        /// 深圳农商银行（借记卡）
        /// </summary>
        [Display(Name = "深圳农商银行（借记卡）")]
        SZRCB_DEBIT,
        /// <summary>
        /// 哈尔滨银行（借记卡）
        /// </summary>
        [Display(Name = "哈尔滨银行（借记卡）")]
        HRBB_DEBIT,
        /// <summary>
        /// 成都银行（借记卡）
        /// </summary>
        [Display(Name = "成都银行（借记卡）")]
        BOCD_DEBIT,
        /// <summary>
        /// 南粤银行 （借记卡）
        /// </summary>
        [Display(Name = "南粤银行 （借记卡）")]
        GDNYB_DEBIT,
        /// <summary>
        /// 南粤银行 （信用卡）
        /// </summary>
        [Display(Name = "南粤银行 （信用卡）")]
        GDNYB_CREDIT,
        /// <summary>
        /// 广州银行（信用卡）
        /// </summary>
        [Display(Name = "广州银行（信用卡）")]
        GZCB_CREDIT,
        /// <summary>
        /// 江苏银行（借记卡）
        /// </summary>
        [Display(Name = "江苏银行（借记卡）")]
        JSB_DEBIT,
        /// <summary>
        /// 江苏银行（信用卡）
        /// </summary>
        [Display(Name = "")]
        JSB_CREDIT,
        /// <summary>
        /// 宁波银行（借记卡）
        /// </summary>
        [Display(Name = "宁波银行（借记卡）")]
        NBCB_DEBIT,
        /// <summary>
        /// 宁波银行（信用卡）
        /// </summary>
        [Display(Name = "宁波银行（信用卡）")]
        NBCB_CREDIT,
        /// <summary>
        /// 南京银行（借记卡）
        /// </summary>
        [Display(Name = "南京银行（借记卡）")]
        NJCB_DEBIT,
        /// <summary>
        /// 青岛银行（借记卡）
        /// </summary>
        [Display(Name = "青岛银行（借记卡）")]
        QDCCB_DEBIT,
        /// <summary>
        ///  浙江泰隆银行（借记卡） 
        /// </summary>
        [Display(Name = " 浙江泰隆银行（借记卡） ")]
        ZJTLCB_DEBIT,
        /// <summary>
        /// 西安银行（借记卡）  
        /// </summary>
        [Display(Name = "西安银行（借记卡）  ")]

        XAB_DEBIT,
        /// <summary>
        /// 常熟农商银行 （借记卡）
        /// </summary>
        [Display(Name = "常熟农商银行 （借记卡）")]
        CSRCB_DEBIT,
        /// <summary>
        /// 齐鲁银行（借记卡）
        /// </summary>
        [Display(Name = "齐鲁银行（借记卡）")]
        QLB_DEBIT,
        /// <summary>
        ///  龙江银行（借记卡）
        /// </summary>
        [Display(Name = " 龙江银行（借记卡）")]
        LJB_DEBIT,
        /// <summary>
        /// 华夏银行（借记卡）
        /// </summary>
        [Display(Name = "华夏银行（借记卡）")]
        HXB_DEBIT,
        /// <summary>
        /// 测试银行借记卡快捷支付 （借记卡）
        /// </summary>
        [Display(Name = "测试银行借记卡快捷支付 （借记卡）")]
        CS_DEBIT,
        /// <summary>
        /// AE （信用卡）
        /// </summary>
        [Display(Name = "AE （信用卡）")]
        AE_CREDIT,
        /// <summary>
        ///  JCB （信用卡）
        /// </summary>
        [Display(Name = " JCB （信用卡）")]
        JCB_CREDIT,
        /// <summary>
        /// MASTERCARD （信用卡）
        /// </summary>
        [Display(Name = "MASTERCARD （信用卡）")]
        MASTERCARD_CREDIT,
        /// <summary>
        /// VISA （信用卡）
        /// </summary>
        [Display(Name = "VISA （信用卡）")]
        VISA_CREDIT,
        /// <summary>
        /// 未知
        /// </summary>
        [Display(Name = "未知")]
        Unknow
    }
}
