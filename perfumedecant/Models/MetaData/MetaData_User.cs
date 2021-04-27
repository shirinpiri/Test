using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace perfumedecant.Models.Domains
{
    internal class MetaData_User
    {
        public int User_ID { get; set; }

        [StringLength(100, ErrorMessage = "مقدار وارد شده بیش از 100 کاراکتر است.")]
        [Display(Name = "نام و نام خانوادگی")]
        public string User_NameFamily { get; set; }

        [Display(Name = "نام کاربری")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفاً نام کاربری را وارد نمائید.")]
        public string User_Username { get; set; }

        [RegularExpression(@"^\w+[\w-\.]*\@\w+((-\w+)|(\w*))\.[a-z]{2,3}$", ErrorMessage = " لطفاًایمیل را به درستی وارد نمایید")]
        [StringLength(200, ErrorMessage = "مقدار وارد شده برای ایمیل بیش از 200 کاراکتر است.")]
        [Display(Name = "ایمیل")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفاً ایمیل را وارد نمائید.")]
        public string User_Email { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفاً کلمه عبور را وارد نمائید.")]
        public string User_Password { get; set; }

        [Display(Name = "آدرس")]
        public string User_Address { get; set; }

        [Display(Name = "شهر")]
        public Nullable<int> User_City_ID { get; set; }

        [StringLength(10, ErrorMessage = "مقدار وارد شده بیش از 10 کاراکتر است.")]
        [Display(Name = "کد پستی")]
        public string User_PostalCode { get; set; }

        [StringLength(11, ErrorMessage = "مقدار وارد شده بیش از 11 کاراکتر است.")]
        [Display(Name = "شماره تلفن ثابت")]
        public string User_Tel { get; set; }

        [Display(Name = "جنسیت")]
        public string User_Gender { get; set; }

        [Display(Name = "تاریخ تولد")]
        public string User_Birthday { get; set; }

        [StringLength(11, ErrorMessage = "مقدار وارد شده بیش از 11 کاراکتر است.")]
        [Display(Name = "شماره تلفن همراه")]
        public string User_Mobile { get; set; }

        [Display(Name = "تاریخ ثبت نام")]
        public System.DateTime User_Date { get; set; }

        [Display(Name = "وضعیت کاربر")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "لطفاً وضعیت کاربر را انتخاب نمائید.")]
        public bool User_Active { get; set; }

        [Display(Name = "نقش")]
        //[Required(AllowEmptyStrings = false, ErrorMessage = "لطفاً نقش کاربر را انتخاب نمائید.")]
        public int User_Role_ID { get; set; }

        [StringLength(10, ErrorMessage = "مقدار وارد شده بیش از 10 کاراکتر است.")]
        [Display(Name = "کد ملی")]
        public string User_NationalCode { get; set; }

        [Display(Name = "تصویر پروفایل")]
        public string User_ImageIndex { get; set; }

        //[Display(Name = "استان")]
        //public Nullable<int> User_State_ID { get; set; }

    }

    [MetadataType(typeof(MetaData_User))]
    public partial class Tbl_User
    { }

}