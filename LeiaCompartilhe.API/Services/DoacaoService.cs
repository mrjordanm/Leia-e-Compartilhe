using LeiaCompartilhe.API.Data;
using LeiaCompartilhe.API.DTOs;
using LeiaCompartilhe.API.Interfaces;
using LeiaCompartilhe.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LeiaCompartilhe.API.Services;

public class DoacaoService : IDoacaoService
{
    private readonly AppDbContext _context;

    private readonly IAuditoriaService _auditoriaService;

    public DoacaoService(AppDbContext context,IAuditoriaService auditoriaService)
    {
        _context = context;
        _auditoriaService = auditoriaService;
    }

    public async Task<DoacaoRespostaDto> CadastrarAsync(DoacaoCadastroDto dto,int usuarioDoadorId)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuarioDoadorId);

        if (usuario == null)
            throw new InvalidOperationException("Usuário doador não encontrado.");

        var doacao = new Doacao
        {
            UsuarioDoadorId = usuarioDoadorId,
            Titulo = dto.Titulo.Trim(),
            Autor = dto.Autor.Trim(),
            Genero = dto.Genero.Trim(),
            Condicao = dto.Condicao.Trim(),
            Observacoes = dto.Observacoes?.Trim(),
            DataSolicitacao = DateTime.UtcNow
        };

        await using var transacao =
            await _context.Database.BeginTransactionAsync();

        if (usuario.TipoUsuario == TipoUsuario.Aluno)
            doacao.Status = StatusDoacao.Pendente;
        else if (usuario.TipoUsuario == TipoUsuario.Educador ||usuario.TipoUsuario == TipoUsuario.Administrador)
        {
            doacao.Status = StatusDoacao.Aprovada;
            doacao.UsuarioDecisorId = usuarioDoadorId;
            doacao.DataDecisao = DateTime.UtcNow;

            var livro = new Livro
            {
                Titulo = doacao.Titulo,
                Autor = doacao.Autor,
                Genero = doacao.Genero,
                Condicao = doacao.Condicao,
                Observacoes = doacao.Observacoes,
                Disponivel = true,
                DataCadastro = DateTime.UtcNow,
                Origem = OrigemLivro.Doacao
            };

            _context.Livros.Add(livro);

            await _context.SaveChangesAsync();

            doacao.LivroId = livro.Id;
        }
        else
            throw new InvalidOperationException("Tipo de usuário sem permissão para doar livros.");

        _context.Doacoes.Add(doacao);

        await _context.SaveChangesAsync();

        if (doacao.Status == StatusDoacao.Aprovada)
        {
            await _auditoriaService.RegistrarAsync(
                usuarioResponsavelId: usuarioDoadorId,
                acao: "Aprovar automaticamente",
                entidade: "Doacao",
                entidadeId: doacao.Id,
                usuarioAlvoId: usuarioDoadorId,
                detalhes: $"Doação '{doacao.Titulo}' aprovada automaticamente no cadastro."
            );
        }

        await transacao.CommitAsync();

        return MapearParaDto(doacao, usuario.Nome);
    }

    public async Task<List<DoacaoRespostaDto>> ListarAsync(StatusDoacao? status = null)
    {
        var consulta = _context.Doacoes.AsNoTracking().AsQueryable();

        if (status.HasValue)
            consulta = consulta.Where(d => d.Status == status.Value);

        var doacoes = await consulta.OrderByDescending(d => d.DataSolicitacao).ToListAsync();

        var usuarios = await _context.Usuarios
            .AsNoTracking()
            .ToDictionaryAsync(u => u.Id, u => u.Nome);

        return doacoes
            .Select(d => MapearParaDto(d,usuarios.GetValueOrDefault(d.UsuarioDoadorId, "Usuário não encontrado")))
            .ToList();
    }

    public async Task<List<DoacaoRespostaDto>> ListarDoUsuarioAsync(int usuarioId,StatusDoacao? status = null)
    {
        var consulta = _context.Doacoes.AsNoTracking().Where(d => d.UsuarioDoadorId == usuarioId);

        if (status.HasValue)
            consulta = consulta.Where(d => d.Status == status.Value);

        var doacoes = await consulta.OrderByDescending(d => d.DataSolicitacao).ToListAsync();

        var usuarios = await _context.Usuarios
            .AsNoTracking()
            .ToDictionaryAsync(u => u.Id, u => u.Nome);

        return doacoes
            .Select(d => MapearParaDto(d,usuarios.GetValueOrDefault(d.UsuarioDoadorId, "Usuário não encontrado")))
            .ToList();
    }

    public async Task<DoacaoRespostaDto?> BuscarPorIdAsync(int id)
    {
        var doacao = await _context.Doacoes
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id);

        if (doacao == null)
            return null;

        var nomeDoador = await _context.Usuarios
            .AsNoTracking()
            .Where(u => u.Id == doacao.UsuarioDoadorId)
            .Select(u => u.Nome)
            .FirstOrDefaultAsync();

        return MapearParaDto(doacao, nomeDoador ?? "Usuário não encontrado");
    }

    public async Task<DoacaoRespostaDto> DecidirAsync(
        int id,
        bool aprovada,
        string? parecer,
        int usuarioDecisorId)
    {
        await using var transacao = await _context.Database.BeginTransactionAsync();

        var doacao = await _context.Doacoes.FirstOrDefaultAsync(d => d.Id == id);

        if (doacao == null)
            throw new InvalidOperationException("Doação não encontrada.");

        if (doacao.Status != StatusDoacao.Pendente)
            throw new InvalidOperationException("Esta doação já foi analisada.");

        var decisor = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuarioDecisorId);

        if (decisor == null ||
            (decisor.TipoUsuario != TipoUsuario.Educador &&
             decisor.TipoUsuario != TipoUsuario.Administrador))
        {
            throw new InvalidOperationException("Usuário sem permissão para decidir sobre doações.");
        }

        doacao.Status = aprovada
            ? StatusDoacao.Aprovada
            : StatusDoacao.Reprovada;

        doacao.UsuarioDecisorId = usuarioDecisorId;
        doacao.Parecer = parecer?.Trim();
        doacao.DataDecisao = DateTime.UtcNow;

        if (aprovada)
        {
            var livro = new Livro
            {
                Titulo = doacao.Titulo,
                Autor = doacao.Autor,
                Genero = doacao.Genero,
                Condicao = doacao.Condicao,
                Observacoes = doacao.Observacoes,
                Disponivel = true,
                DataCadastro = DateTime.UtcNow,
                Origem = OrigemLivro.Doacao
            };

            _context.Livros.Add(livro);

            await _context.SaveChangesAsync();

            doacao.LivroId = livro.Id;
        }

        await _auditoriaService.RegistrarAsync(
            usuarioResponsavelId: usuarioDecisorId,
            acao: aprovada ? "Aprovar" : "Reprovar",
            entidade: "Doacao",
            entidadeId: doacao.Id,
            usuarioAlvoId: doacao.UsuarioDoadorId,
            detalhes: $"Doação '{doacao.Titulo}' - {doacao.Status}. " +
                      $"Parecer: {doacao.Parecer ?? "Não informado"}"
        );

        await _context.SaveChangesAsync();

        await transacao.CommitAsync();

        var nomeDoador = await _context.Usuarios
            .AsNoTracking()
            .Where(u => u.Id == doacao.UsuarioDoadorId)
            .Select(u => u.Nome)
            .FirstOrDefaultAsync();

        return MapearParaDto(doacao, nomeDoador ?? "Usuário não encontrado");
    }

    private static DoacaoRespostaDto MapearParaDto(Doacao doacao, string nomeDoador)
    {
        return new DoacaoRespostaDto
        {
            Id = doacao.Id,
            NomeDoador = nomeDoador,
            UsuarioDoadorId = doacao.UsuarioDoadorId,
            Titulo = doacao.Titulo,
            Autor = doacao.Autor,
            Genero = doacao.Genero,
            Condicao = doacao.Condicao,
            Observacoes = doacao.Observacoes,
            Status = doacao.Status.ToString(),
            DataSolicitacao = doacao.DataSolicitacao,
            UsuarioDecisorId = doacao.UsuarioDecisorId,
            Parecer = doacao.Parecer,
            DataDecisao = doacao.DataDecisao,
            LivroId = doacao.LivroId
        };
    }
}