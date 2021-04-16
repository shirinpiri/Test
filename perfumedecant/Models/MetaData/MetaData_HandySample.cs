using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace perfumedecant.Models.Domains
{
    internal class MetaData_HandySample
    {
        public int HandySample_ID { get; set; }

        [Display(Name = "نام محصول")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا نام محصول را انتخاب نمایید.")]
        public int HandySample_Perfume_ID { get; set; }

        [Display(Name = "حجم کل")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا حجم کل محصول را وارد نمایید.")]
        public int HandySample_AllWeight { get; set; }

        [Display(Name = "قیمت هر میل(تومان)")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا قیمت هر میل را وارد نمایید.")]
        public int HandySample_PricePerMil { get; set; }
    }

    [MetadataType(typeof(MetaData_HandySample))]
    public partial class Tbl_HandySample
    { }
}