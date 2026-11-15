namespace LeiaCompartilhe.API.Models;

public class RegistroAuditoria
{
    public int Id { get; set; }

    public int UsuarioResponsavelId { get; set; }

    public int? UsuarioAlvoId { get; set; }

    public string Acao { get; set; } = string.Empty;

    public string Entidade { get; set; } = string.Empty;

    public int? EntidadeId { get; set; }

    public DateTime DataHora { get; set; } = DateTime.UtcNow;

    public string? Detalhes { get; set; }
}