using perfumedecant.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace perfumedecant.Controllers
{
    public class HomeController : Controller
    {
        PD_DB db = new PD_DB();

        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(int PerfumeID)
        {
            return View();
        }

        public ActionResult SearchResult(String searchTerm)
        {
            ViewBag.searchTerm = searchTerm;
            string[] searchTerms = searchTerm.Split(' ');
            List<Tbl_Perfume> resultList = new List<Tbl_Perfume>();
            foreach (var term in searchTerms)
            {
                resultList = db.Tbl_Perfume.Where(a => a.Perfume_Name.Contains(term) || a.Perfume_Perfumer.Contains(term) ||
                                                      a.Perfume_Country.Contains(term) || a.Perfume_Description.Contains(term) ||
                                                      a.Perfume_Notes.Contains(term) || a.Tbl_Brand.Brand_Title.Contains(term)).ToList();
            }

            return PartialView(resultList);
        }
    }
}