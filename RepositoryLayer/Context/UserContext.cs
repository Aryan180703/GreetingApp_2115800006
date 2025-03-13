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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Make FirstName and LastName optional in the database
            modelBuilder.Entity<GreetingEntity>()
                .Property(g => g.FirstName)
                .IsRequired(false);

            modelBuilder.Entity<GreetingEntity>()
                .Property(g => g.LastName)
                .IsRequired(false);
        }
    }
}


