using Microsoft.EntityFrameworkCore;

namespace SorteosAPI.Models
{
    public class SorteosDbContext : DbContext
    {
        public SorteosDbContext(DbContextOptions<SorteosDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Raffle> Raffles { get; set; }
        public DbSet<RaffleByClient> RafflesByClient { get; set; }
        public DbSet<AssignedNumberRaffer> AssignedNumbers { get; set; }
    }
}