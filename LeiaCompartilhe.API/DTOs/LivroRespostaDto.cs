
namespace LeiaCompartilhe.API.DTOs;

public class LivroRespostaDto
{
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Autor { get; set; } = string.Empty;

    public string Genero { get; set; } = string.Empty;

    public string Condicao { get; set; } = string.Empty;

    public string? Observacoes { get; set; }

    public bool Disponivel { get; set; }

    public DateTime DataCadastro { get; set; }

    public string Origem { get; set; } = string.Empty;
}
