// using FluentAssertions;
// using Microsoft.AspNetCore.Http;
// using NSubstitute;
// using WebUI.Features.DailyScrum.Domain;
// using WebUI.Features.DailyScrum.UseCases.CreateDailyScrumCommand;
//
// namespace UnitTests;
//
// public class SessionDailyScrumRepositoryTests
// {
//     [Fact]
//     public void Save_StoresDailyScrumInSession()
//     {
//         // Arrange
//         var session = Substitute.For<ISession>();
//         var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
//         httpContextAccessor.HttpContext.Returns(new DefaultHttpContext { Session = session });
//         var repository = new SessionDailyScrumRepository(httpContextAccessor);
//         var dailyScrum = CreateDailyScrum();
//
//         // Act
//         repository.Save(dailyScrum);
//
//         // Assert
//         session.Received(1).SetString("DailyScrum", Arg.Any<string>());
//     }
//
//
//     [Fact]
//     public void Get_ReturnsDailyScrumFromSession()
//     {
//         // Arrange
//         var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
//         var httpContext = Substitute.For<HttpContext>();
//         var session = Substitute.For<ISession>();
//
//         httpContextAccessor.HttpContext.Returns(httpContext);
//         httpContext.Session.Returns(session);
//
//         var repository = new SessionDailyScrumRepository(httpContextAccessor);
//         var dailyScrumJson = Newtonsoft.Json.JsonConvert.SerializeObject(CreateDailyScrum());
//         // Example: Mock a method on ISession
//         session.TryGetValue("DailyScrum", out Arg.Any<byte[]>())
//             .Returns(x =>
//             {
//                 x[1] = new byte[] { 1, 2, 3 }; // Set the output parameter
//                 return true; // Return value for TryGetValue
//             });
//         session.TryGetValue("DailyScrum").ReturnsForAnyArgs(dailyScrumJson);
//
//         // Act
//         var result = repository.Get();
//
//         // Assert
//         result.Should().NotBeNull();
//
//     }
//
//     [Fact]
//     public void Get_ReturnsNullWhenSessionIsEmpty()
//     {
//         // Arrange
//         var session = Substitute.For<ISession>();
//         var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
//         httpContextAccessor.HttpContext.Returns(new DefaultHttpContext { Session = session });
//         var repository = new SessionDailyScrumRepository(httpContextAccessor);
//         session.GetString("DailyScrum").Returns((string)null);
//
//         // Act
//         var result = repository.Get();
//
//         // Assert
//         result.Should().BeNull();
//     }
//
//     private DailyScrum CreateDailyScrum()
//     {
//         var userSummary = new UserSummary(1, 2);
//         var yesterday = new List<Project>();
//         var today = new List<Project>();
//         var emailSummary = new EmailSummary("foo");
//         return new DailyScrum(userSummary, yesterday, today, emailSummary);
//     }
// }
