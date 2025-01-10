using SorteosAPI.Models;

namespace SorteosAPI.Services
{
    public interface IUserService
    {
        Task<bool> CreateUserAsync(UserCreate userCreate);
        Task<List<User>> GetUsersByClientIdAsync(string? name = null);
        Task<bool> ClientExistsAsync(int idClient);
        Task<bool> UserExistsAsync(int idClient, string name);
    }
}

