using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace perfumedecant
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "PerfumeDetails",
                url: "Perfume_Details/{PerfumeID}",
                defaults: new { controller = "Perfume", action = "Perfume_Details", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "Perfumes",
               url: "Perfumes/{brandID}/{type}/{currentPageIndex}",
               defaults: new { controller = "Perfume", action = "Perfumes", id = UrlParameter.Optional }
           );
        }
    }
}

