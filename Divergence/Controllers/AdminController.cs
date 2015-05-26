using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Divergence.Controllers
{
    [Authorize(Roles="Administrator")]
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            //stanza listing table
            return View();
        }

        public ActionResult Stanza(long id, string verb)
        {
            //We'll edit stanzas here, inline with choices and everything. If there's no verb or "display" as the verb we'll just show the editor for the id. 
            //if id is -1 or missing we need a new stanza
            return View();
        }
    }
}
