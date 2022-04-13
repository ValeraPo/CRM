﻿using CRM.APILayer.Models;
using FluentValidation;

namespace CRM.APILayer.Validation
{
    public class LeadChangePasswordRequestValidation : AbstractValidator<LeadChangePasswordRequest>
    {
        public LeadChangePasswordRequestValidation()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .WithMessage("New Password is empty")
                .MinimumLength(8)
                .WithMessage("Minimum lenght of Password 8 symbols")
                .MaximumLength(30)
                .WithMessage("Maximum lenght of Password 30 symbols");
        }
    }
}
