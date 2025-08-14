using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PradeepTech.Auth.API.Attributes
{
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        private readonly string[] _claims;

        public CustomAuthorizeAttribute(string roles = "", string claims = "")
        {
            _roles = string.IsNullOrEmpty(roles) ? new string[0] : roles.Split(',');
            _claims = string.IsNullOrEmpty(claims) ? new string[0] : claims.Split(',');
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            // Check roles
            if (_roles.Any() && !_roles.Any(role => user.IsInRole(role)))
            {
                context.Result = new ForbidResult();
                return;
            }

            // Check claims
            if (_claims.Any())
            {
                var userClaims = user.Claims.Select(c => c.Value).ToList();
                if (!_claims.Any(claim => userClaims.Contains(claim)))
                {
                    context.Result = new ForbidResult();
                    return;
                }
            }
        }
    }
}