using SorteosAPI.Models;

namespace SorteosAPI.Services
{
    public interface IClientService
    {
        Task<bool> CreateClientAsync(ClientCreate clientCreate);

        Task<List<Client>> GetClientsAsync();
    }
}