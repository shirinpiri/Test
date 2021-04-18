using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using perfumedecant.Models.Domains;

namespace perfumedecant.Models.Repository
{
    public class Rep_PerfumeImages
    {
        PD_DB db = new PD_DB();

        public IEnumerable<Tbl_PerfumeImages> Get_PerfumeImages(int perfumeID)
        {
            var images = db.Tbl_PerfumeImages.Where(a=>a.PerfumeImages_Perfume_ID == perfumeID).ToList();
            return images.AsEnumerable();
        }
    }
}