
using ConsultaCEP.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ConsultaCEP.Services
{
    public class ViaCepService : IViaCepService
    {
        private readonly HttpClient _client;
        private readonly IMemoryCache _cache;
        private readonly ILogger<ViaCepService> _logger;

        public ViaCepService(HttpClient client, IMemoryCache cache, ILogger<ViaCepService> logger)
        {
            _client = client;
            _cache = cache;
            _logger = logger;
        }

        public async Task<CEP?> GetFromViaCepAsync(string cep)
        {
            _logger.LogInformation("[VIA CEP SERVICE] Buscando CEP: {cep}", cep);
            
            var cacheKey = $"viacep:{cep}";
            if (_cache.TryGetValue<CEP>(cacheKey, out var cached)) return cached;
            
            try
            {
                var response = await _client.GetAsync($"ws/{cep}/json/");
                if (!response.IsSuccessStatusCode) return null;

                var json = await response.Content.ReadFromJsonAsync<Dictionary<string, object?>>();
                if (json == null || json.ContainsKey("erro")) return null;

                var cepObj = new CEP
                {
                    CepNumber = (json.GetValueOrDefault("cep")?.ToString() ?? cep).Replace("-", "").Trim(),
                    Logradouro = json.GetValueOrDefault("logradouro")?.ToString(),
                    Complemento = json.GetValueOrDefault("complemento")?.ToString(),
                    Bairro = json.GetValueOrDefault("bairro")?.ToString(),
                    Localidade = json.GetValueOrDefault("localidade")?.ToString(),
                    Uf = json.GetValueOrDefault("uf")?.ToString(),
                    Ibge = int.Parse(json.GetValueOrDefault("ibge")?.ToString()?? ""),
                    Gia = json.GetValueOrDefault("gia")?.ToString()
                };

                _cache.Set(cacheKey, cepObj, TimeSpan.FromHours(1));
                return cepObj;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[VIA CEP SERVICE] Erro ao buscar o CEP: {Cep}", cep);
                return null;
            }
        }
    }
}