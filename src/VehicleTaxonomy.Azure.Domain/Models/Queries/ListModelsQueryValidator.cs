using FluentValidation;
using VehicleTaxonomy.Azure.Infrastructure.Db;

namespace VehicleTaxonomy.Azure.Domain.Models;

public class ListModelsQueryValidator : AbstractValidator<ListModelsQuery>
{
    public ListModelsQueryValidator()
    {
        RuleFor(c => c.MakeId).NotEmpty().IsSlugId(VehicleTaxonomyContainerDefinition.MakeNameMaxLength);
    }
}
