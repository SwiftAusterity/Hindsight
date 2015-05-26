using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Divergence.Controllers
{
    public class StoryController : Controller
    {
        public ActionResult Index(Guid key)
        {
            //No key brings you to the current story
            return View();
        }
    }
}
