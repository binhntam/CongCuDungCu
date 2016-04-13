using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace CongCuDungCu.Models
{
    public class AccountDataProvider
    {
        private const string UserDataContextKey = "accountKey";
        public static CCDCDataContext db
        {
            get
            {
                if (HttpContext.Current.Items[UserDataContextKey] == null)
                {
                    HttpContext.Current.Items[UserDataContextKey] = new CCDCDataContext();
                }
                return (CCDCDataContext)HttpContext.Current.Items[UserDataContextKey];
            }
        }
        public static DateTime GetSystemDate()
        {
            return db.GetSystemDate();
        }
        public static void Save()
        {
            db.SubmitChanges();
        }
        public static void AddNew(User user)
        {
            db.Users.InsertOnSubmit(user);
        }
        public static string GetUserByUserName(string username, string password)
        {
            return (from user in db.Users
                    where user.UserName == username && user.Password == password
                    select user.UserName).SingleOrDefault();
        }
        public static string GetFullNameByUserName(string username)
        {
            return (from user in db.Users
                    where user.UserName == username
                    select user.FullName).SingleOrDefault();
        }
        public static IEnumerable<User> GetAllUsers()
        {
            return db.Users.AsEnumerable();
        }
        public static void UpdateNewPassword(string username, string newpassword)
        {
            using (var usersContext = new CCDCDataContext())
            {
                var updateLogin = usersContext.Users.SingleOrDefault(d => d.UserName == username);
                updateLogin.Password = newpassword;
                usersContext.SubmitChanges();
            }
        }
        public static void UpdateLastPasswordChangeDate(string username)
        {
            using (var usersContext = new CCDCDataContext())
            {
                var updateLogin = usersContext.Users.SingleOrDefault(d => d.UserName == username);
                updateLogin.LastPasswordChangedDate = GetSystemDate();
                usersContext.SubmitChanges();
            }
        }
        public static string GetPassByUserName(string username)
        {
            return (from user in db.Users
                    where user.UserName == username
                    select user.Password).First<string>();
        }
    }
}
