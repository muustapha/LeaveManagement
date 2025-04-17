using LeaveManagement.Models;

namespace LeaveManagement.Services
{
    public interface ILeaveRequestService
    {
        Task<IEnumerable<LeaveRequestOutputDTO>> GetAllAsync();
        Task<LeaveRequestOutputDTO?> GetByIdAsync(int id);
        Task<LeaveRequestOutputDTO> CreateAsync(LeaveRequestInputDTO dto);
        Task<LeaveRequestOutputDTO?> UpdateAsync(int id, LeaveRequestInputDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdateStatusAsync(int id, string status);
        Task<IEnumerable<LeaveRequestOutputDTO>> FilterAsync(LeaveRequestFilterDTO filter);

    }
}
