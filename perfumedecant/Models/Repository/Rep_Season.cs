using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using perfumedecant.Models.Domains;

namespace perfumedecant.Models.Repository
{
    public class Rep_Season
    {
        PD_DB db = new PD_DB();

       public string Get_Season()
        {
            DateTime date = DateTime.Now;
            if (date.Month == 4 || date.Month == 5 || date.Month == 6)
                return "مناسب فصل بهار";
            if (date.Month == 7 || date.Month == 8 || date.Month == 9)
                return "مناسب فصل تابستان";
            if (date.Month == 10 || date.Month == 11 || date.Month == 12)
                return "مناسب فصل پاییز";
            if (date.Month == 1 || date.Month == 2 || date.Month == 3)
                return "مناسب فصل زمستان";
            else
                return "";
        }

       public IEnumerable<Tbl_Perfume> Get_SeasonPerfumes()
       {
           string season = Get_Season();
           var seasonPerfume = db.Tbl_PerfumeSeason.Where(a=>a.Tbl_Season.Season_Title == season).ToList().Take(8);
           List<Tbl_Perfume> perfumes = new List<Tbl_Perfume>();
           foreach (var item in seasonPerfume)
           {
               var perfume = db.Tbl_Perfume.Where(a => a.Perfume_ID == item.PerfumeSeason_Perfume_ID).SingleOrDefault();
               perfumes.Add(perfume);
           }
           return perfumes.AsEnumerable();
       }
    

    public IEnumerable<Tbl_Perfume> Get_SeasonPerfumes(int brandID)
    {
        string season = Get_Season();
        var seasonPerfume = db.Tbl_PerfumeSeason.Where(a => a.Tbl_Season.Season_Title == season && a.Tbl_Perfume.Perfume_Brand_ID == brandID).ToList().Take(8);
        List<Tbl_Perfume> perfumes = new List<Tbl_Perfume>();
        foreach (var item in seasonPerfume)
        {
            var perfume = db.Tbl_Perfume.Where(a => a.Perfume_ID == item.PerfumeSeason_Perfume_ID).SingleOrDefault();
            perfumes.Add(perfume);
        }
        return perfumes.AsEnumerable();
    }
}
}