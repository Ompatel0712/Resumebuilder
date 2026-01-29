using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeBuilder.Domain.Entities;

public class EducationDetail
{
    [Key]
    public int EducationId { get; set; }
    
    [Required]
    public int ResumeId { get; set; }
    
    [Required]
    [MaxLength(300)]
    public string Institution { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Degree { get; set; } = string.Empty;
    
    [MaxLength(200)]
    public string? FieldOfStudy { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
    
    public bool IsCurrentlyStudying { get; set; }
    
    [Column(TypeName = "decimal(3,2)")]
    public decimal? GPA { get; set; }
    
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    // Navigation Property
    [ForeignKey("ResumeId")]
    public virtual Resume? Resume { get; set; }
}
