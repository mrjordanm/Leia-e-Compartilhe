using System.ComponentModel.DataAnnotations;

namespace LeiaCompartilhe.API.DTOs;

public class EmprestimoRetiradaDto
{
    [Required(ErrorMessage = "A confirmação da retirada é obrigatória.")]
    public bool ConfirmarRetirada { get; set; }
}