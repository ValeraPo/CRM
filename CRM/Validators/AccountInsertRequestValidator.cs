using CRM.APILayer.Models;
using FluentValidation;

namespace CRM.APILayer.Validation
{
    public class AccountInsertRequestValidator : AbstractValidator<AccountInsertRequest>
    {
        public AccountInsertRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Name).MaximumLength(20);
            RuleFor(x => x.CurrencyType).NotEmpty();
            RuleFor(x => x.CurrencyType).InclusiveBetween(1,113);
        }
    }
}
