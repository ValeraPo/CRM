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
                .MaximumLength(20);
            RuleFor(x => x.CurrencyType)
                .NotEmpty()
                .InclusiveBetween(1,113);
        }
    }
}
