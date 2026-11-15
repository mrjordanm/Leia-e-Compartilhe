namespace LeiaCompartilhe.API.DTOs;

public class DoacaoRespostaDto
{
    public int Id { get; set; }

    public int UsuarioDoadorId { get; set; }

    public string NomeDoador { get; set; } = string.Empty;

    public string Titulo { get; set; } = string.Empty;

    public string Autor { get; set; } = string.Empty;

    public string Genero { get; set; } = string.Empty;

    public string Condicao { get; set; } = string.Empty;

    public string? Observacoes { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime DataSolicitacao { get; set; }

    public int? UsuarioDecisorId { get; set; }

    public string? Parecer { get; set; }

    public DateTime? DataDecisao { get; set; }

    public int? LivroId { get; set; }
}
