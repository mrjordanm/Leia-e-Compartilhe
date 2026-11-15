namespace LeiaCompartilhe.API.DTOs;

public class UsuarioRespostaDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Ru { get; set; }
    public string TipoUsuario { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}