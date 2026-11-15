using LeiaCompartilhe.API.DTOs.Parcerias;

namespace LeiaCompartilhe.API.Interfaces;

public interface IParceriaService
{
    Task<ParceriaDto> CriarAsync(CriarParceriaDto dto, int administradorId);

    Task<List<ParceriaDto>> ListarTodasAsync();

    Task<ParceriaDto?> BuscarPorIdAsync(int id);

    Task<ParceriaDto?> AtualizarAsync(int id, CriarParceriaDto dto, int administradorId);

    Task<bool> AlterarStatusAsync(int id, bool ativa, int administradorId);
}