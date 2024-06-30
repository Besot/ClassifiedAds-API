using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AlutaMartAPI.Utilities;

public class AllowAccessAttribute : TypeFilterAttribute
{
    public AllowAccessAttribute(Roles role) : base(typeof(AllowAccessFilter))
    {
        Arguments = [role];
    }

    public class AllowAccessFilter(Roles role) : IAsyncAuthorizationFilter
    {
        readonly Roles _role = role;

        public virtual Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == JwtRegisteredClaimNames.Aud && c.Value == _role.ToString());
            if (!hasClaim) context.Result = new ObjectResult("Forbidden - Unauthorized Access") { StatusCode = 403};
            return Task.CompletedTask;
        }
    }
}

public class BlockAccessAttribute : TypeFilterAttribute
{
    public BlockAccessAttribute(Roles role) : base(typeof(BlockAccessFilter))
    {
        Arguments = [role];
    }

    public class BlockAccessFilter(Roles role) : IAsyncAuthorizationFilter
    {
        readonly Roles _role = role;

        public virtual Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == JwtRegisteredClaimNames.Aud && c.Value == _role.ToString());
            if (hasClaim) context.Result = new ObjectResult("Forbidden - Unauthorized Access") { StatusCode = 403};
            return Task.CompletedTask;
        }
    }
}

public class AccessControlAttribute :  TypeFilterAttribute
{
    public AccessControlAttribute(Roles[] roles, AccessType accessType) : base(typeof(AccessControlFilter))
    {
        Arguments = [roles, accessType];
    }

    public class AccessControlFilter(Roles[] roles, AccessType accessType) : IAsyncAuthorizationFilter
    {
        private readonly Roles[] _roles = roles;
        private readonly AccessType _accessType = accessType;

        public virtual Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var hasClaim = false;
            foreach(var role in _roles)
            {
                if(!hasClaim) hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == JwtRegisteredClaimNames.Aud && c.Value == role.ToString());
            }
            if (hasClaim && _accessType == AccessType.Block) context.Result = new ObjectResult("Forbidden - Unauthorized Access") { StatusCode = 403};
            if (!hasClaim && _accessType == AccessType.Allow) context.Result = new ObjectResult("Forbidden - Unauthorized Access") { StatusCode = 403};
            return Task.CompletedTask;
        }
    }
}

