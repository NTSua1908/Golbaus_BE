using Golbaus_BE.Commons.Constants;
using Golbaus_BE.Commons.Helper;
using Golbaus_BE.Services.Interface;
using MailKit.Security;
using MimeKit;

namespace Golbaus_BE.Services.Implement
{
	public class EmailService : IEmailService
	{
		private readonly IConfiguration _configuration;

		public EmailService(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public async Task SendMailConfirmAsync(EmailContent content, string hostName, string name, string token, string email)
		{
			string UIBaseURL = _configuration.GetSection("UIBaseUrl").Value;

			var confirmationLink = @$"{UIBaseURL}/ConfirmEmail/{token.Replace("/", "@")}/{email}";
			var resendConfirmationLink = @$"{UIBaseURL}/CheckEmail/{email}";
			content.Body = string.Format(EmailConstant.ConfirmEmail, name, confirmationLink, resendConfirmationLink);
			await SendMail(content);
		}

		#region Helper
		private async Task SendMail(EmailContent mailContent)
		{
			var email = new MimeMessage();
			email.Sender = new MailboxAddress(MailSettings.DisplayName, MailSettings.Mail);
			email.From.Add(new MailboxAddress(MailSettings.DisplayName, MailSettings.Mail));
			email.To.Add(MailboxAddress.Parse(mailContent.To));
			if (!string.IsNullOrWhiteSpace(mailContent.CC))
			{
				email.Cc.Add(MailboxAddress.Parse(mailContent.CC));
			}
			email.Subject = mailContent.Subject;

			var builder = new BodyBuilder();
			builder.HtmlBody = mailContent.Body;
			email.Body = builder.ToMessageBody();

			using var smtp = new MailKit.Net.Smtp.SmtpClient();
			try
			{
				smtp.Connect(MailSettings.Host, MailSettings.Port, SecureSocketOptions.StartTls);
				smtp.Authenticate(MailSettings.Mail, MailSettings.Password);
				await smtp.SendAsync(email);
			}
			catch (Exception)
			{
				System.IO.Directory.CreateDirectory("MailsSave");
				var emailsavefile = string.Format(@"MailsSave/{0}.eml", Guid.NewGuid());
				await email.WriteToAsync(emailsavefile);
			}
			smtp.Disconnect(true);
		}

		#endregion
	}
}
