namespace DentalBooking.BookingService.Infrastructure.Outbox
{
    public interface IOutboxRepository
    {
        Task AddAsync(object @event);
        Task AddAsync(string eventType, object payload);
        Task<List<OutboxMessage>> GetUnprocessedAsync();
        Task MarkAsProcessedAsync(Guid id);
    }
}
