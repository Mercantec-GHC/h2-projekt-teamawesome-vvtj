using DomainModels.Models;

namespace API.Interfaces
{
    public interface ILoginAttemptService
    {
        bool IsLockedOut(string email);
        int RecordFailedAttempt(string email);
        int GetRemainingLockoutSeconds(string email);
        LoginAttemptInfo? GetLoginAttemptInfo(string email);
        void RecordSuccessfulLogin(string email);
    }
}
