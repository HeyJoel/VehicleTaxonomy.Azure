using FluentValidation;
using VehicleTaxonomy.Azure.Infrastructure.Db;

namespace VehicleTaxonomy.Azure.Domain.Makes;

public class AddMakeCommandValidator : AbstractValidator<AddMakeCommand>
{
    public AddMakeCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(VehicleTaxonomyContainerDefinition.MakeNameMaxLength);
    }
}
