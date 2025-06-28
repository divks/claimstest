using System.ComponentModel.DataAnnotations;

namespace Claims.Models.DTOs
{
    /// <summary>
    /// Data Transfer Object for Claim operations
    /// </summary>
    public class ClaimDto
    {
        public string? Id { get; set; }

        [Required]
        public string CoverId { get; set; } = string.Empty;

        [Required]
        public DateTime Created { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public ClaimType Type { get; set; }

        [Required]
        [Range(0.01, 100000, ErrorMessage = "Damage cost must be between 0.01 and 100,000")]
        public decimal DamageCost { get; set; }
    }
}
