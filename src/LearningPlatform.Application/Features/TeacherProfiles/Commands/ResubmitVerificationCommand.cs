using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.TeacherProfiles.Commands;

public record ResubmitVerificationCommand : IRequest<ApiResponse>;
