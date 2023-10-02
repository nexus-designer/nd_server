using Microsoft.EntityFrameworkCore;

namespace NexusServer.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Msg> Msgs { get; set; }
        public DbSet<Conversation> Conversations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define foreign key relationship between User and Msg
            modelBuilder.Entity<Msg>()
                .HasKey(m => new { m.id, m.conversationId });

            // Additional configurations for other relationships can be added here.
            modelBuilder.Entity<Conversation>()
                .HasKey(c => new { c.id, c.userId });

            // Define other entity configurations as needed.

            base.OnModelCreating(modelBuilder);
        }
    }
}
