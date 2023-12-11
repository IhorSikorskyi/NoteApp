using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using NoteApp;

public class Context : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Note> Notes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;" +
                                 "Port=5432;" +
                                 "Username=postgres;" +
                                 "Password=password;" +
                                 "Database=noteappdb;");
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.Categories)
            .WithOne(c => c.User)
            .HasForeignKey(c => c.userid)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Category>()
            .HasMany(c => c.Notes)
            .WithOne(n => n.Categories)
            .HasForeignKey(n => n.category_id)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<User>()
            .HasMany(u => u.Notes)
            .WithOne(n => n.User)
            .HasForeignKey(n => n.userid)
            .OnDelete(DeleteBehavior.Cascade);
    }
}