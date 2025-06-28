using Claims.Models.DTOs;
using Claims.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    /// <summary>
    /// Controller for handling claim operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ClaimsController : ControllerBase
    {
        private readonly ILogger<ClaimsController> _logger;
        private readonly IClaimService _claimService;

        public ClaimsController(ILogger<ClaimsController> logger, IClaimService claimService)
        {
            _logger = logger;
            _claimService = claimService;
        }

        /// <summary>
        /// Gets all claims
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Claim>>> GetAsync()
        {
            try
            {
                var claims = await _claimService.GetAllAsync();
                return Ok(claims);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving claims");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a claim by ID
        /// </summary>
        [HttpGet("{id}", Name = "GetClaimById")]
        public async Task<ActionResult<Claim>> GetAsync(string id)
        {
            try
            {
                var claim = await _claimService.GetByIdAsync(id);
                if (claim == null)
                {
                    return NotFound($"Claim with ID {id} not found");
                }
                return Ok(claim);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving claim {ClaimId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Creates a new claim
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Claim>> CreateAsync([FromBody] ClaimDto claimDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var claim = new Claim
                {
                    CoverId = claimDto.CoverId,
                    Created = claimDto.Created,
                    Name = claimDto.Name,
                    Type = claimDto.Type,
                    DamageCost = claimDto.DamageCost
                };

                var result = await _claimService.CreateAsync(claim);
                return CreatedAtAction("GetClaimById", new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating claim");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a claim by ID
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            try
            {
                var existingClaim = await _claimService.GetByIdAsync(id);
                if (existingClaim == null)
                {
                    return NotFound($"Claim with ID {id} not found");
                }

                await _claimService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting claim {ClaimId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
