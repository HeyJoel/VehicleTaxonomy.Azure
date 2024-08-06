using FluentValidation;
using VehicleTaxonomy.Azure.Infrastructure.Db;

namespace VehicleTaxonomy.Azure.Domain.Makes;

public class IsMakeUniqueQueryValidator : AbstractValidator<IsMakeUniqueQuery>
{
    public IsMakeUniqueQueryValidator()
    {
        RuleFor(c => c.Name).NotEmpty().MaximumLength(VehicleTaxonomyContainerDefinition.MakeNameMaxLength);
    }
}
