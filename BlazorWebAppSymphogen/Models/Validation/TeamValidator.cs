using FluentValidation;

namespace BlazorWebAppSymphogen.Models.Validation;

public class TeamValidator : BaseValidator<Team>
{
    public TeamValidator()
    {
        RuleFor(team => team.Name)
            .NotEmpty()
            .WithMessage("'Team Name' is required.");
    }
}
