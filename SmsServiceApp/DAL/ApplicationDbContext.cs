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

        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Time> Times { get; set; }
        public virtual DbSet<Mailing> Mailings { get; set; }
        public virtual DbSet<ContactGroup> ContactGroups { get; set; }
        public virtual DbSet<GroupMailing> GroupMailings { get; set; }
        public DbQuery<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

            // Setting PK
            builder.Entity<Contact>().HasKey(u => u.Id);
            builder.Entity<Group>().HasKey(u => u.Id);
            builder.Entity<Time>().HasKey(m => m.Id);
            builder.Entity<Mailing>().HasKey(m => m.Id);

            // Setting FK

            builder.Entity<ApplicationUser>()
                .HasMany<Contact>(u => u.Contacts)
                .WithOne(u => u.ApplicationUser)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<ApplicationUser>()
                .HasMany<Group>(u => u.Groups)
                .WithOne(g => g.ApplicationUser)
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<ApplicationUser>()
                .HasMany<Mailing>(u => u.Mailings)
                .WithOne(m => m.ApplicationUser)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<Mailing>()
                .HasMany<Time>(m => m.Times)
                .WithOne(t => t.Mailing)
                .HasForeignKey(t => t.MailingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ContactGroup>()
                .HasKey(cg => new { cg.ContactId, cg.GroupId });

            builder.Entity<GroupMailing>()
                .HasKey(cg => new { cg.MailingId, cg.GroupId });

            // Many-to-Many through ContactGroup and GroupMailing

            builder.Entity<ContactGroup>()
                .HasOne<Contact>(cg => cg.Contact)
                .WithMany(uc => uc.ContactGroups)
                .HasForeignKey(cg => cg.ContactId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ContactGroup>()
                .HasOne<Group>(cg => cg.Group)
                .WithMany(ucg => ucg.ContactGroups)
                .HasForeignKey(cg => cg.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<GroupMailing>()
                .HasOne<Group>(gm => gm.Group)
                .WithMany(g => g.GroupMailings)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<GroupMailing>()
                .HasOne<Mailing>(gm => gm.Mailing)
                .WithMany(m => m.GroupMailings)
                .HasForeignKey(gm => gm.MailingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
                .HasMany<Mailing>(u => u.Mailings)
                .WithOne(m => m.ApplicationUser)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull);


            // settting indexes

            builder.Entity<Contact>()
                .Property(uc => uc.PhoneNumber)
                .HasMaxLength(13)
                .IsRequired();

            builder.Entity<Contact>()
                .HasIndex(uc => new { uc.UserId, uc.PhoneNumber })
                .IsUnique();

            builder.Entity<Group>()
                .Property(ur => ur.Title)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<Group>()
                .HasIndex(ur => new { ur.UserId, ur.Title })
                .IsUnique();

            // setting sizes

            builder.Entity<Contact>()
                .Property(u => u.Name)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Entity<Mailing>()
                .Property(ma => ma.Title)
                .IsRequired(true);

            builder.Entity<ApplicationUser>()
                .Property(u => u.PhoneNumber)
                .HasMaxLength(13);

            // setting queries

            // does not work for some reason

            //builder.Query<Message>().ToQuery(() => from t in Times
            //                                       join m in Mailings on t.MailingId equals m.Id
            //                                       join gm in GroupMailings on m.Id equals gm.MailingId
            //                                       join g in Groups on gm.GroupId equals g.Id
            //                                       join cg in ContactGroups on g.Id equals cg.GroupId
            //                                       join c in Contacts on cg.ContactId equals c.Id
            //                                       join u in Users on c.UserId equals u.Id
            //                                       where t.BeenSent == false && t.TimeToSend > DateTime.UtcNow
            //                                       select new Message
            //                                       {
            //                                           Sender = c.PhoneNumber,
            //                                           Reciever = c.PhoneNumber,
            //                                           Text = m.Text
            //                                       });

            // should be redone with proper indexing. Look for Group by and Count_Big

            builder.Query<Message>().ToView("View_MessagesToSend");
            builder.Query<Message>().Property(m => m.Sender).HasColumnName("Sender");
            builder.Query<Message>().Property(m => m.Reciever).HasColumnName("Reciever");
            builder.Query<Message>().Property(m => m.Text).HasColumnName("MessageText");
            builder.Query<Message>().Property(m => m.TimeId).HasColumnName("TimeId");


        }


    }
}
