namespace Claims.Services.Interfaces
{
    /// <summary>
    /// Service interface for auditing operations
    /// </summary>
    public interface IAuditService
    {
        /// <summary>
        /// Audits a claim operation asynchronously
        /// </summary>
        Task AuditClaimAsync(string id, string httpRequestType);

        /// <summary>
        /// Audits a cover operation asynchronously
        /// </summary>
        Task AuditCoverAsync(string id, string httpRequestType);
    }
}
