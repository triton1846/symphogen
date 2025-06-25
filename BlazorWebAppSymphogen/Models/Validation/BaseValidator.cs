using FluentValidation;

namespace BlazorWebAppSymphogen.Models.Validation;

public abstract class BaseValidator<T> : AbstractValidator<T>
{
    public virtual Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
    {
        if (propertyName.StartsWith("Validation"))
            propertyName = propertyName.Replace("Validation", string.Empty);

        var result = await ValidateAsync(ValidationContext<T>.CreateWithOptions((T)model, x => x.IncludeProperties(propertyName)));
        if (result.IsValid)
            return [];

        return result.Errors.Select(e => e.ErrorMessage);
    };
}
