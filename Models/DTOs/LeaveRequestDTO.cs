namespace LeaveManagement.Models.DTOs
{
    // DTO utilisé pour la création (entrée)
    public class LeaveRequestInputDTO
    {
        public int EmployeeId { get; set; }
        public string? LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Reason { get; set; }
    }

    // DTO utilisé pour l'affichage (sortie)
    public class LeaveRequestOutputDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string? LeaveType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Status { get; set; }
        public string? Reason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
