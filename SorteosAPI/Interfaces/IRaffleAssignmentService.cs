using SorteosAPI.Models;
using System.Threading.Tasks;

namespace SorteosAPI.Services
{
    public interface IRaffleAssignmentService
    {
        Task<bool> AssignRaffleToClientAsync(RaffleByClientAssign assignment);
        Task<bool> ClientExistsAsync(int idClient);
        Task<bool> RaffleExistsAsync(int idRaffle);
        Task<List<RaffleByClient>> ListRafflesByClientAsync(int? idClient, int? idRaffle);
    }
}

