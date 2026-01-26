using FluentValidation;
using VehicleTaxonomy.Infrastructure.Db;

namespace VehicleTaxonomy.Domain.Variants;

public class ListVariantsQueryValidator : AbstractValidator<ListVariantsQuery>
{
    public ListVariantsQueryValidator()
    {
        RuleFor(c => c.MakeId).NotEmpty().IsSlugId(VehicleTaxonomyContainerDefinition.MakeNameMaxLength);
        RuleFor(c => c.ModelId).NotEmpty().IsSlugId(VehicleTaxonomyContainerDefinition.ModelNameMaxLength);
    }
}
