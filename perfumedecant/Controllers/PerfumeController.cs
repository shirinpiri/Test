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

        public JsonResult PerfumeFilterring(Array seasons, Array OlfactionGroups, Array TemperOfPerfumes, String gender)
        {
            List<int> perfumeIDs = new List<int>();
            List<String> perfumeImages = new List<String>();
            List<String> perfumeNames = new List<String>();
            List<Tbl_Perfume> perfume = new List<Tbl_Perfume>();

            foreach (var seasonItem in seasons)
            {
                if (seasonItem.ToString() == "")
                {
                    perfume = db.Tbl_Perfume.OrderByDescending(a => a.Perfume_ID).ToList();
                    break;
                }
                var seasonid = db.Tbl_Season.Where(a => a.Season_Title == seasonItem.ToString()).FirstOrDefault();
                List<Tbl_PerfumeSeason> perfume_seasons = new List<Tbl_PerfumeSeason>();
                perfume_seasons = db.Tbl_PerfumeSeason.Where(a => a.PerfumeSeason_Season_ID == seasonid.Season_ID).ToList();
                foreach (var perfume_season in perfume_seasons)
                {
                    var res = db.Tbl_Perfume.Where(a => a.Perfume_ID == perfume_season.PerfumeSeason_Perfume_ID).FirstOrDefault();
                    perfume.Add(res);
                }
            }

            if (perfume.Count > 0)
            {
                foreach (var perfumeItem in perfume.ToList())
                {
                    var OlfactionGroupflag = false;
                    var OlfactionGroupEmpty = false;
                    foreach (var OlfactionGroupItem in OlfactionGroups)
                    {
                        if (OlfactionGroupItem.ToString() == "")
                        {
                            OlfactionGroupEmpty = true;
                            break;
                        }
                        if (perfumeItem.Perfume_OlfactionGroups != null && perfumeItem.Perfume_OlfactionGroups.ToString().Contains(OlfactionGroupItem.ToString())) 
                            OlfactionGroupflag = true;
                    }
                    if (OlfactionGroupEmpty == true) break;
                    if (OlfactionGroupflag == false)
                        perfume.Remove(perfumeItem);
                }
            }

            if (perfume.Count > 0)
            {
                foreach (var perfumeItem in perfume.ToList())
                {
                    var TemperOfPerfumeflag = false;
                    var TemperOfPerfumeEmpty = false;
                    foreach (var TemperOfPerfumeItem in TemperOfPerfumes)
                    {
                        if (TemperOfPerfumeItem.ToString() == "")
                        {
                            TemperOfPerfumeEmpty = true;
                            break;
                        }
                        if (perfumeItem.Perfume_TemperOfPerfume != null && perfumeItem.Perfume_TemperOfPerfume.ToString().Contains(TemperOfPerfumeItem.ToString())) TemperOfPerfumeflag = true;
                    }
                    if (TemperOfPerfumeEmpty == true) break;
                    if (TemperOfPerfumeflag == false)
                        perfume.Remove(perfumeItem);
                }
            }

            if (gender != null)
            {
                if (perfume.Count > 0)
                {
                    foreach (var perfumeItem in perfume.ToList())
                    {
                        var genderflag = false;
                        if (perfumeItem.Perfume_Gender.ToString() == gender.ToString()) genderflag = true;
                        if (genderflag == false)
                            perfume.Remove(perfumeItem);
                    }
                }
            }

            if (perfume.Count > 0)
            {
                foreach (var perfumeItem in perfume)
                {
                    perfumeIDs.Add(perfumeItem.Perfume_ID);
                    perfumeImages.Add(perfumeItem.Perfume_ImageIndex);
                    perfumeNames.Add(perfumeItem.Perfume_Name);
                }
            }

            return Json(new
            {
                perfumeIDs = perfumeIDs,
                perfumeImages = perfumeImages,
                perfumeNames = perfumeNames
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult PerfumeDetails(int PerfumeID)
        {
            String Message = "";
            IEnumerable<Tbl_Season> pfs = new List<Tbl_Season>();
            List<SelectListItem> seasons = new List<SelectListItem>();
            PriceModel prices = new PriceModel();
            List<SelectListItem> images = new List<SelectListItem>();

            var perfume = db.Tbl_Perfume.Where(a => a.Perfume_ID == PerfumeID).SingleOrDefault();
            if (perfume != null)
            {
                prices = InitDropdownLists(PerfumeID);

                Rep_Perfume rp = new Rep_Perfume();
                var seasonList = rp.Get_PerfumeSeasons(PerfumeID);
                foreach (var item in seasonList)
                {
                    seasons.Add(new SelectListItem { Text = item.Season_Title, Value = item.Season_ImageIndex });
                }

                Rep_PerfumeImages rep_images = new Rep_PerfumeImages();
                var imgs = rep_images.Get_PerfumeImages(PerfumeID);
                foreach (var item in imgs)
                {
                    images.Add(new SelectListItem { Text = item.PerfumeImages_ImageIndex, Value = item.PerfumeImages_ID.ToString() });
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
                    seasons = new SelectList(seasons, "Value", "Text"),
                    CategoryList = prices.CategoryList,
                    ColognePrice = prices.ColognePrice,
                    CologneWeightList = prices.CologneWeightList,
                    CompanySamplePrice = prices.CompanySamplePrice,
                    CompanySampleWeightList = prices.CompanySampleWeightList,
                    HandySamplePrice = prices.HandySamplePrice,
                    HandySampleWeightList = prices.HandySampleWeightList,
                    Perfume_ImageIndex = perfume.Perfume_ImageIndex,
                    images = new SelectList(images, "Value", "Text")
                },
                 JsonRequestBehavior.AllowGet);
        }

        public ActionResult Perfumes(int brandID = 0, String type = "", int currentPageIndex = 1)
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

        public PriceModel InitDropdownLists(int perfumeID)
        {
            var categoryList = new List<SelectListItem>();
            Int32 cologne_price = 0;
            Int32 handySample_price = 0;
            var companySample_price = 0;
            var handySample_weightList = new List<SelectListItem>();
            var companySample_weightList = new List<SelectListItem>();
            var cologne_weightList = new List<SelectListItem>();

            var cologne = db.Tbl_Cologne.Where(a => a.Cologne_Perfume_ID == perfumeID && a.Cologne_AllCount > 0).ToList();
            if (cologne != null && cologne.Count() > 0)
            {
                cologne_weightList = db.Tbl_Cologne.Where(a => a.Cologne_Perfume_ID == perfumeID && a.Cologne_AllCount > 0).ToList().Select(rr =>
                 new SelectListItem { Value = rr.Cologne_Weight.ToString(), Text = rr.Cologne_Weight.ToString() }).ToList();

                cologne_price = Convert.ToInt32(cologne.FirstOrDefault().Cologne_PricePerUnit);
                categoryList.Add(new SelectListItem { Text = "ادکلن", Value = "ادکلن" });
                //cologne_weightList.Add(new SelectListItem { Text = cologne.Cologne_Weight.ToString(), Value = cologne.Cologne_Weight.ToString(), Selected = true });
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

            var companySample = db.Tbl_CompanySample.Where(a => a.CompanySample_Perfume_ID == perfumeID && a.CompanySample_AllCount > 0).ToList();
            if (companySample != null && companySample.Count() > 0)
            {
                companySample_price = companySample.FirstOrDefault().CompanySample_Price.Value;
                categoryList.Add(new SelectListItem { Text = "سمپل شرکتی", Value = "سمپل شرکتی" });
                companySample_weightList = db.Tbl_CompanySample.Where(a => a.CompanySample_Perfume_ID == perfumeID && a.CompanySample_AllCount > 0).ToList().Select(rr =>
                  new SelectListItem { Value = rr.CompanySample_Weight.ToString(), Text = rr.CompanySample_Weight.ToString() }).ToList();

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

            PriceModel prices = new PriceModel();
            prices.CategoryList = categoryList;
            prices.ColognePrice = cologne_price;
            prices.HandySamplePrice = handySample_price;
            prices.CompanySamplePrice = companySample_price;
            prices.CologneWeightList = cologne_weightList;
            prices.HandySampleWeightList = handySample_weightList;
            prices.CompanySampleWeightList = companySample_weightList;

            return (prices);
            /*    ViewBag.cologne_price = cologne_price;
                ViewBag.handySample_price = handySample_price;
                ViewBag.companySample_price = companySample_price;
                ViewBag.cologne_weightList = cologne_weightList;
                ViewBag.handySample_weightList = handySample_weightList;
                ViewBag.companySample_weightList = companySample_weightList;*/
        }

        public ActionResult PerfumesBrand(int brandID = 0, string type = "", int currentPageIndex = 1)
        {
            int maxRows = 8;
            List<Tbl_Perfume> perfumes = new List<Tbl_Perfume>();
            perfumes = db.Tbl_Perfume.Where(a => a.Perfume_Brand_ID == brandID).ToList();
            PerfumeModel perfumeModel = new PerfumeModel();

            perfumeModel.Perfumes = perfumes
                        .OrderBy(a => a.Perfume_ID)
                        .Skip((currentPageIndex - 1) * maxRows)
                        .Take(maxRows).ToList();

            double pageCount = (double)((decimal)perfumes.Count() / Convert.ToDecimal(maxRows));
            perfumeModel.PageCount = (int)Math.Ceiling(pageCount);

            perfumeModel.CurrentPageIndex = currentPageIndex;
            perfumeModel.brandID = brandID;
            perfumeModel.type = type;

            return View(perfumeModel);
        }

        public JsonResult GetPrice(int Perfume_ID, float weight, string category)
        {
            var price = 0;
            if (category == "ادکلن")
            {
                var cologne = db.Tbl_Cologne.Where(a => a.Cologne_Perfume_ID == Perfume_ID && a.Cologne_Weight == weight).FirstOrDefault();
                if (cologne != null)
                {
                    price = cologne.Cologne_PricePerUnit;
                }
            }
            else if (category == "سمپل شرکتی")
            {
                var company_sample = db.Tbl_CompanySample.Where(a => a.CompanySample_Perfume_ID == Perfume_ID && a.CompanySample_Weight == weight).FirstOrDefault();
                if (company_sample != null)
                {
                    price = (int)company_sample.CompanySample_Price;
                }
            }
            else
            {
                var handy_sample = db.Tbl_HandySample.Where(a => a.HandySample_Perfume_ID == Perfume_ID).FirstOrDefault();
                if (handy_sample != null)
                {
                    price = (int)(handy_sample.HandySample_PricePerMil * weight);
                }
            }
            return Json(price, JsonRequestBehavior.AllowGet);
        }


    }
}