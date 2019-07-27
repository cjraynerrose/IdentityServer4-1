using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Managers
{
    public class TimekeepingUserManager : UserManager<TimekeepingUser>
    {
        public TimekeepingUserManager(
            IUserStore<TimekeepingUser> store,
            IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<TimekeepingUser> passwordHasher,
            IEnumerable<IUserValidator<TimekeepingUser>> userValidators,
            IEnumerable<IPasswordValidator<TimekeepingUser>> passwordValidators,
            ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors,
            IServiceProvider services,
            ILogger<TimekeepingUserManager> logger)
            :base(store, optionsAccessor, passwordHasher, userValidators,
                 passwordValidators, keyNormalizer, errors, services, logger)
        {

        }
    }
}
