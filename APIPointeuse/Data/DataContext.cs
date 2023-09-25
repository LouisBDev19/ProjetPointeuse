using APIPointeuse.Models;
using Microsoft.EntityFrameworkCore;

namespace APIPointeuse.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Students> Students { get; set; }
        public DbSet<Cycles> Cycles { get; set; }
        public DbSet<Sections> Sections { get; set; }
        public DbSet<Subsections> Subsections { get; set; }
        public DbSet<Schoolclasses> Schoolclasses { get; set; }
        public DbSet<ArrivalDateTime> ArrivalDateTime { get; set; }
        public DbSet<StudentsDevice> StudentsDevice { get; set; }
        public DbSet<Periods> Periods { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
