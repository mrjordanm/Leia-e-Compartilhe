
using LeiaCompartilhe.API.DTOs.Parcerias;
using LeiaCompartilhe.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeiaCompartilhe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ParceriaController : ControllerBase
{
    private readonly IParceriaService _parceriaService;

    public ParceriaController(IParceriaService parceriaService)
    {
        _parceriaService = parceriaService;
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] CriarParceriaDto dto)
    {
        try
        {
            var administradorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var parceria = await _parceriaService.CriarAsync(dto, administradorId);

            return CreatedAtAction(nameof(BuscarPorId), new { id = parceria.Id }, parceria);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
    }

    [Authorize(Roles = "Administrador,Educador")]
    [HttpGet]
    public async Task<IActionResult> ListarTodas()
    {
        var parcerias = await _parceriaService.ListarTodasAsync();
        return Ok(parcerias);
    }

    [Authorize(Roles = "Administrador,Educador")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var parceria = await _parceriaService.BuscarPorIdAsync(id);

        if (parceria == null)
            return NotFound(new { mensagem = "Parceria não encontrada." });

        return Ok(parceria);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Atualizar(int id, [FromBody] CriarParceriaDto dto)
    {
        try
        {
            var administradorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var parceria = await _parceriaService.AtualizarAsync(id, dto, administradorId);

            if (parceria == null)
                return NotFound(new { mensagem = "Parceria não encontrada." });

            return Ok(parceria);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
    }

    [Authorize(Roles = "Administrador")]
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> AlterarStatus(int id, [FromQuery] bool? ativa)
    {
        if (ativa == null)
            return BadRequest(new { mensagem = "Informe se a parceria ficará ativa ou inativa." });

        var administradorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var alterada = await _parceriaService.AlterarStatusAsync(id, ativa.Value, administradorId);

        if (!alterada)
            return NotFound(new { mensagem = "Parceria não encontrada." });

        return NoContent();
    }
}