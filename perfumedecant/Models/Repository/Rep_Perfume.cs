using perfumedecant.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace perfumedecant.Models.Repository
{
    public class Rep_Perfume
    {
        PD_DB db = new PD_DB();

        public IEnumerable<Tbl_Perfume> Get_NewPerfume(string gender)
        {
            var newPerfume = db.Tbl_Perfume.Where(a => a.Perfume_Gender == gender ).OrderByDescending(a=>a.Perfume_ID).ToList().Take(8);
            return newPerfume.AsEnumerable();
        }



        public IEnumerable<Tbl_Perfume> Get_NewPerfume()
        {
            var newPerfume = db.Tbl_Perfume.OrderByDescending(a => a.Perfume_ID).ToList().Take(8);
            return newPerfume.AsEnumerable();
        }

        public IEnumerable<Tbl_Perfume> Get_IsSpecialOffer_Perfumes()
        {
            var perfumes = db.Tbl_Perfume.Where(a=>a.Perfume_SpecialOffer == true).ToList().Take(4);
            return perfumes.AsEnumerable();
        }

        public IEnumerable<Tbl_Perfume> Get_All_Perfumes()
         {
            var perfumes = db.Tbl_Perfume.OrderByDescending(a=>a.Perfume_ID).ToList();
            return perfumes.AsEnumerable();
        }


        public IEnumerable<Tbl_Season> Get_PerfumeSeasons(int perfumeID)
        {
            List<Tbl_Season> seasons = new List<Tbl_Season>();
            var perfume_seasons = db.Tbl_PerfumeSeason.Where(a => a.PerfumeSeason_Perfume_ID == perfumeID).ToList();
            foreach (var item in perfume_seasons)
            {
                var season = db.Tbl_Season.Where(a => a.Season_ID == item.PerfumeSeason_Season_ID).SingleOrDefault();
                seasons.Add(season);
            }
            return seasons.AsEnumerable();
        }

        public int Get_Perfume_Count()
        {
            int count = db.Tbl_Perfume.Distinct().Count();
            return count;
        }

       
    }
}