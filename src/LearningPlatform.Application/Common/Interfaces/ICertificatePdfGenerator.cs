namespace LearningPlatform.Application.Common.Interfaces;

public record CertificatePdfData(
    string CertificateNumber,
    string StudentName,
    string CourseName,
    string InstructorName,
    DateTime IssuedAt);

public interface ICertificatePdfGenerator
{
    byte[] Generate(CertificatePdfData data);
}
