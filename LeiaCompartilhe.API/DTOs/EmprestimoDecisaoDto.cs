using System.ComponentModel.DataAnnotations;

namespace LeiaCompartilhe.API.DTOs;

public class EmprestimoDecisaoDto
{
    [Required(ErrorMessage = "A decisão é obrigatória.")]
    public bool? Aprovado { get; set; }
}