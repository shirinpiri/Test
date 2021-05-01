using perfumedecant.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace perfumedecant.Models
{
    public class BrandModel
    {
        public List<Tbl_Brand> Brands { get; set; }

        public int CurrentPageIndex { get; set; }

        public int PageCount { get; set; }
    }
}