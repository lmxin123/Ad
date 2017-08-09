using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Threading.Tasks;
using Framework.Data;
using Framework.Common.Extensions;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Framework.Common.Json;
using Framework.Common;
using System.Data.Entity;
using System.Web.Mvc;
using Framework.Common.Mvc;

namespace Hdy.Media.Models
{
    /// <summary>
    /// 终端分类表
    /// </summary>
    [Table("DeviceCategory")]
    public class DeviceCategoryModel : BaseModel
    {
        [DisplayName("分类名称")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "{0}必需在{1}个字符以内")]
        [Remote("ValidateNameExist", "DeviceCategory", AdditionalFields = "ID,Name", ErrorMessage = "{0}己存在")]
        public string Name { get; set; }
    }
    /// <summary>
    /// 终端
    /// </summary>
    [Table("Device")]
    public partial class DeviceModel : BaseModel
    {
        [DisplayName("终端名称")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "{0}必需在{1}个字符以内")]
        [Remote("ValidateNameExist", "Device", AdditionalFields = "ID,Province,City,Region", ErrorMessage = "{0}己存在")]
        public string Name { get; set; }
        [StringLength(50)]
        [DisplayName("终端编号")]
        [Required(ErrorMessage = "请输入{0}")]
        [Index("IX_DeviceCode", IsUnique = true)]
        [Remote("ValidateDeviceCodeExist", "Device", AdditionalFields = "ID,DeviceCode", ErrorMessage = "{0}己存在")]
        public string DeviceCode { get; set; }
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
        [StringLength(50)]
        [DisplayName("区域")]
        public string OpenArea { get; set; }
        [DisplayName("详细地址")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "{0}必需在{2}到{1}个字符之间")]
        public string Address { get; set; }
        [StringLength(50)]
        [DisplayName("终端分类")]
        [Required(ErrorMessage = "请选择{0}")]
        public string CategoryID { get; set; }
        [DisplayName("数量")]
        //[Required(ErrorMessage = "请输入{0}")]
        [Range(0, 1000, ErrorMessage = "{0}必需大于{1}小于{2}的正整数")]
        [RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "{0}必需是正整数")]
        public int Count { get; set; }
        [DisplayName("负责人")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "{0}必需在{1}个字符以内")]
        public string Contact { get; set; }
        [DisplayName("联系电话")]
        [Required(ErrorMessage = "请输入{0}")]
        [RegularExpression(@"((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)", ErrorMessage = "{0}格式不正确")]
        public string Phone { get; set; }
        [DisplayName("邮件地址")]
        //[Required(ErrorMessage = "请输入{0}")]
        [EmailAddress(ErrorMessage = "{0}格式不正确")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "{0}必需在{2}到{1}个字符之间")]
        public string Email { get; set; }
        [DisplayName("是否在线")]
        public OnlineType IsOnline { get; set; }

        [ForeignKey("CategoryID")]
        protected virtual DeviceCategoryModel DeviceCategory { get; set; }
    }

    public partial class DeviceModel
    {
        [NotMapped]
        public string IsOnlineDesplay
        {
            get
            {
                return IsOnline.GetDisplayName();
            }
        }
    }

    /// <summary>
    /// 在线状态
    /// </summary>
    public enum OnlineType
    {
        [Display(Name = "全部")]
        All,
        [Display(Name = "在线")]
        Online,
        [Display(Name = "离线")]
        OffLine,
        [Display(Name = "未知")]
        Unkown
    }

    public class DeviceCategoryModelManager : BaseManager<MediaDbContext, DeviceCategoryModel>
    {

    }

    public class DeviceModelManager : BaseManager<MediaDbContext, DeviceModel>
    {
        public async Task<GeneralResponseModel<List<DeviceModel>>> QueryAsync(DeviceModel model, int pageIndex, int pageSize)
        {
            if (model == null)
                return await base.QueryAsync(pageIndex, pageSize, RecordStates.All);

            var query = Model.Where(item => item.RecordState == RecordStates.AuditPass);

            if (!string.IsNullOrEmpty(model.Province))
                query = query.Where(item => item.Province == model.Province);
            if (!string.IsNullOrEmpty(model.City))
                query = query.Where(item => item.City == model.City);
            if (!string.IsNullOrEmpty(model.Region))
                query = query.Where(item => item.Region == model.Region);
            if (!string.IsNullOrEmpty(model.OpenArea))
                query = query.Where(item => item.OpenArea.Contains(model.OpenArea));

            var list = await query
           .OrderByDescending(f => f.CreateDate)
           .Skip(pageSize * (pageIndex - 1))
           .Take(pageSize)
           .ToListAsync();

            var result = new GeneralResponseModel<List<DeviceModel>>
            {
                Data = list,
                TotalCount = await query.CountAsync()
            };

            return result;
        }
        /// <summary>
        /// 生成终端编号，规则是省份城市地区编码加上自动增长的5位数字
        /// </summary>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public string GeneralDeviceCode(string province, string city, string region)
        {
            if (string.IsNullOrEmpty(province))
                throw new ArgumentNullException("province 参数不能为空");
            if (string.IsNullOrEmpty(city))
                throw new ArgumentNullException("city 参数不能为空");
            if (string.IsNullOrEmpty(region))
                throw new ArgumentNullException("region 参数不能为空");

            var maxCodeStr = Model
                .Where(item => item.Province == province && item.City == city && item.Region == region)
                .Max(item => item.DeviceCode);

            long maxCodeInt = 0;
            long.TryParse(maxCodeStr, out maxCodeInt);

            maxCodeInt += 1;

            return maxCodeInt.ToString("d5");
        }
    }
}
