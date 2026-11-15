using LeiaCompartilhe.API.DTOs;
using LeiaCompartilhe.API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LeiaCompartilhe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    [HttpPost("registrar")]
    public async Task<IActionResult> Registrar([FromBody] UsuarioRegistroDto dto)
    {
        try
        {
            var usuario = await _usuarioService.RegistrarAsync(dto);

            return StatusCode(StatusCodes.Status201Created, usuario);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost("cadastrar-educador")]
    public async Task<IActionResult> CadastrarEducador([FromBody] EducadorCadastroDto dto)
    {
        try
        {
            var administradorId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var educador = await _usuarioService.CadastrarEducadorAsync(dto,administradorId);

            return StatusCode(StatusCodes.Status201Created, educador);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UsuarioLoginDto dto)
    {
        try
        {
            var resposta = await _usuarioService.LoginAsync(dto);

            return Ok(resposta);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { mensagem = ex.Message });
        }
    }

    [Authorize]
    [HttpGet("perfil")]
    public async Task<IActionResult> Perfil()
    {
        var identificador = User.FindFirst(
            System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(identificador, out int usuarioId))
            return Unauthorized();

        var perfil = await _usuarioService.BuscarPerfilAsync(usuarioId);

        if (perfil == null)
            return NotFound(new { mensagem = "Usuário não encontrado." });

        return Ok(perfil);
    }

    [Authorize(Roles = "Administrador,Educador")]
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        bool incluirTodos = User.IsInRole("Administrador");

        var usuarios = await _usuarioService.ListarAsync(incluirTodos);

        return Ok(usuarios);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPatch("{id}/status")]
    public async Task<IActionResult> AlterarStatus(
        int id,
        [FromBody] bool ativo)
    {
        int administradorId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!
        );

        if (id == administradorId && !ativo)
            return BadRequest(new { mensagem = "Você não pode desativar sua própria conta." });

        try
        {
            await _usuarioService.AlterarStatusAsync(id, ativo, administradorId);

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { mensagem = ex.Message });
        }
    }
}