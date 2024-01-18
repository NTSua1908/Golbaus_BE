using Golbaus_BE.Commons.ErrorLocalization;
using System.ComponentModel.DataAnnotations;

namespace Golbaus_BE.DTOs.Auths
{
	public class LoginModel
	{
		[Required(ErrorMessageResourceName = "Username required", ErrorMessageResourceType = typeof(ErrorResource))]
		[EmailAddress]
		public string Email { get; set; }
		[Required(ErrorMessageResourceName = "Password required", ErrorMessageResourceType = typeof(ErrorResource))]
		public string Password { get; set; }
	}

	public class ResetPasswordModel
	{
        public string Email { get; set; }
        public string Token { get; set; }
        public string Password { get; set; }
    }
}
