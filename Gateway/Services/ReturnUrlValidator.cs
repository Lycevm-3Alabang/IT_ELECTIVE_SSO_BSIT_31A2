using ITElectiveSSO.Models;
using ITELECTIVE_SSO.Data;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Services
{
    public class ReturnUrlValidator : IReturnUrlValidator
    {
        private readonly SsoDbContext _context;

        public ReturnUrlValidator(SsoDbContext context)
        {
            _context = context;
        }

        public async Task<TenantApp?> ValidateAsync(string? returnUrl, string? ipAddress = null)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                return null;
            }

            // TODO (Task 3): match against TenantApps.ReturnUrl
            // TODO (Task 7): log invalid attempts to AuditLogs
            // TODO (Task 4): return app or null
        }
    }
}