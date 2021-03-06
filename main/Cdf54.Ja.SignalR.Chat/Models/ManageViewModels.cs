﻿using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace Cdf54.Ja.SignalR.Chat.Models
{
    public class IndexViewModel
    {
        public bool HasPassword { get; set; }
        public IList<UserLoginInfo> Logins { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactor { get; set; }
        public bool BrowserRemembered { get; set; }
    }

    //[10006] ADD: Show claims (user and admin mode)
    public class UserClaimsViewModel
    {
        public IList<System.Security.Claims.Claim> CurrentClaims { get; set; }

    }
    public class ManageLoginsViewModel
    {
        public IList<UserLoginInfo> CurrentLogins { get; set; }
        public IList<AuthenticationDescription> OtherLogins { get; set; }
    }
    /* Add extension */
    public class ChangePhotoViewModel
    {
        public string PhotoUrl { get; set; }
        [Display(Name = "Change")]
        [Cdf54.Ja.SignalR.Chat.CustomFiltersAttributes.MyCustomAttributes.ValidateFile]
        public HttpPostedFileWrapper Photo { get; set; }
        [Display(Name = "Remove if checked")]
        public bool IsNoPhotoChecked { get; set; }
        [Display(Name = "Use my Gravatar")]
        public bool UseGravatar { get; set; }
        public string Email { get; set; }
        public string Pseudo { get; set; }
        //[10019] Use provider avatar by default for external login ADD: this function in Profile Change photo. 
        [Display(Name = "Use my Social Network Picture")]
        public bool UseSocialNetworkPicture { get; set; }
        public string ExternalProvider { get; set; }
        public string ParameterProvider { get; set; }
        //[10019]


    }
    public class ChangeProfileViewModel
    {
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Pseudo")]
        public string Pseudo { get; set; }
        public string PhotoUrl { get; set; }
        [Display(Name = "User name")]
        public string UserName { get; set; }

    }
    public class SetPasswordViewModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }


    public class AddPhoneNumberViewModel
    {
        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Number { get; set; }
    }

    public class VerifyPhoneNumberViewModel
    {
        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }

    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }

}