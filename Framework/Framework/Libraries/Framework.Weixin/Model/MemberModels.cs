using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Framework.Data;

namespace Framework.Weixin.Model
{
    /// <summary>
    /// 微信会员实体
    /// </summary>
    public partial class MemberModel : BaseModel
    {
        /// <summary>
        ///微信OpenID
        /// </summary>
        [Display(Name = "微信OpenID")]
        [MaxLength(50)]
        public virtual string OpenID { get; set; }
        /// <summary>
        /// UnionID机制,同一个微信开放平台帐号下的移动应用、网站应用和公众帐号，
        /// 用户的unionid是唯一的.
        /// 默认情况下，如果没有公众号之外的其它应用，此字段跟OpenID相同。
        /// 建议所有跟会员相关的表都以此字段为主键
        /// </summary>
        [Display(Name = "微信UnionID")]
        [MaxLength(50)]
        public virtual string UnionID { get; set; }
        /// <summary>
        ///微信昵称	
        /// </summary>
        [Display(Name = "微信昵称")]
        [StringLength(20, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        public virtual string WeixinName { get; set; }
        /// <summary>
        ///会员名称
        /// </summary>
        [Display(Name = "会员名称")]
        [StringLength(20, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        public virtual string UserName { get; set; }
        /// <summary>
        ///真实姓名	
        /// </summary>
        [Display(Name = "真实姓名")]
        [StringLength(20, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 2)]
        public virtual string RealName { get; set; }
        /// <summary>
        /// 微信头像
        /// </summary>
        [Display(Name = "头像")]
        public virtual string HeadImgUrl { get; set; }
        /// <summary>
        ///密码，默认为123456
        /// </summary>
        [Display(Name = "密码"), DataType(DataType.Password), StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 3)]
        public virtual string Password { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "省份")]
        public virtual string Province { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "城市")]
        public virtual string City { get; set; }
        /// <summary>
        /// 地区
        /// </summary>
        [MaxLength(20)]
        [Display(Name = "地区")]
        public virtual string Region { get; set; }
        /// <summary>
        ///性别	
        /// </summary>
        [Display(Name = "性别")]
        public virtual int Sex { get; set; }
        /// <summary>
        ///生日
        /// </summary>
        [Display(Name = "生日")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public virtual DateTime? Birthday { get; set; }
        /// <summary>
        ///修改日期
        /// </summary>
        [Display(Name = "修改日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:SS}", ApplyFormatInEditMode = true)]
        public virtual DateTime? ModifyDate { get; set; }
        /// <summary>
        /// 最后登录时间
        /// </summary>
        [Display(Name = "最后登录日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:SS}", ApplyFormatInEditMode = true)]
        public virtual DateTime LastLoginDate { get; set; }
    }

    public partial class MemberModel
    {
        /// <summary>
        /// 性别显示字符
        /// </summary>
        [NotMapped]
        public virtual string SexText
        {
            get
            {
                return Sex == 1 ? "男" : "女";
            }
        }
    }
}
