using Hdy.Media.Models;
using Hdy.Media.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hdy.Media.Service.ViewModels
{
    public class SchedulingViewModel
    {
        public string SchedulingName { get; set; }
        public string DeviceCode { get; set; }
        public string DeviceName { get; set; }
        public string BeginPlayDate { get; set; }
        public string EndPlayDate { get; set; }
        public int PlayCount { get; set; }
        public bool Loop { get; set; }
        public List<string> PlayTimes { get; set; }
        public List<AdViewModel> AdList { get; set; }
    }
    public class AdViewModel
    {
        public string AdName { get; set; }
        public long Size { get; set; }
        public string DowloadUrl { get; set; }
        public IEnumerable<RedirectUrlViewModel> Files { get; set; }
    }
}