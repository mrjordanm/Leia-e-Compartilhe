namespace LeiaCompartilhe.API.DTOs;

public class UsuarioLoginRespostaDto
{
    public string Token { get; set; } = string.Empty;

    public DateTime Expiracao { get; set; }

    public UsuarioRespostaDto Usuario { get; set; } = new();
}