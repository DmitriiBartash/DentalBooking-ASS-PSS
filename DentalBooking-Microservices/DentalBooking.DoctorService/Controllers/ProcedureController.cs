using DentalBooking.DoctorService.Application.Dtos;
using DentalBooking.DoctorService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DentalBooking.DoctorService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcedureController(IProcedureService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await service.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await service.GetByIdAsync(id);
        return item is null
            ? NotFound(new { message = "Procedure not found" })
            : Ok(item);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProcedureRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var id = await service.CreateAsync(request);
        return Ok(new
        {
            message = "Procedure created successfully",
            procedureId = id
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProcedureRequest request)
    {
        var ok = await service.UpdateAsync(id, request);
        return ok
            ? Ok(new { message = "Procedure updated successfully" })
            : NotFound(new { message = "Procedure not found" });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var ok = await service.DeleteAsync(id);
        return ok
            ? Ok(new { message = "Procedure deleted successfully" })
            : NotFound(new { message = "Procedure not found" });
    }
}
