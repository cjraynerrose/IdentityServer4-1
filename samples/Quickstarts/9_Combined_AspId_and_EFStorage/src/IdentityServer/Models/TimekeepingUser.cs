﻿using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class TimekeepingUser : IdentityUser
    {
        public void Update(TimekeepingUser updateUser)
        {
            UserName = updateUser.UserName;
        }
    }
}
