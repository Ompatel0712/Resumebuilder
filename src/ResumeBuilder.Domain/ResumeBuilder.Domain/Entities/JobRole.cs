using System.ComponentModel.DataAnnotations;

namespace ResumeBuilder.Domain.Entities;

public class JobRole
{
    [Key]
    public int JobRoleId { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string RoleName { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    [Required]
    [MaxLength(2000)]
    public string RequiredSkills { get; set; } = string.Empty; // Comma-separated skills
    
    [MaxLength(500)]
    public string? Category { get; set; } // IT, Marketing, Finance, etc.
    
    [MaxLength(100)]
    public string? ExperienceLevel { get; set; } // Entry, Mid, Senior, Lead
    
    public decimal? MinSalary { get; set; }
    
    public decimal? MaxSalary { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Property
    public virtual ICollection<ResumeJobMatch> ResumeMatches { get; set; } = new List<ResumeJobMatch>();
}
