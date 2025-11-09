using MediatR;

namespace DentalBooking.Application.Doctors.Commands;

public record AssignDoctorProceduresCommand(int DoctorId, List<int> ProcedureIds) : IRequest<bool>;
