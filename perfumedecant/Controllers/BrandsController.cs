using perfumedecant.Models;
using perfumedecant.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PerfumeDecant.Controllers
{
    public class BrandsController : Controller
    {
        PD_DB db = new PD_DB();
        Log log = new Log();
        LogStatus logStatus = new LogStatus();
        // GET: Brand
        public ActionResult Index(string type = "" , int currentPageIndex = 1)
        {
            //var brands = db.Tbl_Brand.OrderByDescending(a=>a.Brand_ID).ToList();
            ViewBag.type = type;
            return View(this.GetBrands(currentPageIndex));
        }


        private BrandModel GetBrands(int currentPage)
        {
            int maxRows = 8;
            var brands = db.Tbl_Brand.OrderByDescending(a => a.Brand_ID).ToList();

            BrandModel brandModel = new BrandModel();

            brandModel.Brands = brands
                        .OrderBy(a => a.Brand_ID)
                        .Skip((currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();

            double pageCount = (double)((decimal)brands.Count() / Convert.ToDecimal(maxRows));
            brandModel.PageCount = (int)Math.Ceiling(pageCount);

            brandModel.CurrentPageIndex = currentPage;

            return brandModel;
        }

        public JsonResult GetBrandsByLetter(char captal_letter, char small_letter)
        {
            List<Tbl_Brand> brands = new List<Tbl_Brand>();
            brands = db.Tbl_Brand.OrderByDescending(a => a.Brand_ID).ToList();
            List<SelectListItem> filteredBrands = new List<SelectListItem>();

            foreach (var brand in brands.ToList())
            {
                if (brand.Brand_Title[0] == captal_letter || brand.Brand_Title[0] == small_letter)
                    filteredBrands.Add(new SelectListItem { Text = brand.Brand_Title, Value = brand.Brand_ID.ToString()});
            }
            return Json(new
            {
                filteredBrands = new SelectList(filteredBrands, "Value", "Text"),
            }, JsonRequestBehavior.AllowGet);
        }

    }
}