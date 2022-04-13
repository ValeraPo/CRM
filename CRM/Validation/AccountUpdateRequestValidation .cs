using CRM.APILayer.Models;
using FluentValidation;

namespace CRM.APILayer.Validation
{
    public class AccountUpdateRequestValidation : AbstractValidator<AccountUpdateRequest>
    {
        public AccountUpdateRequestValidation()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Name).MaximumLength(20);
        }
    }
}
