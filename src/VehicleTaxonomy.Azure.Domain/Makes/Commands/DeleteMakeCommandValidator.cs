using FluentValidation;
using VehicleTaxonomy.Azure.Infrastructure.Db;

namespace VehicleTaxonomy.Azure.Domain.Makes;

public class DeleteMakeCommandValidator : AbstractValidator<DeleteMakeCommand>
{
    public DeleteMakeCommandValidator()
    {
        RuleFor(c => c.MakeId).NotEmpty().IsSlugId(VehicleTaxonomyContainerDefinition.MakeNameMaxLength);
    }
}
