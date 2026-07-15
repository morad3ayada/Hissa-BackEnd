using System.Reflection;
using LearningPlatform.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.Persistence.Context;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    // Course domain
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseSection> CourseSections => Set<CourseSection>();
    public DbSet<Lesson> Lessons => Set<Lesson>();
    public DbSet<Enrollment> Enrollments => Set<Enrollment>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<CourseProgress> CourseProgresses => Set<CourseProgress>();

    // Assessment
    public DbSet<Quiz> Quizzes => Set<Quiz>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Answer> Answers => Set<Answer>();
    public DbSet<QuizResult> QuizResults => Set<QuizResult>();
    public DbSet<StudentAnswer> StudentAnswers => Set<StudentAnswer>();
    public DbSet<ErrorBank> ErrorBanks => Set<ErrorBank>();

    // Gamification
    public DbSet<Challenge> Challenges => Set<Challenge>();
    public DbSet<StudentChallenge> StudentChallenges => Set<StudentChallenge>();
    public DbSet<Reward> Rewards => Set<Reward>();
    public DbSet<StudentReward> StudentRewards => Set<StudentReward>();
    public DbSet<AvatarItem> AvatarItems => Set<AvatarItem>();
    public DbSet<StudentAvatar> StudentAvatars => Set<StudentAvatar>();
    public DbSet<GamificationProfile> GamificationProfiles => Set<GamificationProfile>();
    public DbSet<GamificationLevel> GamificationLevels => Set<GamificationLevel>();
    public DbSet<PointsTransaction> PointsTransactions => Set<PointsTransaction>();

    // Live sessions
    public DbSet<LiveSession> LiveSessions => Set<LiveSession>();
    public DbSet<LiveSessionAttendance> LiveSessionAttendances => Set<LiveSessionAttendance>();

    // Parent system
    public DbSet<ParentStudent> ParentStudents => Set<ParentStudent>();
    public DbSet<ParentTest> ParentTests => Set<ParentTest>();
    public DbSet<ParentTestQuestion> ParentTestQuestions => Set<ParentTestQuestion>();
    public DbSet<ParentTestResult> ParentTestResults => Set<ParentTestResult>();

    // Reports
    public DbSet<StudentReport> StudentReports => Set<StudentReport>();

    // Platform / account meta
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();
    public DbSet<UserActivity> UserActivities => Set<UserActivity>();
    public DbSet<UserDevice> UserDevices => Set<UserDevice>();
    public DbSet<Certificate> Certificates => Set<Certificate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
