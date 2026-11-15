using System.ComponentModel.DataAnnotations;
using LeiaCompartilhe.API.Validations;

namespace LeiaCompartilhe.API.DTOs;

public class LivroCadastroDto
{
    [Required(ErrorMessage = "O título é obrigatório.")]
    [RequiredTrimmed(ErrorMessage = "O título não pode conter apenas espaços.")]
    [MaxLength(150)]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "O autor é obrigatório.")]
    [RequiredTrimmed(ErrorMessage = "O autor não pode conter apenas espaços.")]
    [MaxLength(150)]
    public string Autor { get; set; } = string.Empty;

    [Required(ErrorMessage = "O gênero é obrigatório.")]
    [RequiredTrimmed(ErrorMessage = "O gênero não pode conter apenas espaços.")]
    [MaxLength(150)]
    public string Genero { get; set; } = string.Empty;

    [Required(ErrorMessage = "A condição do livro é obrigatória.")]
    [RequiredTrimmed(ErrorMessage = "A condição não pode conter apenas espaços.")]
    [MaxLength(50)]
    public string Condicao { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Observacoes { get; set; }
}
