using SorteosAPI.Models;

namespace SorteosAPI.Services
{
    public interface IRaffleService
    {
        Task<bool> CreateRaffleAsync(RaffleCreate raffleCreate);

        Task<List<Raffle>> GetAllRafflesAsync();

        Task<bool> RaffleExistsAsync(string name);
    }
}