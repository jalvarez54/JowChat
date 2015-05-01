using System.Globalization;
using Cdf54.Ja.SignalR.Chat.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;
using JA.UTILS.Helpers;

namespace Cdf54.Ja.SignalR.Chat.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            //[10012]	asp.net login refused if email not confirmed
            var user = await UserManager.FindByNameAsync(model.PseudoOrEmail);
            if (user != null)
                {
                    // [10013] Admin dont need to confirm email
                    if (!await UserManager.IsEmailConfirmedAsync(user.Id) && model.PseudoOrEmail != Utils.GetAppSetting("AdminName") && model.PseudoOrEmail != Utils.GetAppSetting("Administrator"))
                    {
                    // [10013]
                        ModelState.AddModelError("", "YOU NEED TO CONFIRM YOUR EMAIL");
                        return View(model);
                    }
                }
            //[10012]
            //[10009] ADD: Login with name and email
            var username = model.PseudoOrEmail;
            if (model.PseudoOrEmail.Contains("@"))
            {
                // Search UserName for that Email
                var userForEmail = await UserManager.FindByEmailAsync(model.PseudoOrEmail);
                if (userForEmail != null)
                {
                    // [10013] Admin dont need to confirm email
                    if (!await UserManager.IsEmailConfirmedAsync(userForEmail.Id) && model.PseudoOrEmail != Utils.GetAppSetting("AdminName") && model.PseudoOrEmail != Utils.GetAppSetting("Administrator"))
                    {
                        // [10013]
                        ModelState.AddModelError("", "YOU NEED TO CONFIRM YOUR EMAIL");
                        return View(model);
                    }
                    username = userForEmail.UserName;
                }
            }
            //[10009]

            // This doen't count login failures towards lockout only two factor authentication
            // To enable password failures to trigger lockout, change to shouldLockout: true
            //var result = await SignInManager.PasswordSignInAsync(model.Pseudo, model.Password, model.RememberMe, shouldLockout: false);
            var result = await SignInManager.PasswordSignInAsync(username, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            var user = await UserManager.FindByIdAsync(await SignInManager.GetVerifiedUserIdAsync());
            if (user != null)
            {
                ViewBag.Status = "For DEMO purposes the current " + provider + " code is: " + await UserManager.GenerateTwoFactorTokenAsync(user.Id, provider);
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: false, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            /* Add extension */
            RegisterViewModel model = new RegisterViewModel();
            model.PhotoUrl = System.IO.Path.Combine(HttpRuntime.AppDomainAppVirtualPath, @"Content/Avatars", @"BlankPhoto.jpg");
            model.UseGravatar = false;
            return View(model);
            /* \Add extension */
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // pseudo instead of email for username
                //var user = new ApplicationUser { UserName = model.Email, Email = model.Email, Pseudo = model.Pseudo };
                //Changed: [10005] MODIF: Use email for login and pseudo for usernameView
                var user = new ApplicationUser { UserName = model.Pseudo, Email = model.Email, Pseudo = model.Pseudo };

                /* Add extension */
                if (model.UseGravatar == false)
                {
                    // Save file to disk and retreive calculated file name or null if handled exception occure
                    // if user don't provide photo then he don't want photo
                    model.PhotoUrl = Utils.SavePhotoFileToDisk(model.Photo, this, null, model.Photo == null ? true : false);
                    user.PhotoUrl = model.PhotoUrl;
                }
                else
                {
                    user.PhotoUrl = JA.UTILS.Helpers.Utils.GetGravatarUrlForAddress(user.Email);
                }
                user.UseGravatar = model.UseGravatar;
                /* \Add extension */

                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    /* Add extension */
                    // Add user to role Member
                    result = await UserManager.AddToRoleAsync(user.Id, "Member");
                    if (result.Succeeded)
                    {
                        /* \Add extension */
                        var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        var callbackUrl = Url.Action("ConfirmEmail", "Account",
                            new { userId = user.Id, code = code },
                            protocol: Request.Url.Scheme);
                        await UserManager.SendEmailAsync(user.Id,
                            "Confirm your account",
                            "Please confirm your account by clicking this link: <a href=\""
                            + callbackUrl + "\">link</a>");

                        ViewBag.Link = callbackUrl;
                        return View("DisplayEmail");
                    }
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }

                var code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id,
                    "Reset Password",
                    "Please reset your password by clicking here: <a href=\""
                    + callbackUrl + "\">link</a>");

                ViewBag.Link = callbackUrl;
                return View("ForgotPasswordConfirmation");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    ////[10004] ADD: Update external claims
                    //var user = await UserManager.FindAsync(loginInfo.Login);
                    //foreach (var userInfoClaim in loginInfo.ExternalIdentity.Claims)
                    //    if (!(userInfoClaim.Type == ClaimTypes.NameIdentifier))
                    //    {
                    //        await UserManager.RemoveClaimAsync(user.Id, userInfoClaim);
                    //    }
                    //foreach (var userInfoClaim in loginInfo.ExternalIdentity.Claims)
                    //    if (!(userInfoClaim.Type == ClaimTypes.NameIdentifier))
                    //    {
                    //        await UserManager.AddClaimAsync(user.Id, userInfoClaim);
                    //        ApplicationUser appUser = await UserManager.FindByIdAsync(user.Id);
                    //        appUser.Claims.Add(new Microsoft.AspNet.Identity.EntityFramework.IdentityUserClaim()
                    //        {
                    //            UserId = appUser.Id,
                    //            ClaimType = userInfoClaim.Type,
                    //            ClaimValue = userInfoClaim.Value,
                    //        });
                    //    }

                    //var user = await UserManager.FindAsync(loginInfo.Login);  
                    //const string ignoreClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims";
                    //foreach (var c in loginInfo.ExternalIdentity.Claims.Where(c => !c.Type.StartsWith(ignoreClaim)))
                    //{
                    //    if (user.Claims.All(t => t.ClaimType != c.Type))
                    //        await UserManager.AddClaimAsync(user.Id, c);
                    //}
                    //http://www.asp.net/mvc/overview/security/preventing-open-redirection-attacks
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }



                // [10000] Added
                ViewBag.LoginProvider = info.Login.LoginProvider;
                // [10000] Added + Twitter dont provide Email
                if (model.Email == info.Email || info.Login.LoginProvider == "Twitter")
                {
                    // [10000] Change model.Email by info.DefaultUserName (no because user may have same pseudo)
                    //var user = new ApplicationUser { UserName = info.DefaultUserName, Email = model.Email };
                    // so change change currentUser.UserName by currentUser.Pseudo in _LoginPartial
                    var user = new ApplicationUser { Pseudo = info.DefaultUserName, UserName = info.DefaultUserName, Email = model.Email };
                    // [10016] BUG: Default photo for external login Chat user photo
                    user.PhotoUrl =  Utils.AppPath() + "/Content/Avatars/BlankPhoto.jpg";
                    // [10017] Use provider avatar by default for external login
                    if(info.Login.LoginProvider == "Google")
                        user.PhotoUrl = info.ExternalIdentity.Claims.FirstOrDefault(c => c.Type.Equals("urn:google:picture")).Value;
                    //[10017]
                    // [10016]
                    var result = await UserManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        result = await UserManager.AddLoginAsync(user.Id, info.Login);
                        if (result.Succeeded)
                        {
                            //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                            //////[10003] ADD: Claims
                            //////Converting Claims added in OnAuthenticated callback to ApplicationClaim objects to store them in Db
                            //foreach (var userInfoClaim in info.ExternalIdentity.Claims)
                            //    //[10004] To prevent exception in @Html.AntiForgeryToken() _LoginPartial
                            //   if(!(userInfoClaim.Type == ClaimTypes.NameIdentifier)) UserManager.AddClaim(user.Id, userInfoClaim);
                            await SignInManager.SignInAsync(user, isPersistent: true, rememberBrowser: false);

                            const string ignoreClaim = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims";
                            foreach (var c in info.ExternalIdentity.Claims.Where(c => !c.Type.StartsWith(ignoreClaim)))
                            {
                                if (user.Claims.All(t => t.ClaimType != c.Type))
                                    await UserManager.AddClaimAsync(user.Id, c);
                            }
                            return RedirectToLocal(returnUrl);
                        }

                    }
                    AddErrors(result);
                }
                else
                {
                    // [10000] Added
                    var ir = new IdentityResult("Bad Email");
                    AddErrors(ir);
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        #region MyExtensionMethods
        #endregion

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}