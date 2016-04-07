using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheWayFreeClinicVMS.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {       
        public virtual Volunteer Volunteer { get; set; }
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
            : base("VMSContext", throwIfV1Schema: false)
        {
        }

        //Database table creation
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<Volunteer> Volunteers { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<Pagroup> Pagroups { get; set; }
        public DbSet<Worktime> Worklog { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Speak> Speaks { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<Availability> Availabilities { get; set; }
        public DbSet<Econtact> Econtacts { get; set; }
        public DbSet<License> Licenses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)

        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            //// Configure UserId as PK for Volunteer
            //modelBuilder.Entity<Volunteer>()
            //    .HasKey(e => e.ApplicationUser);

            //// Configure User ID as FK for Volunteers
            //modelBuilder.Entity<ApplicationUser>()
            //            .HasOptional(s => s.Volunteer) // Mark Volunteer as optional for appuser
            //            .WithRequired(ad => ad.ApplicationUser); // Create inverse relationship
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<TheWayFreeClinicVMS.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}