using Hdy.Media.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hdy.Media.ViewModels
{
    [NotMapped]
    public class AddSchedulingViewModel : SchedulingModel
    {
        [DisplayName("省份")]
        public string Province { get; set; }
        [DisplayName("月分")]
        public string City { get; set; }
        [DisplayName("区/县")]
        public string Region { get; set; }
        [DisplayName("开通区域")]
        public string OpenAreas { get; set; }
        [DisplayName("终端名称")]
        public string DeviceName { get; set; }
        /// <summary>
        /// 以“|”隔开的广告Id
        /// </summary>
        public string AdIDs { get; set; }
        /// <summary>
        /// 以“|”隔开的广告Id，当在修改的时候用
        /// </summary>
        public string DelAdIDs { get; set; }
        public List<AdvertisementModel> AdItems { get; set; }
    }
}
