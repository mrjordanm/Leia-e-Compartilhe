
using LeiaCompartilhe.API.DTOs;
using LeiaCompartilhe.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeiaCompartilhe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmprestimosController : ControllerBase
{
    private readonly IEmprestimoService _emprestimoService;
    private readonly IAuditoriaService _auditoriaService;

    public EmprestimosController(
        IEmprestimoService emprestimoService,
        IAuditoriaService auditoriaService)
    {
        _emprestimoService = emprestimoService;
        _auditoriaService = auditoriaService;
    }

    private int ObterUsuarioId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claim, out var id) ? id : 0;
    }

    [HttpPost("solicitar")]
    [Authorize(Roles = "Aluno")]
    public async Task<IActionResult> Solicitar([FromBody] EmprestimoCadastroDto dto)
    {
        var usuarioId = ObterUsuarioId();
        if (usuarioId == 0) return Unauthorized();

        try
        {
            var emprestimo = await _emprestimoService.SolicitarAsync(usuarioId, dto);

            await _auditoriaService.RegistrarAsync(
                usuarioId, "Solicitação", "Empréstimo",
                emprestimo.Id, usuarioId,
                $"Solicitou o livro: {emprestimo.TituloLivro}");

            return StatusCode(201, emprestimo);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Educador,Administrador")]
    public async Task<IActionResult> ListarTodos()
    {
        return Ok(await _emprestimoService.ListarTodosAsync());
    }

    [HttpGet("meus")]
    [Authorize(Roles = "Aluno")]
    public async Task<IActionResult> ListarMeus()
    {
        var usuarioId = ObterUsuarioId();
        if (usuarioId == 0) return Unauthorized();

        return Ok(await _emprestimoService.ListarMeusAsync(usuarioId));
    }

    [HttpPut("{id:int}/decidir")]
    [Authorize(Roles = "Educador,Administrador")]
    public async Task<IActionResult> Decidir(int id, [FromBody] EmprestimoDecisaoDto dto)
    {
        var responsavelId = ObterUsuarioId();
        if (responsavelId == 0) return Unauthorized();

        try
        {
            var emprestimo = await _emprestimoService.DecidirAsync(id, responsavelId, dto);

            var acao = dto.Aprovado == true ? "Aprovação" : "Reprovação";

            await _auditoriaService.RegistrarAsync(
                responsavelId, acao, "Empréstimo",
                emprestimo.Id, emprestimo.UsuarioId,
                $"{acao} do empréstimo do livro: {emprestimo.TituloLivro}");

            return Ok(emprestimo);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message == "Empréstimo não encontrado.")
                return NotFound(new { mensagem = ex.Message });

            return Conflict(new { mensagem = ex.Message });
        }
    }

    [HttpPut("{id:int}/retirada")]
    [Authorize(Roles = "Educador,Administrador")]
    public async Task<IActionResult> RegistrarRetirada(int id, [FromBody] EmprestimoRetiradaDto dto)
    {
        var responsavelId = ObterUsuarioId();
        if (responsavelId == 0) return Unauthorized();

        try
        {
            var emprestimo = await _emprestimoService.RegistrarRetiradaAsync(id, responsavelId, dto);

            await _auditoriaService.RegistrarAsync(
                responsavelId, "Retirada", "Empréstimo",
                emprestimo.Id, emprestimo.UsuarioId,
                $"Retirada do livro: {emprestimo.TituloLivro}");

            return Ok(emprestimo);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
    }

    [HttpPut("{id:int}/devolucao")]
    [Authorize(Roles = "Educador,Administrador")]
    public async Task<IActionResult> RegistrarDevolucao(int id, [FromBody] EmprestimoDevolucaoDto dto)
    {
        var responsavelId = ObterUsuarioId();
        if (responsavelId == 0) return Unauthorized();

        try
        {
            var emprestimo = await _emprestimoService.RegistrarDevolucaoAsync(id, responsavelId, dto);

            await _auditoriaService.RegistrarAsync(
                responsavelId, "Devolução", "Empréstimo",
                emprestimo.Id, emprestimo.UsuarioId,
                $"Devolução do livro: {emprestimo.TituloLivro}");

            return Ok(emprestimo);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensagem = ex.Message });
        }
    }
}