﻿using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Golbaus_BE.Extentions
{
	public class PasswordResetTokenProvider<TUser> : DataProtectorTokenProvider<TUser> where TUser : class
	{
		public PasswordResetTokenProvider(IDataProtectionProvider dataProtectionProvider,
			IOptions<PasswordResetTokenProviderOptions> options,
			ILogger<DataProtectorTokenProvider<TUser>> logger)
			: base(dataProtectionProvider, options, logger)
		{
		}
	}
	public class PasswordResetTokenProviderOptions : DataProtectionTokenProviderOptions
	{
	}
}
