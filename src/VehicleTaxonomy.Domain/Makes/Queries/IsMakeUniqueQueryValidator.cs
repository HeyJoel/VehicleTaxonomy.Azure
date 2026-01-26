using FluentValidation;
using VehicleTaxonomy.Infrastructure.Db;

namespace VehicleTaxonomy.Domain.Makes;

public class IsMakeUniqueQueryValidator : AbstractValidator<IsMakeUniqueQuery>
{
    public IsMakeUniqueQueryValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(VehicleTaxonomyContainerDefinition.MakeNameMaxLength);
    }
}
