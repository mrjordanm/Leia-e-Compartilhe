
using LeiaCompartilhe.API.Data;
using LeiaCompartilhe.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeiaCompartilhe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class AuditoriaController : ControllerBase
{
    private readonly AppDbContext _context;

    public AuditoriaController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var usuarios = _context.Set<Usuario>().AsNoTracking();

        var registros = await (
            from r in _context.RegistrosAuditoria.AsNoTracking()
            join responsavel in usuarios
                on r.UsuarioResponsavelId equals responsavel.Id
            join usuarioAlvo in usuarios
                on r.UsuarioAlvoId equals (int?)usuarioAlvo.Id into alvos
            from usuarioAlvo in alvos.DefaultIfEmpty()
            orderby r.DataHora descending
            select new
            {
                r.Id,
                r.UsuarioResponsavelId,
                NomeResponsavel = responsavel.Nome,
                r.UsuarioAlvoId,
                NomeUsuarioAlvo = usuarioAlvo != null ? usuarioAlvo.Nome : null,
                r.Acao,
                r.Entidade,
                r.EntidadeId,
                r.DataHora,
                r.Detalhes
            }
        ).ToListAsync();

        return Ok(registros);
    }
}