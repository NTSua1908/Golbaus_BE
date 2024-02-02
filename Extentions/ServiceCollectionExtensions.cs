using Golbaus_BE.Commons.Helper;
using Golbaus_BE.Entities;
using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Golbaus_BE.DTOs;
using Golbaus_BE.Services.Interface;
using Golbaus_BE.Services.Implement;

namespace Golbaus_BE.Extentions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddCustomDbContext(this IServiceCollection services, WebApplicationBuilder builder)
		{
			services.AddEntityFrameworkMySql().AddDbContext<ApiDbContext>(options =>
				options.UseMySql(
					builder.Configuration.GetConnectionString("ApiDbConnection"),
					ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ApiDbConnection"))
			));

			builder.Configuration.Bind("MailSettings", new MailSettings());
			builder.Configuration.Bind("JWTToken", new JWTToken());
			builder.Configuration.Bind("DefaultAdmin", new DefaultAdmin());
			builder.Configuration.Bind("DefaultSuperAdmin", new DefaultSuperAdmin());

			return services;
		}

		public static IServiceCollection RegisterApiServices(this IServiceCollection services)
		{
			services.AddHttpContextAccessor();
			services.AddScoped<IAuthServices, AuthServices>();
			services.AddScoped<IAccountService, AccountService>();
			services.AddScoped<ICommentService, CommentService>();
			services.AddScoped<IPostService, PostService>();
			services.AddScoped<IQuestionService, QuestionService>();
			services.AddScoped<ITagService, TagService>();
			services.AddScoped<IEmailService, EmailService>();
			services.AddScoped<INotificationService, NotificationService>();
			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddScoped<UserResolverService>();

			services.AddIdentity<User, Role>(options =>
			{
				options.User.RequireUniqueEmail = true;
				options.SignIn.RequireConfirmedEmail = true;
				options.SignIn.RequireConfirmedPhoneNumber = false;
			})
			.AddEntityFrameworkStores<ApiDbContext>()
			.AddDefaultTokenProviders();
			return services;
		}

		public static IServiceCollection AddCustomConfiguration(this IServiceCollection services)
		{
			services.AddMvc().AddJsonOptions(options =>
			{
				options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
				options.JsonSerializerOptions.WriteIndented = true;
				options.JsonSerializerOptions.MaxDepth = 64;
				options.JsonSerializerOptions.IncludeFields = true;
			});

			services.AddCors(options =>
			{
				options.AddPolicy("CorsPolicy",
								policy => policy.AllowAnyHeader()
												.AllowAnyMethod()
												.SetIsOriginAllowed(origin => true)
												.WithExposedHeaders("Content-Disposition")
												.AllowCredentials());
			});

			services.AddIdentity<User, Role>(options =>
			{
				// Password settings
				options.Password.RequireDigit = true;
				options.Password.RequireUppercase = true;
				options.Password.RequiredLength = 8;
				options.Password.RequireNonAlphanumeric = true;

				// Signin settings
				options.SignIn.RequireConfirmedEmail = true;
				options.SignIn.RequireConfirmedPhoneNumber = false;

				// User settings
				options.User.RequireUniqueEmail = true;

				// Username settings
				options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@#$%^&*()+/=<> ";

				// Token settings
				options.Tokens.PasswordResetTokenProvider = "passwordresettoken";
			})
			.AddEntityFrameworkStores<ApiDbContext>()
			.AddDefaultTokenProviders()
			.AddTokenProvider<PasswordResetTokenProvider<User>>("passwordresettoken");

			services.Configure<DataProtectionTokenProviderOptions>(x => x.TokenLifespan = TimeSpan.FromDays(1));

			services.Configure<PasswordResetTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromMinutes(5));

			services.Configure<SecurityStampValidatorOptions>(options => options.ValidationInterval = TimeSpan.FromDays(1));

			return services;
		}

		public static IServiceCollection AddHangfire(this IServiceCollection services, WebApplicationBuilder builder)
		{
			services.AddHangfire(x => x
			.SetDataCompatibilityLevel(CompatibilityLevel.Version_110)
			.UseSimpleAssemblyNameTypeSerializer()
			.UseRecommendedSerializerSettings()
			.UseStorage(
				new MySqlStorage(
					builder.Configuration.GetConnectionString("ApiDbConnection"),
					new MySqlStorageOptions
					{
						QueuePollInterval = TimeSpan.FromSeconds(1),
						JobExpirationCheckInterval = TimeSpan.FromHours(1),
						CountersAggregateInterval = TimeSpan.FromMinutes(5),
						PrepareSchemaIfNecessary = true,
						DashboardJobListLimit = 25000,
						TransactionTimeout = TimeSpan.FromMinutes(5),
						TablesPrefix = "Hangfire",
					}
				)
			));

			// Add the processing server as IHostedService
			services.AddHangfireServer(options => options.WorkerCount = 1);

			return services;
		}

		public static IServiceCollection AddJWT(this IServiceCollection services, WebApplicationBuilder builder)
		{
			// ===== Add Jwt Authentication ========
			JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); // => remove default claims
			services
				.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
				.AddJwtBearer(cfg =>
				{
					cfg.RequireHttpsMetadata = false;
					cfg.SaveToken = true;
					cfg.TokenValidationParameters = new TokenValidationParameters
					{
						ValidAudiences = new List<string>()
						{
							builder.Configuration.GetValue<string>("JWTToken:JwtAudienceId")
						},
						ValidIssuer = builder.Configuration.GetValue<string>("JWTToken:JwtIssuer"),
						ValidAudience = builder.Configuration.GetValue<string>("JWTToken:JwtIssuer"),
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("JWTToken:JwtKey"))),
						ClockSkew = TimeSpan.Zero // remove delay of token when expire,
					};
					cfg.Events = new JwtBearerEvents
					{
						OnAuthenticationFailed = context =>
						{
							if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
							{
								context.Response.Headers.Add("Token-Expired", "true");
							}
							return Task.CompletedTask;
						}
					};
				});//.AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
			return services;
		}

		public static IServiceCollection AddSwagger(this IServiceCollection services)
		{
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Golbaus APP API", Version = "v1.0.0" });
				c.AddSecurityDefinition("Bearer",
					new Microsoft.OpenApi.Models.OpenApiSecurityScheme
					{
						In = Microsoft.OpenApi.Models.ParameterLocation.Header,
						Description = "Please enter into field the word 'Bearer' following by space and JWT",
						Name = "Authorization",
						Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
					});
				c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
					{
					{
						new Microsoft.OpenApi.Models.OpenApiSecurityScheme
						{
							Reference = new Microsoft.OpenApi.Models.OpenApiReference
							{
								Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
								Id = "Bearer"
							},
							Scheme = "oauth2",
							Name = "Bearer",
							In = Microsoft.OpenApi.Models.ParameterLocation.Header
						},
						new List<string>()
					}
					});
			});
			return services;
		}
	}
}
