using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace ET.WebAPI.Api.Tests.Integration.Auth
{
    public class TestAuthorizationRequirement : AuthorizationHandler<TestAuthorizationRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TestAuthorizationRequirement requirement)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
}