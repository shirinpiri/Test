using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace perfumedecant.Models
{
    public class PriceModel
    {
        public int ColognePrice { get; set; }

        public int HandySamplePrice { get; set; }

        public int CompanySamplePrice { get; set; }

        public List<SelectListItem> CategoryList { get; set; }

        public List<SelectListItem> CologneWeightList { get; set; }

        public List<SelectListItem> HandySampleWeightList { get; set; }

        public List<SelectListItem> CompanySampleWeightList { get; set; }
    }
}