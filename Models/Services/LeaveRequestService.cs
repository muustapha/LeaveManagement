using LeaveManagement.Models.Datas;
using LeaveManagement.Models.DTOs;
using LeaveManagement.Validators;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace LeaveManagement.Models.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly LeaveRequestValidator _validator;

        public LeaveRequestService(AppDbContext context, IMapper mapper, LeaveRequestValidator validator)
        {
            _context = context;
            _mapper = mapper;
            _validator = validator;
        }

        public async Task<IEnumerable<LeaveRequestOutputDTO>> GetAllAsync()
        {
            var requests = await _context.LeaveRequests.Include(e => e.Employee).ToListAsync();
            return _mapper.Map<IEnumerable<LeaveRequestOutputDTO>>(requests);
        }

        public async Task<LeaveRequestOutputDTO?> GetByIdAsync(int id)
        {
            var entity = await _context.LeaveRequests.FindAsync(id);
            return entity == null ? null : _mapper.Map<LeaveRequestOutputDTO>(entity);
        }

        public async Task<LeaveRequestOutputDTO> CreateAsync(LeaveRequestInputDTO dto)
        {
            // Validation des données d'entrée
            await _validator.ValidateAsync(dto);

            var entity = _mapper.Map<LeaveRequest>(dto);
            entity.LeaveType = Enum.Parse<LeaveType>(dto.LeaveType ?? "Annual");
            entity.Status = LeaveStatus.Pending;
            entity.CreatedAt = DateTime.UtcNow;

            _context.LeaveRequests.Add(entity);
            await _context.SaveChangesAsync();
            return _mapper.Map<LeaveRequestOutputDTO>(entity);
        }

        public async Task<LeaveRequestOutputDTO?> UpdateAsync(int id, LeaveRequestInputDTO dto)
        {
            var existing = await _context.LeaveRequests.FindAsync(id);
            if (existing == null) return null;

            await _validator.ValidateAsync(dto, id); // exclure cette demande lors du contrôle de chevauchement

            existing.LeaveType = Enum.Parse<LeaveType>(dto.LeaveType ?? "Annual");
            existing.StartDate = dto.StartDate;
            existing.EndDate = dto.EndDate;
            existing.Reason = dto.Reason;

            await _context.SaveChangesAsync();
            return _mapper.Map<LeaveRequestOutputDTO>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var request = await _context.LeaveRequests.FindAsync(id);
            if (request == null) return false;

            _context.LeaveRequests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<LeaveRequestOutputDTO>> FilterAsync(LeaveRequestFilterDTO filter)
        {
            var query = _context.LeaveRequests.Include(l => l.Employee).AsQueryable();

            if (filter.EmployeeId.HasValue)
                query = query.Where(l => l.EmployeeId == filter.EmployeeId.Value);

            if (!string.IsNullOrEmpty(filter.LeaveType) && Enum.TryParse<LeaveType>(filter.LeaveType, true, out var leaveType))
                query = query.Where(l => l.LeaveType == leaveType);

            if (!string.IsNullOrEmpty(filter.Status) && Enum.TryParse<LeaveStatus>(filter.Status, true, out var status))
                query = query.Where(l => l.Status == status);

            if (filter.StartDate.HasValue)
                query = query.Where(l => l.StartDate >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(l => l.EndDate <= filter.EndDate.Value);

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
                query = query.Where(l => l.Reason!.ToLower().Contains(filter.Keyword.ToLower()));

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                bool descending = filter.SortOrder?.ToLower() == "desc";
                query = filter.SortBy.ToLower() switch
                {
                    "startdate" => descending ? query.OrderByDescending(l => l.StartDate) : query.OrderBy(l => l.StartDate),
                    "enddate" => descending ? query.OrderByDescending(l => l.EndDate) : query.OrderBy(l => l.EndDate),
                    "createdat" => descending ? query.OrderByDescending(l => l.CreatedAt) : query.OrderBy(l => l.CreatedAt),
                    _ => query.OrderByDescending(l => l.CreatedAt)
                };
            }

            var skip = (filter.Page - 1) * filter.PageSize;
            var result = await query.Skip(skip).Take(filter.PageSize).ToListAsync();

            return _mapper.Map<IEnumerable<LeaveRequestOutputDTO>>(result);
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            var request = await _context.LeaveRequests.FindAsync(id);
            if (request == null || request.Status != LeaveStatus.Pending)
            return false;

            if (!Enum.TryParse<LeaveStatus>(status, true, out var newStatus))
            return false;

            request.Status = newStatus;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}