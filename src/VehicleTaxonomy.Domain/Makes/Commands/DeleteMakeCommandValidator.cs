using FluentValidation;
using VehicleTaxonomy.Infrastructure.Db;

namespace VehicleTaxonomy.Domain.Makes;

public class DeleteMakeCommandValidator : AbstractValidator<DeleteMakeCommand>
{
    public DeleteMakeCommandValidator()
    {
        RuleFor(c => c.MakeId).NotEmpty().IsSlugId(VehicleTaxonomyContainerDefinition.MakeNameMaxLength);
    }
}
