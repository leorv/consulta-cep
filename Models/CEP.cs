using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsultaCEP.Models;

public class CEP
{
    public int Id { get; set; }
    [Required(ErrorMessage = "O CEP é obrigatório.")]
    [RegularExpression(@"^\d{8}$", ErrorMessage = "O CEP deve conter exatamente 8 dígitos numéricos.")]
    [Display(Name = "CEP")]
    public string? CepNumber { get; set; }
    [Display(Name = "Logradouro")]
    public string? Logradouro { get; set; }
    [Display(Name = "Complemento")]
    public string? Complemento { get; set; }
    [Display(Name = "Bairro")]
    public string? Bairro { get; set; }
    [Display(Name = "Cidade")]
    public string? Localidade { get; set; }
    [Display(Name = "UF")]
    public string? Uf { get; set; }
    public long Unidade { get; set; }
    [Display(Name = "IBGE")]
    public int Ibge { get; set; }
    [Display(Name = "GIA")]
    public string? Gia { get; set; }
}