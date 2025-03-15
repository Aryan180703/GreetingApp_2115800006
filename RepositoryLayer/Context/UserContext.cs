using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Context
{
    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }

        public DbSet<GreetingEntity> Greetings { get; set; }
        public DbSet<UserEntity> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define Relationship: One User → Many Greetings
            modelBuilder.Entity<GreetingEntity>()
                .HasOne(g => g.User)      // One Greeting has One User
                .WithMany(u => u.Greetings) // One User has Many Greetings
                .HasForeignKey(g => g.UserId) // FK: GreetingEntity.UserId
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete (delete greetings if user is deleted)
        }
    }
}


