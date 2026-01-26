using FluentValidation;
using VehicleTaxonomy.Infrastructure.Db;

namespace VehicleTaxonomy.Domain.Models;

public class AddModelCommandValidator : AbstractValidator<AddModelCommand>
{
    public AddModelCommandValidator()
    {
        RuleFor(c => c.MakeId).NotEmpty().IsSlugId(VehicleTaxonomyContainerDefinition.MakeNameMaxLength);
        RuleFor(c => c.Name).NotEmpty().MaximumLength(VehicleTaxonomyContainerDefinition.ModelNameMaxLength);
    }
}
