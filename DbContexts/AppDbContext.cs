using Microsoft.EntityFrameworkCore;
using LeaveManagement.Entities;

namespace LeaveManagement.DbContexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 👤 Employés
            modelBuilder.Entity<Employee>().HasData(
                new Employee { Id = 1, FullName = "Alice Martin", Department = "IT", JoiningDate = new DateTime(2020, 1, 15) },
                new Employee { Id = 2, FullName = "John Doe", Department = "HR", JoiningDate = new DateTime(2019, 7, 1) },
                new Employee { Id = 3, FullName = "Marie Dupont", Department = "Finance", JoiningDate = new DateTime(2021, 3, 10) },
                new Employee { Id = 4, FullName = "Carlos Fernandez", Department = "Sales", JoiningDate = new DateTime(2022, 5, 5) },
                new Employee { Id = 5, FullName = "Léa Moreau", Department = "Marketing", JoiningDate = new DateTime(2023, 1, 2) }
            );

            // 📅 Demandes de congés
            modelBuilder.Entity<LeaveRequest>().HasData(
                new LeaveRequest
                {
                    Id = 1,
                    EmployeeId = 1, // Alice
                    LeaveType = LeaveType.Annual,
                    StartDate = new DateTime(2024, 6, 1),
                    EndDate = new DateTime(2024, 6, 10),
                    Status = LeaveStatus.Pending,
                    Reason = "Summer vacation",
                    CreatedAt = DateTime.UtcNow
                },
                new LeaveRequest
                {
                    Id = 2,
                    EmployeeId = 2, // John
                    LeaveType = LeaveType.Sick,
                    StartDate = new DateTime(2024, 7, 3),
                    EndDate = new DateTime(2024, 7, 5),
                    Status = LeaveStatus.Approved,
                    Reason = "Medical rest",
                    CreatedAt = DateTime.UtcNow
                },
                new LeaveRequest
                {
                    Id = 3,
                    EmployeeId = 4, // Carlos
                    LeaveType = LeaveType.Other,
                    StartDate = new DateTime(2024, 9, 15),
                    EndDate = new DateTime(2024, 9, 20),
                    Status = LeaveStatus.Pending,
                    Reason = "Family event",
                    CreatedAt = DateTime.UtcNow
                }
            );
        }
    }
}
