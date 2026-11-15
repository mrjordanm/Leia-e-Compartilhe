using System.ComponentModel.DataAnnotations;
using LeiaCompartilhe.API.Validations;

namespace LeiaCompartilhe.API.DTOs.Parcerias;

public class CriarParceriaDto
{
    [Required(ErrorMessage = "O nome da instituição é obrigatório.")]
    [RequiredTrimmed(ErrorMessage = "O nome da instituição não pode conter apenas espaços.")]
    [MaxLength(150)]
    public string NomeInstituicao { get; set; } = string.Empty;

    [Required(ErrorMessage = "O tipo de parceria é obrigatório.")]
    [RequiredTrimmed(ErrorMessage = "O tipo de parceria não pode conter apenas espaços.")]
    [MaxLength(100)]
    public string TipoParceria { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres.")]
    public string? Descricao { get; set; }

    [MaxLength(150, ErrorMessage = "O contato deve ter no máximo 150 caracteres.")]
    public string? Contato { get; set; }
}
