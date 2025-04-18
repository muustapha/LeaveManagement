using LeaveManagement.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Repositories
{
    public interface ILeaveRequestRepository
    {
        IQueryable<LeaveRequest> Query();
        Task<LeaveRequest?> GetByIdAsync(int id);
        Task AddAsync(LeaveRequest entity);
        Task RemoveAsync(LeaveRequest entity);
        Task SaveChangesAsync();
    }
}