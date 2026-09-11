using Microsoft.AspNetCore.Mvc;
using src.Interfaces;
using src.Models.DTO;

namespace src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService) => _bookingService = bookingService;

        [HttpPost]
        [ProducesResponseType(typeof(BookingResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<BookingResultDto>> Create([FromBody] CreateBookingDto dto, CancellationToken ct)
        {
            var result = await _bookingService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(BookingResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<BookingResultDto>> GetById(Guid id, CancellationToken ct) =>
            Ok(await _bookingService.GetByIdAsync(id, ct));

        [HttpPost("{id:guid}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
        {
            await _bookingService.CancelAsync(id, ct);
            return NoContent();
        }
    }
}
