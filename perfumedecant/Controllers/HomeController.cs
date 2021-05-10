using perfumedecant.Models.Domains;
using perfumedecant.Models;
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

        public ActionResult SearchResult(String searchTerm, String type = "", int currentPageIndex = 1)
        {
            int maxRows = 8;
            ViewBag.searchTerm = searchTerm;
            string[] searchTerms = searchTerm.Split(' ');
            List<Tbl_Perfume> perfumes = new List<Tbl_Perfume>();

            foreach (var term in searchTerms)
            {
                perfumes = db.Tbl_Perfume.Where(a => a.Perfume_Name.Contains(term) || a.Perfume_Perfumer.Contains(term) ||
                                                      a.Perfume_Country.Contains(term) || a.Perfume_Description.Contains(term) ||
                                                      a.Perfume_Notes.Contains(term) || a.Tbl_Brand.Brand_Title.Contains(term)).ToList();
            }
            PerfumeModel perfumeModel = new PerfumeModel();

            perfumeModel.Perfumes = perfumes
                        .OrderBy(a => a.Perfume_ID)
                        .Skip((currentPageIndex - 1) * maxRows)
                        .Take(maxRows).ToList();


            double pageCount = (double)((decimal)perfumes.Count() / Convert.ToDecimal(maxRows));
            perfumeModel.PageCount = (int)Math.Ceiling(pageCount);

            perfumeModel.CurrentPageIndex = currentPageIndex;
            perfumeModel.type = type;

            return PartialView(perfumeModel);
        }
    }
}