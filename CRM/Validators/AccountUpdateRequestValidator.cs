using CRM.APILayer.Models;
using FluentValidation;

namespace CRM.APILayer.Validation
{
    public class AccountUpdateRequestValidator : AbstractValidator<AccountUpdateRequest>
    {
        public AccountUpdateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(20);
        }
    }
}
