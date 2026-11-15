using LeiaCompartilhe.API.Data;
using LeiaCompartilhe.API.DTOs;
using LeiaCompartilhe.API.Interfaces;
using LeiaCompartilhe.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LeiaCompartilhe.API.Services;

public class EmprestimoService : IEmprestimoService
{
    private readonly AppDbContext _context;

    private readonly IAuditoriaService _auditoriaService;

    public EmprestimoService(AppDbContext context,IAuditoriaService auditoriaService)
    {
        _context = context;
        _auditoriaService = auditoriaService;
    }

    public async Task<EmprestimoRespostaDto> SolicitarAsync(int usuarioId,EmprestimoCadastroDto dto)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == usuarioId);

        if (usuario == null)
            throw new InvalidOperationException("Usuário não encontrado.");

        if (usuario.TipoUsuario != TipoUsuario.Aluno)
            throw new InvalidOperationException("Somente alunos podem solicitar empréstimos.");

        var livro = await _context.Livros.FirstOrDefaultAsync(l => l.Id == dto.LivroId);

        if (livro == null)
            throw new InvalidOperationException("Livro não encontrado.");

        if (!livro.Disponivel)
            throw new InvalidOperationException("Este livro não está disponível para empréstimo.");

        var emprestimoPendente = await _context.Emprestimos.AnyAsync(e =>
                e.LivroId == dto.LivroId &&
                (e.Status == StatusEmprestimo.Pendente ||
                 e.Status == StatusEmprestimo.Aprovado ||
                 e.Status == StatusEmprestimo.Ativo));

        if (emprestimoPendente)
            throw new InvalidOperationException("Este livro já possui uma solicitação em andamento.");

        var emprestimo = new Emprestimo
        {
            LivroId = livro.Id,
            UsuarioId = usuarioId,
            DataSolicitacao = DateTime.UtcNow,
            Status = StatusEmprestimo.Pendente
        };

        _context.Emprestimos.Add(emprestimo);

        await _context.SaveChangesAsync();

        return MapearParaDto(emprestimo, livro.Titulo, usuario.Nome);
    }

    public async Task<List<EmprestimoRespostaDto>> ListarTodosAsync()
    {
        var emprestimos = await _context.Emprestimos
            .AsNoTracking()
            .OrderByDescending(e => e.DataSolicitacao)
            .ToListAsync();

        var livros = await _context.Livros
            .AsNoTracking()
            .ToDictionaryAsync(l => l.Id, l => l.Titulo);

        var usuarios = await _context.Usuarios
            .AsNoTracking()
            .ToDictionaryAsync(u => u.Id, u => u.Nome);

        return emprestimos
            .Select(e => MapearParaDto(
                e,
                livros.GetValueOrDefault(e.LivroId, "Livro não encontrado"),
                usuarios.GetValueOrDefault(e.UsuarioId, "Usuário não encontrado")))
            .ToList();
    }

    public async Task<List<EmprestimoRespostaDto>> ListarMeusAsync(int usuarioId)
    {
        var emprestimos = await _context.Emprestimos
            .AsNoTracking()
            .Where(e => e.UsuarioId == usuarioId)
            .OrderByDescending(e => e.DataSolicitacao)
            .ToListAsync();

        var livros = await _context.Livros
            .AsNoTracking()
            .ToDictionaryAsync(l => l.Id, l => l.Titulo);

        var nomeSolicitante = await _context.Usuarios
            .AsNoTracking()
            .Where(u => u.Id == usuarioId)
            .Select(u => u.Nome)
            .FirstOrDefaultAsync();

        return emprestimos
            .Select(e => MapearParaDto(
                e,
                livros.GetValueOrDefault(e.LivroId, "Livro não encontrado"),
                nomeSolicitante ?? "Usuário não encontrado"))
            .ToList();
    }

    public async Task<EmprestimoRespostaDto> DecidirAsync(int emprestimoId,int responsavelId,EmprestimoDecisaoDto dto)
    {
        await using var transacao = await _context.Database.BeginTransactionAsync();

        var emprestimo = await _context.Emprestimos.FirstOrDefaultAsync(e => e.Id == emprestimoId);

        if (emprestimo == null)
            throw new InvalidOperationException("Empréstimo não encontrado.");

        if (emprestimo.Status != StatusEmprestimo.Pendente)
            throw new InvalidOperationException("Esta solicitação já foi analisada.");

        var responsavel = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == responsavelId);

        if (responsavel == null ||
            (responsavel.TipoUsuario != TipoUsuario.Educador &&
             responsavel.TipoUsuario != TipoUsuario.Administrador))
        {
            throw new InvalidOperationException("Usuário sem permissão para decidir sobre empréstimos.");
        }

        var livro = await _context.Livros.FirstOrDefaultAsync(l => l.Id == emprestimo.LivroId);

        if (livro == null)
            throw new InvalidOperationException("Livro não encontrado.");

        if (dto.Aprovado == true)
        {
            if (!livro.Disponivel)
                throw new InvalidOperationException("Este livro não está mais disponível.");

            emprestimo.Status = StatusEmprestimo.Aprovado;
            emprestimo.DataAprovacao = DateTime.UtcNow;
            emprestimo.ResponsavelAprovacaoId = responsavelId;
            emprestimo.DataLimiteRetirada = DateTime.UtcNow.AddDays(7);

            livro.Disponivel = false;
        }
        else
        {
            emprestimo.Status = StatusEmprestimo.Reprovado;
            emprestimo.DataReprovacao = DateTime.UtcNow;
            emprestimo.ResponsavelReprovacaoId = responsavelId;
        }

        await _auditoriaService.RegistrarAsync(
            usuarioResponsavelId: responsavelId,
            acao: dto.Aprovado == true ? "Aprovar" : "Reprovar",
            entidade: "Emprestimo",
            entidadeId: emprestimo.Id,
            usuarioAlvoId: emprestimo.UsuarioId,
            detalhes: $"Empréstimo {emprestimo.Id} - {emprestimo.Status}."
        );

        await _context.SaveChangesAsync();

        await transacao.CommitAsync();

        var nomeSolicitante = await _context.Usuarios
            .AsNoTracking()
            .Where(u => u.Id == emprestimo.UsuarioId)
            .Select(u => u.Nome)
            .FirstOrDefaultAsync();

        return MapearParaDto(emprestimo,livro.Titulo,nomeSolicitante ?? "Usuário não encontrado");
    }
    public async Task<EmprestimoRespostaDto> RegistrarRetiradaAsync(
        int emprestimoId,
        int responsavelId,
        EmprestimoRetiradaDto dto)
    {
        await using var transacao = await _context.Database.BeginTransactionAsync();

        var emprestimo = await _context.Emprestimos.FirstOrDefaultAsync(e => e.Id == emprestimoId);

        if (emprestimo == null)
            throw new InvalidOperationException("Empréstimo não encontrado.");

        if (emprestimo.Status != StatusEmprestimo.Aprovado)
            throw new InvalidOperationException(
                "Somente empréstimos aprovados podem ter a retirada registrada.");

        var responsavel = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == responsavelId);

        if (responsavel == null ||
            (responsavel.TipoUsuario != TipoUsuario.Educador &&
             responsavel.TipoUsuario != TipoUsuario.Administrador))
        {
            throw new InvalidOperationException(
                "Usuário sem permissão para registrar retiradas.");
        }

        var agora = DateTime.UtcNow;

        if (emprestimo.DataLimiteRetirada == null || agora > emprestimo.DataLimiteRetirada.Value)
            throw new InvalidOperationException("O prazo para retirada deste livro expirou.");

        if (!dto.ConfirmarRetirada)
            throw new InvalidOperationException("É necessário confirmar a retirada do livro.");

        emprestimo.Status = StatusEmprestimo.Ativo;
        emprestimo.DataRetirada = agora;
        emprestimo.ResponsavelRetiradaId = responsavelId;
        emprestimo.DataDevolucaoPrevista = agora.AddDays(30);

        await _auditoriaService.RegistrarAsync(
            usuarioResponsavelId: responsavelId,
            acao: "Registrar retirada",
            entidade: "Emprestimo",
            entidadeId: emprestimo.Id,
            usuarioAlvoId: emprestimo.UsuarioId,
            detalhes: $"Retirada do livro registrada. " +
                      $"Data prevista para devolução: {emprestimo.DataDevolucaoPrevista:dd/MM/yyyy}."
        );

        await _context.SaveChangesAsync();
        await transacao.CommitAsync();

        var livro = await _context.Livros
            .AsNoTracking()
            .Where(l => l.Id == emprestimo.LivroId)
            .Select(l => l.Titulo)
            .FirstOrDefaultAsync();

        var nomeSolicitante = await _context.Usuarios
            .AsNoTracking()
            .Where(u => u.Id == emprestimo.UsuarioId)
            .Select(u => u.Nome)
            .FirstOrDefaultAsync();

        return MapearParaDto(emprestimo,livro ?? "Livro não encontrado",nomeSolicitante ?? "Usuário não encontrado");
    }

    public async Task<EmprestimoRespostaDto> RegistrarDevolucaoAsync(
        int emprestimoId,
        int responsavelId,
        EmprestimoDevolucaoDto dto)
    {
        await using var transacao = await _context.Database.BeginTransactionAsync();

        var emprestimo = await _context.Emprestimos.FirstOrDefaultAsync(e => e.Id == emprestimoId);

        if (emprestimo == null)
            throw new InvalidOperationException("Empréstimo não encontrado.");

        if (emprestimo.Status != StatusEmprestimo.Ativo)
            throw new InvalidOperationException("Somente empréstimos ativos podem ser devolvidos.");

        var responsavel = await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == responsavelId);

        if (responsavel == null ||
            (responsavel.TipoUsuario != TipoUsuario.Educador &&
             responsavel.TipoUsuario != TipoUsuario.Administrador))
            throw new InvalidOperationException("Usuário sem permissão para registrar devoluções.");

        if (!dto.ConfirmarDevolucao)
            throw new InvalidOperationException("É necessário confirmar a devolução do livro.");

        var livro = await _context.Livros.FirstOrDefaultAsync(l => l.Id == emprestimo.LivroId);

        if (livro == null)
            throw new InvalidOperationException("Livro não encontrado.");

        emprestimo.Status = StatusEmprestimo.Devolvido;
        emprestimo.DataDevolucao = DateTime.UtcNow;
        emprestimo.ResponsavelDevolucaoId = responsavelId;

        livro.Disponivel = true;

        await _auditoriaService.RegistrarAsync(
            usuarioResponsavelId: responsavelId,
            acao: "Registrar devolução",
            entidade: "Emprestimo",
            entidadeId: emprestimo.Id,
            usuarioAlvoId: emprestimo.UsuarioId,
            detalhes: $"Devolução do livro registrada. " +
                      $"Data da devolução: {emprestimo.DataDevolucao:dd/MM/yyyy}."
        );

        await _context.SaveChangesAsync();
        await transacao.CommitAsync();

        var nomeSolicitante = await _context.Usuarios
            .AsNoTracking()
            .Where(u => u.Id == emprestimo.UsuarioId)
            .Select(u => u.Nome)
            .FirstOrDefaultAsync();

        return MapearParaDto(emprestimo,livro.Titulo,nomeSolicitante ?? "Usuário não encontrado");
    }

    private static EmprestimoRespostaDto MapearParaDto(Emprestimo emprestimo,string tituloLivro,string nomeSolicitante)
    {
        return new EmprestimoRespostaDto
        {
            Id = emprestimo.Id,
            LivroId = emprestimo.LivroId,
            UsuarioId = emprestimo.UsuarioId,
            TituloLivro = tituloLivro,
            NomeSolicitante = nomeSolicitante,
            DataSolicitacao = emprestimo.DataSolicitacao,
            DataAprovacao = emprestimo.DataAprovacao,
            DataReprovacao = emprestimo.DataReprovacao,
            DataLimiteRetirada = emprestimo.DataLimiteRetirada,
            DataRetirada = emprestimo.DataRetirada,
            DataDevolucaoPrevista = emprestimo.DataDevolucaoPrevista,
            DataDevolucao = emprestimo.DataDevolucao,
            Status = emprestimo.Status.ToString(),
            ResponsavelAprovacaoId = emprestimo.ResponsavelAprovacaoId,
            ResponsavelReprovacaoId = emprestimo.ResponsavelReprovacaoId,
            ResponsavelRetiradaId = emprestimo.ResponsavelRetiradaId,
            ResponsavelDevolucaoId = emprestimo.ResponsavelDevolucaoId,
            DataCancelamento = emprestimo.DataCancelamento,
            ResponsavelCancelamentoId = emprestimo.ResponsavelCancelamentoId
        };
    }
}