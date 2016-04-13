namespace CongCuDungCu
{
    using System;
    using System.Collections.Generic;

    public partial class UserRole
    {
        public int idUserRole { get; set; }
        public Nullable<System.Guid> GIDUser { get; set; }
        public Nullable<int> idRole { get; set; }

        public virtual Roles Roles { get; set; }
        public virtual Users Users { get; set; }
    }
}
