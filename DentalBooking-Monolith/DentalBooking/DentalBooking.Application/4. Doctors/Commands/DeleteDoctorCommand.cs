using MediatR;

namespace DentalBooking.Application.Doctors.Commands;

public record DeleteDoctorCommand(int Id) : IRequest<bool>;
