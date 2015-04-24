using System.Linq;
using System.Security.Claims;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using JA.UTILS.Helpers;

namespace Cdf54.Ja.SignalR.Chat.Models
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            // Configure user lockout defaults
            manager.UserLockoutEnabledByDefault = true;
            manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
            manager.MaxFailedAccessAttemptsBeforeLockout = 5;
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "SecurityCode",
                BodyFormat = "Your security code is {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }

    // Configure the RoleManager used in the application. RoleManager is defined in the ASP.NET Identity core assembly
    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            //// Plug in your email service here to send an email.
            //return Task.FromResult(0);

            /* Email confirmation extension */

            //
            // WORK FINE WITHOUT async
            //
            //// Credentials:
            //var credentialUserName = Utils.GetAppSetting("credentialUserName");
            //var sentFrom = Utils.GetAppSetting("sentFrom");
            //var pwd = Utils.GetAppSetting("pwd");

            //// Configure the client:
            //System.Net.Mail.SmtpClient client =
            //    new System.Net.Mail.SmtpClient(Utils.GetAppSetting("Smtp"));

            //client.Port = Convert.ToInt32(Utils.GetAppSetting("Port"));
            //client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
            //client.UseDefaultCredentials = false;

            //// Create the credentials:
            //System.Net.NetworkCredential credentials =
            //    new System.Net.NetworkCredential(credentialUserName, pwd);

            //client.EnableSsl = Convert.ToBoolean(Utils.GetAppSetting("EnableSsl"));
            //client.Credentials = credentials;

            //// Create the message:
            //var mail =
            //    new System.Net.Mail.MailMessage(sentFrom, message.Destination);

            //mail.Subject = message.Subject;
            //mail.Body = message.Body;

            //// Send:
            //return client.SendMailAsync(mail);


            //http://stackoverflow.com/questions/22797845/asp-net-identity-2-0-how-to-implement-iidentitymessageservice-to-do-async-smtp
            using (var client = new System.Net.Mail.SmtpClient())
            {
                var msg = new System.Net.Mail.MailMessage()
                {
                    Body = message.Body,
                    Subject = message.Subject
                };

                msg.To.Add(message.Destination);

                await client.SendMailAsync(msg);
            }

            //
            // WORK FINE WITHOUT async
            //
            //var msg = new System.Net.Mail.MailMessage()
            //{
            //    Body = message.Body,
            //    Subject = message.Subject
            //};

            //msg.To.Add(message.Destination);

            //var client = new System.Net.Mail.SmtpClient();
            //client.SendCompleted += (s, e) =>
            //{
            //    client.Dispose();
            //};
            //return client.SendMailAsync(msg);

            /* Email confirmation extension */
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            //// Plug in your sms service here to send a text message.
            //return Task.FromResult(0);
            /* SMS confirmation extension */
            string AccountSid = Utils.GetAppSetting("AccountSid");
            string AuthToken = Utils.GetAppSetting("AuthToken");
            string twilioPhoneNumber = Utils.GetAppSetting("twilioPhoneNumber");

            var twilio = new Twilio.TwilioRestClient(AccountSid, AuthToken);
            twilio.SendSmsMessage(twilioPhoneNumber, message.Destination, message.Body);

            // Twilio does not return an async Task, so we need this:
            return Task.FromResult(0);
            /* SMS confirmation extension */

        }
    }

    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        // Create Users with password=P@ssword2015 in the Admin and/or Member role        
        public static void InitializeIdentityForEF(ApplicationDbContext db)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
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

    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager) :
            base(userManager, authenticationManager) { }

        public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
        }
    }
}