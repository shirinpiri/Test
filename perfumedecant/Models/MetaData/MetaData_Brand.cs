using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace perfumedecant.Models.Domains
{
    internal class MetaData_Brand
    {
        [Display(Name = "عنوان برند")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا عنوان برند را وارد نمایید.")]
        public string Brand_Title { get; set; }

        [Display(Name = "عکس برند")]
       // [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا عکس برند را انتخاب نمایید.")]
        public string Brand_ImageIndex { get; set; }
    
    }
    [MetadataType(typeof(MetaData_Brand))]
    public partial class Tbl_Brand
    { 
    }
}