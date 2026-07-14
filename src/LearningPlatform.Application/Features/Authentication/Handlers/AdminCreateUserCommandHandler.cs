using AutoMapper;
using LearningPlatform.Application.Features.Authentication.Commands;
using LearningPlatform.Application.Features.Authentication.DTOs;
using LearningPlatform.Domain.Entities;
using LearningPlatform.Shared.Exceptions;
using LearningPlatform.Shared.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Application.Features.Authentication.Handlers;

public class AdminCreateUserCommandHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
    : IRequestHandler<AdminCreateUserCommand, ApiResponse<UserDto>>
{
    public async Task<ApiResponse<UserDto>> Handle(AdminCreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not null)
            throw new ConflictException("An account with this email already exists.");

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Role = request.Role,
            IsActive = true,
            // Admin-created accounts are pre-trusted; the recipient does not need to
            // click a confirmation link before signing in.
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            throw new BadRequestException(string.Join(" ", result.Errors.Select(e => e.Description)));

        await userManager.AddToRoleAsync(user, request.Role.ToString());

        var response = mapper.Map<UserDto>(user);

        return ApiResponse<UserDto>.Success(response, $"{request.Role} account created successfully.");
    }
}
