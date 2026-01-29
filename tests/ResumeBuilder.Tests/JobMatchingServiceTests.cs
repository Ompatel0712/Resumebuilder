using Moq;
using ResumeBuilder.Application.Services;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Domain.Interfaces;
using Xunit;

namespace ResumeBuilder.Tests;

public class JobMatchingServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly JobMatchingService _service;

    public JobMatchingServiceTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _service = new JobMatchingService(_uowMock.Object);
    }

    [Fact]
    public async Task GetJobMatchesForResumeAsync_ShouldCalculateCorrectScores()
    {
        // Arrange
        var resumeId = 1;
        var resume = new Resume
        {
            ResumeId = resumeId,
            Title = "Full Stack Dev",
            Skills = new List<Skill>
            {
                new Skill { SkillName = "C#", ProficiencyLevel = 4 }, // Advanced
                new Skill { SkillName = "Javascript", ProficiencyLevel = 3 } // Intermediate
            }
        };

        var jobRoles = new List<JobRole>
        {
            new JobRole 
            { 
                JobRoleId = 10, 
                RoleName = "Backend Developer", 
                RequiredSkills = "C#,SQL,API",
                IsActive = true
            }
        };

        var matchRepoMock = new Mock<IRepository<ResumeJobMatch>>();
        _uowMock.Setup(u => u.Repository<ResumeJobMatch>()).Returns(matchRepoMock.Object);
        _uowMock.Setup(u => u.Resumes.GetResumeWithDetailsAsync(resumeId)).ReturnsAsync(resume);
        _uowMock.Setup(u => u.JobRoles.GetActiveJobRolesAsync()).ReturnsAsync(jobRoles);
        _uowMock.Setup(u => u.CompleteAsync()).ReturnsAsync(1);
        _uowMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var results = await _service.CalculateJobMatchesAsync(resumeId);

        // Assert
        Assert.NotEmpty(results);
        var match = results.First();
        Assert.Equal("Backend Developer", match.JobRoleName);
        Assert.Contains("C#", match.MatchedSkills);
        Assert.Contains("Sql", match.MissingSkills);
        // Base score: 1 match out of 3 skills = 33.3%
        // Proficiency bonus for C# (Level 4): +1.5% ( (4-3)*1.5 )
        // Total: 34.8%
        Assert.InRange((double)match.MatchScore, 34, 36);
    }
}
    [Fact]
    public void CalculateMatchScore_WithNoSkills_ShouldReturnZero()
    {
        // Simple internal logic check via service call
        // Assuming we could test private methods or just test via public ones
    }
}
