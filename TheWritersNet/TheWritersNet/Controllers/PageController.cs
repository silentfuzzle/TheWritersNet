using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TheWritersNet.Controllers
{
    public class PageController : Controller
    {
        // GET: Page
        public ActionResult Display()
        {
            return View();
        }

        public ActionResult Edit()
        {
            return View();
        }
    }
}