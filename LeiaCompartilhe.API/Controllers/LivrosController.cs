using LeiaCompartilhe.API.DTOs;
using LeiaCompartilhe.API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LeiaCompartilhe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LivrosController : ControllerBase
{
    private readonly ILivroService _livroService;

    public LivrosController(ILivroService livroService)
    {
        _livroService = livroService;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<List<LivroRespostaDto>>> Listar()
    {
        var livros = await _livroService.ListarAsync();

        return Ok(livros);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<LivroRespostaDto>> BuscarPorId(int id)
    {
        var livro = await _livroService.BuscarPorIdAsync(id);

        if (livro == null)
            return NotFound();

        return Ok(livro);
    }

    [HttpPost]
    [Authorize(Roles = "Educador,Administrador")]
    public async Task<ActionResult<LivroRespostaDto>> Cadastrar([FromBody] LivroCadastroDto dto)
    {
        var livro = await _livroService.CadastrarAsync(dto);

        return CreatedAtAction(nameof(BuscarPorId), new { id = livro.Id }, livro);
    }
}