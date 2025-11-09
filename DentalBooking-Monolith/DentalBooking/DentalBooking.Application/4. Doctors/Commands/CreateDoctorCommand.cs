using MediatR;

namespace DentalBooking.Application.Doctors.Commands;

public record CreateDoctorCommand( string FullName, List<int> SelectedProcedureIds) : IRequest<int>;
