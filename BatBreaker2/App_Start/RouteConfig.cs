using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace BatBreaker2
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
              "BoxScore", // Route name
              "Game/{gameId}/BoxScore", // URL with parameters
              new { gameId = "", controller = "Game", action = "BoxScore" } // Parameter defaults
          );

            routes.MapRoute(
               "NewInning", // Route name
               "Game/{gameId}/NewInning", // URL with parameters
               new { gameId = "", controller = "Inning", action = "New", abId = "" } // Parameter defaults
           );

            routes.MapRoute(
               "NewAtBat", // Route name
               "Game/{gameId}/NewAtBat", // URL with parameters
               new { gameId = "", controller = "AtBat", action = "New", abId = "" } // Parameter defaults
           );

            routes.MapRoute(
               "ThrowPitch", // Route name
               "Game/{gameId}/ThrowPitch/{abId}/{style}/{player}", // URL with parameters
               new { gameId = "", controller = "Pitch", action = "Throw", abId = "", style = "", player = "Pitcher" } // Parameter defaults
           );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
