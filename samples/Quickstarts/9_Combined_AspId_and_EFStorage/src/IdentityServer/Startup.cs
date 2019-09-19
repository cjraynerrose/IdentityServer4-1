
// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using AutoMapper;
using IdentityServer.Managers;
using IdentityServer.Models;
using IdentityServer.Quickstart.Role;
using IdentityServer.Quickstart.User;
using IdentityServerAspNetIdentity.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;

namespace IdentityServerAspNetIdentity
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<TimekeepingUser, IdentityRole>()
                .AddRoleManager<TimekeepingRoleManager>()
                .AddUserManager<TimekeepingUserManager>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc().SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_1);

            services.Configure<IISOptions>(iis =>
            {
                iis.AuthenticationDisplayName = "Windows";
                iis.AutomaticAuthentication = false;
            });

            var builder = services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;
            })
                // this adds the config data from DB (clients, resources)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                // this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                })
                .AddAspNetIdentity<TimekeepingUser>();

            //if (Environment.IsDevelopment())
            //{
            //    builder.AddDeveloperSigningCredential();
            //}
            //else
            //{
            //var tp = "dc84655d78ce21b4d5e90f3135efef9771ba6599";//pmbl.com
            var tp = "071dd553f82ce9dc57a1e2df43d049041fd40d25"; // dev.crit
                var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                store.Open(OpenFlags.ReadOnly);
                var certs = store.Certificates.Find(X509FindType.FindByThumbprint, tp, false);
                store.Close();

                builder.AddSigningCredential(certs[0]);
            //}

            services.AddAuthentication();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseIdentityServer();
            app.UseMvc(x=>
            {
                x.MapRoute(
                    name: "default",
                    template: "{controller=User}/{action=Index}");

            });

            Mapper.Initialize(c=>
            {
                c.CreateMap<TimekeepingUser, UserViewModel>();
                c.CreateMap<TimekeepingUser, UserInputModel>()
                .ForMember(dst => dst.RoleName, opt => opt.Ignore())
                ;
                c.CreateMap<UserInputModel, TimekeepingUser>()
                .ForMember(dst=> dst.AccessFailedCount, opt=> opt.Ignore())
                .ForMember(dst=> dst.ConcurrencyStamp, opt=> opt.Ignore())
                .ForMember(dst=> dst.EmailConfirmed, opt=> opt.Ignore())
                .ForMember(dst=> dst.LockoutEnabled, opt=> opt.Ignore())
                .ForMember(dst=> dst.LockoutEnd, opt=> opt.Ignore())
                .ForMember(dst=> dst.NormalizedEmail, opt=> opt.Ignore())
                .ForMember(dst=> dst.NormalizedUserName, opt=> opt.Ignore())
                .ForMember(dst=> dst.PasswordHash, opt=> opt.Ignore())
                .ForMember(dst=> dst.PhoneNumberConfirmed, opt=> opt.Ignore())
                .ForMember(dst=> dst.SecurityStamp, opt=> opt.Ignore())
                .ForMember(dst=> dst.TwoFactorEnabled, opt=> opt.Ignore())
                ;
                c.CreateMap<TimekeepingRole, RoleViewModel>();
                c.CreateMap<RoleInputModel, TimekeepingRole>();
            });

            Mapper.Configuration.CompileMappings();
        }
    }
}