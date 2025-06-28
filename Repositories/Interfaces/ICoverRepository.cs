namespace Claims.Repositories.Interfaces
{
    /// <summary>
    /// Repository interface for cover data operations
    /// </summary>
    public interface ICoverRepository
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
        /// Adds a new cover
        /// </summary>
        Task<Cover> AddAsync(Cover cover);

        /// <summary>
        /// Deletes a cover by ID
        /// </summary>
        Task DeleteAsync(string id);
    }
}
