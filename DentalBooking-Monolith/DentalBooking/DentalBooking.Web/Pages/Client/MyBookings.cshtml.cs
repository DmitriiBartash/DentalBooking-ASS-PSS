using DentalBooking.Application.Bookings.DTO;
using DentalBooking.Application.Bookings.Queries;
using DentalBooking.Infrastructure.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DentalBooking.Web.Pages.Client;

public class MyBookingsModel(IMediator mediator, UserManager<ApplicationUser> userManager) : PageModel
{
    private readonly IMediator _mediator = mediator;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    public List<BookingDto> Bookings { get; set; } = [];

    public async Task OnGetAsync()
    {
        var clientId = _userManager.GetUserId(User)!;
        Bookings = await _mediator.Send(new GetBookingsQuery(ClientId: clientId));

        ViewData["ActivePage"] = "MyBookings";
    }
}
