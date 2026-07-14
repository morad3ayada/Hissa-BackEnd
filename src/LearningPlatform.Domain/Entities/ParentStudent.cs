using LearningPlatform.Domain.Common;

namespace LearningPlatform.Domain.Entities;

// Link entity between a Parent and a Student account. Uses a surrogate Id (BaseEntity)
// like every other entity so it works with the generic IRepository<T>/UnitOfWork; a unique
// index on (ParentId, StudentId) enforces the same "one link per pair" guarantee a composite
// key would have.
public class ParentStudent : BaseEntity
{
    public Guid ParentId { get; set; }
    public ApplicationUser Parent { get; set; } = null!;

    public Guid StudentId { get; set; }
    public ApplicationUser Student { get; set; } = null!;

    public string? RelationshipType { get; set; }
    public DateTime LinkedAt { get; set; }
}
