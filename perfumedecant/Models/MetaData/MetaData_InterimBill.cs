using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace perfumedecant.Models.Domains
{
    internal class MetaData_InterimBill
    {
        public int InterimBill_ID { get; set; }

        [Display(Name = "تاریخ سفارش")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا تاریخ سفارش را وارد نمایید.")]
        public System.DateTime InterimBill_Date { get; set; }


        public int InterimBill_Perfume_ID { get; set; }

        [Display(Name = "تعداد محصول")]
        public Nullable<int> InterimBill_Count { get; set; }

        [Display(Name = "وزن محصول")]
        public Nullable<int> InterimBill_Weight { get; set; }

        [Display(Name = "قیمت")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا قیمت را وارد نمایید.")]
        public int InterimBill_Price { get; set; }

        [Display(Name = "شماره فاکتور")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا شماره فاکتور را وارد نمایید.")]
        public string InterimBill_InvoiceNum { get; set; }

        [Display(Name = "تاریخ انقضای پیش فاکتور")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفا تاریخ انقضای پیش فاکتور را وارد نمایید.")]
        public Nullable<System.DateTime> InterimBill_ExpDate { get; set; }

        [Display(Name = "نام و نام خانوادگی سفارش دهنده")]
        public int InterimBill_User_ID { get; set; }

        [MetadataType(typeof(MetaData_InterimBill))]
        public partial class Tbl_InterimBill
        { }

    }
}