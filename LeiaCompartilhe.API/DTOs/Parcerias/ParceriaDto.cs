namespace LeiaCompartilhe.API.DTOs.Parcerias;

public class ParceriaDto
{
    public int Id { get; set; }

    public string NomeInstituicao { get; set; } = string.Empty;

    public string TipoParceria { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public string? Contato { get; set; }

    public string Status { get; set; } = string.Empty;

    public DateTime DataCadastro { get; set; }
}