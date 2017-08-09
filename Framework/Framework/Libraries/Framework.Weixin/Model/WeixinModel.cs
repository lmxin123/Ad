using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Framework.Data;

namespace Framework.Weixin.Model
{
    /// <summary>
    /// 继承至CommonModel，把需要与微信用户表关联的属性抽出来
    /// </summary>
    public class WeixinModel : BaseModel
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Required]
        public string MemberID { get; set; }
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
        /// 外键
        /// </summary>
        [ForeignKey("MemberID")]
        protected MemberModel Member { get; set; }
    }
}
