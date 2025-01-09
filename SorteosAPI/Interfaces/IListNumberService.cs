using SorteosAPI.Models;

namespace SorteosAPI.Services
{
    public interface IListNumberService
    {
        Task<(bool Success, List<ListNumberRaffer> Numbers, int TotalCount, string Message)> GetAssignedNumbersPagedAsync(int pageNumber, int pageSize, string clientFilter, string raffleFilter, string userFilter);
    }
}