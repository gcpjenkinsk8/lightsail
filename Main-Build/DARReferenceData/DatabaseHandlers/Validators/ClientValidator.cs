using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DARReferenceData.ViewModels;
using FluentValidation;

namespace DARReferenceData.DatabaseHandlers.Validators
{
    internal class ClientValidator : AbstractValidator<ClientViewModel>
    {
        public ClientValidator()
        {
            RuleFor(x => x.ClientName).Length(1, 250).WithMessage("{propertName}: Length {TotalLength} is invalid.");

            RuleFor(x => x.Description).MaximumLength(250).When(x => !string.IsNullOrEmpty(x.Description));
        }
    }
}