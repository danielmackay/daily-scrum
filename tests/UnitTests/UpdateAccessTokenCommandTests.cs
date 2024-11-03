using FluentAssertions;
using WebUI.Common.Identity;
using WebUI.Features.Identity;

namespace UnitTests;

public class UpdateAccessTokenCommandTests
{
    [Fact]
    public void Handle_GivenValidAccessToken_ShouldSetFirstNameAndLastName()
    {
        var currentUserService = new CurrentUserService();
        var sut = new UpdateAccessTokenCommandHandler(currentUserService);
        var token = "eyJ0eXAiOiJKV1QiLCJub25jZSI6InNyLUUxaUJNVEdRTF92a3Jvbl9PLUw5dGczMjlRZW9IZXRlU252VENIMVUiLCJhbGciOiJSUzI1NiIsIng1dCI6IkwxS2ZLRklfam5YYndXYzIyeFp4dzFzVUhIMCIsImtpZCI6IkwxS2ZLRklfam5YYndXYzIyeFp4dzFzVUhIMCJ9.eyJhdWQiOiIwMDAwMDAwMy0wMDAwLTAwMDAtYzAwMC0wMDAwMDAwMDAwMDAiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC9hYzJmN2MzNC1iOTM1LTQ4ZTktYWJkYy0xMWU1ZDRmY2IyYjAvIiwiaWF0IjoxNzE0ODYzMDkyLCJuYmYiOjE3MTQ4NjMwOTIsImV4cCI6MTcxNDk0OTc5MywiYWNjdCI6MCwiYWNyIjoiMSIsImFpbyI6IkFiUUFTLzhXQUFBQVpFZFlLS29Ua2xFYXBReTNBYUV3S2lrSHpSbHBVek4zaW9KaEw2enk5VCtwOGxJd3pWb3VteTM2S3BzeUt2dlA0TkMxUEduVHBBZjBXVTRiMUhKMWpkQU5tbi9QU2lSU1ErNzdDZjdBRFlXenFBeWloVVdCQ2dqYzk1cmZzV2JnVjVnd1ltUTNvK0tNbElGRlpXQzZaWDg5cUZLU1R4T3NSb2lSV1BmM3RaKzVZY2Y0bG5OdzdJU3c3QTY3SlNpdEpzOERxaUo2clA4eXFSSTZoMlQrSUxaL2FXd2RnQmhidGpwMmNqc0o2OXM9IiwiYW1yIjpbIm1mYSJdLCJhcHBfZGlzcGxheW5hbWUiOiJHcmFwaCBFeHBsb3JlciIsImFwcGlkIjoiZGU4YmM4YjUtZDlmOS00OGIxLWE4YWQtYjc0OGRhNzI1MDY0IiwiYXBwaWRhY3IiOiIwIiwiY2Fwb2xpZHNfbGF0ZWJpbmQiOlsiNmIxOWE1MmUtMGQ4ZC00NzNkLWI1YjQtZDIxZjFmMmUyMTk1Il0sImZhbWlseV9uYW1lIjoiTWFja2F5IiwiZ2l2ZW5fbmFtZSI6IkRhbmllbCIsImlkdHlwIjoidXNlciIsImlwYWRkciI6IjEuMTQ2LjE4LjE3IiwibmFtZSI6IkRhbmllbCBNYWNrYXkgW1NTV10iLCJvaWQiOiI0ODJjNDg2Yi1iMzNjLTQxZDQtOTlkYy0xODE0YTcyMTE2MzgiLCJvbnByZW1fc2lkIjoiUy0xLTUtMjEtNTQxOTA5NDA5LTM2Nzc2NTEwMjQtMTQ1Mzg4OTYwOS01NzY2NiIsInBsYXRmIjoiNSIsInB1aWQiOiIxMDAzMjAwMjJCMzM2NkNCIiwicmgiOiIwLkFXY0FOSHd2ckRXNTZVaXIzQkhsMVB5eXNBTUFBQUFBQUFBQXdBQUFBQUFBQUFCbkFCVS4iLCJzY3AiOiJBY2Nlc3NSZXZpZXcuUmVhZC5BbGwgQWNjZXNzUmV2aWV3LlJlYWRXcml0ZS5BbGwgQWdyZWVtZW50LlJlYWQuQWxsIEFncmVlbWVudC5SZWFkV3JpdGUuQWxsIEFncmVlbWVudEFjY2VwdGFuY2UuUmVhZCBBZ3JlZW1lbnRBY2NlcHRhbmNlLlJlYWQuQWxsIEFuYWx5dGljcy5SZWFkIEFwcENhdGFsb2cuUmVhZFdyaXRlLkFsbCBBdWRpdExvZy5SZWFkLkFsbCBCb29raW5ncy5NYW5hZ2UuQWxsIEJvb2tpbmdzLlJlYWQuQWxsIEJvb2tpbmdzLlJlYWRXcml0ZS5BbGwgQm9va2luZ3NBcHBvaW50bWVudC5SZWFkV3JpdGUuQWxsIENhbGVuZGFycy5SZWFkIENhbGVuZGFycy5SZWFkLlNoYXJlZCBDYWxlbmRhcnMuUmVhZFdyaXRlIENhbGVuZGFycy5SZWFkV3JpdGUuU2hhcmVkIENoYXQuUmVhZCBDaGF0LlJlYWRXcml0ZSBDb250YWN0cy5SZWFkIENvbnRhY3RzLlJlYWQuU2hhcmVkIENvbnRhY3RzLlJlYWRXcml0ZSBDb250YWN0cy5SZWFkV3JpdGUuU2hhcmVkIERldmljZS5Db21tYW5kIERldmljZS5SZWFkIERldmljZU1hbmFnZW1lbnRBcHBzLlJlYWQuQWxsIERldmljZU1hbmFnZW1lbnRBcHBzLlJlYWRXcml0ZS5BbGwgRGV2aWNlTWFuYWdlbWVudENvbmZpZ3VyYXRpb24uUmVhZC5BbGwgRGV2aWNlTWFuYWdlbWVudENvbmZpZ3VyYXRpb24uUmVhZFdyaXRlLkFsbCBEZXZpY2VNYW5hZ2VtZW50TWFuYWdlZERldmljZXMuUHJpdmlsZWdlZE9wZXJhdGlvbnMuQWxsIERldmljZU1hbmFnZW1lbnRNYW5hZ2VkRGV2aWNlcy5SZWFkLkFsbCBEZXZpY2VNYW5hZ2VtZW50TWFuYWdlZERldmljZXMuUmVhZFdyaXRlLkFsbCBEZXZpY2VNYW5hZ2VtZW50UkJBQy5SZWFkLkFsbCBEZXZpY2VNYW5hZ2VtZW50UkJBQy5SZWFkV3JpdGUuQWxsIERldmljZU1hbmFnZW1lbnRTZXJ2aWNlQ29uZmlnLlJlYWQuQWxsIERldmljZU1hbmFnZW1lbnRTZXJ2aWNlQ29uZmlnLlJlYWRXcml0ZS5BbGwgRGlyZWN0b3J5LkFjY2Vzc0FzVXNlci5BbGwgRGlyZWN0b3J5LlJlYWQuQWxsIERpcmVjdG9yeS5SZWFkV3JpdGUuQWxsIEVkdUFkbWluaXN0cmF0aW9uLlJlYWQgRWR1QWRtaW5pc3RyYXRpb24uUmVhZFdyaXRlIEVkdUFzc2lnbm1lbnRzLlJlYWQgRWR1QXNzaWdubWVudHMuUmVhZEJhc2ljIEVkdUFzc2lnbm1lbnRzLlJlYWRXcml0ZSBFZHVBc3NpZ25tZW50cy5SZWFkV3JpdGVCYXNpYyBFZHVSb3N0ZXIuUmVhZEJhc2ljIEZpbGVzLlJlYWQgRmlsZXMuUmVhZC5BbGwgRmlsZXMuUmVhZC5TZWxlY3RlZCBGaWxlcy5SZWFkV3JpdGUgRmlsZXMuUmVhZFdyaXRlLkFsbCBGaWxlcy5SZWFkV3JpdGUuQXBwRm9sZGVyIEZpbGVzLlJlYWRXcml0ZS5TZWxlY3RlZCBGaW5hbmNpYWxzLlJlYWRXcml0ZS5BbGwgR3JvdXAuUmVhZC5BbGwgR3JvdXAuUmVhZFdyaXRlLkFsbCBJZGVudGl0eVByb3ZpZGVyLlJlYWQuQWxsIElkZW50aXR5UHJvdmlkZXIuUmVhZFdyaXRlLkFsbCBJZGVudGl0eVJpc2tFdmVudC5SZWFkLkFsbCBJZGVudGl0eVJpc2tFdmVudC5SZWFkV3JpdGUuQWxsIElkZW50aXR5Umlza3lVc2VyLlJlYWQuQWxsIElkZW50aXR5Umlza3lVc2VyLlJlYWRXcml0ZS5BbGwgTWFpbC5SZWFkIE1haWwuUmVhZC5TaGFyZWQgTWFpbC5SZWFkV3JpdGUgTWFpbC5SZWFkV3JpdGUuU2hhcmVkIE1haWwuU2VuZCBNYWlsLlNlbmQuU2hhcmVkIE1haWxib3hTZXR0aW5ncy5SZWFkV3JpdGUgTm90ZXMuQ3JlYXRlIE5vdGVzLlJlYWQgTm90ZXMuUmVhZC5BbGwgTm90ZXMuUmVhZFdyaXRlIE5vdGVzLlJlYWRXcml0ZS5BbGwgTm90aWZpY2F0aW9ucy5SZWFkV3JpdGUuQ3JlYXRlZEJ5QXBwIG9wZW5pZCBQZW9wbGUuUmVhZCBQZW9wbGUuUmVhZC5BbGwgUHJpdmlsZWdlZEFjY2Vzcy5SZWFkV3JpdGUuQXp1cmVBRCBQcml2aWxlZ2VkQWNjZXNzLlJlYWRXcml0ZS5BenVyZVJlc291cmNlcyBwcm9maWxlIFByb2dyYW1Db250cm9sLlJlYWQuQWxsIFByb2dyYW1Db250cm9sLlJlYWRXcml0ZS5BbGwgUmVwb3J0cy5SZWFkLkFsbCBTZWN1cml0eUV2ZW50cy5SZWFkLkFsbCBTZWN1cml0eUV2ZW50cy5SZWFkV3JpdGUuQWxsIFNpdGVzLkZ1bGxDb250cm9sLkFsbCBTaXRlcy5NYW5hZ2UuQWxsIFNpdGVzLlJlYWQuQWxsIFNpdGVzLlJlYWRXcml0ZS5BbGwgVGFza3MuUmVhZCBUYXNrcy5SZWFkLlNoYXJlZCBUYXNrcy5SZWFkV3JpdGUgVGFza3MuUmVhZFdyaXRlLlNoYXJlZCBVc2VyLlJlYWQgVXNlci5SZWFkLkFsbCBVc2VyLlJlYWRCYXNpYy5BbGwgVXNlci5SZWFkV3JpdGUgVXNlci5SZWFkV3JpdGUuQWxsIFVzZXJBY3Rpdml0eS5SZWFkV3JpdGUuQ3JlYXRlZEJ5QXBwIGVtYWlsIiwic2lnbmluX3N0YXRlIjpbImttc2kiXSwic3ViIjoiVktSWllQVEk2REo1RTk1SS1XX3ZNNFBhNHEtOFhPWmZtUGIwcXN1QkptYyIsInRlbmFudF9yZWdpb25fc2NvcGUiOiJPQyIsInRpZCI6ImFjMmY3YzM0LWI5MzUtNDhlOS1hYmRjLTExZTVkNGZjYjJiMCIsInVuaXF1ZV9uYW1lIjoiRGFuaWVsTWFja2F5QHNzdy5jb20uYXUiLCJ1cG4iOiJEYW5pZWxNYWNrYXlAc3N3LmNvbS5hdSIsInV0aSI6ImxiQmx6bmtMTGtLMG11cjk5X1lQQUEiLCJ2ZXIiOiIxLjAiLCJ3aWRzIjpbImI3OWZiZjRkLTNlZjktNDY4OS04MTQzLTc2YjE5NGU4NTUwOSJdLCJ4bXNfY2MiOlsiQ1AxIl0sInhtc19zc20iOiIxIiwieG1zX3N0Ijp7InN1YiI6IkJfVDVqWFZrajV2bVRfdDVHY2xGQnV5RHhiQmYxZ09SclZDMUxNcEZkYmcifSwieG1zX3RjZHQiOjE0MzkyNTM5MzF9.N-l1IPLCnK2UGXdwW2mE_dEo3_0v5xwMbsy7JhIY7CHfRo7BvKx-Bjlq4pG5hPKb_l7bGsKmX5z2TwLlQbF8RWML2O5oKapZiknQmE46murhFZY-FJpvU8znnqhRE4ch1J5gdOnasANA16hzCnjZDQ5Bcc_KGWZ4aSaAm7R5raI0ywE_I4WlEsGCddqLH4_qFp7ol-xvG1BrU4J3k1zoJFTfFyLivhVos0UZpyzITS2YG1NsNlvsN3Mxy1nf7TlfkwdkXEZ8koQAdTz848SE-s8v-c15PfFQguWSkqE2BXLdhsO3dJBLjEHI-C0lLahGClP_kT968J4DsuiKG-xcvg";

        sut.Handle(new UpdateAccessTokenCommand(token), CancellationToken.None);

        currentUserService.FirstName.Should().Be("Daniel");
        currentUserService.LastName.Should().Be("Mackay");
    }
}