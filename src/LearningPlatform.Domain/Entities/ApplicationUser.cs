using LearningPlatform.Domain.Common;
using LearningPlatform.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatform.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>, IAuditableEntity, ISoftDelete
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public UserRole Role { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }

    // As Instructor
    public ICollection<Course> InstructorCourses { get; set; } = [];
    public ICollection<LiveSession> LiveSessionsHosted { get; set; } = [];

    // As Student
    public ICollection<LiveSessionAttendance> LiveSessionAttendances { get; set; } = [];
    public ICollection<Enrollment> Enrollments { get; set; } = [];
    public ICollection<Payment> Payments { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<CourseProgress> CourseProgresses { get; set; } = [];
    public ICollection<QuizResult> QuizResults { get; set; } = [];
    public ICollection<StudentAnswer> StudentAnswers { get; set; } = [];
    public ICollection<ErrorBank> ErrorBankEntries { get; set; } = [];
    public ICollection<StudentChallenge> StudentChallenges { get; set; } = [];
    public ICollection<StudentReward> StudentRewards { get; set; } = [];
    public ICollection<StudentAvatar> StudentAvatars { get; set; } = [];
    public ICollection<PointsTransaction> PointsTransactions { get; set; } = [];
    public GamificationProfile? GamificationProfile { get; set; }
    public ICollection<Certificate> Certificates { get; set; } = [];

    // As Parent
    public ICollection<ParentStudent> ChildLinks { get; set; } = [];
    public ICollection<ParentTest> ParentTestsAssigned { get; set; } = [];

    // As Child (of a Parent)
    public ICollection<ParentStudent> ParentLinks { get; set; } = [];
    public ICollection<ParentTest> ParentTestsReceived { get; set; } = [];
    public ICollection<ParentTestResult> ParentTestResults { get; set; } = [];

    // Common
    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<UserActivity> Activities { get; set; } = [];
    public ICollection<UserDevice> Devices { get; set; } = [];
    public UserSettings? Settings { get; set; }
}
