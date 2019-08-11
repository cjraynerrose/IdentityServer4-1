using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer.Quickstart.User
{
    public class CreatePasswordModel
    {
        public string Name { get; set; }
        public string CallbackUrl { get; set; }
    }
}
