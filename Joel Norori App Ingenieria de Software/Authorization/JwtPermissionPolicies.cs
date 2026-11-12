using Joel_Norori_App_Ingenieria_de_Software.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Joel_Norori_App_Ingenieria_de_Software.Authorization
{
    public class JwtPermissionPolicies
    {
        public class PermissionRequirement(string permission) : IAuthorizationRequirement
        {
            public string Permission { get; } = permission;


        }


        public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
        {
            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
            {

                if (context.User.HasPermiso(requirement.Permission))
                {
                    context.Succeed(requirement);
                }
                return Task.CompletedTask;
            }
        }

        public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
        {
            public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
            {
                var policy = await base.GetPolicyAsync(policyName);
                if (policy is not null)
                    return policy;
                return new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(policyName))
                    .Build();
            }
        }

    }
}