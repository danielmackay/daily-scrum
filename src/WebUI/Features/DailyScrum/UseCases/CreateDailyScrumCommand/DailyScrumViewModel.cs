﻿using WebUI.Common.ViewModels;
using WebUI.Features.DailyScrum.Domain;

namespace WebUI.Features.DailyScrum.UseCases.CreateDailyScrumCommand;

// public class DailyScrumViewModel
// {
//     public required UserSummary UserSummary { get; init; }
//     public List<ProjectViewModel> YesterdaysProjects { get; init; } = [];
//     public List<ProjectViewModel> TodaysProjects { get; init; } = [];
//
//     public EmailViewModel Email { get; init; } = new();
// }
//
// public class UserSummaryViewModel
// {
//     public required string DaysUntilNextBooking { get; init; }
//     public int InboxCount { get; init; }
//     public required string TrelloBoardUrl { get; init; }
// }
//
// public class EmailViewModel
// {
//     public string Subject { get; init; } = string.Empty;
//     public EmailParticipantViewModel To { get; init; } = new();
//     public List<EmailParticipantViewModel> Cc { get; init; } = [];
// }
//
// public class EmailParticipantViewModel
// {
//     public string? Name { get; init; }
//     public string? Email { get; init; }
// }
//
// public class TaskViewModel
// {
//     public required string Name { get; init; }
// }
