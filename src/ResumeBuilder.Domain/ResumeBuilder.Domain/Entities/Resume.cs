using System.ComponentModel.DataAnnotations;

namespace ResumeBuilder.Domain.Entities;

public class Resume
{
    [Key]
    public int ResumeId { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;
    
    [MaxLength(2000)]
    public string? Summary { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;
    
    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(50)]
    public string? Phone { get; set; }
    
    [MaxLength(500)]
    public string? Address { get; set; }
    
    [MaxLength(500)]
    public string? LinkedInUrl { get; set; }
    
    [MaxLength(500)]
    public string? GitHubUrl { get; set; }
    
    [MaxLength(500)]
    public string? PortfolioUrl { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation Properties
    public virtual ICollection<EducationDetail> EducationDetails { get; set; } = new List<EducationDetail>();
    public virtual ICollection<ExperienceDetail> ExperienceDetails { get; set; } = new List<ExperienceDetail>();
    public virtual ICollection<Skill> Skills { get; set; } = new List<Skill>();
    public virtual ICollection<ResumeJobMatch> JobMatches { get; set; } = new List<ResumeJobMatch>();
}
