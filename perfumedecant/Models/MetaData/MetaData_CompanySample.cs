using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
namespace perfumedecant.Models.Domains
{
    internal class MetaData_CompanySample
    {
        public int CompanySample_ID { get; set; }

        [Display(Name = "نام محصول")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا نام محصول را انتخاب نمایید.")]
        public Nullable<int> CompanySample_Perfume_ID { get; set; }

        [Display(Name = "تعداد کل")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا تعداد کل محصول را وارد نمایید.")]
        public Nullable<int> CompanySample_AllCount { get; set; }

        [Display(Name = "قیمت (تومان)")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا قیمت هر واحد را وارد نمایید.")]
        public Nullable<int> CompanySample_Price { get; set; }

        [Display(Name = "حجم(میل)")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا حجم سمپل را وارد نمایید.")]
        public Nullable<int> CompanySample_Weight { get; set; }
    }

    [MetadataType(typeof(MetaData_CompanySample))]
    public partial class Tbl_CompanySample
    { }
}