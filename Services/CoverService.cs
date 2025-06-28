using Claims.Repositories.Interfaces;
using Claims.Services.Interfaces;

namespace Claims.Services
{
    /// <summary>
    /// Service implementation for cover operations with fixed premium calculation
    /// </summary>
    public class CoverService : ICoverService
    {
        private readonly ICoverRepository _coverRepository;
        private readonly IAuditService _auditService;
        private const decimal BaseDayRate = 1250m;

        public CoverService(ICoverRepository coverRepository, IAuditService auditService)
        {
            _coverRepository = coverRepository;
            _auditService = auditService;
        }

        public async Task<IEnumerable<Cover>> GetAllAsync()
        {
            return await _coverRepository.GetAllAsync();
        }

        public async Task<Cover?> GetByIdAsync(string id)
        {
            return await _coverRepository.GetByIdAsync(id);
        }

        public async Task<Cover> CreateAsync(Cover cover)
        {
            var validation = Validate(cover);
            if (!validation.IsValid)
            {
                throw new ArgumentException($"Validation failed: {string.Join(", ", validation.Errors)}");
            }

            cover.Id = Guid.NewGuid().ToString();
            cover.Premium = ComputePremium(cover.StartDate, cover.EndDate, cover.Type);
            
            var result = await _coverRepository.AddAsync(cover);
            
            // Async audit without blocking
            _ = Task.Run(async () => await _auditService.AuditCoverAsync(cover.Id, "POST"));
            
            return result;
        }

        public async Task DeleteAsync(string id)
        {
            await _coverRepository.DeleteAsync(id);
            
            // Async audit without blocking
            _ = Task.Run(async () => await _auditService.AuditCoverAsync(id, "DELETE"));
        }

        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            var totalDays = (int)(endDate - startDate).TotalDays;
            if (totalDays <= 0) return 0;

            // Get multiplier based on cover type
            var multiplier = GetTypeMultiplier(coverType);
            var basePremiumPerDay = BaseDayRate * multiplier;

            decimal totalPremium = 0;

            for (int day = 1; day <= totalDays; day++)
            {
                if (day <= 30)
                {
                    // First 30 days - full rate
                    totalPremium += basePremiumPerDay;
                }
                else if (day <= 180)
                {
                    // Days 31-180 - discounted rate
                    var discount = coverType == CoverType.Yacht ? 0.05m : 0.02m;
                    totalPremium += basePremiumPerDay * (1 - discount);
                }
                else if (day <= 365)
                {
                    // Days 181-365 - additional discount
                    var firstDiscount = coverType == CoverType.Yacht ? 0.05m : 0.02m;
                    var additionalDiscount = coverType == CoverType.Yacht ? 0.03m : 0.01m;
                    totalPremium += basePremiumPerDay * (1 - firstDiscount - additionalDiscount);
                }
            }

            return totalPremium;
        }

        public ValidationResult Validate(Cover cover)
        {
            var result = new ValidationResult { IsValid = true };

            // Validate start date is not in the past
            if (cover.StartDate < DateTime.Today)
            {
                result.IsValid = false;
                result.Errors.Add("StartDate cannot be in the past");
            }

            // Validate insurance period does not exceed 1 year
            var insurancePeriod = (cover.EndDate - cover.StartDate).TotalDays;
            if (insurancePeriod > 365)
            {
                result.IsValid = false;
                result.Errors.Add("Total insurance period cannot exceed 1 year");
            }

            if (cover.EndDate <= cover.StartDate)
            {
                result.IsValid = false;
                result.Errors.Add("EndDate must be after StartDate");
            }

            return result;
        }

        private decimal GetTypeMultiplier(CoverType coverType)
        {
            return coverType switch
            {
                CoverType.Yacht => 1.1m,
                CoverType.PassengerShip => 1.2m,
                CoverType.Tanker => 1.5m,
                _ => 1.3m // ContainerShip, BulkCarrier, and other types
            };
        }
    }
}
