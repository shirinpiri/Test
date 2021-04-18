using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using perfumedecant.Models.Domains;

namespace perfumedecant.Models.Repository
{
    public class Rep_Brand
    {
        PD_DB db = new PD_DB();
        public IEnumerable<Tbl_Brand> GetAllBrands()
        {
            var brands = db.Tbl_Brand.OrderByDescending(a=>a.Brand_ID).ToList();
            return brands.AsEnumerable();
        }
    }
}