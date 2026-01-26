using FluentValidation;
using VehicleTaxonomy.Infrastructure.Db;

namespace VehicleTaxonomy.Domain.Variants;

public class DeleteVariantCommandValidator : AbstractValidator<DeleteVariantCommand>
{
    public DeleteVariantCommandValidator()
    {
        RuleFor(c => c.MakeId).NotEmpty().IsSlugId(VehicleTaxonomyContainerDefinition.MakeNameMaxLength);
        RuleFor(c => c.ModelId).NotEmpty().IsSlugId(VehicleTaxonomyContainerDefinition.ModelNameMaxLength);
        RuleFor(c => c.VariantId).NotEmpty().IsSlugId(VehicleTaxonomyContainerDefinition.VariantNameMaxLength);
    }
}
