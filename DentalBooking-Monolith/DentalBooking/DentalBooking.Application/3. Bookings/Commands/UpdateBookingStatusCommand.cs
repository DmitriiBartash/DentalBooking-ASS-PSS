using DentalBooking.Domain.Enums;
using MediatR;

namespace DentalBooking.Application.Bookings.Commands;

public record UpdateBookingStatusCommand(int BookingId, BookingStatus NewStatus) : IRequest<bool>;
