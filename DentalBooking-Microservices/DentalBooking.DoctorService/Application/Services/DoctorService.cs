using DentalBooking.DoctorService.Application.Dtos;
using DentalBooking.DoctorService.Application.Interfaces;
using DentalBooking.DoctorService.Domain.Entities;
using DentalBooking.DoctorService.Domain.Patterns.Factories;
using DentalBooking.DoctorService.Domain.Patterns.Interfaces;
using DentalBooking.DoctorService.Domain.Patterns.Prototypes;

namespace DentalBooking.DoctorService.Application.Services;

public class DoctorService(IDoctorRepository repo, IServiceProvider serviceProvider) : IDoctorService
{
    public async Task<List<DoctorDto>> GetAllAsync()
    {
        var doctors = await repo.GetAllAsync();

        return [.. doctors.Select(d => new DoctorDto(
            d.Id,
            d.FirstName,
            d.LastName,
            d.Specialty,
            d.DoctorProcedures?
                .Where(dp => dp.Procedure != null)
                .Select(dp => dp.Procedure!.Name)
                .ToList() ?? []
        ))];
    }

    public async Task<DoctorDto?> GetByIdAsync(int id)
    {
        var doctor = await repo.GetByIdAsync(id);
        if (doctor == null) return null;

        return new DoctorDto(
            doctor.Id,
            doctor.FirstName,
            doctor.LastName,
            doctor.Specialty,
            doctor.DoctorProcedures?
                .Where(dp => dp.Procedure != null)
                .Select(dp => dp.Procedure!.Name)
                .ToList() ?? []
        );
    }

    public async Task<DoctorDto> CreateAsync(CreateDoctorRequest request)
    {
        Doctor doctor;

        if (request.ProcedureIds == null || request.ProcedureIds.Count == 0)
        {
            IDoctorPrototype prototype = request.Type switch
            {
                "Surgeon" => new SurgeonPrototype(),
                "Therapist" => new TherapistPrototype(),
                _ => throw new InvalidOperationException($"Unknown doctor type: {request.Type}")
            };

            doctor = (Doctor)prototype.Clone();
            doctor.FirstName = request.FirstName;
            doctor.LastName = request.LastName;
        }
        else
        {
            var factory = AbstractFactoryRegistry.GetFactory(request.Type, serviceProvider);
            doctor = factory.Create(request.FirstName, request.LastName, request.ProcedureIds);
        }

        await repo.AddAsync(doctor);
        await repo.SaveChangesAsync();

        return new DoctorDto(doctor.Id, doctor.FirstName, doctor.LastName, doctor.Specialty);
    }

    public async Task<bool> UpdateAsync(int id, UpdateDoctorRequest request)
    {
        var doctor = await repo.GetByIdAsync(id);
        if (doctor == null) return false;

        doctor.FirstName = request.FirstName;
        doctor.LastName = request.LastName;
        doctor.Specialty = request.Type;

        if (request.ProcedureIds != null)
        {
            doctor.DoctorProcedures ??= [];

            var toRemove = doctor.DoctorProcedures
                .Where(dp => !request.ProcedureIds.Contains(dp.ProcedureId))
                .ToList();

            foreach (var dp in toRemove)
                doctor.DoctorProcedures.Remove(dp);

            foreach (var pid in request.ProcedureIds)
            {
                if (doctor.DoctorProcedures.All(dp => dp.ProcedureId != pid))
                {
                    doctor.DoctorProcedures.Add(new DoctorProcedure
                    {
                        DoctorId = doctor.Id,
                        ProcedureId = pid
                    });
                }
            }
        }

        await repo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var doctor = await repo.GetByIdAsync(id);
        if (doctor == null) return false;

        await repo.DeleteAsync(doctor);
        await repo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignProceduresAsync(int doctorId, List<int> procedureIds)
    {
        var doctor = await repo.GetByIdAsync(doctorId);
        if (doctor == null) return false;

        doctor.DoctorProcedures ??= [];

        foreach (var pid in procedureIds)
        {
            if (doctor.DoctorProcedures.All(dp => dp.ProcedureId != pid))
                doctor.DoctorProcedures.Add(new DoctorProcedure { DoctorId = doctor.Id, ProcedureId = pid });
        }

        await repo.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RemoveProceduresAsync(int doctorId, List<int> procedureIds)
    {
        var doctor = await repo.GetByIdAsync(doctorId);
        if (doctor?.DoctorProcedures == null || doctor.DoctorProcedures.Count == 0)
            return false;

        var toRemove = doctor.DoctorProcedures
            .Where(dp => procedureIds.Contains(dp.ProcedureId))
            .ToList();

        foreach (var dp in toRemove)
            doctor.DoctorProcedures.Remove(dp);

        await repo.SaveChangesAsync();
        return true;
    }

    public async Task<DoctorDto> CloneAsync(int doctorId, string newFirstName, string newLastName)
    {
        var existing = await repo.GetByIdAsync(doctorId);
        if (existing is not IDoctorPrototype proto)
            throw new InvalidOperationException("Clone unsupported for this type");

        var clone = (Doctor)proto.Clone();
        clone.FirstName = newFirstName;
        clone.LastName = newLastName;

        await repo.AddAsync(clone);
        await repo.SaveChangesAsync();

        return new DoctorDto(clone.Id, clone.FirstName, clone.LastName, clone.Specialty);
    }

    public async Task<List<DoctorDto>> GetByProcedureAsync(int procedureId)
    {
        var doctors = await repo.GetByProcedureAsync(procedureId);

        return [.. doctors.Select(d => new DoctorDto(
            d.Id,
            d.FirstName,
            d.LastName,
            d.Specialty,
            d.DoctorProcedures?
                .Where(dp => dp.Procedure != null)
                .Select(dp => dp.Procedure!.Name)
                .ToList() ?? []
        ))];
    }
}
