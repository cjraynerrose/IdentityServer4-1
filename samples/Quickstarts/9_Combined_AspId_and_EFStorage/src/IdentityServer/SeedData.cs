// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using IdentityModel;
using IdentityServer.Managers;
using IdentityServer.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServerAspNetIdentity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerAspNetIdentity
{
    public class SeedData
    {
        public static void EnsureSeedData(IServiceProvider provider)
        {
            provider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
            provider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();
            provider.GetRequiredService<ConfigurationDbContext>().Database.Migrate();
            {
                var roleMgr = provider.GetRequiredService<TimekeepingRoleManager>();

                var admin = roleMgr.FindByNameAsync("Administrator").Result;
                if (admin == null)
                {
                    admin = new TimekeepingRole
                    {
                        Name = "Administrator"
                    };
                    var result = roleMgr.CreateAsync(admin).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    Console.WriteLine("Administrator Role Created");
                }
                else
                {
                    Console.WriteLine("Administrator Role Already Exists");
                }

                var analyst = roleMgr.FindByNameAsync("Analyst").Result;
                if (analyst == null)
                {
                    analyst = new TimekeepingRole
                    {
                        Name = "Analyst"
                    };
                    var result = roleMgr.CreateAsync(analyst).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    Console.WriteLine("Analyst Role Created");
                }
                else
                {
                    Console.WriteLine("Analyst Role Already Exists");
                }

            }
            {
                var userMgr = provider.GetRequiredService<TimekeepingUserManager>();
                var alice = userMgr.FindByNameAsync("alice").Result;
                if (alice == null)
                {
                    alice = new TimekeepingUser
                    {
                        UserName = "alice",
                        Email = "alice@fakemail.com",
                        PhoneNumber = "01234 567890"
                    };
                    var result = userMgr.CreateAsync(alice, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    alice = userMgr.FindByNameAsync("alice").Result;

                    result = userMgr.AddClaimsAsync(alice, new Claim[]{
                                new Claim(JwtClaimTypes.Name, "Alice Smith"),
                                new Claim(JwtClaimTypes.GivenName, "Alice"),
                                new Claim(JwtClaimTypes.FamilyName, "Smith"),
                                new Claim(JwtClaimTypes.Email, "AliceSmith@email.com"),
                                new Claim(JwtClaimTypes.EmailVerified, "true", ClaimValueTypes.Boolean),
                                new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                                new Claim(JwtClaimTypes.Address, @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }", IdentityServer4.IdentityServerConstants.ClaimValueTypes.Json)
                            }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Console.WriteLine("alice created");
                }
                else
                {
                    Console.WriteLine("alice already exists");
                }

                var bob = userMgr.FindByNameAsync("bob").Result;
                if (bob == null)
                {
                    bob = new TimekeepingUser
                    {
                        UserName = "bob",
                        Email = "bob@fakemail.com",
                        PhoneNumber = "098765 43210"
                    };
                    var result = userMgr.CreateAsync(bob, "Pass123$").Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }

                    bob = userMgr.FindByNameAsync("bob").Result;
                    result = userMgr.AddClaimsAsync(bob, new Claim[]{
                                new Claim(JwtClaimTypes.Name, "Bob Smith"),
                                new Claim(JwtClaimTypes.GivenName, "Bob"),
                                new Claim(JwtClaimTypes.FamilyName, "Smith"),
                                new Claim(JwtClaimTypes.Email, "BobSmith@email.com"),
                            }).Result;
                    if (!result.Succeeded)
                    {
                        throw new Exception(result.Errors.First().Description);
                    }
                    Console.WriteLine("bob created");
                }
                else
                {
                    Console.WriteLine("bob already exists");
                }
            }

            {
                var context = provider.GetRequiredService<ConfigurationDbContext>();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.GetClients())
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.GetIdentityResources())
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiResources.Any())
                {
                    foreach (var resource in Config.GetApis())
                    {
                        context.ApiResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}