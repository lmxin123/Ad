using System;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Framework.Data;
using Framework.Common.Json;
using Framework.Common;

using Hdy.Media.ViewModels;
using System.Web.Mvc;

namespace Hdy.Media.Models
{
    /// <summary>
    /// 己开通业务的区域表
    /// </summary>
    [Table("OpenArea")]
    public partial class OpenAreaModel : BaseModel
    {
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
        [DisplayName("区域名称")]
        [Required(ErrorMessage = "请输入{0}")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "{0}必需在{1}个字符以内")]
        [Remote("ValidateNameExist", "OpenArea", AdditionalFields = "ID,Province,City,Region", ErrorMessage = "{0}己存在")]
        public string Name { get; set; }
    }

    public partial class OpenAreaModel
    {
        [NotMapped]
        public string FullName
        {
            get
            {
                return string.Concat(Province, City, Region, Name);
            }
        }
    }

    public class OpenAreaModelManager : BaseManager<MediaDbContext, OpenAreaModel>
    {
        public async Task<GeneralResponseModel<List<OpenAreaModel>>> QueryAsync(OpenAreaModel model, int pageIndex, int pageSize)
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

            var list = await query
           .OrderByDescending(f => f.CreateDate)
           .Skip(pageSize * (pageIndex - 1))
           .Take(pageSize)
           .ToListAsync();

            var result = new GeneralResponseModel<List<OpenAreaModel>>
            {
                Data = list,
                TotalCount = await query.CountAsync()
            };

            return result;
        }

        public async Task<IEnumerable<OpenAreaSelectorViewModel>> GetSelectorListAsync(string province, string city, string region)
        {
            var list = await Model
                .Where(m => m.Province == province && m.City == city && m.Region == region)
                .Select(m => new { m.ID, m.Name, m.CreateDate })
                .OrderByDescending(f => f.CreateDate)
                .ToListAsync();

            return list.Select(m => new OpenAreaSelectorViewModel
            {
                Value = m.ID,
                Name = m.Name,
                Checked = list.Count == 1 ? "Checked" : string.Empty
            });
        }
    }
}
