using LeiaCompartilhe.API.DTOs;
using LeiaCompartilhe.API.Models;

namespace LeiaCompartilhe.API.Interfaces;

public interface IDoacaoService
{
    Task<DoacaoRespostaDto> CadastrarAsync(
        DoacaoCadastroDto dto,
        int usuarioDoadorId
    );

    Task<List<DoacaoRespostaDto>> ListarAsync(StatusDoacao? status = null);

    Task<List<DoacaoRespostaDto>> ListarDoUsuarioAsync(int usuarioId,StatusDoacao? status = null);

    Task<DoacaoRespostaDto?> BuscarPorIdAsync(int id);

    Task<DoacaoRespostaDto> DecidirAsync(
        int id,
        bool aprovada,
        string? parecer,
        int usuarioDecisorId
    );
}
