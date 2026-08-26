using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Models;

namespace TaskFlow.Data;

// Inherit from IdentityDbContext<User> instead of plain DbContext — this automatically adds all Identity tables (Users, Roles, Claims, etc.)
public class ApplicationDbContext : IdentityDbContext<User>
{
    // Constructor used by DI to inject configured options (connection string etc.) from Program.cs into this context
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // Represents the table in DB
    public DbSet<TaskItem> TaskItems { get; set; }
}