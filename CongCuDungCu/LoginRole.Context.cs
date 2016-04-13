namespace CongCuDungCu
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class LoginRoleEntities : DbContext
    {
        public LoginRoleEntities()
            : base("name=LoginRoleEntities")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<UserRole> UserRole { get; set; }
        public virtual DbSet<Users> Users { get; set; }
    }
}
