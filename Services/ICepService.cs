
using ConsultaCEP.Dtos;
using ConsultaCEP.Models;

namespace ConsultaCEP.Services
{
    public interface ICepService
    {
        Task<QueryResult> AddAsync(CEP cep);
        Task<CEP?> GetCepAsync(string cepNumber);
        Task<bool> UpdateAsync(CEP cep);
        Task<bool> RemoveAsync(int id);
        Task<IEnumerable<CEP>> GetCepsByUF(string uf);
        Task<(IEnumerable<CEP> Ceps, int TotalCount)> GetCepsByUF(string uf, int pageNumber, int pageSize);
        Task<bool> ExistsInDB(string cep);
    }
}