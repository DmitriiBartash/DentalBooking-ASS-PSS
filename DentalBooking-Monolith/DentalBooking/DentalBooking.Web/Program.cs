using DentalBooking.Application;
using DentalBooking.Application.Behaviors;
using DentalBooking.Application.Common.Interfaces;
using DentalBooking.Infrastructure.Identity;
using DentalBooking.Infrastructure.Persistence;
using DentalBooking.Infrastructure.Repositories;
using DentalBooking.Infrastructure.Services;
using DentalBooking.Infrastructure.Settings;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ---------- DbContext ----------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ---------- Identity ----------
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// ---------- MediatR + Validation ----------
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// ---------- Repositories + Services ----------
builder.Services.AddTransient<IEmailSender, ConsoleEmailSender>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IProcedureRepository, ProcedureRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<INotificationLogRepository, NotificationLogRepository>();
builder.Services.AddHostedService<ReminderBackgroundService>();

// ---------- App Settings ----------
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddScoped<IAppSettingsProvider, AppSettingsProvider>();

// ---------- Razor Pages ----------
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/Admin", "AdminPolicy");
    options.Conventions.AuthorizeFolder("/Client", "ClientPolicy");
});

// ---------- Authorization Policies ----------
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"))
    .AddPolicy("ClientPolicy", policy => policy.RequireRole("Client"));

var app = builder.Build();

// ---------- Data Seeding ----------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var config = services.GetRequiredService<IConfiguration>();
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger("DataSeeder");

    await DataSeeder.SeedRolesAndAdminAsync(roleManager, userManager, config, logger);
}

// ---------- Middleware Pipeline ----------
app.UseStaticFiles();
app.UseMiddleware<DentalBooking.Web.Middleware.ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// ---------- Redirect Root ----------
app.MapGet("/", context =>
{
    context.Response.Redirect("/Login");
    return Task.CompletedTask;
});

app.Run();
