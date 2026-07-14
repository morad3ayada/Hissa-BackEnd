using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Application.Features.Certificates.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;
using MediatR;

namespace LearningPlatform.Application.Features.Certificates.Handlers;

public class DownloadCertificateQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser, IFileStorageService fileStorageService)
    : IRequestHandler<DownloadCertificateQuery, CertificateFileResult>
{
    public async Task<CertificateFileResult> Handle(DownloadCertificateQuery request, CancellationToken cancellationToken)
    {
        var certificate = await unitOfWork.Repository<Certificate>().GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Certificate), request.Id);

        var isOwner = currentUser.UserId == certificate.StudentId;
        var isAdmin = currentUser.IsInRole(nameof(UserRole.Admin));

        if (!isOwner && !isAdmin)
            throw new ForbiddenException("You do not have permission to download this certificate.");

        if (string.IsNullOrWhiteSpace(certificate.CertificateUrl))
            throw new NotFoundException("This certificate has no generated PDF file.");

        var stream = await fileStorageService.DownloadAsync(certificate.CertificateUrl, cancellationToken);

        return new CertificateFileResult(stream, "application/pdf", $"{certificate.CertificateNumber}.pdf");
    }
}
