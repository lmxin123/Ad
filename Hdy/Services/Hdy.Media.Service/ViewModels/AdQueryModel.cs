using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hdy.Media.Service.ViewModels
{
    public class SchedulingQueryModel
    {
        public string DeviceCode { get; set; }
        public string BeginDate { get; set; }
        public string EndDate { get; set; }
    }
}