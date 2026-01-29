using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeBuilder.Domain.Entities;

public class ExperienceDetail
{
    [Key]
    public int ExperienceId { get; set; }
    
    [Required]
    public int ResumeId { get; set; }
    
    [Required]
    [MaxLength(300)]
    public string CompanyName { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string JobTitle { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? Location { get; set; }
    
    [MaxLength(3000)]
    public string? Description { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public bool IsCurrent { get; set; }
    
    // Navigation Property
    [ForeignKey("ResumeId")]
    public virtual Resume? Resume { get; set; }
}
