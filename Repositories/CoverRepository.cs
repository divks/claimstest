using Claims.Data;
using Claims.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Claims.Repositories
{
    /// <summary>
    /// Repository implementation for cover data operations
    /// </summary>
    public class CoverRepository : ICoverRepository
    {
        private readonly ClaimsContext _context;

        public CoverRepository(ClaimsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cover>> GetAllAsync()
        {
            return await _context.Covers.ToListAsync();
        }

        public async Task<Cover?> GetByIdAsync(string id)
        {
            return await _context.Covers
                .Where(cover => cover.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<Cover> AddAsync(Cover cover)
        {
            _context.Covers.Add(cover);
            await _context.SaveChangesAsync();
            return cover;
        }

        public async Task DeleteAsync(string id)
        {
            var cover = await GetByIdAsync(id);
            if (cover is not null)
            {
                _context.Covers.Remove(cover);
                await _context.SaveChangesAsync();
            }
        }
    }
}
