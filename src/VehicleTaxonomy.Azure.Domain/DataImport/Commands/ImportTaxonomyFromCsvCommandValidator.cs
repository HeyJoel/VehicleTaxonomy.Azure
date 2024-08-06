using FluentValidation;

namespace VehicleTaxonomy.Azure.Domain.DataImport;

public class ImportTaxonomyFromCsvCommandValidator : AbstractValidator<ImportTaxonomyFromCsvCommand>
{
    public ImportTaxonomyFromCsvCommandValidator()
    {
        RuleFor(c => c.File).NotNull();
    }
}
