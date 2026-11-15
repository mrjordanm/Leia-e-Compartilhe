namespace LeiaCompartilhe.API.Models;

public class Usuario
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;
    
    public string Instituicao { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string EmailNormalizado { get; set; } = string.Empty;

    public string SenhaHash { get; set; } = string.Empty;

    public string? Ru { get; set; }

    public TipoUsuario TipoUsuario { get; set; }

    public StatusUsuario Status { get; set; } = StatusUsuario.Ativo;

    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
}

public enum TipoUsuario
{
    Aluno,
    Educador,
    Administrador
}

public enum StatusUsuario
{
    Ativo,
    Inativo
}
