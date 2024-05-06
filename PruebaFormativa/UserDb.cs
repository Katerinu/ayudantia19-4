using Microsoft.EntityFrameworkCore;

class UserDb : DbContext
{
    public DbSet<User> Users { get; set; }

    public UserDb(DbContextOptions<UserDb> options) : base(options)
    {}
    public DbSet<User> User => Set<User>();
}