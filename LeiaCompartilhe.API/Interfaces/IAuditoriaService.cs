namespace LeiaCompartilhe.API.Interfaces;

public interface IAuditoriaService
{
    Task RegistrarAsync(
        int usuarioResponsavelId,
        string acao,
        string entidade,
        int? entidadeId = null,
        int? usuarioAlvoId = null,
        string? detalhes = null);
}