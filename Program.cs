using Golbaus_BE.Commons.Helper;
using Golbaus_BE.Entities;
using Golbaus_BE.Extentions;
using Hangfire;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
	options.JsonSerializerOptions.Converters.Add(new TrimStringConverter());
}); ;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCustomDbContext(builder);
builder.Services.AddHangfire(builder);
builder.Services.RegisterApiServices();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(1); 
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.Cookie.SameSite = SameSiteMode.None;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddCors(options =>
{
	options.AddPolicy("CorsPolicy",
					policy => policy.AllowAnyHeader()
									.AllowAnyMethod()
									.SetIsOriginAllowed(origin => true)
									//.WithOrigins("http://localhost:3000")
									.AllowCredentials());
});

builder.Services.AddJWT(builder);
builder.Services.AddSwagger();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	using (var scope = app.Services.CreateScope())
	{
		var services = scope.ServiceProvider;
		var context = services.GetRequiredService<ApiDbContext>();
		context.Database.Migrate();
	}
}

//if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
//{
//	app.UseSwagger();
//	app.UseSwaggerUI();
//}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

//var options = new DashboardOptions
//{
//	Authorization = new[]{
//		new HangfireAuthorizationFilter()
//	}
//};

//app.UseEndpoints(endpoints =>
//	endpoints.MapHangfireDashboard("/hangfire", new DashboardOptions
//	{
//		IgnoreAntiforgeryToken = true
//	})
//);

app.UseSession();

//app.UseStringTrimmingMiddleware();

app.UseHangfireDashboard("/hangfire");

app.UseCors("CorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
