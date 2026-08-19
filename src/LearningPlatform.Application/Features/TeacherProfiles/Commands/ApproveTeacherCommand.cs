using LearningPlatform.Shared.Wrappers;
using MediatR;

namespace LearningPlatform.Application.Features.TeacherProfiles.Commands;

public record ApproveTeacherCommand(Guid TeacherProfileId) : IRequest<ApiResponse>;
