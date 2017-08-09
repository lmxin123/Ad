using System;
using System.Threading.Tasks;
using System.Web.Mvc;

using Framework.Common.Json;
using Hdy.Media.Models;
using Hdy.Media.ViewModels;

namespace Hdy.Media.Controllers
{
    public class AdDowloadController : MediaBaseController
    {
        protected readonly AdDowloadRecordModelManager _AdDowloadRecordManager;

        public AdDowloadController()
        {
            _AdDowloadRecordManager = _AdDowloadRecordManager ?? new AdDowloadRecordModelManager();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetList(AdDowloadQueryViewModel model, int pageIndex, int pageSize)
        {
            try
            {
                var result = await _AdDowloadRecordManager.QueryAsync(model, pageIndex, pageSize);

                return Success(result);
            }
            catch (Exception e)
            {
                return Fail(ErrorCode.DataError, e.Message);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_AdDowloadRecordManager != null)
                {
                    _AdDowloadRecordManager.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}