using System;

namespace Joel_Norori_App_Ingenieria_de_Software.Authorization
{
    public class JwtPermissionPolicies
    {
        public class PermisoRequirement : IAuthorizationRequirement
        {
            public string Permiso { get; } = Permiso;
        }

        public class PermisoRequirementHandler : AuthorizationHandler<PermisoRequirement>
        {
            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermisoRequirement requirement)
            {
                if (context.User.HasClaim(c => c.Type == "Permiso" && c.Value == requirement.Permiso))
                {
                    context.Succeed(requirement);
                }
                return Task.CompletedTask;
            }
        }
    }
    public class PermisoPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
    {
       public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
       {
           var policy = await base.GetPolicyAsync(policyName);
           if (policy is not null)
                return policy;
            return new AuthorizationPolicyBuilder()
                 .AddRequirements(new JwtPermissionPolicies.PermisoRequirement(policyName))
                 .Build();

        }
    }
}