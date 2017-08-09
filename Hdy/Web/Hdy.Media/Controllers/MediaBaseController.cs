using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Framework.Common.Mvc;
using Framework.Auth;

namespace Hdy.Media.Controllers
{
    //  [NoCache]
    [ActionAuthorize]
    public class MediaBaseController : BaseController
    {
        protected string UserId
        {
            get
            {
                return User.Identity.GetUserId();
            }
        }
    }
}