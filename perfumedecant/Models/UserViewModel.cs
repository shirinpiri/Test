using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace perfumedecant.Models
{
    public class UserViewModel
    {
        [Display(Name = "نام کاربری")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفاً نام کاربری را وارد نمائید.")]
        public string User_Username { get; set; }

        [RegularExpression(@"^\w+[\w-\.]*\@\w+((-\w+)|(\w*))\.[a-z]{2,3}$", ErrorMessage = " لطفاًایمیل را به درستی وارد نمایید")]
        [StringLength(200, ErrorMessage = "مقدار وارد شده برای ایمیل بیش از 200 کاراکتر است.")]
        [Display(Name = "ایمیل")]
        public string User_Email { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "لطفاً کلمه عبور را وارد نمائید.")]
        public string User_Password { get; set; }
    }
}