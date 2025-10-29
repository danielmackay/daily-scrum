// using ModelContextProtocol.Server;
// using System.ComponentModel;
// using Infrastructure.Identity;
//
// namespace Mcp.Features.Identity;
//
// [McpServerToolType]
// public class SetAccessTokenTool
// {
//     private readonly ICurrentUserService _currentUserService;
//
//     public SetAccessTokenTool(ICurrentUserService currentUserService)
//     {
//         _currentUserService = currentUserService;
//     }
//
//     [McpServerTool, Description("Set the access token for authentication")]
//     public string SetAccessToken([Description("The access token to set")] string accessToken)
//     {
//         _currentUserService.UpdateAccessToken(accessToken);
//         return "Access token updated successfully";
//     }
// }
