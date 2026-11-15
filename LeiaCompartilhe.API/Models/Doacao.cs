namespace LeiaCompartilhe.API.Models;

public class Doacao
{
    public int Id { get; set; }

    public int UsuarioDoadorId { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Autor { get; set; } = string.Empty;

    public string Genero { get; set; } = string.Empty;

    public string Condicao { get; set; } = string.Empty;

    public string? Observacoes { get; set; }

    public StatusDoacao Status { get; set; } = StatusDoacao.Pendente;

    public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;

    public int? UsuarioDecisorId { get; set; }

    public string? Parecer { get; set; }

    public DateTime? DataDecisao { get; set; }

    public int? LivroId { get; set; }
}

public enum StatusDoacao
{
    Pendente,
    Aprovada,
    Reprovada
}