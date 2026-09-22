using ITElectiveSSO.Models;
using ITELECTIVE_SSO.Data;
using System.Threading.Tasks;

namespace Gateway.Services
{
    public class AuditService : IAuditService
    {
        private readonly SsoDbContext _context;

        public AuditService(SsoDbContext context)
        {
            _context = context;
        }

        public async Task LogLoginAsync(string? userId, string email, bool success, string? reason = null, string? ipAddress = null)
        {
            // TODO (Tasks 2 & 3): log successful and failed logins
        }

        public async Task LogActionAsync(string? userId, string action, string details, string? ipAddress = null)
        {
            // TODO (Task 4): log admin actions
        }
    }
}