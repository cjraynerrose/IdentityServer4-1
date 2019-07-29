using IdentityServer.Models;
using IdentityServer.Quickstart.User;
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

        public async Task<List<UserViewModel>> AddRolesToModelAsync(List<UserViewModel> models)
        {
            foreach (var model in models)
            {
                model.RoleName = await GetRoleFromUserIdAsync(model.Id);
            }

            return models;
        }

        public async Task<UserViewModel> AddRolesToModelAsync(UserViewModel model)
        {
            model.RoleName = await GetRoleFromUserIdAsync(model.Id);
            return model;
        }

        public async Task<string> GetRoleFromUserIdAsync(string userId)
        {
            var user = await FindByIdAsync(userId);

            if(user == null)
            {
                return string.Empty;
            }

            var roles = await GetRolesAsync(user);

            return roles.FirstOrDefault();
        }

    }
}
