namespace DomainModels.Models
{
    public class LoginAttemptInfo
    {
        public int FailedAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }
    }
}
