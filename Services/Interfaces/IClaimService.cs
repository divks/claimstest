namespace Claims.Services.Interfaces
{
    /// <summary>
    /// Service interface for claim operations
    /// </summary>
    public interface IClaimService
    {
        /// <summary>
        /// Gets all claims
        /// </summary>
        Task<IEnumerable<Claim>> GetAllAsync();

        /// <summary>
        /// Gets a claim by ID
        /// </summary>
        Task<Claim?> GetByIdAsync(string id);

        /// <summary>
        /// Creates a new claim
        /// </summary>
        Task<Claim> CreateAsync(Claim claim);

        /// <summary>
        /// Deletes a claim by ID
        /// </summary>
        Task DeleteAsync(string id);

        /// <summary>
        /// Validates a claim against business rules
        /// </summary>
        Task<ValidationResult> ValidateAsync(Claim claim);
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
