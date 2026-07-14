namespace LearningPlatform.Domain.Common;

public interface ISoftDelete
{
    bool IsDeleted { get; set; }
}
