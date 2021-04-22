using Newtonsoft.Json;
using perfumedecant.Models;
using perfumedecant.Models.Domains;
using perfumedecant.Models.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace perfumedecant.Controllers
{
    public class PerfumeController : Controller
    {
        PD_DB db = new PD_DB();
        Log log = new Log();
        LogStatus logStatus = new LogStatus();

        public JsonResult PerfumeDetails(int PerfumeID)
        {
            String Message = "";
            IEnumerable<Tbl_Season> pfs = new List<Tbl_Season>();
            List<SelectListItem> seasons = new List<SelectListItem>();

            var perfume = db.Tbl_Perfume.Where(a => a.Perfume_ID == PerfumeID).SingleOrDefault();
            if (perfume != null)
            {
                InitDropdownLists(PerfumeID);
                Rep_Perfume rp = new Rep_Perfume();
                var seasonList = rp.Get_PerfumeSeasons(PerfumeID);
                foreach (var item in seasonList)
                {
                    seasons.Add(new SelectListItem { Text = item.Season_Title, Value = item.Season_ImageIndex });
                }

            }
            else
            {
                Message = "perfume with ID" + PerfumeID + "not found.";
                log.addLog(Message, "AddCart", "Cart", logStatus.EventLog);
                ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
            }

            return Json(
                new
                {
                Perfume_Name = perfume.Perfume_Name,
                Perfume_Gender = perfume.Perfume_Gender,
                Brand_Title = perfume.Tbl_Brand.Brand_Title,
                Perfume_Country = perfume.Perfume_Country,
                Perfume_Description = perfume.Perfume_Description,
                Perfume_Notes = perfume.Perfume_Notes,
                Perfume_OlfactionGroups = perfume.Perfume_OlfactionGroups,
                Perfume_Perfumer = perfume.Perfume_Perfumer,
                Perfume_TemperOfPerfume = perfume.Perfume_TemperOfPerfume,
                PerfumeType_Title = perfume.Tbl_PerfumeType.PerfumeType_Title,
                seasons = new SelectList(seasons, "Value", "Text")
                },
                 JsonRequestBehavior.AllowGet);
        }

     /*   public ActionResult PerfumeDetails(int PerfumeID)
        {
            String Message = "";
            var perfume = db.Tbl_Perfume.Where(a => a.Perfume_ID == PerfumeID).SingleOrDefault();
            if (perfume != null)
            {
                InitDropdownLists(PerfumeID);
                return View(perfume);
            }
            else
            {
                Message = "perfume with ID" + PerfumeID + "not found.";
                log.addLog(Message, "AddCart", "Cart", logStatus.EventLog);
                ViewBag.Error = "محصول پیدا نشد، لطفا دوباره تلاش کنید.";
                return RedirectToAction("Index", "Home");
            }
        }*/

        public ActionResult Perfumes(int brandID = 0 , String type = "" , int currentPageIndex = 1)
        {          
            var perfumes = this.GetPerfumes(brandID, type, currentPageIndex);
            ViewBag.title = TempData["title"];
            return View(perfumes);
        }

        private PerfumeModel GetPerfumes(int brandID = 0, String type = "", int currentPage = 0)
        {
            int maxRows = 8;
            //var brands = db.Tbl_Brand.OrderByDescending(a => a.Brand_ID).ToList();
            List<Tbl_Perfume> perfumes = new List<Tbl_Perfume>();

            /////////////////////////////
            if (type == "specialOffer")
            {
                //Rep_Perfume rep_perfume = new Rep_Perfume();
                // var perfumes = rep_perfume.Get_IsSpecialOffer_Perfumes();
                perfumes = db.Tbl_Perfume.Where(a => a.Perfume_SpecialOffer == true && a.Perfume_Brand_ID == brandID).OrderByDescending(a => a.Perfume_ID).ToList();

                TempData["title"] = "پیشنهاد ویژه";
            }
            else if (type == "women")
            {
                //Rep_Perfume rep_perfume = new Rep_Perfume();
                //var perfumes = rep_perfume.Get_NewPerfume("زنانه");
                perfumes = db.Tbl_Perfume.Where(a => a.Perfume_Gender == "زنانه" && a.Perfume_Brand_ID == brandID).OrderByDescending(a => a.Perfume_ID).ToList();

                TempData["title"] = "زنانه";
            }
            else if (type == "man")
            {
                //Rep_Perfume rep_perfume = new Rep_Perfume();
                //var perfumes = rep_perfume.Get_NewPerfume("مردانه");
                perfumes = db.Tbl_Perfume.Where(a => a.Perfume_Gender == "مردانه" && a.Perfume_Brand_ID == brandID).OrderByDescending(a => a.Perfume_ID).ToList();

                TempData["title"] = "مردانه";
            }
            else if (type == "unisex")
            {
                // Rep_Perfume rep_perfume = new Rep_Perfume();
                // var perfumes = rep_perfume.Get_NewPerfume("یونیسکس");

                perfumes = db.Tbl_Perfume.Where(a => a.Perfume_Gender == "یونیسکس" && a.Perfume_Brand_ID == brandID).OrderByDescending(a => a.Perfume_ID).ToList();


                TempData["title"] = "یونیسکس";
            }
            else if (type == "seasonOffers")
            {
                Rep_Season rep_season = new Rep_Season();
                perfumes = rep_season.Get_SeasonPerfumes(brandID).ToList();
                TempData["title"] = "عطر مناسب فصل";
            }
            else if (type == "companySample")
            {
                Rep_CompanySample rep_companySample = new Rep_CompanySample();
                //List<Tbl_Perfume> perfumes1 = new List<Tbl_Perfume>();
                var companySamples = rep_companySample.Get_All_CompanySamples();
                foreach (var sample in companySamples)
                {
                    perfumes.Add(sample.Tbl_Perfume);
                }
                TempData["title"] = "سمپل شرکتی";
            }
            else if (type == "handySample")
            {
                Rep_HandySample rep_handySample = new Rep_HandySample();
                //List<Tbl_Perfume> perfumes = new List<Tbl_Perfume>();
                var handySamples = rep_handySample.Get_All_HandySamples();
                foreach (var sample in handySamples)
                {
                    perfumes.Add(sample.Tbl_Perfume);
                }
                TempData["title"] = "دکانت و سمپل دست ریز";
            }
            else
            {
                // Rep_Perfume rep_perfume = new Rep_Perfume();
                // var perfumes = rep_perfume.Get_All_Perfumes();
                perfumes = db.Tbl_Perfume.Where(a => a.Perfume_Brand_ID == brandID).OrderByDescending(a => a.Perfume_ID).ToList();

                TempData["title"] = "همه محصولات";
            }

////////////////////////
            PerfumeModel perfumeModel = new PerfumeModel();

            perfumeModel.Perfumes = perfumes
                        .OrderBy(a => a.Perfume_ID)
                        .Skip((currentPage - 1) * maxRows)
                        .Take(maxRows).ToList();

            double pageCount = (double)((decimal)perfumes.Count() / Convert.ToDecimal(maxRows));
            perfumeModel.PageCount = (int)Math.Ceiling(pageCount);

            perfumeModel.CurrentPageIndex = currentPage;
            perfumeModel.brandID = brandID;
            perfumeModel.type = type;

            return perfumeModel;
        }


        public void InitDropdownLists(int perfumeID)
        {
            var categoryList = new List<SelectListItem>();
            Int32 cologne_price = 0;
            Int32 handySample_price = 0;
            var companySample_price = 0;
            var handySample_weightList = new List<SelectListItem>();
            var companySample_weightList = new List<SelectListItem>();
            var cologne_weightList = new List<SelectListItem>();

            var cologne = db.Tbl_Cologne.Where(a => a.Cologne_Perfume_ID == perfumeID).FirstOrDefault();
            if (cologne != null)
            {
                cologne_price = Convert.ToInt32(cologne.Cologne_PricePerUnit);
                categoryList.Add(new SelectListItem { Text = "ادکلن", Value = "ادکلن" });
                cologne_weightList.Add(new SelectListItem { Text = cologne.Cologne_Weight.ToString(), Value = cologne.Cologne_Weight.ToString(), Selected = true });
            }

            var handySample = db.Tbl_HandySample.Where(a => a.HandySample_Perfume_ID == perfumeID).FirstOrDefault();
            if (handySample != null)
            {
                handySample_price = Convert.ToInt32(handySample.HandySample_PricePerMil);
                categoryList.Add(new SelectListItem { Text = "سمپل دست ریز", Value = "سمپل دست ریز" });
                handySample_weightList.Add(new SelectListItem { Text = "5", Value = "5", Selected = true });
                handySample_weightList.Add(new SelectListItem { Text = "10", Value = "10" });
                handySample_weightList.Add(new SelectListItem { Text = "15", Value = "15" });
                handySample_weightList.Add(new SelectListItem { Text = "20", Value = "20" });
                handySample_weightList.Add(new SelectListItem { Text = "25", Value = "25" });
                handySample_weightList.Add(new SelectListItem { Text = "30", Value = "30" });

            }

            var companySample = db.Tbl_CompanySample.Where(a => a.CompanySample_Perfume_ID == perfumeID).FirstOrDefault();
            if (companySample != null)
            {
                companySample_price = companySample.CompanySample_Price.Value;
                categoryList.Add(new SelectListItem { Text = "سمپل شرکتی", Value = "سمپل شرکتی" });
                companySample_weightList.Add(new SelectListItem { Text = companySample.CompanySample_Weight.ToString(), Value = companySample.CompanySample_Weight.ToString(), Selected = true });
            }

            ViewBag.categoryList = categoryList;

            if (categoryList.Count() > 0)
            {
                ViewBag.categoryCount = categoryList.Count();
            }
            else
            {
                ViewBag.categoryCount = 0;
            }

            ViewBag.cologne_price = cologne_price;
            ViewBag.handySample_price = handySample_price;
            ViewBag.companySample_price = companySample_price;
            ViewBag.cologne_weightList = cologne_weightList;
            ViewBag.handySample_weightList = handySample_weightList;
            ViewBag.companySample_weightList = companySample_weightList;
        }

        public JsonResult GetWeights(string category, int perfumeID)
        {
            List<SelectListItem> weights = new List<SelectListItem>();
            switch (category)
            {
                case "ادکلن":
                    weights = db.Tbl_Cologne.Where(a => a.Cologne_Perfume_ID == perfumeID).ToList().Select(rr =>
                                      new SelectListItem { Value = rr.Cologne_Weight.ToString(), Text = rr.Cologne_Weight.ToString() }).ToList();
                    break;
                case "سمپل شرکتی":
                    weights = db.Tbl_CompanySample.Where(a => a.CompanySample_Perfume_ID == perfumeID).ToList().Select(rr =>
                  new SelectListItem { Value = rr.CompanySample_Weight.ToString(), Text = rr.CompanySample_Weight.ToString() }).ToList();
                    break;

                case "سمپل دست ریز":
                    weights = new List<SelectListItem>
                    {
                        new SelectListItem{ Text="3", Value = "3" , Selected = true},
                        new SelectListItem{ Text="5", Value = "5" },
                        new SelectListItem{ Text="10", Value = "10"},
                        new SelectListItem{ Text="15", Value = "15" },
                        new SelectListItem{ Text="20", Value = "20" },
                        new SelectListItem{ Text="25", Value = "25" },
                    };
                    break;
            }

            return Json(new SelectList(weights, "Value", "Text"));
        }
    }
}