using LeaveManagement.Repositories;
using LeaveManagement.Entities;
using LeaveManagement.Models;
using LeaveManagement.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace LeaveManagement.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestRepository _repo;
        private readonly IMapper _mapper;
        private readonly LeaveRequestValidator _validator;

        public LeaveRequestService(
            ILeaveRequestRepository repo,
            IMapper mapper,
            LeaveRequestValidator validator)
        {
            _repo      = repo;
            _mapper    = mapper;
            _validator = validator;
        }

        public async Task<IEnumerable<LeaveRequestOutputDTO>> GetAllAsync()
        {
            var requests = _repo.Query().ToList();
            return _mapper.Map<IEnumerable<LeaveRequestOutputDTO>>(requests);
        }

        public async Task<LeaveRequestOutputDTO?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<LeaveRequestOutputDTO>(entity);
        }

        public async Task<LeaveRequestOutputDTO> CreateAsync(LeaveRequestInputDTO dto)
        {
            await _validator.ValidateAsync(dto);

            var entity = _mapper.Map<LeaveRequest>(dto);
            entity.LeaveType = Enum.Parse<LeaveType>(dto.LeaveType ?? "Annual");
            entity.Status    = LeaveStatus.Pending;
            entity.CreatedAt = DateTime.UtcNow;

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return _mapper.Map<LeaveRequestOutputDTO>(entity);
        }

        public async Task<LeaveRequestOutputDTO?> UpdateAsync(int id, LeaveRequestInputDTO dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                return null;

            await _validator.ValidateAsync(dto, id);

            existing.LeaveType = Enum.Parse<LeaveType>(dto.LeaveType ?? "Annual");
            existing.StartDate = dto.StartDate;
            existing.EndDate   = dto.EndDate;
            existing.Reason    = dto.Reason;

            await _repo.SaveChangesAsync();
            return _mapper.Map<LeaveRequestOutputDTO>(existing);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null)
                return false;

            await _repo.RemoveAsync(entity);
            await _repo.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<LeaveRequestOutputDTO>> FilterAsync(LeaveRequestFilterDTO filter)
        {
            var query = _repo.Query();

            if (filter.EmployeeId.HasValue)
                query = query.Where(l => l.EmployeeId == filter.EmployeeId.Value);

            if (!string.IsNullOrEmpty(filter.LeaveType)
                && Enum.TryParse<LeaveType>(filter.LeaveType, true, out var leaveType))
            {
                query = query.Where(l => l.LeaveType == leaveType);
            }

            if (!string.IsNullOrEmpty(filter.Status)
                && Enum.TryParse<LeaveStatus>(filter.Status, true, out var status))
            {
                query = query.Where(l => l.Status == status);
            }

            if (filter.StartDate.HasValue)
                query = query.Where(l => l.StartDate >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(l => l.EndDate <= filter.EndDate.Value);

            if (!string.IsNullOrWhiteSpace(filter.Keyword))
                query = query.Where(l => l.Reason!.ToLower().Contains(filter.Keyword.ToLower()));

            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                bool desc = filter.SortOrder?.ToLower() == "desc";
                query = filter.SortBy.ToLower() switch
                {
                    "startdate" => desc ? query.OrderByDescending(l => l.StartDate) : query.OrderBy(l => l.StartDate),
                    "enddate"   => desc ? query.OrderByDescending(l => l.EndDate)   : query.OrderBy(l => l.EndDate),
                    "createdat" => desc ? query.OrderByDescending(l => l.CreatedAt) : query.OrderBy(l => l.CreatedAt),
                    _ => query.OrderByDescending(l => l.CreatedAt)
                };
            }

            var skip = (filter.Page - 1) * filter.PageSize;
            var items = query.Skip(skip).Take(filter.PageSize).ToList();

            return _mapper.Map<IEnumerable<LeaveRequestOutputDTO>>(items);
        }

        public async Task<bool> UpdateStatusAsync(int id, string status)
        {
            var request = await _repo.GetByIdAsync(id);
            if (request == null || request.Status != LeaveStatus.Pending)
                return false;

            if (!Enum.TryParse<LeaveStatus>(status, true, out var newStatus))
                return false;

            request.Status = newStatus;
            await _repo.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<LeaveRequestReportDTO>> GetReportAsync(LeaveRequestReportFilterDTO filter)
        {
            var query = _repo.Query().Where(l => l.StartDate.Year == filter.Year);

            if (!string.IsNullOrWhiteSpace(filter.Department))
                query = query.Where(l => l.Employee!.Department == filter.Department);

            if (filter.StartDate.HasValue)
                query = query.Where(l => l.StartDate >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(l => l.EndDate <= filter.EndDate.Value);

            var grouped = query
                .GroupBy(l => new { l.EmployeeId, l.Employee!.FullName })
                .ToList();

            return grouped.Select(g => new LeaveRequestReportDTO
            {
                EmployeeId   = g.Key.EmployeeId,
                EmployeeName = g.Key.FullName,
                TotalLeaves  = g.Count(),
                AnnualLeaves = g.Count(l => l.LeaveType == LeaveType.Annual),
                SickLeaves   = g.Count(l => l.LeaveType == LeaveType.Sick)
            });
        }

        public async Task<bool> ApproveAsync(int id)
        {
            var req = await _repo.GetByIdAsync(id);
            if (req == null || req.Status != LeaveStatus.Pending)
                return false;

            req.Status = LeaveStatus.Approved;
            await _repo.SaveChangesAsync();

            return true;
        }
    }
}