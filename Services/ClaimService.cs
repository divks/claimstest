using Claims.Repositories.Interfaces;
using Claims.Services.Interfaces;

namespace Claims.Services
{
    /// <summary>
    /// Service implementation for claim operations
    /// </summary>
    public class ClaimService : IClaimService
    {
        private readonly IClaimRepository _claimRepository;
        private readonly ICoverRepository _coverRepository;
        private readonly IAuditService _auditService;

        public ClaimService(
            IClaimRepository claimRepository, 
            ICoverRepository coverRepository,
            IAuditService auditService)
        {
            _claimRepository = claimRepository;
            _coverRepository = coverRepository;
            _auditService = auditService;
        }

        public async Task<IEnumerable<Claim>> GetAllAsync()
        {
            return await _claimRepository.GetAllAsync();
        }

        public async Task<Claim?> GetByIdAsync(string id)
        {
            return await _claimRepository.GetByIdAsync(id);
        }

        public async Task<Claim> CreateAsync(Claim claim)
        {
            var validation = await ValidateAsync(claim);
            if (!validation.IsValid)
            {
                throw new ArgumentException($"Validation failed: {string.Join(", ", validation.Errors)}");
            }

            claim.Id = Guid.NewGuid().ToString();
            var result = await _claimRepository.AddAsync(claim);
            
            // Async audit without blocking
            _ = Task.Run(async () => await _auditService.AuditClaimAsync(claim.Id, "POST"));
            
            return result;
        }

        public async Task DeleteAsync(string id)
        {
            await _claimRepository.DeleteAsync(id);
            
            // Async audit without blocking
            _ = Task.Run(async () => await _auditService.AuditClaimAsync(id, "DELETE"));
        }

        public async Task<ValidationResult> ValidateAsync(Claim claim)
        {
            var result = new ValidationResult { IsValid = true };

            // Validate damage cost
            if (claim.DamageCost > 100000)
            {
                result.IsValid = false;
                result.Errors.Add("DamageCost cannot exceed 100,000");
            }

            if (claim.DamageCost <= 0)
            {
                result.IsValid = false;
                result.Errors.Add("DamageCost must be greater than 0");
            }

            // Validate created date is within cover period
            var cover = await _coverRepository.GetByIdAsync(claim.CoverId);
            if (cover != null)
            {
                if (claim.Created < cover.StartDate || claim.Created > cover.EndDate)
                {
                    result.IsValid = false;
                    result.Errors.Add("Created date must be within the period of the related Cover");
                }
            }
            else
            {
                result.IsValid = false;
                result.Errors.Add("Related cover not found");
            }

            return result;
        }
    }
}
