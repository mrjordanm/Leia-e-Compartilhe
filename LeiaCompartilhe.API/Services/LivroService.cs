
using LeiaCompartilhe.API.Data;
using LeiaCompartilhe.API.DTOs;
using LeiaCompartilhe.API.Interfaces;
using LeiaCompartilhe.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LeiaCompartilhe.API.Services;

public class LivroService : ILivroService
{
    private readonly AppDbContext _context;

    public LivroService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LivroRespostaDto> CadastrarAsync(
        LivroCadastroDto dto)
    {
        var livro = new Livro
        {
            Titulo = dto.Titulo.Trim(),
            Autor = dto.Autor.Trim(),
            Genero = dto.Genero.Trim(),
            Condicao = dto.Condicao.Trim(),
            Observacoes = dto.Observacoes?.Trim(),
            Disponivel = true,
            DataCadastro = DateTime.UtcNow,
            Origem = OrigemLivro.Catalogo
        };

        _context.Livros.Add(livro);

        await _context.SaveChangesAsync();

        return MapearParaDto(livro);
    }

    public async Task<List<LivroRespostaDto>> ListarAsync()
    {
        var livros = await _context.Livros
            .AsNoTracking()
            .OrderBy(l => l.Titulo)
            .ToListAsync();

        return livros.Select(MapearParaDto).ToList();
    }

    public async Task<LivroRespostaDto?> BuscarPorIdAsync(int id)
    {
        var livro = await _context.Livros
            .AsNoTracking()
            .FirstOrDefaultAsync(l => l.Id == id);

        if (livro == null)
        {
            return null;
        }

        return MapearParaDto(livro);
    }

    private static LivroRespostaDto MapearParaDto(Livro livro)
    {
        return new LivroRespostaDto
        {
            Id = livro.Id,
            Titulo = livro.Titulo,
            Autor = livro.Autor,
            Genero = livro.Genero,
            Condicao = livro.Condicao,
            Observacoes = livro.Observacoes,
            Disponivel = livro.Disponivel,
            DataCadastro = livro.DataCadastro,
            Origem = livro.Origem.ToString()
        };
    }
}
