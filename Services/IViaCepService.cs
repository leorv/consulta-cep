using ConsultaCEP.Models;

namespace ConsultaCEP.Services
{
    public interface IViaCepService
    {
            Task<CEP?> GetFromViaCepAsync(string cep);
    }
}