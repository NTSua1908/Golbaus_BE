namespace Golbaus_BE.Commons.Constants
{
	public static class RoleContants
	{
		public const string SuperAdmin = "0";
		public const string Admin = "1";
		public const string User = "2";
	}

	public static class EmailConstant
	{
		public const string ConfirmEmail = "<html lang=\"en\"> <head>   <meta charset=\"UTF-8\">   <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">   <title>Activate Your Account</title>   <style>     body  {{       font-family: Arial, sans-serif;       line-height: 1.6;       margin: 0;       padding: 0;     }}      #container  {{       max-width: 600px;       margin: 20px auto;       padding: 20px;       border: 1px solid #ccc;       border-radius: 5px;     }}      h1  {{       color: #333;     }}      p  {{       color: #666;     }}      a  {{       color: #007bff;       text-decoration: none;       font-weight: bold;     }}   </style> </head> <body>   <div id=\"container\">  " +
			"   <h1>Activate Your Account</h1>     <p>Dear  {0},</p>     <p>Thank you for choosing Golbaus. We're excited to have you on board!</p>    " +
			"   <p>To activate your account, please click on the following link:</p>   " +
			"   <p><a href=\" {1}\">Activate Now</a></p>     <p>If you encounter any issues, please copy and paste the link into your browser.</p>  " +
			"   <p>Please note that this link is valid for a limited time. If the link has expired, you can request a new activation link by clicking <a href=\" {2}\">Resend Activation Email</a>.</p>     <p>Thank you,<br>  " +
			"   The Golbaus Development Team</p>   </div> </body> </html> ";

		public const string ResetPassword = "<!DOCTYPE html><html lang=\"en\"><head>  <meta charset=\"UTF-8\">  <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">  <title>Password Reset</title>  <style>    body {{      font-family: Arial, sans-serif;      line-height: 1.6;      margin: 0;      padding: 0;    }}    #container {{      max-width: 600px;      margin: 20px auto;      padding: 20px;      border: 1px solid #ccc;      border-radius: 5px;    }}    h1 {{      color: #333;    }}   " +
			" p {{      color: #666;    }}    a {{      color: #007bff;      text-decoration: none;      font-weight: bold;    }}  </style></head><body>  <div id=\"container\">    <h1>Password Reset</h1>    " +
			"<p>Dear {0},</p>    <p>We received a request to reset your password. If you didn't make this request, you can safely ignore this email.</p>    <p>To reset your password, please click on the following link:</p>   " +
			" <p><a href=\"{1}\">Reset Password</a></p>    <p>If you encounter any issues, please copy and paste the link into your browser.</p> " +
			"   <p>This link is valid for a limited time. If you don't reset your password within this period, you may need to request a new link.</p>    <p>Thank you,<br>    The Golbaus Development Team</p>  </div></body></html>";
	}

	public static class NotificationConstant
	{
		public const string NEW_POST = "has published a new post.";
		public const string NEW_QUESTION = "has published a new question.";
		public const string FOLLOWER = "is now following you. Welcome aboard!";
		public const string NEW_COMMENT = "has left a new comment on your post.";
		public const string NEW_ANSWER = "has left a new answer on your question.";
		public const string REPLY = "has replied to your comment.";
	}
}
