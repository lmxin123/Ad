using System;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Framework.Common.Mvc
{
    public class BaseJsonResult : JsonResult
    {
        public string DateFormatStr { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            HttpResponseBase response = context.HttpContext.Response;

            response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;

            if (this.ContentEncoding != null)
            {
                response.ContentEncoding = this.ContentEncoding;
            }

            if (Data == null) return;
            var convert = new IsoDateTimeConverter { DateTimeFormat = DateFormatStr };
            var jsonString = JsonConvert.SerializeObject(Data, Formatting.None, convert);

            response.Write(jsonString);
        }
    }
}
