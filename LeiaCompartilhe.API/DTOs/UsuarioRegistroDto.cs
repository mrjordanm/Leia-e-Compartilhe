using System.ComponentModel.DataAnnotations;
using LeiaCompartilhe.API.Validations;

namespace LeiaCompartilhe.API.DTOs;

public class UsuarioRegistroDto
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [RequiredTrimmed(ErrorMessage = "O nome não pode conter apenas espaços.")]
    [MaxLength(120)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A instituição é obrigatória.")]
    [RequiredTrimmed(ErrorMessage = "A instituição não pode conter apenas espaços.")]
    [MaxLength(150)]
    public string Instituicao { get; set; } = string.Empty;

    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [MinLength(8, ErrorMessage = "A senha deve ter pelo menos 8 caracteres.")]
    [RequiredTrimmed(ErrorMessage = "A senha não pode conter apenas espaços.")]
    public string Senha { get; set; } = string.Empty;

    [Required(ErrorMessage = "O RU é obrigatório.")]
    [RequiredTrimmed(ErrorMessage = "O RU não pode conter apenas espaços.")]
    [MaxLength(30)]
    public string Ru { get; set; } = string.Empty;
}