using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Divergence.Controllers
{
    public class RootController : Controller
    {
        public ActionResult Index()
        {
            //splash page, lists recent stories
            return View();
        }
    }
}
