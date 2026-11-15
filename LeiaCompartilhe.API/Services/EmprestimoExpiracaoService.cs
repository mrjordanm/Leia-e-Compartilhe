
using LeiaCompartilhe.API.Data;
using LeiaCompartilhe.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LeiaCompartilhe.API.Services;

public class EmprestimoExpiracaoService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmprestimoExpiracaoService> _logger;

    public EmprestimoExpiracaoService(
        IServiceScopeFactory scopeFactory,
        ILogger<EmprestimoExpiracaoService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        do
        {
            try
            {
                await ProcessarEmprestimosExpiradosAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao processar empréstimos expirados.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task ProcessarEmprestimosExpiradosAsync(
        CancellationToken stoppingToken)
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider
            .GetRequiredService<AppDbContext>();

        var agora = DateTime.UtcNow;

        var emprestimosExpirados = await context.Emprestimos
            .Where(e =>
                 e.Status == StatusEmprestimo.Aprovado &&
                 e.DataLimiteRetirada <= agora)
            .ToListAsync(stoppingToken);

        foreach (var emprestimo in emprestimosExpirados)
        {
            emprestimo.Status = StatusEmprestimo.Cancelado;
            emprestimo.DataCancelamento = agora;
            emprestimo.ResponsavelCancelamentoId = null;

            var livro = await context.Livros
                .FirstOrDefaultAsync(
                    l => l.Id == emprestimo.LivroId,
                    stoppingToken);

            if (livro != null)
                livro.Disponivel = true;
        }

        if (emprestimosExpirados.Count > 0)
        {
            await context.SaveChangesAsync(stoppingToken);

            _logger.LogInformation(
                "{Quantidade} empréstimo(s) expirado(s) cancelado(s).",
                emprestimosExpirados.Count);
        }
    }
}