using LeaveManagement.DbContexts;
using LeaveManagement.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Repositories
{
    public class LeaveRequestRepository : ILeaveRequestRepository
    {
        private readonly AppDbContext _context;
        public LeaveRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public IQueryable<LeaveRequest> Query()
            => _context.LeaveRequests.Include(l => l.Employee);

        public Task<LeaveRequest?> GetByIdAsync(int id)
            => _context.LeaveRequests.FindAsync(id).AsTask();

        public Task AddAsync(LeaveRequest entity)
        {
            _context.LeaveRequests.Add(entity);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(LeaveRequest entity)
        {
            _context.LeaveRequests.Remove(entity);
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync()
            => _context.SaveChangesAsync();
    }
}
