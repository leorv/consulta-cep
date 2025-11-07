
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ConsultaCEP.Models
{
    public class CepByUfViewModel
    {
        public PaginatedList<CEP>? CEPsPaginados { get; set; } 
        public SelectList? UFs { get; set; } 
        public string? SelectedUf { get; set; } 
    }
}