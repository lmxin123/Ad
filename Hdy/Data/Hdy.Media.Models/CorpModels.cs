using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Framework.Data;
using Framework.Common;
using Framework.Common.Mvc;
using Framework.Common.Json;
using Framework.Common.Extensions;

using Hdy.Media.ViewModels;

namespace Hdy.Media.Models
{
    /// <summary>
    /// 商户信息表
    /// </summary>
    [Table("Corp")]
    public partial class CorpModel : BaseModel
    {
        /// <summary>
        /// 关联用户Id
        /// </summary>
        [StringLength(50)]
        public string UserID { get; set; }
        [DisplayName("商户名称")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "{0}必需在{2}到{1}个字符之间")]
        [Remote("ValidateNameExist", "CorpInfo", AdditionalFields = "ID", ErrorMessage = "{0}己存在")]
        public string Name { get; set; }
        [DisplayName("商户类型")]
        [Required(ErrorMessage = "请选择{0}")]
        public MerchantTypes MerchantType { get; set; }
        [StringLength(50)]
        [DisplayName("省份")]
        [Required(ErrorMessage = "请选择{0}")]
        public string Province { get; set; }
        [StringLength(50)]
        [DisplayName("城市")]
        [Required(ErrorMessage = "请选择{0}")]
        public string City { get; set; }
        [StringLength(50)]
        [DisplayName("县/区")]
        [Required(ErrorMessage = "请选择{0}")]
        public string Region { get; set; }
        [DisplayName("地址")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "{0}必需在{2}到{1}个字符之间")]
        public string Address { get; set; }
        [DisplayName("邮政编码")]
        [DataType(DataType.PostalCode, ErrorMessage = "{0}格式不正确")]
        [RegularExpression(@"[1-9]\d{5}(?!\d)", ErrorMessage = "{0}格式不正确")]
        public int? PostCode { get; set; }
        [DisplayName("联系人")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "{0}必需在{2}到{1}个字符之间")]
        public string Contact { get; set; }
        [DisplayName("联系电话")]
        [Required(ErrorMessage = "请输入{0}")]
        [DataType(DataType.PhoneNumber)]
        [StringLength(20, ErrorMessage = "{0}必需在{1}个字符以内")]
        [RegularExpression(@"((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)", ErrorMessage = "{0}格式不正确")]
        public string Phone { get; set; }
        [DisplayName("传真")]
        [StringLength(50, ErrorMessage = "{0}必需在{1}个字符以内")]
        public string Fax { get; set; }
        [DisplayName("电子邮箱")]
        [EmailAddress(ErrorMessage = "{0}格式不正确")]
        [StringLength(50, ErrorMessage = "{0}必需在{1}个字符以内")]
        [DataType(DataType.EmailAddress, ErrorMessage = "{0}格式不正确")]
        public string Email { get; set; }
        [DisplayName("法人代表")]
        [StringLength(50, ErrorMessage = "{0}必需在{1}个字符以内")]
        public string LegalPerson { get; set; }
        [DisplayName("法人证件类型")]
        public LegalPersonCertificateTypes LegalPersonCertificateType { get; set; }
        [DisplayName("证件号码")]
        [IdentityCard(ErrorMessage = "{0}无效")]
        [StringLength(50, ErrorMessage = "{0}必需在{1}个字符以内")]
        public string LegalPersonIdentityNo { get; set; }
        [DisplayName("营业执照号")]
        [StringLength(50, ErrorMessage = "{0}必需在{1}个字符以内")]
        public string BusinessLicenseNo { get; set; }
        [DisplayName("税务登记号")]
        [StringLength(50, ErrorMessage = "{0}超出了{1}个字符长度")]
        public string TaxRegistrationNo { get; set; }
        [DisplayName("营业执照照片")]
        [StringLength(500, ErrorMessage = "{0}必需在{1}个字符以内")]
        public string BusinessLicensePath { get; set; }
        [DisplayName("经营范围")]
        [StringLength(500, ErrorMessage = "{0}必需在{1}个字符以内")]
        public string BusinessScope { get; set; }
        [DisplayName("登记机关")]
        [StringLength(500, ErrorMessage = "{0}必需在{1}个字符以内")]
        public string RegisteredAddress { get; set; }
        [DisplayName("简介")]
        [StringLength(1000, ErrorMessage = "{0}必需在{1}个字符以内")]
        public string Summary { get; set; }
        /// <summary>
        /// 拒绝理由，审核时用
        /// </summary>
        [DisplayName("拒绝理由")]
        [StringLength(500, ErrorMessage = "{0}必需在{1}个字符以内")]
        public string RefusalReasons { get; set; }
    }

    public partial class CorpModel
    {
        [NotMapped]
        public string MerchantTypeDisplay
        {
            get
            {
                return MerchantType.GetDisplayName();
            }
        }

        [NotMapped]
        public string LegalPersonCertificateTypeDisplay
        {
            get
            {
                return LegalPersonCertificateType.GetDisplayName();
            }
        }

        public override RecordStates RecordState { get; set; } = RecordStates.PendingAudit;
    }

    public enum MerchantTypes
    {
        [Display(Name = "请选择")]
        All,
        [Display(Name = "个人独资")]
        SoleProprietorship,
        [Display(Name = "合伙企业")]
        PartnershipEnterprise,
        [Display(Name = "公司")]
        Company
    }

    /// <summary>
    /// 法人证件类型
    /// </summary>
    public enum LegalPersonCertificateTypes
    {
        [Display(Name = "请选择")]
        All,
        [Display(Name = "居民身份证")]
        IdentityCard,
        [Display(Name = "国外 护照")]
        ForeignPassport
    }

    public class CorpModelManager : BaseManager<MediaDbContext, CorpModel>
    {
        public async Task<GeneralResponseModel<List<CorpModel>>> QueryAsync(MerchantQueryViewModel model, int pageIndex, int pageSize)
        {
            if (model == null)
                return await base.QueryAsync(pageIndex, pageSize, RecordStates.All);

            var query = Model.Where(item => 1 == 1);

            if (!string.IsNullOrEmpty(model.UserID))
                query = query.Where(item => item.UserID == model.UserID);

            if (!string.IsNullOrEmpty(model.Province))
                query = query.Where(item => item.Province == model.Province);

            if (!string.IsNullOrEmpty(model.City))
                query = query.Where(item => item.City == model.City);

            if (!string.IsNullOrEmpty(model.Region))
                query = query.Where(item => item.Region == model.Region);

            if (model.MerchantType != MerchantTypes.All)
                query = query.Where(item => item.MerchantType == model.MerchantType);

            if (model.LegalPersonCertificateType != LegalPersonCertificateTypes.All)
                query = query.Where(item => item.LegalPersonCertificateType == model.LegalPersonCertificateType);

            if (model.RecordState != RecordStates.All)
                query = query.Where(item => item.RecordState == model.RecordState);

            if (!string.IsNullOrEmpty(model.QueryText))
                query = query.Where(item => item.Name.Contains(model.QueryText)
                || item.Contact.Contains(model.QueryText)
                || item.Phone.Contains(model.QueryText));

            var list = query
           .OrderByDescending(f => f.CreateDate)
           .Skip(pageSize * (pageIndex - 1))
           .Take(pageSize)
           .ToList();

            var result = new GeneralResponseModel<List<CorpModel>>
            {
                Data = list,
                TotalCount = query.Count()
            };

            return result;
        }

        public async Task<CorpModel> GetByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentNullException("userId 不能为空");

            return await Model.FirstOrDefaultAsync(m => m.UserID == userId);
        }
    }
}
