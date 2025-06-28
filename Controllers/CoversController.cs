using Claims.Models.DTOs;
using Claims.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Claims.Controllers
{
    /// <summary>
    /// Controller for handling cover operations
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CoversController : ControllerBase
    {
        private readonly ICoverService _coverService;
        private readonly ILogger<CoversController> _logger;

        public CoversController(ICoverService coverService, ILogger<CoversController> logger)
        {
            _coverService = coverService;
            _logger = logger;
        }

        /// <summary>
        /// Computes premium for given parameters
        /// </summary>
        [HttpPost("compute")]
        public ActionResult<decimal> ComputePremiumAsync([FromBody] PremiumComputeRequest request)
        {
            try
            {
                var premium = _coverService.ComputePremium(request.StartDate, request.EndDate, request.CoverType);
                return Ok(premium);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error computing premium");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets all covers
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cover>>> GetAsync()
        {
            try
            {
                var covers = await _coverService.GetAllAsync();
                return Ok(covers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving covers");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Gets a cover by ID
        /// </summary>
        [HttpGet("{id}", Name = "GetCoverById")]
        public async Task<ActionResult<Cover>> GetAsync(string id)

        {
            try
            {
                var cover = await _coverService.GetByIdAsync(id);
                if (cover == null)
                {
                    return NotFound($"Cover with ID {id} not found");
                }
                return Ok(cover);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cover {CoverId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Creates a new cover
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Cover>> CreateAsync([FromBody] CoverDto coverDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var cover = new Cover
                {
                    StartDate = coverDto.StartDate,
                    EndDate = coverDto.EndDate,
                    Type = coverDto.Type
                };

                var result = await _coverService.CreateAsync(cover);
                return CreatedAtAction("GetCoverById", new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cover");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// Deletes a cover by ID
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAsync(string id)
        {
            try
            {
                var existingCover = await _coverService.GetByIdAsync(id);
                if (existingCover == null)
                {
                    return NotFound($"Cover with ID {id} not found");
                }

                await _coverService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cover {CoverId}", id);
                return StatusCode(500, "Internal server error");
            }
        }
    }

    /// <summary>
    /// Request model for premium computation
    /// </summary>
    public class PremiumComputeRequest
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public CoverType CoverType { get; set; }
    }
}
