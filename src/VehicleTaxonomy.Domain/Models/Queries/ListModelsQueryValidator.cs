using FluentValidation;
using VehicleTaxonomy.Infrastructure.Db;

namespace VehicleTaxonomy.Domain.Models;

public class ListModelsQueryValidator : AbstractValidator<ListModelsQuery>
{
    public ListModelsQueryValidator()
    {
        RuleFor(c => c.MakeId).NotEmpty().IsSlugId(VehicleTaxonomyContainerDefinition.MakeNameMaxLength);
    }
}
