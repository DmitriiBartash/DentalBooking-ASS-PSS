using System.Text;
using DentalBooking.Client.Services.Api.Endpoints;
using DentalBooking.Client.Services.Api.Facade;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient("Gateway", client =>
{
    var baseUrl = builder.Configuration["ApiSettings:GatewayUrl"];
    if (string.IsNullOrWhiteSpace(baseUrl))
        throw new InvalidOperationException("ApiSettings:GatewayUrl configuration value is missing or empty.");

    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        var key = Encoding.UTF8.GetBytes(
            builder.Configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("Jwt:Key configuration value is missing or empty.")
        );

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                if (context.Request.Cookies.TryGetValue("AuthToken", out var token))
                {
                    context.Token = token;
                    Console.WriteLine($"[Jwt] Token from cookie: {token[..Math.Min(20, token.Length)]}...");
                }
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine($"[Jwt] Authentication failed: {ctx.Exception.Message}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.Redirect("/Account/Login");
                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                context.Response.Redirect("/Account/Login");
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddScoped<AuthApiService>();
builder.Services.AddScoped<BookingApiService>();
builder.Services.AddScoped<DoctorApiService>();
builder.Services.AddScoped<ProcedureApiService>();
builder.Services.AddScoped<ReportsApiService>();
builder.Services.AddScoped<IApiFacade, ApiFacade>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 401 || context.Response.StatusCode == 403)
    {
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.Redirect("/Account/Login");
        }
    }
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
