﻿using FluentValidation;

namespace BlazorWebAppSymphogen.Models.Validation;

public class TeamValidator : BaseValidator<Team>
{
    public TeamValidator()
    {
        RuleFor(team => team.Name)
            .NotEmpty()
            .WithMessage("'Team Name' is required.");

        RuleFor(team => team.Users)
            .Must(users =>
            {
                if (users == null || users.Count == 0)
                    return true; // Allow empty users

                // Check if all users exist
                return users.All(user => user.UserExists);
            }).WithMessage("'Users' cannot include users that do not exist.")
            .Must(users =>
            {
                if (users == null || users.Count == 0)
                    return true; // Allow empty users

                // Check for duplicate users
                return users
                    .GroupBy(user => user.Id)
                    .All(group => group.Count() == 1);
            }).WithMessage("'Users' cannot include duplicates.");

        RuleFor(team => team.SuperUsers)
            .Must(superUsers =>
            {
                if (superUsers == null || superUsers.Count == 0)
                    return true; // Allow empty super users

                // Check if all super users exist
                return superUsers.All(superUser => superUser.UserExists);
            }).WithMessage("'Super Users' cannot include users that do not exist.")
            .Must(superUsers =>
            {
                if (superUsers == null || superUsers.Count == 0)
                    return true; // Allow empty super users

                // Check for duplicate super users
                return superUsers
                    .GroupBy(superUser => superUser.Id)
                    .All(group => group.Count() == 1);
            }).WithMessage("'Super Users' cannot include duplicates.");
    }
}
