using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SaVeIT_Final.Infrastructure;
namespace SaVeIT_Final.Controllers
{
    public class StatisticsController : Controller
    {
        // GET: Statistics
        public ActionResult Index()
        {
            Rscript.Run("network");
            Rscript.Run("WordCloud");
            Rscript.Run("WordFrequencyAOI");
            Rscript.Run("PL_Trend");

            return View();
        }
    }
}