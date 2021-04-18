using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using perfumedecant.Models.Domains;

namespace perfumedecant.Models.Repository
{
    public class Rep_Cologne
    {
        PD_DB db = new PD_DB();
        public IEnumerable<Tbl_Cologne> Get_All_Colognes()
        {
            var colognes = db.Tbl_Cologne.OrderByDescending(a => a.Cologne_ID).ToList();
            return colognes.AsEnumerable();
        }
    }
}