using Microsoft.AspNetCore.Mvc;
using src.Interfaces;
using src.Models.DTO;

namespace src.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class HallsController : ControllerBase
    {
        private readonly IHallService _hallService;

        public HallsController(IHallService hallService) => _hallService = hallService;

        [HttpGet]
        [ProducesResponseType(typeof(List<HallDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<HallDto>>> GetAll(CancellationToken ct) =>
            Ok(await _hallService.GetAllAsync(ct));

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(HallDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HallDto>> GetById(Guid id, CancellationToken ct) =>
            Ok(await _hallService.GetByIdAsync(id, ct));

        [HttpPost]
        [ProducesResponseType(typeof(HallDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<HallDto>> Create([FromBody] CreateHallDto dto, CancellationToken ct)
        {
            var created = await _hallService.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(HallDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<HallDto>> Update(Guid id, [FromBody] UpdateHallDto dto, CancellationToken ct) =>
            Ok(await _hallService.UpdateAsync(id, dto, ct));

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await _hallService.DeleteAsync(id, ct);
            return NoContent();
        }

        [HttpGet("available")]
        [ProducesResponseType(typeof(List<HallDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<HallDto>>> SearchAvailable(
            [FromQuery] DateOnly date,
            [FromQuery] TimeOnly startTime,
            [FromQuery] TimeOnly endTime,
            [FromQuery] int minCapacity,
            CancellationToken ct)
        {
            var dto = new HallAvailabilitySearchDto
            {
                Date = date,
                StartTime = startTime,
                EndTime = endTime,
                MinCapacity = minCapacity
            };
            return Ok(await _hallService.SearchAvailableAsync(dto, ct));
        }
    }
}
