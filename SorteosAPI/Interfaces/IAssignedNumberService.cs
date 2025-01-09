using SorteosAPI.Models;

namespace SorteosAPI.Services
{
    public interface IAssignedNumberService
    {
        Task<(bool Success, AssignedNumberRaffer Model, string Message)> AssignRandomNumberAsync(AssignedNumberRaffer model);
    }
}