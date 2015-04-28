using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Cdf54.Ja.SignalR.Chat.Models;
using Owin;
using System;
using JA.UTILS.Helpers;
using System.Security.Claims;

namespace Cdf54.Ja.SignalR.Chat
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and role manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Enable logging with third party login providers

            ///
            /// MICROSOFT
            ///
            //[10003] ADD: Claims
            var microsoftProvider = new Microsoft.Owin.Security.MicrosoftAccount.MicrosoftAccountAuthenticationProvider
            {
                OnAuthenticated = (context) =>
                {
                    foreach (var claim in context.User)
                    {
                        var claimType = string.Format("urn:microsoft:{0}", claim.Key);
                        string claimValue = claim.Value.ToString();
                        if (!context.Identity.HasClaim(claimType, claimValue))
                            context.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue, "XmlSchemaString", "Microsoft"));
                    }
                    return System.Threading.Tasks.Task.FromResult(0);
                }

            };
            var mio = new Microsoft.Owin.Security.MicrosoftAccount.MicrosoftAccountAuthenticationOptions
            {
                ClientId = Utils.GetAppSetting("MicrosoftClientId"),
                ClientSecret = Utils.GetAppSetting("MicrosoftClientSecret"),
                CallbackPath = new PathString("/signin-microsoft"),
                Provider = microsoftProvider,
            };
            //mio.Scope.Add("wl.basic");
            //mio.Scope.Add("wl.emails");
            //mio.Scope.Add("wl.birthday");
            //mio.Scope.Add("wl.postal_addresses");
            app.UseMicrosoftAccountAuthentication(mio);

            ///
            /// TWITTER
            ///
            //[10003] ADD: Claims
            app.UseTwitterAuthentication( new Microsoft.Owin.Security.Twitter.TwitterAuthenticationOptions
            {
                ConsumerKey = Utils.GetAppSetting("TwitterConsumerKey"),
                ConsumerSecret = Utils.GetAppSetting("TwitterConsumerSecret"),
            });

            ///
            /// FACEBOOK
            ///
            //[10003] ADD: Claims
            var facebookProvider = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationProvider
            {
                OnAuthenticated = (context) =>
                {
                    foreach (var claim in context.User)
                    {
                        var claimType = string.Format("urn:facebook:{0}", claim.Key);
                        string claimValue = claim.Value.ToString();
                        if (!context.Identity.HasClaim(claimType, claimValue))
                            context.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue, "XmlSchemaString", "Facebook"));
                    }
                    return System.Threading.Tasks.Task.FromResult(0);
                }
            };
            var fao = new Microsoft.Owin.Security.Facebook.FacebookAuthenticationOptions
            {
                AppId = Utils.GetAppSetting("FaceBookAppId"),
                AppSecret = Utils.GetAppSetting("FaceBookAppSecret"),
                Provider = facebookProvider,
            };
            //fao.Scope.Add("email");
            //fao.Scope.Add("user_birthday");
            //fao.Scope.Add("friends_about_me");
            //fao.Scope.Add("friends_photos");
            app.UseFacebookAuthentication(fao);

            ///
            /// GOOGLE
            ///
            //[10003] ADD: Claims
            var googleProvider = new Microsoft.Owin.Security.Google.GoogleOAuth2AuthenticationProvider
            {
                OnAuthenticated = (context) =>
                {
                    foreach (var claim in context.User)
                    {
                        var claimType = string.Format("urn:google:{0}", claim.Key);
                        string claimValue = claim.Value.ToString();
                        if (!context.Identity.HasClaim(claimType, claimValue))
                            context.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue, "XmlSchemaString", "Google"));
                    }
                    return System.Threading.Tasks.Task.FromResult(0);
                }
            };
            var goo = new Microsoft.Owin.Security.Google.GoogleOAuth2AuthenticationOptions
            {
                ClientId = Utils.GetAppSetting("GoogleClientId"),
                ClientSecret = Utils.GetAppSetting("GoogleClientSecret"),
                CallbackPath = new PathString("/signin-google"),
                Provider = googleProvider,
            };
            app.UseGoogleAuthentication(goo);

        }
    }
}