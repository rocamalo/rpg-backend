using Microsoft.EntityFrameworkCore;
using rpg.Models;

namespace rpg.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Skill>().HasData(
             new Skill { Id = 1, Name = "Fireball", Damage = 30 },
             new Skill { Id = 2, Name = "Frenzy", Damage = 20 },
             new Skill { Id = 3, Name = "Blizzard", Damage = 50 }
            );
        }

        public DbSet<Character> Characters { get; set; } //creates a table
        public DbSet<User> Users { get; set; } //creates a table

        public DbSet<Weapon> Weapons { get; set; } //creates a table

        public DbSet<Skill> Skills { get; set; } //creates a table
    }
}
