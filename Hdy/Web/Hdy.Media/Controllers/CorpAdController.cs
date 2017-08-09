using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

using Framework.Data;
using Framework.Common;
using Framework.Common.Json;

using Hdy.Media.Models;
using Hdy.Media.ViewModels;
using Framework.Auth;
using Framework.Common.IO;

namespace Hdy.Media.Controllers
{
    public class CorpAdController : MediaBaseController
    {
        protected readonly CorpModelManager _CorpManager;
        protected readonly AdListController _AdListController;

        public CorpAdController()
        {
            _CorpManager = _CorpManager ?? new CorpModelManager();
            _AdListController = _AdListController ?? new AdListController();
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);

            _AdListController.ControllerContext = ControllerContext;
        }
        public async Task<ActionResult> Index()
        {
            var userMgr = new ApplicationUserManager(new UserStore<ApplicationUser>(new AuthDbContext()));
            var user = await userMgr.FindByNameAsync(User.Identity.Name);
            var merchant = await _CorpManager.GetByExpreAsync(item => item.UserID == user.Id);

            ViewBag.MerchantID = merchant?.ID;

            return View();
        }

        [HttpPost]
        public async Task<JsonResult> GetList(AdQueryViewModel model, int pageIndex, int pageSize)
        {
            return await _AdListController.GetList(model, pageIndex, pageSize);
        }

        [HttpPost]
        public async Task<JsonResult> GetDetailList(string adId)
        {
            return await _AdListController.GetDetailList(adId);
        }

        [HttpGet]
        public async Task<ActionResult> Create(string aId, string mId)
        {
            return await _AdListController.Create("", mId);
        }

        [HttpGet]
        public async Task<ActionResult> Update(string aId, string mId)
        {
            return await _AdListController.Update(aId, mId);
        }

        [HttpPost]
        public async Task<ActionResult> Create(AdvertisementModel model)
        {
            return await _AdListController.Create(model);
        }

        [HttpPost]
        public async Task<ActionResult> Update(AdvertisementModel model)
        {
            return await _AdListController.Create(model);
        }

        [HttpPost]
        public ActionResult Delete(string id)
        {
            return _AdListController.Delete(id);
        }

        [HttpGet]
        public JsonResult GetBusinessLicense(string filePath)
        {
            string data = string.Empty;

            if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
            {
                data = Utility.ImageToBase64(filePath);
                return Success(data);
            }
            return Fail(ErrorCode.DataError, "营业执照不存在！");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_CorpManager != null)
                {
                    _CorpManager.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}