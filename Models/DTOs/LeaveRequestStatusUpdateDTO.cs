namespace LeaveManagement.Models.DTOs
{
    public class LeaveRequestStatusUpdateDTO
    {
        public string Status { get; set; } = string.Empty; // "Approved" ou "Rejected"
    }
}
