using ConsultaCEP.Models;
using ConsultaCEP.Services;
using ConsultaCEP.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConsultaCEP.Controllers
{
    [Route("[controller]")]
    public class CepController : Controller
    {
        private readonly ICepService _service;
        private readonly IViaCepService _viaCep;
        private readonly ILogger<CepController> _logger;


        public CepController(ICepService service, IViaCepService viaCep, ILogger<CepController> logger)
        {
            _service = service;
            _viaCep = viaCep;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string cepNumber)
        {
            ViewBag.Erro = null;
            _logger.LogInformation("[CONTROLLER] Buscando o CEP {cep}.", cepNumber);
            if (!string.IsNullOrEmpty(cepNumber))
            {
                var exists = await _service.ExistsInDB(cepNumber);
                var cepModel = await _service.GetCepAsync(cepNumber);
                if (exists)
                {
                    _logger.LogInformation("[CONTROLLER] O CEP {cep} existe no banco de dados, retornando objeto para a View.", cepNumber);
                    ViewBag.Message = "";
                    _logger.LogInformation(">>> CEP retornado: {cepNumber}, {logradouro}.", cepModel?.CepNumber, cepModel?.Logradouro);
                    return View(cepModel);
                }
                else
                {
                    ViewBag.Message = "CEP não existe no banco de dados. As informações serão obtidas usando o ViaCep.";
                    if (cepModel is null)
                    {
                        ViewBag.Message = "CEP não existe no banco de dados. Não foi encontrado usando o ViaCep.";
                        ViewBag.Erro = "CEP inválido.";
                        return View(new CEP { CepNumber = "" });
                    }
                    return View(cepModel);
                }
            }
            ViewBag.Message = "";
            ViewBag.Erro = "CEP inválido.";
            return View(new CEP { CepNumber = "" });
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create(string cepNumber)
        {
            var cepModel = await _service.GetCepAsync(cepNumber);
            if (cepModel is not null)
            {
                return View(cepModel);
            }
            return View(new CEP());
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CEP model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Message = "Verifique os campos informados.";
                return View(model);
            }

            _logger.LogInformation("[CONTROLLER] Tentando cadastrar CEP {cep}", model.CepNumber);

            var exists = await _service.ExistsInDB(model.CepNumber!);
            if (exists)
            {
                ViewBag.Message = "Este CEP já está cadastrado no banco de dados.";
                return View(model);
            }

            var inserted = await _service.AddAsync(model);
            if (inserted.Data is not null)
            {
                ViewBag.Message = "CEP cadastrado com sucesso!";
                return View(new CEP());
            }

            ViewBag.Message = "Erro ao cadastrar o CEP.";
            return View(model);
        }

        private const int PageSize = 10;
        [HttpGet("CepsByUf")]
        public async Task<IActionResult> CepsByUf(string selectedUf, int pageNumber = 1)
        {
            pageNumber = Math.Max(1, pageNumber);

            var ufList = new SelectList(UfHelper.UFsBrasil);

            var viewModel = new CepByUfViewModel
            {
                UFs = ufList,
                SelectedUf = selectedUf
            };

            if (!string.IsNullOrEmpty(selectedUf))
            {
                var (ceps, totalCount) = await _service.GetCepsByUF(selectedUf, pageNumber, PageSize);

                if (ceps.Any())
                {
                    var paginatedCeps = new PaginatedList<CEP>(
                        ceps.ToList(),
                        totalCount,
                        pageNumber,
                        PageSize
                    );
                    viewModel.CEPsPaginados = paginatedCeps;
                }
            }

            return View(viewModel);
        }
    }
}