using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace Cdf54.Ja.SignalR.Chat.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        /* Add extension */
        public string PhotoUrl { get; set; }
        [NotMapped]
        public HttpPostedFileWrapper Photo { get; set; }
        [NotMapped]
        public bool IsNoPhotoChecked { get; set; }
        public string Pseudo { get; set; }
        public bool UseGravatar { get; set; }
        //[10019] Use provider avatar by default for external login ADD: this function in Profile Change photo.
        public bool UseSocialNetworkPicture { get; set; }
        /* \Add extension */

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        static ApplicationDbContext()
        {
            // Set the database intializer which is run once during application start
            // This seeds the database with admin user credentials and admin role
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        //public System.Data.Entity.DbSet<Cdf54.Ja.SignalR.Chat.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}