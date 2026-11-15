
using System.ComponentModel.DataAnnotations;

namespace LeiaCompartilhe.API.DTOs;

public class DoacaoDecisaoDto
{
    [Required(ErrorMessage = "A decisão é obrigatória.")]
    public bool? Aprovada { get; set; }

    [MaxLength(1000)]
    public string? Parecer { get; set; }
}
