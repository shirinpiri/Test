//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace perfumedecant.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Tbl_ResetPasswordRequests
    {
        public System.Guid ResetPass_ID { get; set; }
        public int ResetPass_User_ID { get; set; }
        public Nullable<System.DateTime> ResetPass_Date { get; set; }
    
        public virtual Tbl_User Tbl_User { get; set; }
    }
}
