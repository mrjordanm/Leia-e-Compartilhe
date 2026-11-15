using LeiaCompartilhe.API.DTOs;

namespace LeiaCompartilhe.API.Interfaces;

public interface IUsuarioService
{
    Task<UsuarioRespostaDto> RegistrarAsync(UsuarioRegistroDto dto);

    Task<UsuarioRespostaDto> CadastrarEducadorAsync(EducadorCadastroDto dto,int administradorId);

    Task<UsuarioLoginRespostaDto> LoginAsync(UsuarioLoginDto dto);

    Task<UsuarioPerfilDto?> BuscarPerfilAsync(int usuarioId);

    Task<List<UsuarioRespostaDto>> ListarAsync(bool incluirTodos);

    Task AlterarStatusAsync(int usuarioId, bool ativo, int administradorId);

    Task CriarAdministradorInicialAsync();
}