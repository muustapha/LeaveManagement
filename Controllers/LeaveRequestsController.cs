using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LeaveManagement.Models;
using LeaveManagement.Services;

namespace LeaveManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly ILeaveRequestService _leaveService;
        public LeaveRequestsController(ILeaveRequestService leaveService)
            => _leaveService = leaveService;

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveRequestOutputDTO>>> GetLeaveRequests()
            => Ok(await _leaveService.GetAllAsync());

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequestOutputDTO>> GetLeaveRequest(int id)
        {
            var dto = await _leaveService.GetByIdAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        [Authorize(Roles = "Employee")]
        [HttpPost]
        public async Task<ActionResult<LeaveRequestOutputDTO>> CreateLeaveRequest([FromBody] LeaveRequestInputDTO dto)
        {
            var created = await _leaveService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetLeaveRequest), new { id = created.Id }, created);
        }

        [Authorize(Roles = "Employee")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeaveRequest(int id, [FromBody] LeaveRequestInputDTO dto)
        {
            var updated = await _leaveService.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        [Authorize(Roles = "Employee")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveRequest(int id)
            => (await _leaveService.DeleteAsync(id)) ? NoContent() : NotFound();

        [Authorize(Roles = "Employee,Manager")]
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<LeaveRequestOutputDTO>>> Filter([FromQuery] LeaveRequestFilterDTO filter)
            => Ok(await _leaveService.FilterAsync(filter));

        [Authorize(Roles = "Manager")]
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] LeaveRequestStatusUpdateDTO dto)
            => (await _leaveService.UpdateStatusAsync(id, dto.Status)) ? NoContent() : BadRequest();
    }
}
