using System.ComponentModel.DataAnnotations;

namespace LeiaCompartilhe.API.DTOs;

public class EmprestimoDevolucaoDto
{
    [Required(ErrorMessage = "A confirmação da devolução é obrigatória.")]
    public bool ConfirmarDevolucao { get; set; }
}