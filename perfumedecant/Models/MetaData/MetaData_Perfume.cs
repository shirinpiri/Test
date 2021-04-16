using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace perfumedecant.Models.Domains
{
    internal class MetaData_Perfume
    {
        public int Perfume_ID { get; set; }

        [Display(Name = "نام محصول")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا نام محصول را وارد نمایید.")]
        public string Perfume_Name { get; set; }

        [Display(Name = "برند")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا نام برند را انتخاب نمایید.")]
        public int Perfume_Brand_ID { get; set; }

        //[Display(Name = "دسته بندی محصول")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "لطفا دسته بندی محصول را انتخاب نمایید.")]
        //public int Perfume_Category_ID { get; set; }

        //[Display(Name = "حجم کل محصول")]
        //public Nullable<int> Perfume_AllWeight { get; set; }

        //[Display(Name = "تعداد کل محصول")]
        //public Nullable<int> Perfume_AllCount { get; set; }

        [Display(Name = "تصویر محصول")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا عکس محصول را وارد نمایید.")]
        public string Perfume_ImageIndex { get; set; }

        [Display(Name = "عطار")]
        public string Perfume_Perfumer { get; set; }

        [Display(Name = "طبع عطر")]
        public string Perfume_TemperOfPerfume { get; set; }

        [Display(Name = "جنسیت")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا جنسیت را انتخاب نمایید.")]
        public string Perfume_Gender { get; set; }

        [Display(Name = "گروه بویایی")]
        public string Perfume_OlfactionGroups { get; set; }

        [Display(Name = "کشور سازنده")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا کشور سازنده را وارد نمایید.")]
        public string Perfume_Country { get; set; }

        [Display(Name = "نوع غلظت")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا نوع غلظت را انتخاب نمایید.")]
        public int Perfume_Type_ID { get; set; }

        [Display(Name = "نت ها")]
        public string Perfume_Notes { get; set; }

        //[Display(Name = "قیمت هر واحد")]
        //public Nullable<int> Perfume_PricePerUnit { get; set; }

        //[Display(Name = "قیمت هر میل")]
        //public Nullable<int> Perfume_PricePerMil { get; set; }

        [Display(Name = "پیشنهاد ویژه")]
        public Nullable<bool> Perfume_SpecialOffer { get; set; }

        [Display(Name = "توضیحات")]
        public string Perfume_Description { get; set; }

    }

    [MetadataType(typeof(MetaData_Perfume))]
    public partial class Tbl_Perfume
    { }
}