using DentalBooking.BookingService.Application.Commands;
using DentalBooking.BookingService.Application.Handlers;
using DentalBooking.BookingService.Application.Mediator;
using DentalBooking.BookingService.Application.Notifications;
using DentalBooking.BookingService.Domain.Events;
using DentalBooking.BookingService.Infrastructure.External;
using DentalBooking.BookingService.Infrastructure.Messaging;
using DentalBooking.BookingService.Infrastructure.Outbox;
using DentalBooking.BookingService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

builder.Services.AddScoped<IMediator, Mediator>();

builder.Services.AddScoped<ICommandHandler<CreateBookingCommand, int>, CreateBookingHandler>();
builder.Services.AddScoped<ICommandHandler<CancelBookingCommand>, CancelBookingHandler>();

builder.Services.AddScoped<INotificationHandler<BookingCreatedEvent>, LogEventHandler>();
builder.Services.AddScoped<INotificationHandler<BookingCreatedEvent>, OutboxEventHandler>();
builder.Services.AddScoped<INotificationHandler<BookingCancelledEvent>, LogEventHandler>();
builder.Services.AddScoped<INotificationHandler<BookingCancelledEvent>, OutboxEventHandler>();

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IOutboxRepository, OutboxRepository>();
builder.Services.AddSingleton<IRabbitPublisher, RabbitPublisher>();
builder.Services.AddHostedService<OutboxPublisher>();

builder.Services.AddHttpClient<IAuthApiClient, AuthApiClient>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    var db = sp.GetRequiredService<DataContext>();
    var logger = sp.GetRequiredService<ILogger<Program>>();

    await db.Database.MigrateAsync();
    await BookingSeeder.SeedAsync(db, logger);
}

app.Run();
