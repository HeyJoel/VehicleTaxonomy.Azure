using FluentValidation;
using VehicleTaxonomy.Infrastructure.Db;

namespace VehicleTaxonomy.Domain.Models;

public class IsModelUniqueQueryValidator : AbstractValidator<IsModelUniqueQuery>
{
    public IsModelUniqueQueryValidator()
    {
        RuleFor(c => c.MakeId).NotEmpty().IsSlugId(VehicleTaxonomyContainerDefinition.MakeNameMaxLength);
        RuleFor(c => c.Name).NotEmpty().MaximumLength(VehicleTaxonomyContainerDefinition.ModelNameMaxLength);
    }
}
