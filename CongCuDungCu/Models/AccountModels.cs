using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Web.Security;
using System.Linq;

namespace CongCuDungCu.Models
{
    public class UsersContext : DbContext
    {
        public UsersContext()
            : base("DefaultConnection")
        {
        }
        public DbSet<UserProfile> UserProfiles { get; set; }
    }
    [Table("UserProfile")]
    public class UserProfile
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
    }


    public class ChangePasswordModel
    {
        [Display(Name = "Mật khẩu cũ:")]
        [Required(ErrorMessage = "Đòi hỏi phải nhập mật khẩu cũ")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        /**/
        [Display(Name = "Mật khẩu mới:")]
        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
        [StringLength(100, ErrorMessage = "{0} phải tối thiểu {2} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        /**/
        [Display(Name = "Xác nhận lại mật khẩu:")]
        [Required]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Mật khẩu mới và xác nhận mật khẩu không trùng nhau.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        /**/
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        private bool? rememberMe;
        [Display(Name = "Remember me?")]
        public bool? RememberMe
        {
            get
            {
                return rememberMe ?? false;
            }
            set
            {
                rememberMe = value;
            }
        }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public interface IMembershipService
    {
        bool ValidateUser(string username, string password);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        bool CheckPassword(string username, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email);
    }
    public class AccountMembershipService : IMembershipService
    {
        private readonly MembershipProvider _provider;
        public AccountMembershipService()
            : this(null)
        {
        }
        public AccountMembershipService(MembershipProvider provider)
        {
            _provider = provider ?? Membership.Provider;
        }

        public bool ValidateUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Value cannot be null or empty", "username");
            }
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Value cannot be null or empty", "password");
            }
            var user = string.Empty;
            user = AccountDataProvider.GetUserByUserName(username, password);
            if (user != null)
            {
                return true;
            }
            return false;
        }
        private void UpdateFailureCount(string username, string failureType)
        {
            throw new NotImplementedException();
        }
        public bool CheckAccount(string userName)
        {
            var currentUser = _provider.GetUser(userName, true );
            if (currentUser == null)
            {
                return false;
            }
            return true;
        }
        public MembershipCreateStatus CreateUser(string userName, string password, string email)
        {
            MembershipCreateStatus status;
            _provider.CreateUser(userName, password, email, null, null, true, null, out status);
            return status;
        }
        public bool CheckPassword(string username, string password)
        {
            var typedPass = password;

            var dbpassword = AccountDataProvider.GetPassByUserName(username);












            if (typedPass.ToLower() == dbpassword.ToLower())
            {
                return true;
            }

            return false;
        }
        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            var currentUser = AccountDataProvider.GetAllUsers().SingleOrDefault(u => u.UserName == userName);
            if (currentUser == null)
            {
                return false;
            }



            if (!CheckPassword(userName, oldPassword))
            {
                return false;
            }
            return ValidateUser(userName, oldPassword);
        }
    }
    public interface IFormsAuthenticationService
    {
        void SignIn(string username, bool createPersistentCookie);
        void SignOut();
    }
    public class FormsAuthenticationService : IFormsAuthenticationService
    {
        public void SignIn(string username, bool createPersistentCookie)
        {
            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException("Value cannot be null or empty", "username");
            }
            FormsAuthentication.SetAuthCookie(username, createPersistentCookie);
        }
        public void SignOut()
        {
            FormsAuthentication.SignOut();
        }
    }
}
