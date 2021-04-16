using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace perfumedecant.Models.Domains
{
    internal class MetaData_Bill
    {
        public int Bill_ID { get; set; }

        [Display(Name = "تاریخ صدور فاکتور")]
        public System.DateTime Bill_Date { get; set; }

        [Display(Name = "مبلغ پرداخت شده(تومان)")]
        public int Bill_PayPrice { get; set; }
        
        [Display(Name = "نحوه ارسال سفارش")]
        public int Bill_PostType_ID { get; set; }

        [Display(Name = "شماره فاکتور")]
        public string Bill_InvoiceNumber { get; set; }

        [Display(Name = "شماره پیگیری")]
        public long Bill_RefID { get; set; }

        [Display(Name = "شماره تراکنش")]
        public string Bill_TransNo { get; set; }

        [Display(Name = "وضعیت تراکنش")]
        public Nullable<bool> Bill_Status { get; set; }

        [Display(Name = "کد پیگیری")]
        public string Bill_TrackingCode { get; set; }

       [Display(Name = "شماره مرجع")]  
        public string Bill_RefNo { get; set; }

        [Display(Name = "مبلغ تخفیف(تومان)")]
        public Nullable<int> Bill_Off { get; set; }

        [Display(Name = "هزینه های اضافی(تومان)")]
        public Nullable<int> Bill_OtherPrice { get; set; }

        [Display(Name = "نام کاربری")]
        public Nullable<int> Bill_User_ID { get; set; }


        [MetadataType(typeof(MetaData_Bill))]
        public partial class Tbl_Bill
        { }
    }
}