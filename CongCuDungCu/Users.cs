namespace CongCuDungCu
{
    using System;
    using System.Collections.Generic;

    public partial class Users
    {
        public Users()
        {
            UserRole = new HashSet<UserRole>();
        }

        public System.Guid GIDUser { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public Nullable<System.DateTime> LastActivityDate { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }
        public Nullable<System.DateTime> LastPasswordChangedDate { get; set; }
        public Nullable<System.DateTime> CreationDate { get; set; }
        public Nullable<bool> IsOnLine { get; set; }
        public Nullable<bool> IsLockedOut { get; set; }
        public Nullable<System.DateTime> LastLockedOutDate { get; set; }
        public Nullable<int> FailedPasswordAttemptCount { get; set; }
        public string MaDonVi { get; set; }

        public virtual ICollection<UserRole> UserRole { get; set; }
    }
}
