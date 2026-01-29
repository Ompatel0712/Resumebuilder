using ResumeBuilder.Application.DTOs;

namespace ResumeBuilder.Application.Interfaces;

public interface IPdfGeneratorService
{
    Task<byte[]> GenerateResumePdfAsync(int resumeId, string template = "Modern");
    Task<byte[]> GenerateResumePdfAsync(ResumeDto resume, string template = "Modern");
    IEnumerable<string> GetAvailableTemplates();
}
