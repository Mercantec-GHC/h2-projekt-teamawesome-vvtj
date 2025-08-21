using API.Interfaces;
using DomainModels.Models;
using Microsoft.Extensions.Caching.Memory;

namespace API.Services
{
    public class LoginAttemptService: ILoginAttemptService
    {
        private readonly IMemoryCache _cache;
        private readonly int _maxAttempts = 5;
        private readonly int _lockoutSeconds = 60;

        public LoginAttemptService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public bool IsLockedOut(string email)
        {
            var info = GetLoginAttemptInfo(email);
            return info != null && info.LockoutEnd.HasValue && info.LockoutEnd > DateTime.UtcNow;
        }

        public int RecordFailedAttempt(string email)
        {
            var info = GetOrCreateAttemptInfo(email);
            info.FailedAttempts++;

            if (info.FailedAttempts >= _maxAttempts)
            {
                info.LockoutEnd = DateTime.UtcNow.AddSeconds(_lockoutSeconds);
            }

            SetLoginAttemptInfo(email, info);

            // Return number of attempts left before lockout
            var remaining = _maxAttempts - info.FailedAttempts;
            return remaining;
        }

        public int GetRemainingLockoutSeconds(string email)
        {
            var info = GetLoginAttemptInfo(email);
            if (info == null || !info.LockoutEnd.HasValue)
                return 0;

            var remaining = (int)(info.LockoutEnd.Value - DateTime.UtcNow).TotalSeconds;
            return Math.Max(0, remaining);
        }

        public LoginAttemptInfo? GetLoginAttemptInfo(string email)
        {
            _cache.TryGetValue(email, out LoginAttemptInfo? info);
            return info;
        }

        public void RecordSuccessfulLogin(string email)
        {
            _cache.Remove(email);
        }

        private LoginAttemptInfo GetOrCreateAttemptInfo(string email)
        {
            var info = GetLoginAttemptInfo(email);
            if (info == null)
            {
                info = new LoginAttemptInfo();
                SetLoginAttemptInfo(email, info);
            }
            return info;
        }

        private void SetLoginAttemptInfo(string email, LoginAttemptInfo info)
        {
            _cache.Set(email, info, TimeSpan.FromMinutes(60));
        }
    }
}
