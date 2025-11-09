using DentalBooking.DoctorService.Application.Interfaces;
using DentalBooking.DoctorService.Application.Services;
using DentalBooking.DoctorService.Infrastructure.Persistence;
using DentalBooking.DoctorService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DoctorDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IProcedureRepository, ProcedureRepository>();
builder.Services.AddScoped<IProcedureService, ProcedureService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DoctorDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var sp = scope.ServiceProvider;
    await DoctorSeeder.SeedAsync(db, logger, sp);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
