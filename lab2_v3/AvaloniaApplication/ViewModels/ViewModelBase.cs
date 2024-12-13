using Avalonia.Controls;
using Microsoft.EntityFrameworkCore;
using ReactiveUI;

namespace AvaloniaApplication.ViewModels;

public class Task
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public int Generation {get; set;}
}

public class Grid
{
    public long Id { get; set; }
    public long TaskId { get; set; }
}

public class RectangleDb
{
    public int Id { get; set; }
    public long GridId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Length { get; set; }
}

public partial class ViewModelBase : DbContext
{
    public ViewModelBase()
    {
        Database.EnsureCreated();
    }
    public ViewModelBase(DbContextOptions<ViewModelBase> options)
        : base(options){}
    
    public virtual DbSet<Task> Tasks { get; set; }
    public virtual DbSet<Grid> Grids { get; set; }
    public virtual DbSet<RectangleDb> Rectangles { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite("Filename=./Tasks.db");
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Task>().HasAlternateKey(u => u.Name);
    }
}