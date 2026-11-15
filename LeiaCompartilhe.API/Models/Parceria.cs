namespace LeiaCompartilhe.API.Models;

public class Parceria
{
    public int Id { get; set; }

    public string NomeInstituicao { get; set; } = string.Empty;

    public string TipoParceria { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public string? Contato { get; set; }

    public StatusParceria Status { get; set; } = StatusParceria.Ativa;

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;
}

public enum StatusParceria
{
    Ativa,
    Inativa
}