using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ResumeBuilder.Domain.Entities;
using ResumeBuilder.Infrastructure.Identity;

namespace ResumeBuilder.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
        : base(options)
    {
    }

    public DbSet<Resume> Resumes { get; set; }
    public DbSet<EducationDetail> EducationDetails { get; set; }
    public DbSet<ExperienceDetail> ExperienceDetails { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<JobRole> JobRoles { get; set; }
    public DbSet<ResumeJobMatch> ResumeJobMatches { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Resume Configuration
        builder.Entity<Resume>(entity =>
        {
            entity.HasKey(e => e.ResumeId);
            entity.HasIndex(e => e.UserId);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(200);
            
            entity.HasMany(e => e.EducationDetails)
                  .WithOne(e => e.Resume)
                  .HasForeignKey(e => e.ResumeId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasMany(e => e.ExperienceDetails)
                  .WithOne(e => e.Resume)
                  .HasForeignKey(e => e.ResumeId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasMany(e => e.Skills)
                  .WithOne(e => e.Resume)
                  .HasForeignKey(e => e.ResumeId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasMany(e => e.JobMatches)
                  .WithOne(e => e.Resume)
                  .HasForeignKey(e => e.ResumeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // EducationDetail Configuration
        builder.Entity<EducationDetail>(entity =>
        {
            entity.HasKey(e => e.EducationId);
            entity.Property(e => e.Institution).IsRequired().HasMaxLength(300);
            entity.Property(e => e.Degree).IsRequired().HasMaxLength(200);
        });

        // ExperienceDetail Configuration
        builder.Entity<ExperienceDetail>(entity =>
        {
            entity.HasKey(e => e.ExperienceId);
            entity.Property(e => e.CompanyName).IsRequired().HasMaxLength(300);
            entity.Property(e => e.JobTitle).IsRequired().HasMaxLength(200);
        });

        // Skill Configuration
        builder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.SkillId);
            entity.Property(e => e.SkillName).IsRequired().HasMaxLength(100);
        });

        // JobRole Configuration
        builder.Entity<JobRole>(entity =>
        {
            entity.HasKey(e => e.JobRoleId);
            entity.Property(e => e.RoleName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RequiredSkills).IsRequired();
            entity.HasIndex(e => e.IsActive);
            
            entity.HasMany(e => e.ResumeMatches)
                  .WithOne(e => e.JobRole)
                  .HasForeignKey(e => e.JobRoleId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ResumeJobMatch Configuration
        builder.Entity<ResumeJobMatch>(entity =>
        {
            entity.HasKey(e => e.MatchId);
            entity.HasIndex(e => new { e.ResumeId, e.JobRoleId }).IsUnique();
            entity.Property(e => e.MatchScore).HasColumnType("decimal(5,2)");
        });

        // Seed default job roles
        SeedJobRoles(builder);
    }

    private void SeedJobRoles(ModelBuilder builder)
    {
        builder.Entity<JobRole>().HasData(
            new JobRole
            {
                JobRoleId = 1,
                RoleName = "Full Stack Developer",
                Description = "Develop and maintain web applications using both frontend and backend technologies.",
                RequiredSkills = "C#,ASP.NET,JavaScript,React,SQL Server,HTML,CSS,Git",
                Category = "IT",
                ExperienceLevel = "Mid",
                MinSalary = 60000,
                MaxSalary = 120000,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new JobRole
            {
                JobRoleId = 2,
                RoleName = "Frontend Developer",
                Description = "Create responsive user interfaces and implement frontend logic.",
                RequiredSkills = "JavaScript,React,Angular,Vue.js,HTML,CSS,TypeScript,Git",
                Category = "IT",
                ExperienceLevel = "Entry",
                MinSalary = 45000,
                MaxSalary = 85000,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new JobRole
            {
                JobRoleId = 3,
                RoleName = "Backend Developer",
                Description = "Design and develop server-side applications and APIs.",
                RequiredSkills = "C#,ASP.NET Core,Python,Java,SQL Server,PostgreSQL,Redis,Docker",
                Category = "IT",
                ExperienceLevel = "Mid",
                MinSalary = 55000,
                MaxSalary = 110000,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new JobRole
            {
                JobRoleId = 4,
                RoleName = "Data Scientist",
                Description = "Analyze complex data and build machine learning models.",
                RequiredSkills = "Python,Machine Learning,TensorFlow,PyTorch,SQL,Statistics,Data Analysis,R",
                Category = "Data Science",
                ExperienceLevel = "Senior",
                MinSalary = 80000,
                MaxSalary = 150000,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new JobRole
            {
                JobRoleId = 5,
                RoleName = "DevOps Engineer",
                Description = "Implement and manage CI/CD pipelines and cloud infrastructure.",
                RequiredSkills = "Docker,Kubernetes,AWS,Azure,Jenkins,Terraform,Linux,Git,Python",
                Category = "IT",
                ExperienceLevel = "Mid",
                MinSalary = 70000,
                MaxSalary = 130000,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new JobRole
            {
                JobRoleId = 6,
                RoleName = "Mobile App Developer",
                Description = "Develop mobile applications for iOS and Android platforms.",
                RequiredSkills = "Swift,Kotlin,React Native,Flutter,iOS,Android,REST API,Git",
                Category = "IT",
                ExperienceLevel = "Mid",
                MinSalary = 55000,
                MaxSalary = 115000,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new JobRole
            {
                JobRoleId = 7,
                RoleName = "UI/UX Designer",
                Description = "Design user interfaces and create engaging user experiences.",
                RequiredSkills = "Figma,Adobe XD,Sketch,UI Design,UX Research,Prototyping,HTML,CSS",
                Category = "Design",
                ExperienceLevel = "Entry",
                MinSalary = 40000,
                MaxSalary = 90000,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new JobRole
            {
                JobRoleId = 8,
                RoleName = "Project Manager",
                Description = "Lead and manage software development projects and teams.",
                RequiredSkills = "Agile,Scrum,JIRA,Communication,Leadership,Project Planning,Risk Management",
                Category = "Management",
                ExperienceLevel = "Senior",
                MinSalary = 75000,
                MaxSalary = 140000,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new JobRole
            {
                JobRoleId = 9,
                RoleName = "Cloud Architect",
                Description = "Design and implement cloud solutions and architecture.",
                RequiredSkills = "AWS,Azure,GCP,Kubernetes,Docker,Terraform,Security,Networking",
                Category = "IT",
                ExperienceLevel = "Lead",
                MinSalary = 100000,
                MaxSalary = 180000,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1)
            },
            new JobRole
            {
                JobRoleId = 10,
                RoleName = "QA Engineer",
                Description = "Ensure software quality through testing and automation.",
                RequiredSkills = "Selenium,TestNG,JUnit,API Testing,Automation,Manual Testing,Git,SQL",
                Category = "IT",
                ExperienceLevel = "Entry",
                MinSalary = 40000,
                MaxSalary = 80000,
                IsActive = true,
                CreatedAt = new DateTime(2026, 1, 1)
            }
        );
    }
}
