using perfumedecant.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace perfumedecant.Models.Repository
{
    public class Rep_CompanySample
    {
        PD_DB db = new PD_DB();
        public IEnumerable<Tbl_CompanySample> Get_All_CompanySamples()
        {
            var samples = db.Tbl_CompanySample.OrderByDescending(a => a.CompanySample_ID).ToList();
            return samples.AsEnumerable();
        }
    }
}