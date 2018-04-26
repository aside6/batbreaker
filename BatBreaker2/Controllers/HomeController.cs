using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BatBreaker2.Models;
using CommonLibrary;

namespace BatBreaker2.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            BatBreakerContext db = new BatBreakerContext();
            int userId = User.plainUserId();
            Player player = db.Players.Where(p => p.userId.Equals(userId)).FirstOrDefault();

            if (player != null)
            {
                return View(player);
            }
            else
            {
                return RedirectToAction("CreatePlayer", "MyPlayer");
            }
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
