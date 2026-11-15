using LeiaCompartilhe.API.Data;
using LeiaCompartilhe.API.DTOs.Parcerias;
using LeiaCompartilhe.API.Interfaces;
using LeiaCompartilhe.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LeiaCompartilhe.API.Services;

public class ParceriaService : IParceriaService
{
    private readonly AppDbContext _context;

    private readonly IAuditoriaService _auditoriaService;

    public ParceriaService(AppDbContext context,IAuditoriaService auditoriaService)
    {
        _context = context;
        _auditoriaService = auditoriaService;
    }

    public async Task<ParceriaDto> CriarAsync(CriarParceriaDto dto,int administradorId)
    {
        var nome = dto.NomeInstituicao.Trim();

        var existe = await _context.Parcerias.AnyAsync(p => p.NomeInstituicao == nome);

        if (existe)
            throw new InvalidOperationException("Já existe uma parceria cadastrada com esse nome.");

        var parceria = new Parceria
        {
            NomeInstituicao = nome,
            TipoParceria = dto.TipoParceria.Trim(),
            Descricao = dto.Descricao?.Trim(),
            Contato = dto.Contato?.Trim(),
            Status = StatusParceria.Inativa,
            DataCadastro = DateTime.UtcNow
        };

        _context.Parcerias.Add(parceria);

        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAsync(
            usuarioResponsavelId: administradorId,
            acao: "Criar",
            entidade: "Parceria",
            entidadeId: parceria.Id,
            detalhes: $"Administrador cadastrou a parceria '{parceria.NomeInstituicao}'."
        );

        return MapearParaDto(parceria);
    }

    public async Task<List<ParceriaDto>> ListarTodasAsync()
    {
        var parcerias = await _context.Parcerias
            .AsNoTracking()
            .OrderBy(p => p.NomeInstituicao)
            .ToListAsync();

        return parcerias.Select(MapearParaDto).ToList();
    }

    public async Task<ParceriaDto?> BuscarPorIdAsync(int id)
    {
        var parceria = await _context.Parcerias
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (parceria == null)
        {
            return null;
        }

        return MapearParaDto(parceria);
    }

    public async Task<ParceriaDto?> AtualizarAsync(int id,CriarParceriaDto dto,int administradorId)
    {
        var parceria = await _context.Parcerias
            .FirstOrDefaultAsync(p => p.Id == id);

        if (parceria == null)
        {
            return null;
        }

        var nome = dto.NomeInstituicao.Trim();

        var existe = await _context.Parcerias
            .AnyAsync(p => p.Id != id &&
                           p.NomeInstituicao == nome);

        if (existe)
        {
            throw new InvalidOperationException(
                "Já existe outra parceria cadastrada com esse nome.");
        }

        parceria.NomeInstituicao = nome;
        parceria.TipoParceria = dto.TipoParceria.Trim();
        parceria.Descricao = dto.Descricao?.Trim();
        parceria.Contato = dto.Contato?.Trim();

        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAsync(
            usuarioResponsavelId: administradorId,
            acao: "Atualizar",
            entidade: "Parceria",
            entidadeId: parceria.Id,
            detalhes: $"Administrador atualizou os dados da parceria '{parceria.NomeInstituicao}'."
        );

        return MapearParaDto(parceria);
    }

    public async Task<bool> AlterarStatusAsync(int id,bool ativa,int administradorId)
    {
        var parceria = await _context.Parcerias
            .FirstOrDefaultAsync(p => p.Id == id);

        if (parceria == null)
        {
            return false;
        }

        parceria.Status = ativa
            ? StatusParceria.Ativa
            : StatusParceria.Inativa;

        await _context.SaveChangesAsync();

        await _auditoriaService.RegistrarAsync(
            usuarioResponsavelId: administradorId,
            acao: ativa ? "Ativar" : "Inativar",
            entidade: "Parceria",
            entidadeId: parceria.Id,
            detalhes: $"Administrador {(ativa ? "ativou" : "inativou")} a parceria '{parceria.NomeInstituicao}'."
        );

        return true;
    }

    private static ParceriaDto MapearParaDto(Parceria parceria)
    {
        return new ParceriaDto
        {
            Id = parceria.Id,
            NomeInstituicao = parceria.NomeInstituicao,
            TipoParceria = parceria.TipoParceria,
            Descricao = parceria.Descricao,
            Contato = parceria.Contato,
            Status = parceria.Status.ToString(),
            DataCadastro = parceria.DataCadastro
        };
    }
}
