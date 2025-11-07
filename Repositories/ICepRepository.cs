
using ConsultaCEP.Models;

namespace ConsultaCEP.Repositories
{
    public interface ICepRepository
    {
        Task<CEP?> GetByCepAsync(string cep);
        Task<List<CEP>?> GetCepsByUf(string uf);
        Task<List<CEP>?> GetCepsByUf(string uf, int pageNumber, int pageSize);
        Task<int> CountCepsByUf(string uf);
        Task<bool> AddAsync(CEP cep);
        Task<bool> UpdateAsync(CEP cep);
        Task<bool> RemoveAsync(int id);
    }
}