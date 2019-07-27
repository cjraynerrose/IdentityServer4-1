using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Managers
{
    public class TimekeepingRoleManager : RoleManager<IdentityRole>
    {
        public TimekeepingRoleManager(
            IRoleStore<IdentityRole> store,
            IEnumerable<IRoleValidator<IdentityRole>> roleValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            ILogger<TimekeepingRoleManager> logger)
            :base(store, roleValidators, keyNormalizer, errors, logger)
        {

        }
    }
}
