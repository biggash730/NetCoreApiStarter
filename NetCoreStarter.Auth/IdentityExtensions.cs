﻿using NetCoreStarter.Data;
using NetCoreStarter.Data.Classes;
using NetCoreStarter.Data.Repositories;
using System.Security.Principal;
using System.Threading.Tasks;

namespace NetCoreAuth
{
    public static class IdentityExtensions
    {
        public static async Task<User> AsAppUser(this IIdentity identity)
        {
            var user = new UserRepository().Get(identity.Name);
            return await Task.FromResult(user);
        }
    }
}
