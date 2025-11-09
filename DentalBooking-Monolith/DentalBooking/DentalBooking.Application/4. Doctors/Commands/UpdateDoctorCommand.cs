using MediatR;

namespace DentalBooking.Application.Doctors.Commands;

public record UpdateDoctorCommand(
    int Id,
    string FullName,
    List<int> SelectedProcedureIds
) : IRequest<bool>;
