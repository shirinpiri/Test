using perfumedecant.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace perfumedecant.Models
{
    public class PerfumeModel
    {
        public List<Tbl_Perfume> Perfumes { get; set; }

        public int CurrentPageIndex { get; set; }

        public int PageCount { get; set; }

        public int brandID { get; set; }

        public string type { get; set; }
    }
}