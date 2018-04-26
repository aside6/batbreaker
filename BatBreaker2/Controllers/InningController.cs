using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BatBreaker2.Models;
using BatBreaker2.Common;
using CommonLibrary;

namespace BatBreaker.Controllers
{
    public class InningController : Controller
    {

        public JsonResult New(Guid gameId)
        {
            return Json(DoNewInning(gameId), JsonRequestBehavior.AllowGet);
        }

        public InningOutput DoNewInning(Guid gameId)
        {
            BatBreakerContext db = new BatBreakerContext();
            Game game = db.Games.Where(g => g.guId.Equals(gameId)).First();

            Innings last = game.Innings.Last();

            Innings inning = db.Innings.Create();
            inning.gameId = game.Id;
            if (last.TopOfInning)
            {
                inning.Number = game.Innings.Last().Number;
                inning.TopOfInning = false;
            }
            else
            {
                inning.Number = game.Innings.Last().Number + 1;
                inning.TopOfInning = true;
            }
            inning.StartDate = DateTime.Now;

            db.Innings.Add(inning);
            db.SaveChanges();

            InningOutput output = new InningOutput();
            output.Id = inning.Id;
            output.inningString = (!last.TopOfInning ? "Top " : "Bottom ") + " of Inning #" + inning.Number;

            return output;
        }

        public class InningOutput {
            public int Id { get; set; }
            public string inningString { get; set; }
        }

    }
}
