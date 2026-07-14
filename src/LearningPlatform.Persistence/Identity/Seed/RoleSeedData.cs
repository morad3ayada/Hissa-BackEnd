using LearningPlatform.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Persistence.Identity.Seed;

/// <summary>
/// Fixed, deterministic role rows for EF Core migration HasData seeding.
/// Ids must never change once a migration referencing them has shipped.
/// </summary>
public static class RoleSeedData
{
    public static readonly Guid AdminRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid InstructorRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    public static readonly Guid StudentRoleId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    public static readonly Guid ParentRoleId = Guid.Parse("44444444-4444-4444-4444-444444444444");

    public static IdentityRole<Guid>[] Roles =>
    [
        Build(AdminRoleId, UserRole.Admin),
        Build(InstructorRoleId, UserRole.Instructor),
        Build(StudentRoleId, UserRole.Student),
        Build(ParentRoleId, UserRole.Parent)
    ];

    private static IdentityRole<Guid> Build(Guid id, UserRole role) => new()
    {
        Id = id,
        Name = role.ToString(),
        NormalizedName = role.ToString().ToUpperInvariant(),
        ConcurrencyStamp = id.ToString()
    };
}
