using Microsoft.EntityFrameworkCore;
using TodoTracker.Models;

namespace TodoTracker.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Эта коллекция превратится в таблицу Tasks в базе данных
    public DbSet<TodoItem> Tasks => Set<TodoItem>();
}