using FluentValidation;
using VehicleTaxonomy.Infrastructure.Db;

namespace VehicleTaxonomy.Domain.Variants;

public class IsVariantUniqueQueryValidator : AbstractValidator<IsVariantUniqueQuery>
{
    public IsVariantUniqueQueryValidator()
    {
        RuleFor(c => c.MakeId).NotEmpty().IsSlugId(VehicleTaxonomyContainerDefinition.MakeNameMaxLength);
        RuleFor(c => c.ModelId).NotEmpty().IsSlugId(VehicleTaxonomyContainerDefinition.ModelNameMaxLength);
        RuleFor(c => c.Name).NotEmpty().MaximumLength(VehicleTaxonomyContainerDefinition.VariantNameMaxLength);
    }
}
