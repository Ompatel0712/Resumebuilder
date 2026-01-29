using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ResumeBuilder.Application.DTOs;
using ResumeBuilder.Application.Interfaces;
using ResumeBuilder.Domain.Interfaces;

namespace ResumeBuilder.Application.Services;

public class PdfGeneratorService : IPdfGeneratorService
{
    private readonly IUnitOfWork _unitOfWork;

    public PdfGeneratorService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateResumePdfAsync(int resumeId, string template = "Modern")
    {
        var resume = await _unitOfWork.Resumes.GetResumeWithDetailsAsync(resumeId);
        if (resume == null)
            throw new InvalidOperationException("Resume not found");

        var resumeDto = MapToDto(resume);
        return await GenerateResumePdfAsync(resumeDto, template);
    }

    public Task<byte[]> GenerateResumePdfAsync(ResumeDto resume, string template = "Modern")
    {
        var document = template.ToLower() switch
        {
            "classic" => CreateClassicTemplate(resume),
            "corporate" => CreateCorporateTemplate(resume),
            _ => CreateModernTemplate(resume)
        };

        var pdfBytes = document.GeneratePdf();
        return Task.FromResult(pdfBytes);
    }

    public IEnumerable<string> GetAvailableTemplates()
    {
        return new[] { "Modern", "Classic", "Corporate" };
    }

    private Document CreateModernTemplate(ResumeDto resume)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header().Element(c => ModernHeader(c, resume));
                page.Content().Element(c => ModernContent(c, resume));
                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" of ");
                    x.TotalPages();
                });
            });
        });
    }

    private void ModernHeader(IContainer container, ResumeDto resume)
    {
        container.Background("#2563eb").Padding(20).Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text(resume.FullName).Bold().FontSize(24).FontColor(Colors.White);
                col.Item().Text(resume.Title).FontSize(14).FontColor("#bfdbfe");
                col.Item().PaddingTop(10).Row(r =>
                {
                    if (!string.IsNullOrEmpty(resume.Email))
                    {
                        r.AutoItem().Text($"✉ {resume.Email}").FontSize(9).FontColor(Colors.White);
                        r.AutoItem().PaddingHorizontal(10).Text("|").FontColor(Colors.White);
                    }
                    if (!string.IsNullOrEmpty(resume.Phone))
                    {
                        r.AutoItem().Text($"📞 {resume.Phone}").FontSize(9).FontColor(Colors.White);
                        r.AutoItem().PaddingHorizontal(10).Text("|").FontColor(Colors.White);
                    }
                    if (!string.IsNullOrEmpty(resume.Address))
                    {
                        r.AutoItem().Text($"📍 {resume.Address}").FontSize(9).FontColor(Colors.White);
                    }
                });
            });
        });
    }

    private void ModernContent(IContainer container, ResumeDto resume)
    {
        container.PaddingTop(15).Column(col =>
        {
            // Summary
            if (!string.IsNullOrEmpty(resume.Summary))
            {
                col.Item().Element(c => SectionHeader(c, "Professional Summary", "#2563eb"));
                col.Item().PaddingLeft(10).Text(resume.Summary).LineHeight(1.4f);
                col.Item().PaddingVertical(10);
            }

            // Experience
            if (resume.ExperienceDetails.Any())
            {
                col.Item().Element(c => SectionHeader(c, "Work Experience", "#2563eb"));
                foreach (var exp in resume.ExperienceDetails)
                {
                    col.Item().PaddingLeft(10).PaddingBottom(10).Column(expCol =>
                    {
                        expCol.Item().Row(r =>
                        {
                            r.RelativeItem().Text(exp.JobTitle).Bold().FontSize(11);
                            r.AutoItem().Text(FormatDateRange(exp.StartDate, exp.EndDate, exp.IsCurrent))
                                .FontSize(9).FontColor("#6b7280");
                        });
                        expCol.Item().Text($"{exp.CompanyName}{(string.IsNullOrEmpty(exp.Location) ? "" : $", {exp.Location}")}")
                            .FontColor("#374151");
                        if (!string.IsNullOrEmpty(exp.Description))
                        {
                            expCol.Item().PaddingTop(5).Text(exp.Description).FontSize(9).LineHeight(1.3f);
                        }
                    });
                }
                col.Item().PaddingVertical(5);
            }

            // Education
            if (resume.EducationDetails.Any())
            {
                col.Item().Element(c => SectionHeader(c, "Education", "#2563eb"));
                foreach (var edu in resume.EducationDetails)
                {
                    col.Item().PaddingLeft(10).PaddingBottom(8).Column(eduCol =>
                    {
                        eduCol.Item().Row(r =>
                        {
                            r.RelativeItem().Text($"{edu.Degree}{(string.IsNullOrEmpty(edu.FieldOfStudy) ? "" : $" in {edu.FieldOfStudy}")}")
                                .Bold().FontSize(11);
                            r.AutoItem().Text(FormatDateRange(edu.StartDate, edu.EndDate, edu.IsCurrentlyStudying))
                                .FontSize(9).FontColor("#6b7280");
                        });
                        eduCol.Item().Text(edu.Institution).FontColor("#374151");
                        if (edu.GPA.HasValue)
                        {
                            eduCol.Item().Text($"GPA: {edu.GPA:F2}").FontSize(9).FontColor("#6b7280");
                        }
                    });
                }
                col.Item().PaddingVertical(5);
            }

            // Skills
            if (resume.Skills.Any())
            {
                col.Item().Element(c => SectionHeader(c, "Skills", "#2563eb"));
                col.Item().PaddingLeft(10).Row(row =>
                {
                    row.RelativeItem().Column(skillCol =>
                    {
                        skillCol.Item().Row(skillRow =>
                        {
                            foreach (var skill in resume.Skills)
                            {
                                skillRow.AutoItem().Padding(2).Background("#e0e7ff").Padding(5)
                                    .Text(skill.SkillName).FontSize(9).FontColor("#3730a3");
                            }
                        });
                    });
                });
            }

            // Links
            if (!string.IsNullOrEmpty(resume.LinkedInUrl) || !string.IsNullOrEmpty(resume.GitHubUrl) || !string.IsNullOrEmpty(resume.PortfolioUrl))
            {
                col.Item().PaddingTop(15).Element(c => SectionHeader(c, "Links", "#2563eb"));
                col.Item().PaddingLeft(10).Column(linkCol =>
                {
                    if (!string.IsNullOrEmpty(resume.LinkedInUrl))
                        linkCol.Item().Text($"LinkedIn: {resume.LinkedInUrl}").FontSize(9).FontColor("#2563eb");
                    if (!string.IsNullOrEmpty(resume.GitHubUrl))
                        linkCol.Item().Text($"GitHub: {resume.GitHubUrl}").FontSize(9).FontColor("#2563eb");
                    if (!string.IsNullOrEmpty(resume.PortfolioUrl))
                        linkCol.Item().Text($"Portfolio: {resume.PortfolioUrl}").FontSize(9).FontColor("#2563eb");
                });
            }
        });
    }

    private Document CreateClassicTemplate(ResumeDto resume)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Times New Roman"));

                page.Header().Column(col =>
                {
                    col.Item().AlignCenter().Text(resume.FullName).Bold().FontSize(20);
                    col.Item().AlignCenter().Text(resume.Title).FontSize(12).Italic();
                    col.Item().AlignCenter().PaddingTop(5).Text(
                        $"{resume.Email} | {resume.Phone ?? ""} | {resume.Address ?? ""}".Trim(' ', '|'))
                        .FontSize(9);
                    col.Item().PaddingTop(10).LineHorizontal(1).LineColor(Colors.Black);
                });

                page.Content().PaddingTop(10).Column(col =>
                {
                    if (!string.IsNullOrEmpty(resume.Summary))
                    {
                        col.Item().Text("OBJECTIVE").Bold().FontSize(11);
                        col.Item().PaddingLeft(15).PaddingTop(5).Text(resume.Summary).LineHeight(1.4f);
                        col.Item().PaddingVertical(10);
                    }

                    if (resume.ExperienceDetails.Any())
                    {
                        col.Item().Text("EXPERIENCE").Bold().FontSize(11);
                        foreach (var exp in resume.ExperienceDetails)
                        {
                            col.Item().PaddingLeft(15).PaddingTop(8).Column(expCol =>
                            {
                                expCol.Item().Text($"{exp.JobTitle}, {exp.CompanyName}").Bold();
                                expCol.Item().Text(FormatDateRange(exp.StartDate, exp.EndDate, exp.IsCurrent))
                                    .Italic().FontSize(9);
                                if (!string.IsNullOrEmpty(exp.Description))
                                    expCol.Item().PaddingTop(3).Text(exp.Description).FontSize(9);
                            });
                        }
                        col.Item().PaddingVertical(10);
                    }

                    if (resume.EducationDetails.Any())
                    {
                        col.Item().Text("EDUCATION").Bold().FontSize(11);
                        foreach (var edu in resume.EducationDetails)
                        {
                            col.Item().PaddingLeft(15).PaddingTop(8).Column(eduCol =>
                            {
                                eduCol.Item().Text($"{edu.Degree}, {edu.Institution}").Bold();
                                eduCol.Item().Text(FormatDateRange(edu.StartDate, edu.EndDate, edu.IsCurrentlyStudying))
                                    .Italic().FontSize(9);
                            });
                        }
                        col.Item().PaddingVertical(10);
                    }

                    if (resume.Skills.Any())
                    {
                        col.Item().Text("SKILLS").Bold().FontSize(11);
                        col.Item().PaddingLeft(15).PaddingTop(5)
                            .Text(string.Join(" • ", resume.Skills.Select(s => s.SkillName)));
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                });
            });
        });
    }

    private Document CreateCorporateTemplate(ResumeDto resume)
    {
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(25);
                page.DefaultTextStyle(x => x.FontSize(9).FontFamily("Calibri"));

                page.Content().Row(row =>
                {
                    // Left sidebar
                    row.ConstantItem(180).Background("#1e3a5f").Padding(15).Column(col =>
                    {
                        col.Item().AlignCenter().Text(resume.FullName).Bold().FontSize(16).FontColor(Colors.White);
                        col.Item().AlignCenter().PaddingBottom(15).Text(resume.Title).FontSize(10).FontColor("#93c5fd");

                        col.Item().PaddingTop(10).Text("CONTACT").Bold().FontSize(10).FontColor("#fbbf24");
                        col.Item().LineHorizontal(1).LineColor("#fbbf24");
                        col.Item().PaddingTop(5).Text(resume.Email ?? "").FontSize(8).FontColor(Colors.White);
                        col.Item().Text(resume.Phone ?? "").FontSize(8).FontColor(Colors.White);
                        col.Item().Text(resume.Address ?? "").FontSize(8).FontColor(Colors.White);

                        if (resume.Skills.Any())
                        {
                            col.Item().PaddingTop(20).Text("SKILLS").Bold().FontSize(10).FontColor("#fbbf24");
                            col.Item().LineHorizontal(1).LineColor("#fbbf24");
                            foreach (var skill in resume.Skills)
                            {
                                col.Item().PaddingTop(5).Row(r =>
                                {
                                    r.RelativeItem().Text(skill.SkillName).FontSize(8).FontColor(Colors.White);
                                    r.AutoItem().Text(GetProficiencyStars(skill.ProficiencyLevel))
                                        .FontSize(8).FontColor("#fbbf24");
                                });
                            }
                        }

                        if (!string.IsNullOrEmpty(resume.LinkedInUrl) || !string.IsNullOrEmpty(resume.GitHubUrl))
                        {
                            col.Item().PaddingTop(20).Text("LINKS").Bold().FontSize(10).FontColor("#fbbf24");
                            col.Item().LineHorizontal(1).LineColor("#fbbf24");
                            if (!string.IsNullOrEmpty(resume.LinkedInUrl))
                                col.Item().PaddingTop(5).Text("LinkedIn").FontSize(8).FontColor("#93c5fd");
                            if (!string.IsNullOrEmpty(resume.GitHubUrl))
                                col.Item().PaddingTop(3).Text("GitHub").FontSize(8).FontColor("#93c5fd");
                        }
                    });

                    // Right content
                    row.RelativeItem().Padding(20).Column(col =>
                    {
                        if (!string.IsNullOrEmpty(resume.Summary))
                        {
                            col.Item().Text("PROFESSIONAL SUMMARY").Bold().FontSize(11).FontColor("#1e3a5f");
                            col.Item().PaddingBottom(3).LineHorizontal(2).LineColor("#1e3a5f");
                            col.Item().PaddingTop(5).Text(resume.Summary).LineHeight(1.3f);
                            col.Item().PaddingVertical(10);
                        }

                        if (resume.ExperienceDetails.Any())
                        {
                            col.Item().Text("WORK EXPERIENCE").Bold().FontSize(11).FontColor("#1e3a5f");
                            col.Item().PaddingBottom(3).LineHorizontal(2).LineColor("#1e3a5f");
                            foreach (var exp in resume.ExperienceDetails)
                            {
                                col.Item().PaddingTop(8).Column(expCol =>
                                {
                                    expCol.Item().Row(r =>
                                    {
                                        r.RelativeItem().Text(exp.JobTitle).Bold().FontSize(10);
                                        r.AutoItem().Text(FormatDateRange(exp.StartDate, exp.EndDate, exp.IsCurrent))
                                            .FontSize(8).FontColor("#6b7280");
                                    });
                                    expCol.Item().Text(exp.CompanyName).FontColor("#4b5563");
                                    if (!string.IsNullOrEmpty(exp.Description))
                                        expCol.Item().PaddingTop(3).Text(exp.Description).FontSize(8);
                                });
                            }
                            col.Item().PaddingVertical(10);
                        }

                        if (resume.EducationDetails.Any())
                        {
                            col.Item().Text("EDUCATION").Bold().FontSize(11).FontColor("#1e3a5f");
                            col.Item().PaddingBottom(3).LineHorizontal(2).LineColor("#1e3a5f");
                            foreach (var edu in resume.EducationDetails)
                            {
                                col.Item().PaddingTop(8).Column(eduCol =>
                                {
                                    eduCol.Item().Text($"{edu.Degree} - {edu.FieldOfStudy ?? ""}").Bold().FontSize(10);
                                    eduCol.Item().Text(edu.Institution).FontColor("#4b5563");
                                    eduCol.Item().Text(FormatDateRange(edu.StartDate, edu.EndDate, edu.IsCurrentlyStudying))
                                        .FontSize(8).FontColor("#6b7280");
                                });
                            }
                        }
                    });
                });
            });
        });
    }

    private void SectionHeader(IContainer container, string title, string color)
    {
        container.Column(col =>
        {
            col.Item().Text(title).Bold().FontSize(12).FontColor(color);
            col.Item().PaddingBottom(5).LineHorizontal(2).LineColor(color);
        });
    }

    private static string FormatDateRange(DateTime start, DateTime? end, bool isCurrent)
    {
        var endText = isCurrent ? "Present" : (end?.ToString("MMM yyyy") ?? "Present");
        return $"{start:MMM yyyy} - {endText}";
    }

    private static string GetProficiencyStars(int level)
    {
        return new string('★', level) + new string('☆', 5 - level);
    }

    private static ResumeDto MapToDto(Domain.Entities.Resume resume)
    {
        return new ResumeDto
        {
            ResumeId = resume.ResumeId,
            UserId = resume.UserId,
            Title = resume.Title,
            Summary = resume.Summary,
            FullName = resume.FullName,
            Email = resume.Email,
            Phone = resume.Phone,
            Address = resume.Address,
            LinkedInUrl = resume.LinkedInUrl,
            GitHubUrl = resume.GitHubUrl,
            PortfolioUrl = resume.PortfolioUrl,
            CreatedAt = resume.CreatedAt,
            UpdatedAt = resume.UpdatedAt,
            EducationDetails = resume.EducationDetails?.Select(e => new EducationDto
            {
                EducationId = e.EducationId,
                ResumeId = e.ResumeId,
                Institution = e.Institution,
                Degree = e.Degree,
                FieldOfStudy = e.FieldOfStudy,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                IsCurrentlyStudying = e.IsCurrentlyStudying,
                GPA = e.GPA,
                Description = e.Description
            }).ToList() ?? new List<EducationDto>(),
            ExperienceDetails = resume.ExperienceDetails?.Select(e => new ExperienceDto
            {
                ExperienceId = e.ExperienceId,
                ResumeId = e.ResumeId,
                CompanyName = e.CompanyName,
                JobTitle = e.JobTitle,
                Location = e.Location,
                Description = e.Description,
                StartDate = e.StartDate,
                EndDate = e.EndDate,
                IsCurrent = e.IsCurrent
            }).ToList() ?? new List<ExperienceDto>(),
            Skills = resume.Skills?.Select(s => new SkillDto
            {
                SkillId = s.SkillId,
                ResumeId = s.ResumeId,
                SkillName = s.SkillName,
                ProficiencyLevel = s.ProficiencyLevel,
                Category = s.Category
            }).ToList() ?? new List<SkillDto>()
        };
    }
}
