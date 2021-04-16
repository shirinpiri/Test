using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace perfumedecant.Models.Domains
{
    internal class MetaData_Article
    {
        [Display(Name = "عنوان مقاله")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا عنوان مقاله را وارد نمایید.")]
        public string Article_Title { get; set; }

        [Display(Name = "فایل مقاله")]
         [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا فایل مقاله را انتخاب نمایید.")]
        public string Article_Path { get; set; }

    }
    [MetadataType(typeof(MetaData_Article))]
    public partial class Tbl_Article
    {
    }
}
