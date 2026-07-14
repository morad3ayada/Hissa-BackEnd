using LearningPlatform.Application.Features.Quizzes.DTOs;
using LearningPlatform.Domain.Entities;

namespace LearningPlatform.Application.Features.Quizzes.Mappings;

/// <summary>
/// Builds QuizDto/QuestionDto graphs with the answer key (IsCorrect/Explanation) either
/// included (instructor/admin management views) or stripped (student attempt views).
/// Manual on purpose: the visibility toggle isn't expressible as a static AutoMapper profile.
/// </summary>
public static class QuizDtoBuilder
{
    public static QuizDto Build(Quiz quiz, bool includeAnswerKey)
    {
        var questions = quiz.Questions
            .OrderBy(q => q.Order)
            .Select(q => BuildQuestion(q, includeAnswerKey))
            .ToList();

        return new QuizDto
        {
            Id = quiz.Id,
            Title = quiz.Title,
            Scope = quiz.Scope.ToString(),
            CourseId = quiz.CourseId,
            LessonId = quiz.LessonId,
            IsFinalExam = quiz.IsFinalExam,
            TimeLimitInMinutes = quiz.TimeLimitInMinutes,
            PassingScore = quiz.PassingScore,
            MaxAttempts = quiz.MaxAttempts,
            IsPublished = quiz.IsPublished,
            TotalPoints = quiz.Questions.Sum(q => q.Points),
            Questions = questions
        };
    }

    public static QuestionDto BuildQuestion(Question question, bool includeAnswerKey) => new()
    {
        Id = question.Id,
        Text = question.Text,
        Type = question.Type.ToString(),
        Order = question.Order,
        Points = question.Points,
        Explanation = includeAnswerKey ? question.Explanation : null,
        Answers = question.Answers
            .OrderBy(a => a.Order)
            .Select(a => new AnswerDto
            {
                Id = a.Id,
                Text = a.Text,
                Order = a.Order,
                IsCorrect = includeAnswerKey ? a.IsCorrect : null
            })
            .ToList()
    };

    public static QuizResultDto BuildResult(QuizResult result, string quizTitle)
    {
        var answers = result.StudentAnswers
            .OrderBy(a => a.Question.Order)
            .Select(a =>
            {
                var correctAnswer = a.Question.Answers.FirstOrDefault(o => o.IsCorrect);

                return new ReviewAnswerDto
                {
                    QuestionId = a.QuestionId,
                    QuestionText = a.Question.Text,
                    StudentSelectedAnswerId = a.SelectedAnswerId,
                    StudentAnswerText = a.SelectedAnswer?.Text,
                    CorrectAnswerId = correctAnswer?.Id,
                    CorrectAnswerText = correctAnswer?.Text,
                    IsCorrect = a.IsCorrect,
                    Explanation = a.Question.Explanation
                };
            })
            .ToList();

        return new QuizResultDto
        {
            Id = result.Id,
            QuizId = result.QuizId,
            QuizTitle = quizTitle,
            Score = result.Score,
            TotalQuestions = result.TotalQuestions,
            CorrectAnswers = result.CorrectAnswers,
            IsPassed = result.IsPassed,
            AttemptNumber = result.AttemptNumber,
            StartedAt = result.StartedAt,
            CompletedAt = result.CompletedAt,
            Answers = answers
        };
    }

    public static QuizSummaryDto BuildSummary(Quiz quiz) => new()
    {
        Id = quiz.Id,
        Title = quiz.Title,
        IsFinalExam = quiz.IsFinalExam,
        TimeLimitInMinutes = quiz.TimeLimitInMinutes,
        PassingScore = quiz.PassingScore,
        MaxAttempts = quiz.MaxAttempts,
        IsPublished = quiz.IsPublished,
        QuestionsCount = quiz.Questions.Count
    };
}
