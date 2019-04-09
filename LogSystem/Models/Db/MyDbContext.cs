using Microsoft.EntityFrameworkCore;

namespace LogSystem.Models.Db
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
    }
}