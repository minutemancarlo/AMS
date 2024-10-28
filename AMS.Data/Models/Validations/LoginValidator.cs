using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMS.Data.Models.Entities;
using Blazor.SubtleCrypto;
using FluentValidation;
using Microsoft.AspNetCore.Components;
namespace AMS.Data.Models.Validations
{
    public class LoginValidator : AbstractValidator<UserAccount>
    {        
        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<UserAccount>.CreateWithOptions((UserAccount)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
        public LoginValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty()
                .WithMessage("Username is Required")
                .Length(1, 100);
            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage("Password is Required")
                .Length(1, 100);
        }
       
    }
}
