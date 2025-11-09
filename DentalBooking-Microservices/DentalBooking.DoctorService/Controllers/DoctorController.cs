using DentalBooking.DoctorService.Application.Dtos;
using DentalBooking.DoctorService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DentalBooking.DoctorService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorController(IDoctorService service) : ControllerBase
{
    #region Doctor

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var doctors = await service.GetAllAsync();
        return Ok(doctors);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var doctor = await service.GetByIdAsync(id);
        return doctor is null
            ? NotFound(new { message = "Doctor not found" })
            : Ok(doctor);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDoctorRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await service.CreateAsync(request);
        return Ok(new
        {
            message = "Doctor created successfully",
            doctor = created
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateDoctorRequest request)
    {
        var updated = await service.UpdateAsync(id, request);
        return updated
            ? Ok(new { message = "Doctor updated successfully" })
            : NotFound(new { message = "Doctor not found" });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await service.DeleteAsync(id);
        return deleted
            ? Ok(new { message = "Doctor deleted successfully" })
            : NotFound(new { message = "Doctor not found" });
    }

    [HttpGet("byProcedure/{procedureId}")]
    public async Task<IActionResult> GetByProcedure(int procedureId)
    {
        var doctors = await service.GetByProcedureAsync(procedureId);
        return Ok(doctors);
    }

    #endregion

    #region Doctor–Procedure Links

    [HttpPost("{id:int}/procedures")]
    public async Task<IActionResult> AssignProcedures(int id, [FromBody] List<int> procedureIds)
    {
        if (procedureIds == null || procedureIds.Count == 0)
            return BadRequest(new { message = "Procedure list cannot be empty." });

        var result = await service.AssignProceduresAsync(id, procedureIds);
        return result
            ? Ok(new { message = "Procedures assigned successfully" })
            : NotFound(new { message = "Doctor not found" });
    }

    [HttpDelete("{id:int}/procedures")]
    public async Task<IActionResult> RemoveProcedures(int id, [FromBody] List<int> procedureIds)
    {
        var result = await service.RemoveProceduresAsync(id, procedureIds);
        return result
            ? Ok(new { message = "Procedures removed successfully" })
            : NotFound(new { message = "Doctor not found" });
    }

    #endregion
}
