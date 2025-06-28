namespace Claims.Services.Interfaces
{
    /// <summary>
    /// Service interface for cover operations
    /// </summary>
    public interface ICoverService
    {
        /// <summary>
        /// Gets all covers
        /// </summary>
        Task<IEnumerable<Cover>> GetAllAsync();

        /// <summary>
        /// Gets a cover by ID
        /// </summary>
        Task<Cover?> GetByIdAsync(string id);

        /// <summary>
        /// Creates a new cover
        /// </summary>
        Task<Cover> CreateAsync(Cover cover);

        /// <summary>
        /// Deletes a cover by ID
        /// </summary>
        Task DeleteAsync(string id);

        /// <summary>
        /// Computes premium for a cover
        /// </summary>
        decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType);

        /// <summary>
        /// Validates a cover against business rules
        /// </summary>
        ValidationResult Validate(Cover cover);
    }
}
