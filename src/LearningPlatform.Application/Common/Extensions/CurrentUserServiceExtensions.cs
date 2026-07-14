using LearningPlatform.Application.Common.Interfaces;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Exceptions;

namespace LearningPlatform.Application.Common.Extensions;

public static class CurrentUserServiceExtensions
{
    /// <summary>
    /// Throws unless the current user is an Admin or the owning instructor of the course.
    /// </summary>
    public static void EnsureCanManageCourse(this ICurrentUserService currentUser, Course course)
    {
        if (currentUser.IsInRole(nameof(UserRole.Admin)))
            return;

        if (currentUser.UserId == course.InstructorId)
            return;

        throw new ForbiddenException("You do not have permission to manage this course.");
    }
}
