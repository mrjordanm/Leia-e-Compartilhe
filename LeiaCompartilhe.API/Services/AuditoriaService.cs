using LeiaCompartilhe.API.Data;
using LeiaCompartilhe.API.Interfaces;
using LeiaCompartilhe.API.Models;

namespace LeiaCompartilhe.API.Services;

public class AuditoriaService : IAuditoriaService
{
    private readonly AppDbContext _context;

    public AuditoriaService(AppDbContext context)
    {
        _context = context;
    }

    public async Task RegistrarAsync(
        int usuarioResponsavelId,
        string acao,
        string entidade,
        int? entidadeId = null,
        int? usuarioAlvoId = null,
        string? detalhes = null)
    {
        var registro = new RegistroAuditoria
        {
            UsuarioResponsavelId = usuarioResponsavelId,
            UsuarioAlvoId = usuarioAlvoId,
            Acao = acao,
            Entidade = entidade,
            EntidadeId = entidadeId,
            DataHora = DateTime.UtcNow,
            Detalhes = detalhes
        };

        _context.RegistrosAuditoria.Add(registro);

        await _context.SaveChangesAsync();
    }
}