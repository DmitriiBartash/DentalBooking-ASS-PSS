using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using DentalBooking.BookingService.Infrastructure.Persistence;

namespace DentalBooking.BookingService.Infrastructure.Outbox;

public class OutboxRepository(DataContext context) : IOutboxRepository
{
    private readonly DataContext _context = context;

    public async Task AddAsync(string eventType, object payload)
    {
        var message = new OutboxMessage
        {
            Type = eventType,
            Payload = JsonSerializer.Serialize(payload),
            CreatedUtc = DateTime.UtcNow,
            Processed = false
        };

        _context.OutboxMessages.Add(message);
        await _context.SaveChangesAsync();
    }

    public async Task AddAsync(object @event)
    {
        var message = new OutboxMessage
        {
            Type = @event.GetType().Name,
            Payload = JsonSerializer.Serialize(@event),
            CreatedUtc = DateTime.UtcNow,
            Processed = false
        };

        _context.OutboxMessages.Add(message);
        await _context.SaveChangesAsync();
    }

    public async Task<List<OutboxMessage>> GetUnprocessedAsync()
        => await _context.OutboxMessages
            .Where(m => !m.Processed)
            .OrderBy(m => m.CreatedUtc)
            .ToListAsync();

    public async Task MarkAsProcessedAsync(Guid id)
    {
        var msg = await _context.OutboxMessages.FirstOrDefaultAsync(m => m.Id == id);
        if (msg == null) return;

        msg.Processed = true;
        await _context.SaveChangesAsync();
    }
}
