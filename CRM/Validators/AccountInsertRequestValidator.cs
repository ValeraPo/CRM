using CRM.APILayer.Models;
using FluentValidation;

namespace CRM.APILayer.Validation
{
    public class AccountInsertRequestValidator : AbstractValidator<AccountInsertRequest>
    {
        public AccountInsertRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is empty")
                .MaximumLength(20)
                .WithMessage("Maximum lenght of Name 20 symbols");
            RuleFor(x => x.CurrencyType)
                .NotEmpty()
                .WithMessage("Currency Type is empty")
                .InclusiveBetween(1,113)
                .WithMessage("Currency Type is from 1 to 113");
        }
    }
}
