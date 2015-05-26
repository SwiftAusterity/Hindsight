using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Divergence.Controllers
{
    public class PathwayController : Controller
    {
        public ActionResult Index(Guid arcKey, Guid key)
        {
            //Default will need to reroute to either the graph or the identified node if you supply a key
            return View();
        }

        public ActionResult Index(Guid arcKey)
        {
            //Show the pathway graph
            return View();
        }
    }
}
