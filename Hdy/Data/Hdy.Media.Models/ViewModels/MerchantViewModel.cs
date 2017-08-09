using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hdy.Media.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hdy.Media.ViewModels
{
    [NotMapped]
    public class MerchantQueryViewModel:CorpModel
    {
        public string QueryText { get; set; }
    }
}