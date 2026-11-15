using System.ComponentModel.DataAnnotations;

namespace LeiaCompartilhe.API.Validations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public sealed class RequiredTrimmedAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        return value is string texto &&
               !string.IsNullOrWhiteSpace(texto);
    }
}
