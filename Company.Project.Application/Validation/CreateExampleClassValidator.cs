using Company.Project.Application.DTO;
using FluentValidation;

namespace Company.Project.Application.Validation
{
    public class CreateExampleClassValidator : AbstractValidator<CreateExampleClassDto>
    {
        public CreateExampleClassValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");
        }
    }
}
