using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace perfumedecant.Models.Domains
{
    internal class MetaData_Cologne
    {
        public int Cologne_ID { get; set; }

        [Display(Name = "نام محصول")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا نام محصول را انتخاب نمایید.")]
        public int Cologne_Perfume_ID { get; set; }

        [Display(Name = "تعداد کل")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا تعداد کل محصول را وارد نمایید.")]
        public int Cologne_AllCount { get; set; }

        [Display(Name = "قیمت هر واحد(تومان)")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا قیمت هر واحد را وارد نمایید.")]
        public int Cologne_PricePerUnit { get; set; }

        [Display(Name = "حجم(میل)")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا حجم محصول را وارد نمایید.")]
        public int Cologne_Weight { get; set; }
    }

    [MetadataType(typeof(MetaData_Cologne))]
    public partial class Tbl_Cologne
    { }
}