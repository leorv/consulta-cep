
using ConsultaCEP.Dtos;
using ConsultaCEP.Models;
using ConsultaCEP.Repositories;

namespace ConsultaCEP.Services
{
    public class CepService : ICepService
    {
        private readonly ICepRepository _repo;
        private readonly IViaCepService _viaCep;
        private readonly ILogger<CepService> _logger;
        public CepService(ICepRepository repo, IViaCepService viaCep, ILogger<CepService> logger)
        {
            _repo = repo;
            _viaCep = viaCep;
            _logger = logger;
        }

        public async Task<QueryResult> AddAsync(CEP cep)
        {
            _logger.LogInformation("[CEP SERVICE] Verificando se o CEP {cep} já existe no DB.", cep.CepNumber);
            var existing = await _repo.GetByCepAsync(cep.CepNumber ?? "");
            if (existing != null)
            {
                _logger.LogInformation("[CEP SERVICE] O CEP {cep} já existe no DB. Não haverá inserção.", cep.CepNumber);
                return new QueryResult { Data = existing, Exists = true, Source = "db" };
            }

            _logger.LogInformation("[CEP SERVICE] O CEP {cep} não existe no DB. Será adicionado com base nas informações passadas.", cep);

            try
            {
                var success = await _repo.AddAsync(cep);
                if (success)
                {
                    _logger.LogInformation("[CEP SERVICE] O CEP {cep} foi adicionado com sucesso.", cep.CepNumber);
                    return new QueryResult { Success = true, Data = cep, Source = "" };
                }
                else
                    return new QueryResult { Success = false, Errors = ["Falha ao inserir o CEP no banco de dados."] };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[CEP SERVICE] Erro ao cadastrar CEP {Cep}", cep.CepNumber);
                return new QueryResult { Success = false, Errors = ["Erro ao cadastrar o CEP."] };
            }
        }

        public async Task<bool> ExistsInDB(string cep)
        {
            _logger.LogInformation("[CEP SERVICE] Verificando se o CEP existe no banco de dados: {cet}", cep);
            var existing = await _repo.GetByCepAsync(cep);
            if (existing != null)
            {
                _logger.LogInformation("[CEP SERVICE] O CEP {cet} existe no banco de dados.", cep);
                return true;
            }
            _logger.LogInformation("[CEP SERVICE] O CEP {cet} não existe no banco de dados.", cep);
            return false;
        }

        public async Task<CEP?> GetCepAsync(string cepNumber)
        {
            _logger.LogInformation("[CEP SERVICE] Buscando CEP: {cepNumber}", cepNumber);

            var cepFromDb = await _repo.GetByCepAsync(cepNumber);
            if (cepFromDb != null)
            {
                _logger.LogInformation("[CEP SERVICE] CEP {cepNumber} encontrado no DB.", cepNumber);
                return cepFromDb;
            }

            _logger.LogInformation("[CEP SERVICE] CEP {cepNumber} não encontrado no DB. Buscando no ViaCEP...", cepNumber);
            var cepFromViaCep = await _viaCep.GetFromViaCepAsync(cepNumber);

            if (cepFromViaCep == null)
            {
                _logger.LogWarning("[CEP SERVICE] CEP {cepNumber} não encontrado no ViaCEP.", cepNumber);
                return null;
            }
            _logger.LogWarning("[CEP SERVICE] CEP {cepNumber} sendo retornado com ViaCEP.", cepFromViaCep.CepNumber);
            return cepFromViaCep;
        }

        public async Task<IEnumerable<CEP>> GetCepsByUF(string uf)
        {
            _logger.LogInformation("[CEP SERVICE] Buscando CEPs por UF: {uf}", uf);
            var ceps = await _repo.GetCepsByUf(uf);
            return ceps ?? Enumerable.Empty<CEP>();
        }

        public async Task<(IEnumerable<CEP> Ceps, int TotalCount)> GetCepsByUF(string uf, int pageNumber, int pageSize)
        {
            _logger.LogInformation("[CEP SERVICE] Buscando CEPs paginados por UF: {uf}, Página: {pageNumber}", uf, pageNumber);
            var ceps = await _repo.GetCepsByUf(uf, pageNumber, pageSize);
            var totalCount = await _repo.CountCepsByUf(uf);

            return (ceps ?? Enumerable.Empty<CEP>(), totalCount);
        }

        public Task<bool> RemoveAsync(int id)
        {
            _logger.LogInformation("[CEP SERVICE] Removendo CEP com Id: {id}", id);
            return _repo.RemoveAsync(id);
        }

        public Task<bool> UpdateAsync(CEP cep)
        {
            _logger.LogInformation("[SERVICE] Atualizando CEP com Id: {id}", cep.Id);
            return _repo.UpdateAsync(cep);
        }
    }
}