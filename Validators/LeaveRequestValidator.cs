using LeaveManagement.Entities;
using LeaveManagement.DbContexts;
using LeaveManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Validators
{
    public class LeaveRequestValidator
    {
        private readonly AppDbContext _context;

        public LeaveRequestValidator(AppDbContext context)
        {
            _context = context;
        }

        public async Task ValidateAsync(LeaveRequestInputDTO dto, int? excludeRequestId = null)
        {
            var leaveType = Enum.Parse<LeaveType>(dto.LeaveType ?? "Annual");

            // ✅ Règle 1 : pas de chevauchement
            bool overlap = await _context.LeaveRequests.AnyAsync(l =>
                l.EmployeeId == dto.EmployeeId &&
                l.StartDate < dto.EndDate &&
                l.EndDate > dto.StartDate &&
                (excludeRequestId == null || l.Id != excludeRequestId)
            );

            if (overlap)
                throw new InvalidOperationException("L'employé a déjà une demande de congé sur cette période.");

            // ✅ Règle 2 : 20 jours max pour congé annuel
            if (leaveType == LeaveType.Annual)
            {
                var year = dto.StartDate.Year;
                var totalDays = await _context.LeaveRequests
                    .Where(l => l.EmployeeId == dto.EmployeeId
                             && l.LeaveType == LeaveType.Annual
                             && l.StartDate.Year == year)
                    .SumAsync(l => (l.EndDate - l.StartDate).Days); // ✅ Correction ici

                var newDays = (dto.EndDate - dto.StartDate).Days;
                if (totalDays + newDays > 20)
                    throw new InvalidOperationException("Limite de 20 jours annuels atteinte.");
            }

            // ✅ Règle 3 : congé maladie sans raison
            if (leaveType == LeaveType.Sick && string.IsNullOrWhiteSpace(dto.Reason))
                throw new InvalidOperationException("Une raison est obligatoire pour un congé maladie.");
        }
    }
}
