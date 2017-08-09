using Framework.Common.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Framework.Common.Mvc
{
    public class BaseController : Controller
    {
        private const string DateFormat = "yyyy-MM-ddTHH:mm:ss";

        protected JsonResult Fail(ErrorCode errorCode, string message, bool mvcJson = false, JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet)
        {
            var resp = new GeneralResponseModel<object>
            {
                Success = false,
                Data = null,
                Message = message,
                MessageCode = (int)errorCode
            };

            if (mvcJson)
            {
                return new JsonResult { Data = resp, JsonRequestBehavior = behavior };
            }
            else
            {
                return Json(resp, behavior);
            }
        }

        protected JsonResult Success()
        {
            return Success(true);
        }

        protected JsonResult Success(object data, int totalCount = 0, bool mvcJson = false, JsonRequestBehavior behavior = JsonRequestBehavior.AllowGet, string format = DateFormat)
        {
            var t = data.GetType();
            object resp; ;

            if (t.Name.Equals(typeof(GeneralResponseModel<object>).Name))
            {
                resp = data;
            }
            else
            {
                resp = new GeneralResponseModel<object>
                {
                    Data = data,
                    TotalCount = totalCount
                };
            }

            if (mvcJson)
            {
                return new JsonResult { Data = resp, JsonRequestBehavior = behavior };
            }
            else
            {
                return Json(resp, behavior);
            }
        }

        protected override JsonResult Json(object data, string contentType, Encoding contentEncoding, JsonRequestBehavior behavior)
        {
            return new BaseJsonResult
            {
                Data = data,
                ContentType = contentType,
                ContentEncoding = contentEncoding,
                JsonRequestBehavior = behavior,
                DateFormatStr = DateFormat
            };
        }

        protected JsonResult Json(object data, JsonRequestBehavior behavior, string format = DateFormat)
        {
            return new BaseJsonResult
            {
                Data = data,
                DateFormatStr = format
            };
        }

        [AllowAnonymous]
        public ActionResult AuthActionFaild()
        {
            return View();
        }
    }
}
