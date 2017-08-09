using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Data.Entity;
using Newtonsoft.Json;

using Framework.Data;
using Framework.Common;
using Framework.Common.Json;
using Hdy.Media.ViewModels;

namespace Hdy.Media.Models
{
    /// <summary>
    /// 广告内容表
    /// </summary>
    [Table("Advertisement")]
    public partial class AdvertisementModel : BaseModel
    {
        [DisplayName("商户Id")]
        [Required(ErrorMessage = "缺少{0}")]
        [StringLength(50, ErrorMessage = "{0}必需在{1}个字符以内")]
        public string MerchantID { get; set; }
        [DisplayName("用户Id")]
        [Required(ErrorMessage = "缺少{0}")]
        [StringLength(50, ErrorMessage = "{0}必需在{1}个字符以内")]
        public string UserID { get; set; }
        [DisplayName("内容名称")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "{0}最少为两个长度")]
        [Remote("ValidateAdNameExist", "AdList", AdditionalFields = "ID", ErrorMessage = "{0}己存在")]
        public string Name { get; set; }
        [StringLength(50)]
        [DisplayName("播放省份")]
        [Required(ErrorMessage = "请选择{0}")]
        public string Province { get; set; }
        [StringLength(50)]
        [DisplayName("播放城市")]
        [Required(ErrorMessage = "请选择{0}")]
        public string City { get; set; }
        [StringLength(50)]
        [DisplayName("播放县/区")]
        [Required(ErrorMessage = "请选择{0}")]
        public string Region { get; set; }
        [StringLength(int.MaxValue)]
        [DisplayName("播放区域")]
        [Required(ErrorMessage = "请选择{0}")]
        public string OpenAreas { get; set; }
        [DisplayName("播放日期")]
        [Required(ErrorMessage = "请选择{0}")]
        [StringLength(10, ErrorMessage = "{0}必需在{1}个字符以内")]
        public string BeginPlayDate { get; set; } = DateTime.Now.ToString(DateFormetStr);
        [DisplayName("结束日期")]
        [Required(ErrorMessage = "请选择{0}")]
        [StringLength(10, ErrorMessage = "{0}必需在{1}个字符以内")]
        public string EndPlayDate { get; set; } = DateTime.Now.ToString(DateFormetStr);
        [StringLength(int.MaxValue)]
        [DisplayName("播放间段")]
        [Required(ErrorMessage = "请选择{0}")]
        public string PlayTimes { get; set; }
        [DisplayName("播放次数")]
        public int PlayCount { get; set; }
        public override RecordStates RecordState { get; set; } = RecordStates.PendingAudit;
        /// <summary>
        /// 审核的时候用
        /// </summary>
        [DisplayName("失败原因")]
        [StringLength(100, ErrorMessage = "{0}必需在{1}个字符以内")]
        public string RefusalReasons { get; set; }

        [ForeignKey("MerchantID")]
        public virtual CorpModel Merchant { get; set; }

        public virtual IEnumerable<AdvertisementDetailModel> Details { get; set; }
    }

    public partial class AdvertisementModel
    {
        [NotMapped]
        public string OpenAreaDisplay
        {
            get
            {
                if (!string.IsNullOrEmpty(OpenAreas))
                {
                    var items = JsonConvert.DeserializeObject<List<OpenAreaSelectorViewModel>>(OpenAreas);
                    return string.Join("，", items.Select(item => item.Name));
                }
                return string.Empty;
            }
        }

        [NotMapped]
        public string PlayTimeDisplay
        {
            get
            {
                if (!string.IsNullOrEmpty(PlayTimes))
                {
                    var items = JsonConvert.DeserializeObject<List<PlayTimesSelectorViewModel>>(PlayTimes);
                    return string.Join("，", items.Select(item => item.Name));
                }
                return string.Empty;
            }
        }
    }

    [Table("AdvertisementDetail")]
    public partial class AdvertisementDetailModel : BaseModel
    {
        [StringLength(50)]
        public string AdvertisementID { get; set; }
        [Required]
        [StringLength(50)]
        [DisplayName("名称")]
        public string FileName { get; set; }
        [JsonIgnore]
        [Required]
        [DisplayName("文件大小")]
        public long Size { get; set; }
        [StringLength(2000)]
        [DisplayName("跳转地址")]
        public string RedirectUrl { get; set; }
        [Required]
        [StringLength(50)]
        [DisplayName("类型")]
        public string ContentType { get; set; }
        [Required]
        [StringLength(200)]
        [DisplayName("存储路径")]
        public string StoragePath { get; set; }

        [ForeignKey("AdvertisementID")]
        public virtual AdvertisementModel Advertisement { get; set; }
    }

    public partial class AdvertisementDetailModel
    {
        [NotMapped]
        public string SizeDisplay
        {
            get
            {
                return $"{Math.Round(Size / (decimal)1048576, 2)}mb";
            }
        }
        [NotMapped]
        public string ThumbPath { get; set; }
    }
    /// <summary>
    /// 广告类型枚举
    /// </summary>
    public enum AdType
    {
        [Display(Name = "全部")]
        All,//主要用于查询
        [Display(Name = "文本")]
        Text,
        [Display(Name = "图片")]
        Image,
        [Display(Name = "视频")]
        Vedio
    }

    public class AdvertisementModelManager : BaseManager<MediaDbContext, AdvertisementModel>
    {
        public async Task<GeneralResponseModel<List<AdvertisementModel>>> QueryAsync(AdQueryViewModel model, int pageIndex, int pageSize)
        {
            if (model == null)
                return await base.QueryAsync(pageIndex, pageSize, RecordStates.All);

            var query = Model.Where(item => 1 == 1);
            if (!string.IsNullOrEmpty(model.MerchantID))
                query = query.Where(item => item.MerchantID == model.MerchantID);
            if (!string.IsNullOrEmpty(model.Province))
                query = query.Where(item => item.Province == model.Province);
            if (!string.IsNullOrEmpty(model.City))
                query = query.Where(item => item.City == model.City);
            if (!string.IsNullOrEmpty(model.Region))
                query = query.Where(item => item.Region == model.Region);
            if (!string.IsNullOrEmpty(model.OpenAreas))
                query = query.Where(item => item.OpenAreas.Contains(model.OpenAreas));
            if (!string.IsNullOrEmpty(model.UserID))
                query = query.Where(item => item.UserID == model.UserID);

            if (model.RecordState != RecordStates.All)
                query = query.Where(item => item.RecordState == model.RecordState);
            if (!string.IsNullOrEmpty(model.QueryText))
                query = query.Where(item => item.Name.Contains(model.QueryText));
            if (!model.IncludeExpired)
            {
                var now = DateTime.Now.ToString("yyyy-MM-dd");
                query = query.Where(item => string.Compare(item.EndPlayDate, now, StringComparison.Ordinal) >= 0);
            }



            var list = query
           .OrderByDescending(f => f.CreateDate)
           .Skip(pageSize * (pageIndex - 1))
           .Take(pageSize)
           .ToList();

            var result = new GeneralResponseModel<List<AdvertisementModel>>
            {
                Data = list,
                TotalCount = query.Count()
            };

            return result;
        }
    }

    public class AdvertisementDetailModelManager : BaseManager<MediaDbContext, AdvertisementDetailModel>
    {
        public List<AdvertisementDetailModel> GetListByAdId(string adId)
        {
            return Model.Where(a => a.AdvertisementID == adId).ToList();
        }
    }

}
