namespace LeiaCompartilhe.API.Models;

public class Emprestimo
{
    public int Id { get; set; }

    public int LivroId { get; set; }

    public int UsuarioId { get; set; }

    public DateTime DataSolicitacao { get; set; } = DateTime.UtcNow;

    public DateTime? DataAprovacao { get; set; }

    public DateTime? DataReprovacao { get; set; }

    public int? ResponsavelReprovacaoId { get; set; }

    public DateTime? DataLimiteRetirada { get; set; }

    public DateTime? DataRetirada { get; set; }

    public DateTime? DataDevolucaoPrevista { get; set; }

    public DateTime? DataDevolucao { get; set; }

    public StatusEmprestimo Status { get; set; } = StatusEmprestimo.Pendente;

    public int? ResponsavelAprovacaoId { get; set; }

    public int? ResponsavelRetiradaId { get; set; }

    public int? ResponsavelDevolucaoId { get; set; }

    public DateTime? DataCancelamento { get; set; }

    public int? ResponsavelCancelamentoId { get; set; }
}

public enum StatusEmprestimo
{
    Pendente,
    Aprovado,
    Ativo,
    Devolvido,
    Cancelado,
    Reprovado
}