using FluentValidation;
using Company.Project.DTO.DTO.ExampleClass;

namespace Company.Project.DTO.Validation
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
