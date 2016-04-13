namespace CongCuDungCu
{
    using System;
    using System.Collections.Generic;

    public partial class Roles
    {
        public Roles()
        {
            UserRole = new HashSet<UserRole>();
        }

        public int idRole { get; set; }
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }

        public virtual ICollection<UserRole> UserRole { get; set; }
    }
}
