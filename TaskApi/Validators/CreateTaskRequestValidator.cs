using FluentValidation;
using TaskApi.Contracts;

namespace TaskApi.Validators;

public sealed class CreateTaskRequestValidator
 : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title must not be empty.")
            .MaximumLength(200)
            .WithMessage("Title must contain at most 200 characters.");

        RuleFor(x => x.Priority)
            .IsInEnum()
            .When(x => x.Priority.HasValue)
            .WithMessage("Priority must be Low, Medium or High.");
    }
}