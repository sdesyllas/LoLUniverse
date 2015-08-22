using LoLUniverse.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebGrease.Css.Extensions;

namespace LoLUniverse.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<LoLUniverse.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            ContextKey = "LoLUniverse.Models.ApplicationDbContext";
        }

        protected override void Seed(LoLUniverse.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            // seed roles
            var store = new RoleStore<IdentityRole>(context);
            var manager = new RoleManager<IdentityRole>(store);
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var role = new IdentityRole { Name = "Admin" };
                manager.Create(role);
            }
            if (!context.Roles.Any(r => r.Name == "ContentAdmin"))
            {
                var role = new IdentityRole { Name = "ContentAdmin" };
                manager.Create(role);
            }
            if (!context.Roles.Any(r => r.Name == "User"))
            {
                var role = new IdentityRole { Name = "User" };
                manager.Create(role);
            }

            // seed users
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(userStore);
            if (!(context.Users.Any(u => u.UserName == "spyros2283@gmail.com")))
            {
                var userToInsert = new ApplicationUser
                {
                    UserName = "spyros2283@gmail.com", PhoneNumber = "+306940112629", Email = "spyros2283@gmail.com",
                    EmailConfirmed = true, TwoFactorEnabled = true
                };
                userManager.Create(userToInsert, "p@$$w0rd");

                userManager.AddToRole(userToInsert.Id, "Admin");
                userManager.AddToRole(userToInsert.Id, "ContentAdmin");
                userManager.AddToRole(userToInsert.Id, "User");
            }

            base.Seed(context);
        }
    }
}
