using IdentityServer.Quickstart.User;
using IdentityServer4.Quickstart.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc
{
    public static class UrlHelperExtensions
    {
        public static string CreatePasswordCallbackLink(this IUrlHelper urlHelper, string code, string scheme)
        {
            scheme = "http";
            return urlHelper.Action(
                action: nameof(AccountController.CreatePassword),
                controller: "Account",
                values: new { code },
                protocol: scheme);
        }
    }
}
