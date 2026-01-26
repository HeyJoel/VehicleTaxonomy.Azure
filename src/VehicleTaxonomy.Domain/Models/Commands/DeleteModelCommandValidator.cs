using FluentValidation;
using VehicleTaxonomy.Infrastructure.Db;

namespace VehicleTaxonomy.Domain.Models;

public class DeleteModelCommandValidator : AbstractValidator<DeleteModelCommand>
{
    public DeleteModelCommandValidator()
    {
        RuleFor(c => c.MakeId).NotEmpty().IsSlugId(VehicleTaxonomyContainerDefinition.MakeNameMaxLength);
        RuleFor(c => c.ModelId).NotEmpty().IsSlugId(VehicleTaxonomyContainerDefinition.ModelNameMaxLength);
    }
}
