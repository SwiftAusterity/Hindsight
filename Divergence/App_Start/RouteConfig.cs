using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Divergence
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //Pathway is its own controller but story and arc is all I want in the base urls
            routes.MapRoute(
                name: "Pathway Default with optional key",
                url: "Arc/{arcKey}/Pathway/{key}",
                defaults: new { controller = "Pathway", action = "Index" }
            );

            routes.MapRoute(
                name: "Pathway Default with optional key",
                url: "Arc/{arcKey}/Pathway",
                defaults: new { controller = "Pathway", action = "Graph" }
            );

            routes.MapRoute(
                name: "Arc Default with key",
                url: "Arc/{key}",
                defaults: new { controller = "Arc", action = "Index" }
            );

            //No key or substructure means you want to see the graph
            routes.MapRoute(
                name: "Arc Default",
                url: "Arc",
                defaults: new { controller = "Arc", action = "Graph" }
            );

            routes.MapRoute(
                name: "Story Default with key",
                url: "Story/{key}",
                defaults: new { controller = "Story", action = "Index", key = UrlParameter.Optional }
            );

            //No key or substructure means you want to see the graph
            routes.MapRoute(
                name: "Admin Generic",
                url: "Admin/{action}/{id}/{verb}",
                defaults: new { controller = "Admin", action = "Index", verb = UrlParameter.Optional, id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{key}",
                defaults: new { controller = "Root", action = "Index", key = UrlParameter.Optional }
            );
        }
    }
}