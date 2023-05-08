using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WordRepeaterBot.DataAccess.Models;

namespace WordRepeaterBot.DataAccess;

public class WordRepeaterBotDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Phrase> Phrases { get; set; }
    public DbSet<Settings> Settings { get; set; }
    
    public WordRepeaterBotDbContext(DbContextOptions<WordRepeaterBotDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
