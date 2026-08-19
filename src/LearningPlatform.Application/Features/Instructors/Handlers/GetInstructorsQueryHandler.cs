using LearningPlatform.Application.Features.Instructors.DTOs;
using LearningPlatform.Application.Features.Instructors.Queries;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Domain.Enums;
using LearningPlatform.Shared.Pagination;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Application.Features.Instructors.Handlers;

public class GetInstructorsQueryHandler(UserManager<ApplicationUser> userManager)
    : IRequestHandler<GetInstructorsQuery, PaginatedResponse<InstructorDto>>
{
    public async Task<PaginatedResponse<InstructorDto>> Handle(
        GetInstructorsQuery request,
        CancellationToken cancellationToken)
    {
        var query = userManager.Users
            .Where(u => u.Role == UserRole.Instructor
                && u.TeacherProfile != null
                && u.TeacherProfile.VerificationStatus == TeacherVerificationStatus.Approved
                && u.TeacherProfile.AcceptingBookings)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(u => (u.FirstName + " " + u.LastName).Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new InstructorDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                ProfilePictureUrl = u.ProfilePictureUrl,
                Bio = u.Bio,
                CoursesCount = u.InstructorCourses.Count(c => c.Status == CourseStatus.Published),
                Courses = u.InstructorCourses
                    .Where(c => c.Status == CourseStatus.Published)
                    .OrderByDescending(c => c.CreatedAt)
                    .Select(c => new InstructorCourseDto
                    {
                        Id = c.Id,
                        Title = c.Title,
                        Slug = c.Slug,
                        ThumbnailUrl = c.ThumbnailUrl,
                        Category = c.Category,
                        Price = c.Price,
                        DiscountPrice = c.DiscountPrice,
                        Level = c.Level.ToString(),
                        DurationInMinutes = c.DurationInMinutes
                    })
                    .ToList()
            })
            .ToListAsync(cancellationToken);

        var paginated = new PaginatedList<InstructorDto>(items, totalCount, request.PageNumber, request.PageSize);
        return PaginatedResponse<InstructorDto>.Create(paginated);
    }
}
