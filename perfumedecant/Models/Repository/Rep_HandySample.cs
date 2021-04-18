using perfumedecant.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace perfumedecant.Models.Repository
{
    public class Rep_HandySample
    {
        PD_DB db = new PD_DB();
        public IEnumerable<Tbl_HandySample> Get_All_HandySamples()
        {
            var samples = db.Tbl_HandySample.OrderByDescending(a => a.HandySample_ID).ToList();
            return samples.AsEnumerable();
        }
    }
}