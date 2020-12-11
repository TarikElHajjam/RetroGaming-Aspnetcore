using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Retro_Gamer.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Retro_Gamer.Models
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {    
        public AppDbContext(DbContextOptions<AppDbContext> options)
            :base(options)
        {
           
        }
        public DbSet<Game> DbGame { get; set; }    
        public DbSet<Memories> DbMemories { get; set; }
        public DbSet<PendingGame> DbPendingGames { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
