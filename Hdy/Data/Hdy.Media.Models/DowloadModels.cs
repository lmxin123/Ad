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
using Newtonsoft.Json;
using Framework.Common.Json;
using Hdy.Media.ViewModels;
using Framework.Common;
using System.Data.Entity;

namespace Hdy.Media.Models
{
    /// <summary>
    /// 根据请求参数生成的资源包
    /// </summary>
    [Table("AdPackage")]
    public partial class AdPackageModel : BaseModel
    {
        [StringLength(50)]
        public string SchedulingID { get; set; }
        [StringLength(50)]
        public string AdvertisementID { get; set; }
        [StringLength(50)]
        [DisplayName("名称")]
        [Required(ErrorMessage = "请输入{0}")]
        public string Name { get; set; }
        [JsonIgnore]
        [StringLength(500)]
        [DisplayName("存储路径")]
        [Required(ErrorMessage = "请输入{0}")]
        public string Path { get; set; }
        [JsonIgnore]
        [DisplayName("大小")]
        [Required(ErrorMessage = "请输入{0}")]
        public long Size { get; set; }
        [DisplayName("文件数")]
        [Required(ErrorMessage = "请输入{0}")]
        public int Count { get; set; }

        [ForeignKey("SchedulingID")]
        public virtual SchedulingModel Scheduling { get; set; }
        [ForeignKey("AdvertisementID")]
        public virtual AdvertisementModel Advertisement { get; set; }
        /// <summary>
        /// 排期详情
        /// </summary>
        [NotMapped]
        public virtual ICollection<SchedulingDetailModel> Details { get; set; }
    }

    public partial class AdPackageModel
    {
        [NotMapped]
        public string SizeDisplay
        {
            get
            {
                return $"{Size / 1024 * 1024}/mb";
            }
        }
        [NotMapped]
        public string PathName
        {
            get
            {
                return Path + Name;
            }
        }
    }

    /// <summary>
    /// 广告资源包下载记录
    /// </summary>
    [Table("AdDowloadRecord")]
    public partial class AdDowloadRecordModel : BaseModel
    {
        /// <summary>
        /// 下载终端Id
        /// </summary>
        [StringLength(50)]
        public string DevieID { get; set; }
        [StringLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// 包Id
        /// </summary>
        public string AdPackageID { get; set; }
        [DisplayName("终端IP")]
        [Required(ErrorMessage = "请输入{0}")]
        public string IP { get; set; }
        [StringLength(50)]
        [DisplayName("包名称")]
        [Required(ErrorMessage = "请输入{0}")]
        public string AdPackageName { get; set; }
        [StringLength(100)]
        [DisplayName("包存储路径")]
        [Required(ErrorMessage = "请输入{0}")]
        public string AdPackagePath { get; set; }
        [StringLength(500)]
        [DisplayName("下载地址")]
        [Required(ErrorMessage = "请输入{0}")]
        public string Url { get; set; }
        [DisplayName("包大小")]
        [Required(ErrorMessage = "请输入{0}")]
        public long Size { get; set; }
        [DisplayName("文件数")]
        [Required(ErrorMessage = "请输入{0}")]
        public int FileCount { get; set; }
        [StringLength(50)]
        [DisplayName("省份")]
        [Required(ErrorMessage = "请输入{0}")]
        public string Province { get; set; }
        [StringLength(50)]
        [DisplayName("城市")]
        [Required(ErrorMessage = "请输入{0}")]
        public string City { get; set; }
        [StringLength(50)]
        [DisplayName("县/区")]
        [Required(ErrorMessage = "请输入{0}")]
        public string Region { get; set; }
        [StringLength(50)]
        [DisplayName("区域")]
        //  [Required(ErrorMessage = "请输入{0}")]
        public string OpenArea { get; set; }
        [DisplayName("下载次数")]
        [Required(ErrorMessage = "请输入{0}")]
        public int Count { get; set; }
        [DisplayName("最后下载时间")]
        public DateTime LastDowloadDate { get; set; }

        [ForeignKey("DevieID")]
        public virtual DeviceModel Device { get; set; }
        [ForeignKey("AdPackageID")]
        public virtual AdPackageModel AdPackage { get; set; }
    }

    public partial class AdDowloadRecordModel
    {
        [NotMapped]
        public string Path
        {
            get
            {
                return AdPackagePath + AdPackageName;
            }
        }
        [NotMapped]
        public string LastDowloadDateDisplay
        {
            get
            {
                return LastDowloadDate.ToString(DateTimeFormetStr);
            }
        }
    }
    public class AdPackageModelManager : BaseManager<MediaDbContext, AdPackageModel>
    {

    }

    public class AdDowloadRecordModelManager : BaseManager<MediaDbContext, AdDowloadRecordModel>
    {
        public async Task<GeneralResponseModel<List<AdDowloadRecordModel>>> QueryAsync(AdDowloadQueryViewModel model, int pageIndex, int pageSize)
        {
            Db.Configuration.LazyLoadingEnabled = false;

            if (model == null)
                return await base.QueryAsync(pageIndex, pageSize, RecordStates.All);

            var query = Model.Where(item => item.RecordState == RecordStates.AuditPass);

            if (!string.IsNullOrEmpty(model.Province))
                query = query.Where(item => item.Province == model.Province);
            if (!string.IsNullOrEmpty(model.City))
                query = query.Where(item => item.City == model.City);
            if (!string.IsNullOrEmpty(model.Region))
                query = query.Where(item => item.Region == model.Region);

            var list = await query
           .OrderByDescending(f => f.CreateDate)
           .Skip(pageSize * (pageIndex - 1))
           .Take(pageSize)
           .ToListAsync();

            var result = new GeneralResponseModel<List<AdDowloadRecordModel>>
            {
                Data = list,
                TotalCount = await query.CountAsync()
            };

            return result;
        }
    }
}
