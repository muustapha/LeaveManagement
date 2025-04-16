using Microsoft.AspNetCore.Mvc;
using LeaveManagement.Models.Services;
using LeaveManagement.Models.DTOs;

namespace LeaveManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly ILeaveRequestService _leaveService;

        public LeaveRequestsController(ILeaveRequestService leaveService)
        {
            _leaveService = leaveService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveRequestOutputDTO>>> GetLeaveRequests()
        {
            var results = await _leaveService.GetAllAsync();
            return Ok(results);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveRequestOutputDTO>> GetLeaveRequest(int id)
        {
            var leave = await _leaveService.GetByIdAsync(id);
            if (leave == null) return NotFound();
            return Ok(leave);
        }

        [HttpPost]
        public async Task<ActionResult<LeaveRequestOutputDTO>> CreateLeaveRequest(LeaveRequestInputDTO dto)
        {
            var created = await _leaveService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetLeaveRequest), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateLeaveRequest(int id, LeaveRequestInputDTO dto)
        {
            var updated = await _leaveService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveRequest(int id)
        {
             var deleted = await _leaveService.DeleteAsync(id);
             if (!deleted) return NotFound();

             return NoContent();
        }
       [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] LeaveRequestStatusUpdateDTO dto)
       {
            var result = await _leaveService.UpdateStatusAsync(id, dto.Status);
            if (!result)
           
            return BadRequest("Impossible de mettre à jour le statut.");

            return NoContent();
}


    }
}

