using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeBuilder.Domain.Entities;

public class ResumeJobMatch
{
    [Key]
    public int MatchId { get; set; }
    
    [Required]
    public int ResumeId { get; set; }
    
    [Required]
    public int JobRoleId { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal MatchScore { get; set; } // 0.00 to 100.00
    
    [MaxLength(2000)]
    public string? MatchedSkills { get; set; } // Comma-separated matched skills
    
    [MaxLength(2000)]
    public string? MissingSkills { get; set; } // Comma-separated missing skills
    
    public DateTime MatchedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    [ForeignKey("ResumeId")]
    public virtual Resume? Resume { get; set; }
    
    [ForeignKey("JobRoleId")]
    public virtual JobRole? JobRole { get; set; }
}
