//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LoginTest.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class M_ApplyLeave
    {
        public int AutoCode { get; set; }
        public Nullable<int> F_UserID { get; set; }
        public Nullable<int> F_LeaveTypeID { get; set; }
        public Nullable<System.DateTime> LeaveFrom { get; set; }
        public Nullable<System.DateTime> LeaveTo { get; set; }
        public Nullable<int> Month { get; set; }
        public Nullable<int> Year { get; set; }
        public Nullable<System.DateTime> InsertedOn { get; set; }
        public string InsertedBy { get; set; }
        public string InsertedIP { get; set; }
        public string LeaveAcceptedYN { get; set; }
        public Nullable<System.DateTime> LeaveAcceptedOn { get; set; }
        public string LeaveAcceptedBy { get; set; }
        public string LeaveAcceptedIP { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ModifiedIP { get; set; }
        public string LeaveFor { get; set; }
    
        public virtual M_LeaveType M_LeaveType { get; set; }
        public virtual M_Users M_Users { get; set; }
    }
}
