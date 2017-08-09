using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hdy.Media.Models;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Hdy.Media.ViewModels
{
    [NotMapped]
    public class AdQueryViewModel : AdvertisementModel
    {
        public string QueryText { get; set; }
        /// <summary>
        /// 包含过期内容
        /// </summary>
        public bool IncludeExpired { get; set; } = true;
    }

    [NotMapped]
    public class AdDowloadQueryViewModel : AdDowloadRecordModel
    {
        public string QueryText { get; set; }
    }

    public class SchedulingQueryViewModel
    {
        [DisplayName("省份")]
        public string Province { get; set; }
        [DisplayName("月分")]
        public string City { get; set; }
        [DisplayName("区/县")]
        public string Region { get; set; }
        [DisplayName("开通区域")]
        public string OpenArea { get; set; }
        [DisplayName("月份")]
        public string PlayMonth { get; set; } = DateTime.Now.ToString("yyyy-MM");
        public string[] DevicesIds { get; set; }
    }

    public class PlayTimesSelectorViewModel
    {
        public string Value { get; set; }
        public string Name { get; set; }
        public string Checked { get; set; }

        [JsonIgnore]
        public static List<PlayTimesSelectorViewModel> Items
        {
            get
            {
                var items = new List<PlayTimesSelectorViewModel>();
                var selected = new int[] { 11, 12, 13, 18, 19, 20 };
                for (int i = 0; i < 4; i++)
                {
                    string item = string.Concat((i * 6).ToString("d2"), ":00", "-", ((i + 1) * 6).ToString("d2"), ":00");
                    items.Add(new PlayTimesSelectorViewModel
                    {
                        Name = item,
                        Value = item,
                        Checked = string.Empty // selected.Contains(i) ? "Checked" : string.Empty
                    });
                }
                return items;
            }
        }
    }

    public class RedirectUrlViewModel
    {
        public string Name { get; set; }
        public string RedirectUrl { get; set; }
    }
}