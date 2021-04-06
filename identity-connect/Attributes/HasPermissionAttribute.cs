using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;
using identity_connect.Expansions;

namespace identity_connect.Attributes
{
    public class HasPermissionsAttrubute : TypeFilterAttribute
    {
        public HasPermissionsAttrubute(params string[] permissions) : base(typeof(HasPermissionsFilter))
        {
            Arguments = permissions;
        }
    }

    public class HasPermissionsFilter : IAuthorizationFilter
    {
        private readonly string[] Permissions;

        public HasPermissionsFilter(string[] permissions)
        {
            Permissions = permissions;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (user.IsAdmin())
                return;

            foreach (var permission in Permissions)
            {
                if (!user.IsAllowed(permission))
                {
                    context.Result = Forbidden();
                    return;
                }
            }
        }

        private ForbidResult Forbidden() => new ForbidResult("Нет доступа!");
    }
}
