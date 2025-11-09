using System.Text;
using DentalBooking.Gateway;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var jwt = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Chain of Responsibility: all requests go through the handler chain
var pipeline = new GatewayPipeline(builder.Configuration);

app.Use(async (context, next) =>
{
    await pipeline.ExecuteAsync(context);
    if (!context.Response.HasStarted)
        await next();
});

app.MapControllers();

// Fallback for unmatched routes
app.MapFallback(async context =>
{
    if (!context.Response.HasStarted)
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("No route matched in Gateway.");
    }
});

app.Run();
