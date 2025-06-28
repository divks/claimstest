using Claims.Data;
using Claims.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Repositories
{
    /// <summary>
    /// Repository implementation for claim data operations
    /// </summary>
    public class ClaimRepository : IClaimRepository
    {
        private readonly ClaimsContext _context;

        public ClaimRepository(ClaimsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Claim>> GetAllAsync()
        {
            return await _context.Claims.ToListAsync();
        }

        public async Task<Claim?> GetByIdAsync(string id)
        {
            return await _context.Claims
                .Where(claim => claim.Id == id)
                .SingleOrDefaultAsync();
        }

        public async Task<Claim> AddAsync(Claim claim)
        {
            _context.Claims.Add(claim);
            await _context.SaveChangesAsync();
            return claim;
        }

        public async Task DeleteAsync(string id)
        {
            var claim = await GetByIdAsync(id);
            if (claim is not null)
            {
                _context.Claims.Remove(claim);
                await _context.SaveChangesAsync();
            }
        }
    }
}
