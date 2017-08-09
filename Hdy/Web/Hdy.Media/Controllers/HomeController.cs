using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hdy.Media.Controllers
{
    public class HomeController : MediaBaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
