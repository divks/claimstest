using System.ComponentModel.DataAnnotations;

namespace Claims.Models.DTOs
{
    /// <summary>
    /// Data Transfer Object for Cover operations
    /// </summary>
    public class CoverDto
    {
        public string? Id { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public CoverType Type { get; set; }

        public decimal Premium { get; set; }
    }
}
