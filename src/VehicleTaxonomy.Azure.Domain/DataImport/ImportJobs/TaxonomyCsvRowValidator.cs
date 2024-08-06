using FluentValidation;
using VehicleTaxonomy.Azure.Infrastructure.Db;

namespace VehicleTaxonomy.Azure.Domain.DataImport;

public class TaxonomyCsvRowValidator : AbstractValidator<TaxonomyCsvRow>
{
    public TaxonomyCsvRowValidator()
    {
        // Note that the slug will never be longer than the name length so we don't need to
        // validate it here, preventing duplicate errors
        RuleFor(c => c.MakeId).NotEmpty().IsSlugId();
        RuleFor(c => c.MakeName).NotEmpty().MaximumLength(VehicleTaxonomyContainerDefinition.MakeNameMaxLength).WithMessage(StandardErrorMessages.StringMaxLength);

        RuleFor(c => c.ModelId).NotEmpty().IsSlugId();
        RuleFor(c => c.ModelName).NotEmpty().MaximumLength(VehicleTaxonomyContainerDefinition.ModelNameMaxLength).WithMessage(StandardErrorMessages.StringMaxLength);

        RuleFor(c => c.VariantId).NotEmpty().IsSlugId();
        RuleFor(c => c.VariantName).NotEmpty().MaximumLength(VehicleTaxonomyContainerDefinition.VariantNameMaxLength).WithMessage(StandardErrorMessages.StringMaxLength);

        RuleFor(c => c.EngineSizeInCC).LessThan(50000);
    }
}
