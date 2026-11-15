namespace LeiaCompartilhe.API.Models;

public class Livro
{
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Autor { get; set; } = string.Empty;

    public string Genero { get; set; } = string.Empty;

    public string Condicao { get; set; } = string.Empty;

    public string? Observacoes { get; set; }

    public bool Disponivel { get; set; } = true;

    public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

    public OrigemLivro Origem { get; set; } = OrigemLivro.Catalogo;
}

public enum OrigemLivro
{
    Catalogo,
    Doacao
}