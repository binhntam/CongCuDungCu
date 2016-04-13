using System;
using System.Linq;
using System.Web.Security;


namespace CongCuDungCu.Models
{
    public class CustomRoleProvider : RoleProvider
    {
        public override bool IsUserInRole(string username, string roleName)
        {
            using (var usersContext = new LoginRoleEntities())
            {
                var user = usersContext.Users.First(u => u.UserName == username);
                if (user == null)
                {
                    return false;
                }
                return user.UserRole != null && user.UserRole.Select(u => u.Roles).Any(r => r.RoleName == roleName);
            }
        }

        public override string[] GetRolesForUser(string username)
        {
            using (var usersContext = new LoginRoleEntities())
            {
                var user = usersContext.Users.FirstOrDefault (u => u.UserName == username);
                if (user == null)
                {
                    return new string[] { };
                }
                var roleName = (from u in usersContext.Roles
                                join ur in usersContext.UserRole on u.idRole equals ur.Roles.idRole
                                where ur.Users.GIDUser == user.GIDUser
                                select u.RoleName).ToArray () ;
                return roleName;
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            using (var usersContext = new LoginRoleEntities ())
            {
                return usersContext.Roles.Select(r => r.RoleName).ToArray();
            }
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName { get; set; }
    }
}
