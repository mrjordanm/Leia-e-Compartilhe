
using LeiaCompartilhe.API.DTOs;

namespace LeiaCompartilhe.API.Interfaces;

public interface ILivroService
{
    Task<LivroRespostaDto> CadastrarAsync(LivroCadastroDto dto);

    Task<List<LivroRespostaDto>> ListarAsync();

    Task<LivroRespostaDto?> BuscarPorIdAsync(int id);
}
