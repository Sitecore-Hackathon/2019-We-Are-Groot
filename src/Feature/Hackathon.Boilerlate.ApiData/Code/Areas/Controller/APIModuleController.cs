using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hackathon.Boilerlate.Api.Areas.Controller
{
    public class APIModuleController : SitecoreController
    {
        // GET: Controller/APIModule
        public override ActionResult Index()
        {
            var datasource = RenderingContext.Current.Rendering.Item;
            return View("~/Views/APIModule.cshtml",datasource);
        }
    }
}