using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebCustomerApp.Models;


namespace WebCustomerApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {
			Database.EnsureCreated();
		}

        public virtual DbSet<UserContact> UserContacts { get; set; }
        public virtual DbSet<UserContactGroup> UserContactGroups { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Mailing> Mailings { get; set; }
        public virtual DbSet<ContactGroup> ContactGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            // Setting PK
            builder.Entity<UserContact>().HasKey(u => u.Id);
            builder.Entity<UserContactGroup>().HasKey(u => u.Id);
            builder.Entity<Message>().HasKey(m => m.Id);
            builder.Entity<Mailing>().HasKey(m => m.Id);

            // Setting FK

            builder.Entity<ApplicationUser>()
                .HasMany<UserContact>(u => u.UserContacts)
                .WithOne(uc => uc.ApplicationUser)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<ApplicationUser>()
                .HasMany<UserContactGroup>(u => u.UserContactGroups)
                .WithOne(ucg => ucg.ApplicationUser)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<ContactGroup>()
                .HasKey(cg => new { cg.UserContactId, cg.UserContactGroupId });

            // Many-to-Many through ContactGroup

            builder.Entity<ContactGroup>()
                .HasOne<UserContact>(cg => cg.UserContact)
                .WithMany(uc => uc.ContactGroups)
                .HasForeignKey(cg => cg.UserContactId);

            builder.Entity<ContactGroup>()
                .HasOne<UserContactGroup>(cg => cg.UserContactGroup)
                .WithMany(ucg => ucg.ContactGroups)
                .HasForeignKey(cg => cg.UserContactGroupId);

            builder.Entity<ApplicationUser>()
                .HasMany<Mailing>(u => u.Mailings)
                .WithOne(m => m.ApplicationUser)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<Mailing>()
                .HasMany<Message>(u => u.Messages)
                .WithOne(m => m.Mailing)
                .HasForeignKey(m => m.MailingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserContact>()
                .HasMany<Message>(u => u.Messages)
                .WithOne(m => m.UserContact)
                .HasForeignKey(m => m.RecieverId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<UserContact>()
                .Property(uc => uc.PhoneNumber)
                .HasMaxLength(13)
                .IsRequired();

            builder.Entity<UserContact>()
                .HasIndex(uc => new { uc.UserId, uc.PhoneNumber })
                .IsUnique();

            builder.Entity<UserContactGroup>()
                .Property(ur => ur.Group)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<UserContactGroup>()
                .HasIndex(ur => new { ur.UserId, ur.Group })
                .IsUnique();

            //Frequent calls for TimeOfMessage, unnecessary, exist by default
            builder.Entity<Message>()
                .HasIndex(m => m.TimeOfSending);

            //setting sizes

            builder.Entity<UserContact>()
                .Property(u => u.Name)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Entity<Mailing>()
                .Property(ma => ma.Title)
                .IsRequired(true);




        }


    }
}
