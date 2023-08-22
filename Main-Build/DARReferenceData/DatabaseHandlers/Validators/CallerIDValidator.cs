using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DARReferenceData.ViewModels;
using FluentValidation;

namespace DARReferenceData.DatabaseHandlers.Validators
{
    public class CallerIDValidator : AbstractValidator<CallerIDViewModel>
    {
        public CallerIDValidator()
        {
            RuleFor(x => x.CallerID).NotEmpty()
                .Length(7, 250).WithMessage("{Propertyname}: {PropertyValue} must be between {MinLength} and {MaxLength}");

            RuleFor(x => x.EmailAddress).EmailAddress();
        }
    }
}