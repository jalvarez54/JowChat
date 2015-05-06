using Cdf54.Ja.SignalR.Chat.Models;
using JA.UTILS.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Cdf54.Ja.SignalR.Chat.Controllers
{
    [Authorize]
    public class ManageController : Controller
    {
        public ManageController()
        {
        }

        public ManageController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
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
        // GET: /Account/Index
        public async Task<ActionResult> Index(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ChangePhotoSuccess ? "Your photo has been changed."
                : message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
                : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
                : message == ManageMessageId.SetTwoFactorSuccess ? "Your two factor provider has been set."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.AddPhoneSuccess ? "The phone number was added."
                : message == ManageMessageId.RemovePhoneSuccess ? "Your phone number was removed."
                : "";

            var model = new IndexViewModel
            {
                HasPassword = HasPassword(),
                PhoneNumber = await UserManager.GetPhoneNumberAsync(User.Identity.GetUserId()),
                TwoFactor = await UserManager.GetTwoFactorEnabledAsync(User.Identity.GetUserId()),
                Logins = await UserManager.GetLoginsAsync(User.Identity.GetUserId()),
                BrowserRemembered = await AuthenticationManager.TwoFactorBrowserRememberedAsync(User.Identity.GetUserId())
            };
            return View(model);
        }


        // - [10006] ADD: Show claims (user and admin mode)
        // GET: /Manage/GetClaims
        public async Task<ActionResult> GetUserClaims()
        {
            //Version: Mine (show stored claims)
            var model = await UserManager.GetClaimsAsync(User.Identity.GetUserId());
            //var model = new UserClaimsViewModel() { CurrentClaims = userClaims };
            return View(model);
        }

        // - [10006] ADD: Show claims (user and admin mode)
        // GET: /Manage/GetClaims
        public ActionResult GetUserIdentityClaims()
        {
            //Version: http://www.apress.com/files/extra/ASP_NET_Identity_Chapters.pdf (show identity claims)
            System.Security.Claims.ClaimsIdentity ident = HttpContext.User.Identity as System.Security.Claims.ClaimsIdentity;
            if (ident == null)
            {
                return View("Error", new string[] { "No claims available" });
            }
            else
            {
                return View(ident.Claims);
            }
        }

        //
        // GET: /Users/Delete/5
        public async Task<ActionResult> RemoveAccount(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /Users/Delete/5
        [HttpPost, ActionName("RemoveAccount")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveAccountConfirmed(string id)
        {
            if (ModelState.IsValid)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
                }

                var user = await UserManager.FindByIdAsync(id);
                if (user == null)
                {
                    return HttpNotFound();
                }

                // Remove photo if exist before remove account
                //if Photo exist and is not a gravatar
                if ((user.PhotoUrl != null && !user.PhotoUrl.Contains("http://")))
                {
                    // Delete file if not the BlankPhoto.jpg and if we change for a gravatar
                    if (!user.PhotoUrl.Contains("BlankPhoto.jpg"))
                    {
                        string fileToDelete = Path.GetFileName(user.PhotoUrl);

                        var path = Path.Combine(Server.MapPath("~/Content/Avatars"), fileToDelete);
                        FileInfo fi = new FileInfo(path);
                        if (fi.Exists)
                            fi.Delete();
                    }
                }
                var result = await UserManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }

                AuthenticationManager.SignOut();
                return RedirectToAction("Index", "Home");
            }
            return View();
        }


        // GET: /Manage/ChangeProfile
        public ActionResult ChangeProfile(ManageMessageId? message = null)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ModifSuccess ? "Your profile has been updated."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";

            var user = UserManager.Users.First(u => u.UserName == User.Identity.Name);

            var model = new ChangeProfileViewModel();
            model.Pseudo = user.Pseudo;
            model.Email = user.Email;
            ViewBag.UserName = user.UserName;

            return View(model);
        }
        //
        // POST: /Manage/ChangeProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangeProfile(ChangeProfileViewModel model)
        {

            var user = UserManager.Users.First(u => u.UserName == User.Identity.Name);

            if (ModelState.IsValid)
            {
                user.Pseudo = model.Pseudo;
                user.Email = model.Email;
                if (user.UseGravatar == true)
                {
                    user.PhotoUrl = JA.UTILS.Helpers.Utils.GetGravatarUrlForAddress(model.Email);
                }
                var result = await UserManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", result.Errors.First());
                    return View();
                }


                return RedirectToAction("ChangeProfile", new { Message = ManageMessageId.ModifSuccess });
            }
            // If we got this far, something failed, redisplay form
            return View(model);

        }

        // GET: /Manage/ChangePhoto
        public ActionResult ChangePhoto(ManageMessageId? message = null)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.ModifSuccess ? "Your photo has been changed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : message == ManageMessageId.NoChange ? "No change have been made."
                : message == ManageMessageId.NoExternalProvider ? "No external login provider found."
                : "";

            var user = UserManager.Users.First(u => u.UserName == User.Identity.Name);

            var model = new ChangePhotoViewModel();
            if (user.PhotoUrl == null)
            {
                model.PhotoUrl = System.IO.Path.Combine(HttpRuntime.AppDomainAppVirtualPath, @"Content/Avatars", @"BlankPhoto.jpg");
            }
            else
            {
                model.PhotoUrl = user.PhotoUrl;
            }
            model.UseGravatar = user.UseGravatar;
            model.Email = user.Email;
            //[10021]
            model.Pseudo = user.Pseudo;

            //[10019] Use provider avatar by default for external login ADD: this function in Profile Change photo.
            model.UseSocialNetworkPicture = user.UseSocialNetworkPicture;
            var owc = Request.GetOwinContext();
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            if (linkedAccounts.Count != 0)
            {
                model.ExternalProvider = linkedAccounts[0].LoginProvider;

                if (model.ExternalProvider == "Google")
                    model.ParameterProvider = owc.Authentication.User.Claims.FirstOrDefault(c => c.Type.Equals("urn:google:picture")).Value;
                if (model.ExternalProvider == "Microsoft")
                {
                    var microsoftAccountId = owc.Authentication.User.Claims.FirstOrDefault(c => c.Type.Equals("urn:microsoftaccount:id")).Value;
                    model.ParameterProvider = microsoftAccountId;
                }
                if (model.ExternalProvider == "Facebook")
                {
                    var facebookAccountId = owc.Authentication.User.Claims.FirstOrDefault(c => c.Type.Equals("urn:facebook:id")).Value;
                    model.ParameterProvider = facebookAccountId;
                }
                if (model.ExternalProvider == "Twitter")
                {
                    var twitterScreenname = owc.Authentication.User.Claims.FirstOrDefault(c => c.Type.Equals("urn:twitter:screenname")).Value;
                    model.ParameterProvider = twitterScreenname;
                }
                //[10026] ADD: Github for external login
                if (model.ExternalProvider == "GitHub")
                {
                    var githibId = owc.Authentication.User.Claims.FirstOrDefault(c => c.Type.Equals("urn:github:id")).Value;
                    model.ParameterProvider = githibId;
                }
                //[10026]
            }
            //[10019]

            return View(model);
        }
        //
        // POST: /Manage/ChangePhoto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePhoto(ChangePhotoViewModel model)
        {
            var user = UserManager.Users.First(u => u.UserName == User.Identity.Name);

            if (ModelState.IsValid)
            {
                // No change
                if (model.Photo == null && !model.IsNoPhotoChecked && model.UseGravatar == user.UseGravatar && model.UseSocialNetworkPicture == user.UseSocialNetworkPicture)
                {
                    return RedirectToAction("ChangePhoto", new { Message = ManageMessageId.NoChange });
                }
                // Delete photo if photo exist and is not a gravatar or socialnetworkPicture.
                if (user.PhotoUrl != null)
                {
                    if (!(user.PhotoUrl.Contains("http://") || user.PhotoUrl.Contains("https://")))
                    {
                        // Delete file if not the BlankPhoto.jpg and if we change for a gravatar or socialnetworkpicture or new photo
                        if (!user.PhotoUrl.Contains("BlankPhoto.jpg"))
                        {
                            if (model.IsNoPhotoChecked || model.UseGravatar || model.UseSocialNetworkPicture || model.Photo != null)
                            {
                                string fileToDelete = Path.GetFileName(user.PhotoUrl);

                                var path = Path.Combine(Server.MapPath("~/Content/Avatars"), fileToDelete);
                                FileInfo fi = new FileInfo(path);
                                if (fi.Exists)
                                    fi.Delete();
                            }
                        }
                    }
                }
                if (model.IsNoPhotoChecked)
                {
                    //string vPath = HttpRuntime.AppDomainAppVirtualPath;
                    //var path = Path.Combine(vPath, "/Content/Avatars/BlankPhoto.jpg");
                    // [10015] PB: Migration files VS dev env
                    string vPath = Utils.AppPath();
                    var path = vPath + "/Content/Avatars/BlankPhoto.jpg";
                    // [10015]
                    model.PhotoUrl = path;
                }
                else
                    // Save on disk only uploaded picture.
                    if (model.Photo != null )
                    {
                        model.PhotoUrl = Utils.SavePhotoFileToDisk(model.Photo, this, user.PhotoUrl, false);
                    }
                if (model.UseGravatar == true)
                {
                    model.PhotoUrl = JA.UTILS.Helpers.Utils.GetGravatarUrlForAddress(user.Email);
                }
                //[10019] Use provider avatar by default for external login ADD: this function in Profile Change photo.
                if (model.UseSocialNetworkPicture == true)
                {
                    var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
                    var owc = Request.GetOwinContext();

                    if (linkedAccounts.Count == 0)
                    {
                        return RedirectToAction("ChangePhoto", new { Message = ManageMessageId.NoExternalProvider });
                    }

                    model.ExternalProvider = linkedAccounts[0].LoginProvider;

                    if (model.ExternalProvider == "Google")
                        model.PhotoUrl = owc.Authentication.User.Claims.FirstOrDefault(c => c.Type.Equals("urn:google:picture")).Value;
                    if (model.ExternalProvider == "Microsoft")
                    {
                        var microsoftAccountId = owc.Authentication.User.Claims.FirstOrDefault(c => c.Type.Equals("urn:microsoftaccount:id")).Value;
                        model.PhotoUrl = string.Format("https://apis.live.net/v5.0/{0}/picture", microsoftAccountId);
                    }
                    if (model.ExternalProvider == "Facebook")
                    {
                        var facebookAccountId = owc.Authentication.User.Claims.FirstOrDefault(c => c.Type.Equals("urn:facebook:id")).Value;
                        model.PhotoUrl = string.Format("http://graph.facebook.com/{0}/picture", facebookAccountId);
                    }
                    if (model.ExternalProvider == "Twitter")
                    {
                        var twitterScreenname = owc.Authentication.User.Claims.FirstOrDefault(c => c.Type.Equals("urn:twitter:screenname")).Value;
                        model.PhotoUrl = string.Format("https://twitter.com/{0}/profile_image?size=original", twitterScreenname);
                    }
                    if (model.ExternalProvider == "GitHub")
                    {
                        var githubId = owc.Authentication.User.Claims.FirstOrDefault(c => c.Type.Equals("urn:github:id")).Value;
                        model.PhotoUrl = string.Format("https://avatars.githubusercontent.com/u/{0}?v=3", githubId);
                    }
                }
                //[10019]

                user.PhotoUrl = model.PhotoUrl;
                user.UseGravatar = model.UseGravatar;
                user.UseSocialNetworkPicture = model.UseSocialNetworkPicture;
                await UserManager.UpdateAsync(user);

                return RedirectToAction("ChangePhoto", new { Message = ManageMessageId.ModifSuccess });
            }
            model.PhotoUrl = user.PhotoUrl;

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/RemoveLogin
        public ActionResult RemoveLogin()
        {
            var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
            ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
            return View(linkedAccounts);
        }

        //
        // POST: /Manage/RemoveLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveLogin(string loginProvider, string providerKey)
        {
            ManageMessageId? message;
            var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                message = ManageMessageId.RemoveLoginSuccess;
            }
            else
            {
                message = ManageMessageId.Error;
            }
            return RedirectToAction("ManageLogins", new { Message = message });
        }

        //
        // GET: /Account/AddPhoneNumber
        public ActionResult AddPhoneNumber()
        {
            return View();
        }

        //
        // POST: /Account/AddPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddPhoneNumber(AddPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            // Generate the token and send it
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), model.Number);
            if (UserManager.SmsService != null)
            {
                var message = new IdentityMessage
                {
                    Destination = model.Number,
                    Body = "Your security code is: " + code
                };
                await UserManager.SmsService.SendAsync(message);
            }
            return RedirectToAction("VerifyPhoneNumber", new { PhoneNumber = model.Number });
        }

        //
        // POST: /Manage/RememberBrowser
        [HttpPost]
        public ActionResult RememberBrowser()
        {
            var rememberBrowserIdentity = AuthenticationManager.CreateTwoFactorRememberBrowserIdentity(User.Identity.GetUserId());
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, rememberBrowserIdentity);
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/ForgetBrowser
        [HttpPost]
        public ActionResult ForgetBrowser()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/EnableTFA
        [HttpPost]
        public async Task<ActionResult> EnableTFA()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), true);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // POST: /Manage/DisableTFA
        [HttpPost]
        public async Task<ActionResult> DisableTFA()
        {
            await UserManager.SetTwoFactorEnabledAsync(User.Identity.GetUserId(), false);
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", "Manage");
        }

        //
        // GET: /Account/VerifyPhoneNumber
        public async Task<ActionResult> VerifyPhoneNumber(string phoneNumber)
        {
            // This code allows you exercise the flow without actually sending codes
            // For production use please register a SMS provider in IdentityConfig and generate a code here.
            var code = await UserManager.GenerateChangePhoneNumberTokenAsync(User.Identity.GetUserId(), phoneNumber);
            ViewBag.Status = "For DEMO purposes only, the current code is " + code;
            return phoneNumber == null ? View("Error") : View(new VerifyPhoneNumberViewModel { PhoneNumber = phoneNumber });
        }

        //
        // POST: /Account/VerifyPhoneNumber
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyPhoneNumber(VerifyPhoneNumberViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePhoneNumberAsync(User.Identity.GetUserId(), model.PhoneNumber, model.Code);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.AddPhoneSuccess });
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("", "Failed to verify phone");
            return View(model);
        }

        //
        // GET: /Account/RemovePhoneNumber
        public async Task<ActionResult> RemovePhoneNumber()
        {
            var result = await UserManager.SetPhoneNumberAsync(User.Identity.GetUserId(), null);
            if (!result.Succeeded)
            {
                return RedirectToAction("Index", new { Message = ManageMessageId.Error });
            }
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user != null)
            {
                await SignInAsync(user, isPersistent: false);
            }
            return RedirectToAction("Index", new { Message = ManageMessageId.RemovePhoneSuccess });
        }

        //
        // GET: /Manage/ChangePassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/Manage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                if (user != null)
                {
                    await SignInAsync(user, isPersistent: false);
                }
                return RedirectToAction("Index", new { Message = ManageMessageId.ChangePasswordSuccess });
            }
            AddErrors(result);
            return View(model);
        }

        //
        // GET: /Manage/SetPassword
        public ActionResult SetPassword()
        {
            return View();
        }

        //
        // POST: /Manage/SetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SetPassword(SetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
                if (result.Succeeded)
                {
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        await SignInAsync(user, isPersistent: false);
                    }
                    return RedirectToAction("Index", new { Message = ManageMessageId.SetPasswordSuccess });
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Manage
        public async Task<ActionResult> ManageLogins(ManageMessageId? message)
        {
            ViewBag.StatusMessage =
                message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
                : message == ManageMessageId.Error ? "An error has occurred."
                : "";
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
            if (user == null)
            {
                return View("Error");
            }
            var userLogins = await UserManager.GetLoginsAsync(User.Identity.GetUserId());
            var otherLogins = AuthenticationManager.GetExternalAuthenticationTypes().Where(auth => userLogins.All(ul => auth.AuthenticationType != ul.LoginProvider)).ToList();
            ViewBag.ShowRemoveButton = user.PasswordHash != null || userLogins.Count > 1;
            return View(new ManageLoginsViewModel
            {
                CurrentLogins = userLogins,
                OtherLogins = otherLogins
            });
        }

        //
        // POST: /Manage/LinkLogin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            return new AccountController.ChallengeResult(provider, Url.Action("LinkLoginCallback", "Manage"), User.Identity.GetUserId());
        }

        //
        // GET: /Manage/LinkLoginCallback
        public async Task<ActionResult> LinkLoginCallback()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
            if (loginInfo == null)
            {
                return RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
            }
            var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
            return result.Succeeded ? RedirectToAction("ManageLogins") : RedirectToAction("ManageLogins", new { Message = ManageMessageId.Error });
        }

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

        private async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie, DefaultAuthenticationTypes.TwoFactorCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private bool HasPassword()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PasswordHash != null;
            }
            return false;
        }

        private bool HasPhoneNumber()
        {
            var user = UserManager.FindById(User.Identity.GetUserId());
            if (user != null)
            {
                return user.PhoneNumber != null;
            }
            return false;
        }

        public enum ManageMessageId
        {
            NoChange,
            NoExternalProvider,
            ModifSuccess,
            AddPhoneSuccess,
            ChangePhotoSuccess,
            ChangePasswordSuccess,
            SetTwoFactorSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
            RemovePhoneSuccess,
            Error
        }

        #endregion
    }
}