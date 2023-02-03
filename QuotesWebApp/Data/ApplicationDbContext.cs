using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using QuotesWebApp.Models;
using System.Reflection.Emit;

namespace QuotesWebApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<QuotesWebApp.Models.Quote> Quote { get; set; }
        public DbSet<QuotesWebApp.Models.UserFavQuotes> UserFavQuotes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserFavQuotes>()
                .HasKey(uq => new { uq.UserId, uq.QuoteId });

            //builder.Entity<UserFavQuotes>()
            //    .HasOne(uq => uq.User)
            //    .WithMany(u => u.UserQuotes)
            //    .HasForeignKey(uq => uq.UserId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //builder.Entity<UserFavQuotes>()
            //    .HasOne(uq => uq.Quote)
            //    .WithMany(q => q.UserQuotes)
            //    .HasForeignKey(uq => uq.QuoteId)
            //    .OnDelete(DeleteBehavior.Cascade);
        }

    }
}