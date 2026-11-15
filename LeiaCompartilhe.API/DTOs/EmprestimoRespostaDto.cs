namespace LeiaCompartilhe.API.DTOs;

public class EmprestimoRespostaDto
{
    public int Id { get; set; }

    public int LivroId { get; set; }

    public int UsuarioId { get; set; }

    public string TituloLivro { get; set; } = string.Empty;

    public string NomeSolicitante { get; set; } = string.Empty;

    public DateTime DataSolicitacao { get; set; }

    public DateTime? DataAprovacao { get; set; }

    public DateTime? DataReprovacao { get; set; }

    public int? ResponsavelReprovacaoId { get; set; }

    public DateTime? DataLimiteRetirada { get; set; }

    public DateTime? DataRetirada { get; set; }

    public DateTime? DataDevolucaoPrevista { get; set; }

    public DateTime? DataDevolucao { get; set; }

    public string Status { get; set; } = string.Empty;

    public int? ResponsavelAprovacaoId { get; set; }

    public int? ResponsavelRetiradaId { get; set; }

    public int? ResponsavelDevolucaoId { get; set; }

    public DateTime? DataCancelamento { get; set; }

    public int? ResponsavelCancelamentoId { get; set; }
}