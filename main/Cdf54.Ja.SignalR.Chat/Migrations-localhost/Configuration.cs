namespace Cdf54.Ja.SignalR.Chat.Migrations
{
    using Cdf54.Ja.SignalR.Chat.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Cdf54.Ja.SignalR.Chat.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            //AutomaticMigrationDataLossAllowed = false; 
            ContextKey = "Cdf54.Ja.SignalR.Chat.Models.ApplicationDbContext";
        }

        protected override void Seed(Cdf54.Ja.SignalR.Chat.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var userStore = new UserStore<ApplicationUser>(context);
            var userManager = new ApplicationUserManager(userStore);

            const string name = "admin@free.fr";
            const string password = "P@ssword2015";
            const string pseudo = "admin";
            const string roleName = "Admin";

            //Create Role Admin if it does not exist
            var role = roleManager.FindByName(roleName);
            if (role == null)
            {
                role = new IdentityRole(roleName);
                var roleresult = roleManager.Create(role);
            }

            var user = userManager.FindByName(name);
            if (user == null)
            {
                user = new ApplicationUser { UserName = pseudo, Email = name, Pseudo = pseudo };
                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name))
            {
                var result = userManager.AddToRole(user.Id, role.Name);
            }

            const string roleName1 = "Member";
            //Create Role Member if it does not exist
            var role1 = roleManager.FindByName(roleName1);
            if (role1 == null)
            {
                role1 = new IdentityRole(roleName1);
                var roleresult1 = roleManager.Create(role1);
            }

            const string name0 = "jose.alvarez54@free.fr";
            const string password0 = "P@ssword2015";
            const string pseudo0 = "jow";
            var user0 = userManager.FindByName(name0);
            if (user0 == null)
            {
                user0 = new ApplicationUser { UserName = pseudo0, Email = name0, Pseudo = pseudo0, UseGravatar = true, PhotoUrl = JA.UTILS.Helpers.Utils.GetGravatarUrlForAddress(name0) };
                var result0 = userManager.Create(user0, password0);
                result0 = userManager.SetLockoutEnabled(user0.Id, false);
            }
            // Add user jow to Role Member if not already added
            var rolesForUser0 = userManager.GetRoles(user0.Id);
            if (!rolesForUser0.Contains(role1.Name))
            {
                var result0 = userManager.AddToRole(user0.Id, role1.Name);
            }
            // Add user jow to Role Admin if not already added
            var rolesForUser01 = userManager.GetRoles(user0.Id);
            if (!rolesForUser01.Contains(role.Name))
            {
                var result0 = userManager.AddToRole(user0.Id, role.Name);
            }

            const string name1 = "ie@free.fr";
            const string password1 = "P@ssword2015";
            const string pseudo1 = "ie";
            var user1 = userManager.FindByName(name1);
            if (user1 == null)
            {
                user1 = new ApplicationUser { UserName = pseudo1, Email = name1, Pseudo = pseudo1 };
                var result1 = userManager.Create(user1, password1);
                result1 = userManager.SetLockoutEnabled(user1.Id, false);
            }
            // Add user ie to Role Member if not already added
            var rolesForUser1 = userManager.GetRoles(user1.Id);
            if (!rolesForUser1.Contains(role1.Name))
            {
                var result1 = userManager.AddToRole(user1.Id, role1.Name);
            }

            const string name2 = "firefox@free.fr";
            const string password2 = "P@ssword2015";
            const string pseudo2 = "firefox";
            var user2 = userManager.FindByName(name2);
            if (user2 == null)
            {
                user2 = new ApplicationUser { UserName = pseudo2, Email = name2, Pseudo = pseudo2 };
                var result2 = userManager.Create(user2, password2);
                result2 = userManager.SetLockoutEnabled(user2.Id, false);
            }
            // Add user firefox to Role Member if not already added
            var rolesForUser2 = userManager.GetRoles(user2.Id);
            if (!rolesForUser2.Contains(role1.Name))
            {
                var result2 = userManager.AddToRole(user2.Id, role1.Name);
            }

            const string name3 = "chrome@free.fr";
            const string password3 = "P@ssword2015";
            const string pseudo3 = "chrome";
            var user3 = userManager.FindByName(name3);
            if (user3 == null)
            {
                user3 = new ApplicationUser { UserName = pseudo3, Email = name3, Pseudo = pseudo3 };
                var result3 = userManager.Create(user3, password3);
                result3 = userManager.SetLockoutEnabled(user3.Id, false);
            }
            // Add user chrome to Role Member if not already added
            var rolesForUser3 = userManager.GetRoles(user3.Id);
            if (!rolesForUser3.Contains(role1.Name))
            {
                var result3 = userManager.AddToRole(user3.Id, role1.Name);
            }

            const string name4 = "opera@free.fr";
            const string password4 = "P@ssword2015";
            const string pseudo4 = "opera";
            var user4 = userManager.FindByName(name4);
            if (user4 == null)
            {
                user4 = new ApplicationUser { UserName = pseudo4, Email = name4, Pseudo = pseudo4 };
                var result4 = userManager.Create(user4, password4);
                result4 = userManager.SetLockoutEnabled(user4.Id, false);
            }
            // Add user opera to Role Member if not already added
            var rolesForUser4 = userManager.GetRoles(user4.Id);
            if (!rolesForUser4.Contains(role1.Name))
            {
                var result4 = userManager.AddToRole(user4.Id, role1.Name);
            }


        }
    }
}
