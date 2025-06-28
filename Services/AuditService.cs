using Claims.Auditing;
using Claims.Services.Interfaces;

namespace Claims.Services
{
    /// <summary>
    /// Asynchronous audit service implementation
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly AuditContext _auditContext;
        private readonly ILogger<AuditService> _logger;

        public AuditService(AuditContext auditContext, ILogger<AuditService> logger)
        {
            _auditContext = auditContext;
            _logger = logger;
        }

        public async Task AuditClaimAsync(string id, string httpRequestType)
        {
            try
            {
                var claimAudit = new ClaimAudit()
                {
                    Created = DateTime.Now,
                    HttpRequestType = httpRequestType,
                    ClaimId = id
                };

                _auditContext.Add(claimAudit);
                await _auditContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to audit claim {ClaimId} for operation {Operation}", id, httpRequestType);
                // Don't throw - auditing failure shouldn't break the main operation
            }
        }

        public async Task AuditCoverAsync(string id, string httpRequestType)
        {
            try
            {
                var coverAudit = new CoverAudit()
                {
                    Created = DateTime.Now,
                    HttpRequestType = httpRequestType,
                    CoverId = id
                };

                _auditContext.Add(coverAudit);
                await _auditContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to audit cover {CoverId} for operation {Operation}", id, httpRequestType);
                // Don't throw - auditing failure shouldn't break the main operation
            }
        }
    }
}
