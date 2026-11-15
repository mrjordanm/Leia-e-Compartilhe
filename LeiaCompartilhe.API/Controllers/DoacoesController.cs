
using LeiaCompartilhe.API.DTOs;
using LeiaCompartilhe.API.Interfaces;
using LeiaCompartilhe.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LeiaCompartilhe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DoacoesController : ControllerBase
{
    private readonly IDoacaoService _doacaoService;
    private readonly IAuditoriaService _auditoriaService;

    public DoacoesController(
        IDoacaoService doacaoService,
        IAuditoriaService auditoriaService)
    {
        _doacaoService = doacaoService;
        _auditoriaService = auditoriaService;
    }

    private int ObterUsuarioId()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(usuarioId, out var id))
            throw new InvalidOperationException("Identificação do usuário não encontrada no token.");

        return id;
    }

    [HttpPost]
    [Authorize(Roles = "Aluno,Educador,Administrador")]
    public async Task<ActionResult<DoacaoRespostaDto>> Cadastrar(DoacaoCadastroDto dto)
    {
        var usuarioId = ObterUsuarioId();
        var doacao = await _doacaoService.CadastrarAsync(dto, usuarioId);

        await _auditoriaService.RegistrarAsync(
            usuarioId,
            "Cadastro",
            "Doação",
            doacao.Id,
            doacao.UsuarioDoadorId,
            $"Cadastrou a doação do livro: {doacao.Titulo}");

        return StatusCode(StatusCodes.Status201Created, doacao);
    }

    [HttpGet]
    [Authorize(Roles = "Educador,Administrador")]
    public async Task<ActionResult<List<DoacaoRespostaDto>>> Listar([FromQuery] string? status = null)
    {
        StatusDoacao? statusFiltro = null;

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<StatusDoacao>(status, true, out var statusConvertido) ||
                !Enum.IsDefined(statusConvertido))
                return BadRequest("Status inválido. Utilize Pendente, Aprovada ou Reprovada.");

            statusFiltro = statusConvertido;
        }

        var doacoes = await _doacaoService.ListarAsync(statusFiltro);
        return Ok(doacoes);
    }

    [HttpGet("minhas")]
    [Authorize(Roles = "Aluno,Educador,Administrador")]
    public async Task<ActionResult<List<DoacaoRespostaDto>>> ListarMinhas([FromQuery] string? status = null)
    {
        StatusDoacao? statusFiltro = null;

        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<StatusDoacao>(status, true, out var statusConvertido) ||
                !Enum.IsDefined(statusConvertido))
                return BadRequest("Status inválido. Utilize Pendente, Aprovada ou Reprovada.");

            statusFiltro = statusConvertido;
        }

        var usuarioId = ObterUsuarioId();
        var doacoes = await _doacaoService.ListarDoUsuarioAsync(usuarioId, statusFiltro);

        return Ok(doacoes);
    }

    [HttpPut("{id:int}/decidir")]
    [Authorize(Roles = "Educador,Administrador")]
    public async Task<ActionResult<DoacaoRespostaDto>> Decidir(int id, DoacaoDecisaoDto dto)
    {
        try
        {
            if (dto.Aprovada == null)
                return BadRequest(new { mensagem = "Informe se a doação foi aprovada ou reprovada." });

            var usuarioId = ObterUsuarioId();
            var doacao = await _doacaoService.DecidirAsync(
                id, dto.Aprovada.Value, dto.Parecer, usuarioId);

            var acao = dto.Aprovada.Value ? "Aprovação" : "Reprovação";

            await _auditoriaService.RegistrarAsync(
                usuarioId,
                acao,
                "Doação",
                doacao.Id,
                doacao.UsuarioDoadorId,
                $"{acao} da doação do livro: {doacao.Titulo}. Parecer: {doacao.Parecer}");

            return Ok(doacao);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message == "Doação não encontrada.")
                return NotFound(new { mensagem = ex.Message });

            return Conflict(new { mensagem = ex.Message });
        }
    }
}
