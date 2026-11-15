using LeiaCompartilhe.API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeiaCompartilhe.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Administrador")]
public class DatabaseController : ControllerBase
{
    private readonly AppDbContext _context;

    public DatabaseController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("testar-conexao")]
    public async Task<IActionResult> TestarConexao()
    {
        try
        {
            bool conectado = await _context.Database.CanConnectAsync();

            if (conectado)
            {
                return Ok("Conexão com o MySQL realizada com sucesso!");
            }

            return StatusCode(500,
                "Não foi possível conectar ao MySQL.");
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                mensagem = "Erro ao conectar com o banco de dados."
            });
        }
    }
}