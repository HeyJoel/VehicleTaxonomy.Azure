using FluentValidation;
using VehicleTaxonomy.Infrastructure.Db;

namespace VehicleTaxonomy.Domain.Makes;

public class AddMakeCommandValidator : AbstractValidator<AddMakeCommand>
{
    public AddMakeCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(VehicleTaxonomyContainerDefinition.MakeNameMaxLength);
    }
}
