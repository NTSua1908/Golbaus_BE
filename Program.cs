using Golbaus_BE.Entities;
using Golbaus_BE.Extentions;
using Hangfire;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCustomDbContext(builder);
builder.Services.AddHangfire(builder);
builder.Services.RegisterApiServices();

builder.Services.AddCors(options =>
{
	options.AddPolicy("CorsPolicy",
					policy => policy.AllowAnyHeader()
									.AllowAnyMethod()
									.SetIsOriginAllowed(origin => true)
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
//    endpoints.MapHangfireDashboard("/hangfire", new DashboardOptions
//    {
//        IgnoreAntiforgeryToken = true
//    })
//);

//app.UseHangfireDashboard("/hangfire", options);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
