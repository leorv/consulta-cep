using System.Data;
using ConsultaCEP.Models;
using Dapper;

namespace ConsultaCEP.Repositories
{
    public class CepRepository : ICepRepository
    {
        private readonly IDbConnection _connection;
        private readonly ILogger<CepRepository> _logger;

        public CepRepository(IDbConnection connection, ILogger<CepRepository> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        public async Task<bool> AddAsync(CEP cep)
        {
            _logger.LogInformation("[REPOSITORY] Inserindo CEP: {cep}", cep.CepNumber);
            const string sql = @"INSERT INTO [dbo].[CEP]
                                    ([cep]
                                    ,[logradouro]
                                    ,[complemento]
                                    ,[bairro]
                                    ,[localidade]
                                    ,[uf]
                                    ,[unidade]
                                    ,[ibge]
                                    ,[gia])
                                VALUES (@CepNumber, @Logradouro, @Complemento, @Bairro, @Localidade, @Uf, @Unidade, @Ibge, @Gia);";
            try
            {
                var rows = await _connection.ExecuteAsync(sql, cep);
                return rows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[REPOSITORY] Erro ao inserir o CEP: {Cep}", cep.CepNumber);
                throw;
            }
        }

        public async Task<CEP?> GetByCepAsync(string cep)
        {
            _logger.LogInformation("[REPOSITORY] Buscando pelo cep: {cep}", cep);
            const string sql = @"
                 SELECT TOP (1) [Id]
                    ,[cep] AS CepNumber
                    ,[logradouro]
                    ,[complemento]
                    ,[bairro]
                    ,[localidade]
                    ,[uf]
                    ,[unidade]
                    ,[ibge]
                    ,[gia]
                FROM [dbo].[CEP]
                WHERE [cep] = @cep;";

            try
            {
                _logger.LogInformation("[REPOSITORY] Enviando consulta ao banco de dados: {cep}", cep);
                return await _connection.QueryFirstOrDefaultAsync<CEP>(sql, new { cep });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[REPOSITORY] Erro ao buscar o CEP: {Cep}", cep);
                throw;
            }
        }

        public async Task<List<CEP>?> GetCepsByUf(string uf)
        {
            _logger.LogInformation("[REPOSITORY] Buscando CEPs por UF: {uf}", uf);
            const string sql = @"
                 SELECT Id, cep AS CepNumber, logradouro, complemento, bairro, localidade, uf, unidade, ibge, gia
                 FROM [dbo].[CEP]
                 WHERE [uf] = @uf;
            ";

            try
            {
                var ceps = await _connection.QueryAsync<CEP>(sql, new { uf });
                var result = ceps.ToList();
                return result.Count != 0 ? result : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[REPOSITORY] Erro ao buscar CEPs por UF: {Uf}", uf);
                throw;
            }
        }

        public async Task<List<CEP>?> GetCepsByUf(string uf, int pageNumber, int pageSize)
        {
            _logger.LogInformation("[REPOSITORY] Buscando CEPs paginados por UF: {uf}, Pagina: {pageNumber}, Tamanho: {pageSize}", uf, pageNumber, pageSize);

            var offset = (pageNumber - 1) * pageSize;

            const string sql = @"
             SELECT Id, cep AS CepNumber, logradouro, complemento, bairro, localidade, uf, unidade, ibge, gia
             FROM [dbo].[CEP]
             WHERE [uf] = @uf
             ORDER BY Id 
             OFFSET @Offset ROWS 
             FETCH NEXT @PageSize ROWS ONLY;";

            try
            {
                var parameters = new { uf, Offset = offset, PageSize = pageSize };
                var ceps = await _connection.QueryAsync<CEP>(sql, parameters);
                var result = ceps.ToList();

                return result.Count != 0 ? result : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[REPOSITORY] Erro ao buscar CEPs paginados por UF: {Uf}", uf);
                throw;
            }
        }

        public async Task<int> CountCepsByUf(string uf)
        {
            _logger.LogInformation("[REPOSITORY] Contando CEPs por UF: {uf}", uf);
            const string sql = "SELECT COUNT(Id) FROM [dbo].[CEP] WHERE [uf] = @uf;";

            try
            {
                return await _connection.ExecuteScalarAsync<int>(sql, new { uf });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[REPOSITORY] Erro ao contar CEPs por UF: {Uf}", uf);
                throw;
            }
        }

        public async Task<bool> RemoveAsync(int id)
        {
            _logger.LogInformation("[REPOSITORY] Removendo CEP com Id: {id}", id);
            const string sql = "DELETE FROM CEP WHERE Id = @Id;";

            try
            {
                var rows = await _connection.ExecuteAsync(sql, new { Id = id });
                return rows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[REPOSITORY] Erro ao remover o CEP com Id: {Id}", id);
                throw;
            }
        }

        public async Task<bool> UpdateAsync(CEP cep)
        {
            _logger.LogInformation("[REPOSITORY] Atualizando CEP com Id: {id}", cep.Id);
            const string sql = @"
                UPDATE CEP SET
                    cep = @CepNumber,
                    logradouro = @Logradouro,
                    complemento = @Complemento,
                    bairro = @Bairro,
                    localidade = @Localidade,
                    uf = @Uf,
                    unidade = @Unidade,
                    ibge = @Ibge,
                    gia = @Gia
                WHERE Id = @Id;
            ";

            try
            {
                var rows = await _connection.ExecuteAsync(sql, cep);
                return rows > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[REPOSITORY] Erro ao atualizar o CEP com Id: {Id}", cep.Id);
                throw;
            }
        }
    }
}