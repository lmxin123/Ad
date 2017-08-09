using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Data.Entity;

using Framework.Data;
using Framework.Common;
using Framework.Common.Extensions;
using Hdy.Media.ViewModels;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Hdy.Media.Models
{
    /// <summary>
    /// 排期状态
    /// </summary>
    public enum SchedulingStatus
    {
        [Display(Name = "全部")]
        All,//主要用于查询
        [Display(Name = "等待播放")]
        WaitingForPlay,
        [Display(Name = "播放中")]
        Playing,
        [Display(Name = "暂停播放")]
        Pause,
        [Display(Name = "己过期")]
        HasExpired
    }
    /// <summary>
    /// 排期表
    /// </summary>
    [Table("Scheduling")]
    public partial class SchedulingModel : BaseModel
    {
        [StringLength(50)]
        [DisplayName("设备")]
        public string DeviceID { get; set; }
        [DisplayName("排期名称")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "{0}必需在{2}到{1}个字符之间")]
        [Remote("ValidateNameExist", "AdScheduling", AdditionalFields = "ID", ErrorMessage = "{0}己存在")]
        public string Name { get; set; }
        [StringLength(10, ErrorMessage = "{0}必需在{1}个字符以内")]
        [DisplayName("播放日期")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "请选择{0}")]
        public string BeginPlayDate { get; set; }
        [StringLength(10, ErrorMessage = "{0}必需在{1}个字符以内")]
        [DisplayName("结束日期")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "请选择{0}")]
        public string EndPlayDate { get; set; }
        [DisplayName("播放时段")]
        [Required(ErrorMessage = "请选择{0}")]
        public string PlayTimes { get; set; }
        [DisplayName("时段内播放次数")]
        [Required(ErrorMessage = "请输入{0}")]
        [RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "{0}必需是正整数")]
        public int PlayCount { get; set; }
        [DisplayName("播放顺序")]
        [Required(ErrorMessage = "请输入{0}")]
        [RegularExpression(@"^[0-9]*[1-9][0-9]*$", ErrorMessage = "{0}必需是正整数")]
        public int Order { get; set; }
        [DisplayName("循环播放")]
        public bool Loop { get; set; } = true;
        [DisplayName("排期状态")]
        public SchedulingStatus SchedulingStatu { get; set; } = SchedulingStatus.WaitingForPlay;
        /// <summary>
        /// 是否己打包，己打包的排期不能修改删除
        /// </summary>
        [DisplayName("己打包")]
        public bool HasBeenPackage { get; set; } = false;

        [ForeignKey("DeviceID")]
        public virtual DeviceModel Device { get; set; }
        /// <summary>
        /// 排期详情
        /// </summary>
        public virtual ICollection<SchedulingDetailModel> Details { get; set; }
    }

    public partial class SchedulingModel
    {
        [NotMapped]
        public string SchedulingStatuDisplay
        {
            get
            {
                return SchedulingStatu.GetDisplayName();
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
    /// <summary>
    /// 排期表详情表
    /// </summary>
    [Table("SchedulingDetail")]
    public class SchedulingDetailModel : BaseModel
    {
        /// <summary>
        /// 排期Id
        /// </summary>
        [Required]
        [StringLength(50)]
        public string SchedulingID { get; set; }
        /// <summary>
        /// 广告Id
        /// </summary>
        [Required]
        [StringLength(50)]
        public string AdvertisementID { get; set; }

        [ForeignKey("SchedulingID")]
        public virtual SchedulingModel Scheduling { get; set; }
        [ForeignKey("AdvertisementID")]
        public virtual AdvertisementModel Advertisement { get; set; }
    }

    public class SchedulingModelManager : BaseManager<MediaDbContext, SchedulingModel>
    {
        public SchedulingModelManager() : base() { }

        public SchedulingModelManager(MediaDbContext dbContext) : base(dbContext) { }

        public async Task<List<SchedulingModel>> QueryAsync(SchedulingQueryViewModel model, int pageIndex, int pageSize)
        {
            var items = new List<SchedulingModel>();

            if (model == null) return items;

            var query = Model.Where(item =>
                              item.BeginPlayDate.Contains(model.PlayMonth) &&
                              model.DevicesIds.Contains(item.DeviceID));

            //if (!string.IsNullOrEmpty(model.Province))
            //{
            //    query = query.Where(item => item.Province == model.Province);
            //}
            //if (!string.IsNullOrEmpty(model.City))
            //{
            //    query = query.Where(item => item.City == model.City);
            //}
            //if (!string.IsNullOrEmpty(model.Region))
            //{
            //    query = query.Where(item => item.Region == model.Region);
            //}
            //if (!string.IsNullOrEmpty(model.OpenArea))
            //{
            //    query = query.Where(item => item.OpenAreas == model.OpenArea);
            //}

            query = query.Where(item => item.RecordState == RecordStates.AuditPass);

            items = await query
                      .OrderByDescending(f => f.CreateDate)
                      .Skip(pageSize * (pageIndex - 1))
                      .Take(pageSize)
                      .ToListAsync();

            return items;
        }
    }

    public class SchedulingDetailModelManager : BaseManager<MediaDbContext, SchedulingDetailModel>
    {
        public SchedulingDetailModelManager() : base() { }

        public SchedulingDetailModelManager(MediaDbContext dbContext) : base(dbContext) { }

        public async Task<List<SchedulingDetailModel>> GetBySIdAsync(string sId)
        {
            return await Model.Where(s => s.SchedulingID == sId).ToListAsync();
        }
    }
}
