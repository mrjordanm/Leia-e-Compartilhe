using System.ComponentModel.DataAnnotations;
using LeiaCompartilhe.API.Validations;

namespace LeiaCompartilhe.API.DTOs;

public class UsuarioLoginDto
{
    [Required(ErrorMessage = "O e-mail é obrigatório.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "A senha é obrigatória.")]
    [RequiredTrimmed(ErrorMessage = "A senha não pode conter apenas espaços.")]
    public string Senha { get; set; } = string.Empty;
}