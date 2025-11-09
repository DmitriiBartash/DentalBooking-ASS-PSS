using DentalBooking.Domain.Entities;
using MediatR;

namespace DentalBooking.Application.Doctors.Queries;

public record GetAvailableDoctorsQuery(int ProcedureId, DateTime StartUtc) : IRequest<List<Doctor>>;
