using Golbaus_BE.Commons.Helper;

namespace Golbaus_BE.Services.Interface
{
	public interface IEmailService
	{
		Task SendMailConfirmAsync(EmailContent content, string name, string token, string email);
		Task SendMailResetPasswordAsync(EmailContent content, string name, string token, string email);
	}
}
