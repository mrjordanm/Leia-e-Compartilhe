using LeiaCompartilhe.API.DTOs;

namespace LeiaCompartilhe.API.Interfaces;

public interface IEmprestimoService
{
    Task<EmprestimoRespostaDto> SolicitarAsync(
        int usuarioId,
        EmprestimoCadastroDto dto);

    Task<List<EmprestimoRespostaDto>> ListarTodosAsync();

    Task<List<EmprestimoRespostaDto>> ListarMeusAsync(int usuarioId);

    Task<EmprestimoRespostaDto> DecidirAsync(
        int emprestimoId,
        int responsavelId,
        EmprestimoDecisaoDto dto);

    Task<EmprestimoRespostaDto> RegistrarRetiradaAsync(
    int emprestimoId,
    int responsavelId,
    EmprestimoRetiradaDto dto);

    Task<EmprestimoRespostaDto> RegistrarDevolucaoAsync(
    int emprestimoId,
    int responsavelId,
    EmprestimoDevolucaoDto dto);
}