using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResumeBuilder.Domain.Entities;

public class Skill
{
    [Key]
    public int SkillId { get; set; }
    
    [Required]
    public int ResumeId { get; set; }
    
    [Required]
    [MaxLength(100)]
    public string SkillName { get; set; } = string.Empty;
    
    [Range(1, 5)]
    public int ProficiencyLevel { get; set; } = 3; // 1=Beginner, 2=Elementary, 3=Intermediate, 4=Advanced, 5=Expert
    
    [MaxLength(50)]
    public string? Category { get; set; } // Technical, Soft Skills, Languages, etc.
    
    // Navigation Property
    [ForeignKey("ResumeId")]
    public virtual Resume? Resume { get; set; }
}
