using System.ComponentModel.DataAnnotations.Schema;
using DentalBooking.BookingService.Domain.Entities;

namespace DentalBooking.BookingService.Domain.Events;

[NotMapped]
public class BookingCancelledEvent(Booking booking) : BaseDomainEvent
{
    public Booking Booking { get; } = booking;
}
