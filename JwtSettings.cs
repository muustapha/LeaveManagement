namespace LeaveManagement
{
    public class JwtSettings
    {
        public string SecretKey          { get; set; } = string.Empty;
        public int    TokenExpiryInHours { get; set; } = 1;
    }
}