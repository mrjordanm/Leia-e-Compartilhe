namespace LeiaCompartilhe.API.DTOs;

public class UsuarioPerfilDto
{
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Ru { get; set; }
    public string Instituicao { get; set; } = string.Empty;
}