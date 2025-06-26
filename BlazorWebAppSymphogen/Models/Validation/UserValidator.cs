using FluentValidation;
using System.Text.RegularExpressions;

namespace BlazorWebAppSymphogen.Models.Validation;

public class UserValidator : BaseValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.FullName)
            .NotEmpty().WithMessage("'Full Name' must not be empty.")
            .Must(fullName =>
                !string.IsNullOrWhiteSpace(fullName) &&
                fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length >= 2
            ).WithMessage("'Full Name' must contain at least a first and last name.");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("'Full Name' must not be empty.")
            .EmailAddress().WithMessage("'Email' is not a valid email address.")
            .Must(email =>
                !string.IsNullOrWhiteSpace(email) &&
                Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$")
            ).WithMessage("'Email' is not a valid email address.");

        RuleFor(user => user.Department).NotEmpty().WithMessage("'Department' is required.");

        RuleFor(user => user.Location).NotEmpty().WithMessage("'Location' is required.");

        RuleFor(user => user.JobTitle).NotEmpty().WithMessage("'Job Title' is required.");

        RuleFor(user => user.OfficePhoneNumber)
            .Matches(@"^(\+?\d{1,4}|\(?\d{1,4}\)?)[\s\-]?(\d{1,4}[\s\-]?){1,5}$")
            .When(user => !string.IsNullOrWhiteSpace(user.OfficePhoneNumber))
            .WithMessage("Invalid phone number format.");

        RuleFor(user => user.Initials)
            .NotEmpty()
            .WithMessage("'Initials' are required.");

        RuleFor(user => user.Teams)
             .Must(teams =>
             {
                    if (teams == null || teams.Count == 0)
                        return true; // Allow empty teams
    
                    // Check if all teams exist
                    return teams.All(team => team.TeamExists);
             }).WithMessage("'Teams' cannot include teams that do not exist.")
             .Must(teams =>
             {
                 if (teams == null || teams.Count == 0)
                     return true; // Allow empty teams

                 // Check for duplicate teams
                 return teams
                     .GroupBy(team => team.Id)
                     .All(group => group.Count() == 1);
             }).WithMessage("'Teams' cannot include duplicates.");
    }
}
