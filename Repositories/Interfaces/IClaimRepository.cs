namespace Claims.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for claim data operations
    /// </summary>
    public interface IClaimRepository
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
        /// Adds a new claim
        /// </summary>
        Task<Claim> AddAsync(Claim claim);

        /// <summary>
        /// Deletes a claim by ID
        /// </summary>
        Task DeleteAsync(string id);
    }
}
