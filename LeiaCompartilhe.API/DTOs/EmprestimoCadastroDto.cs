using System.ComponentModel.DataAnnotations;

namespace LeiaCompartilhe.API.DTOs;

public class EmprestimoCadastroDto
{
    [Required(ErrorMessage = "O livro é obrigatório.")]
    [Range(1, int.MaxValue, ErrorMessage = "Informe um ID de livro válido.")]
    public int LivroId { get; set; }
}
