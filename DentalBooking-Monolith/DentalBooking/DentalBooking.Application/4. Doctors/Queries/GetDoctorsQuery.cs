using DentalBooking.Application.Doctors.DTO;
using MediatR;

namespace DentalBooking.Application.Doctors.Queries;

public record GetDoctorsQuery : IRequest<List<DoctorDto>>;
