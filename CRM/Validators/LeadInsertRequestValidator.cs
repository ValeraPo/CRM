using CRM.APILayer.Models;
using FluentValidation;
using System.Text.RegularExpressions;

namespace CRM.APILayer.Validation
{
    public class LeadInsertRequestValidator : AbstractValidator<LeadInsertRequest>
    {
        public  LeadInsertRequestValidator()
        {
           RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is empty")
                .MinimumLength(8)
                .WithMessage("Minimum lenght of Password 8 symbols")
                .MaximumLength(30)
                .WithMessage("Maximum lenght of Password 30 symbols");
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("Email is empty")
                .EmailAddress()
                .WithMessage("Email address is not valid");
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
