using CRM.APILayer.Models;
using FluentValidation;
using System.Text.RegularExpressions;

namespace CRM.APILayer.Validation
{
    public class LeadUpdateRequestValidator : AbstractValidator<LeadUpdateRequest>
    {
        public LeadUpdateRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is empty")
                .MaximumLength(20)
                .WithMessage("Maximum lenght of Name 20 symbols");
            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last Name is empty")
                .MaximumLength(20)
                .WithMessage("Maximum lenght of Last Name 20 symbols");
            RuleFor(x => x.BirthDate)
                .NotEmpty()
                .WithMessage("Birthday is empty");
            RuleFor(x => x.Phone)
                .Matches(new Regex(@"^\+?(?:[0-9]?){6,14}[0-9]$"))
                .WithMessage("PhoneNumber not valid");
            RuleFor(x => x.City)
               .MaximumLength(20)
               .WithMessage("Maximum lenght of City 20 symbols");
        }
    }
}
