using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace perfumedecant.Models.Domains
{

    internal class MetaData_PerfumeImages
    {
        public int PerfumeImages_ID { get; set; }
        public int PerfumeImages_Perfume_ID { get; set; }
        public string PerfumeImages_ImageIndex { get; set; }

        [MetadataType(typeof(MetaData_PerfumeImages))]
        public partial class Tbl_PerfumeImages
        { }
    }
}