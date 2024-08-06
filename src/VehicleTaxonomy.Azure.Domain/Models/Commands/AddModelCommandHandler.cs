using VehicleTaxonomy.Azure.Infrastructure.Db;

namespace VehicleTaxonomy.Azure.Domain.Models;

public class AddModelCommandHandler
{
    private readonly IVehicleTaxonomyRepository _vehicleTaxonomyRepository;
    private readonly IsModelUniqueQueryHandler _isModelUniqueQueryHandler;
    private readonly TimeProvider _timeProvider;

    public AddModelCommandHandler(
        IVehicleTaxonomyRepository vehicleTaxonomyRepository,
        IsModelUniqueQueryHandler isModelUniqueQueryHandler,
        TimeProvider timeProvider
        )
    {
        _vehicleTaxonomyRepository = vehicleTaxonomyRepository;
        _isModelUniqueQueryHandler = isModelUniqueQueryHandler;
        _timeProvider = timeProvider;
    }

    public async Task<CommandResponse<AddEntityResult>> ExecuteAsync(AddModelCommand command, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(command);

        var publicId = EntityIdFormatter.Format(command.Name);
        var validationResult = await ValidateAsync(command, publicId, cancellationToken);
        if (validationResult != null)
        {
            return validationResult;
        }

        var now = _timeProvider.GetUtcNow().DateTime;
        await _vehicleTaxonomyRepository.AddAsync(new()
        {
            CreateDate = now,
            EntityType = VehicleTaxonomyEntity.Model,
            PublicId = publicId,
            ParentPath = VehicleTaxonomyPath.FormatParentPath(
                VehicleTaxonomyEntity.Model,
                command.MakeId
                ),
            Name = command.Name.Trim()
        }, cancellationToken);

        return CommandResponse<AddEntityResult>.Success(new()
        {
            Id = publicId
        });
    }

    private async Task<CommandResponse<AddEntityResult>?> ValidateAsync(
        AddModelCommand command,
        string id,
        CancellationToken cancellationToken
        )
    {
        // Basic Validation
        var validator = new AddModelCommandValidator();
        var result = validator.Validate(command);

        if (!result.IsValid)
        {
            return CommandResponse<AddEntityResult>.Error(result);
        }

        // Id Validation
        if (string.IsNullOrEmpty(id))
        {
            return CommandResponse<AddEntityResult>.Error(
                StandardErrorMessages.NameCouldNotBeFormattedAsAnId,
                nameof(command.Name)
                );
        }

        // Parent make exists
        var make = await _vehicleTaxonomyRepository.GetByIdAsync(
            VehicleTaxonomyEntity.Make,
            command.MakeId,
            null,
            null,
            cancellationToken
            );
        if (make == null)
        {
            return CommandResponse<AddEntityResult>.Error(
                "Make does not exist.",
                nameof(command.MakeId)
                );
        }

        // Uniqueness Validation
        var isUniqueResult = await _isModelUniqueQueryHandler.ExecuteAsync(new()
        {
            MakeId = command.MakeId,
            Name = command.Name
        }, cancellationToken);

        isUniqueResult.ThrowIfInvalid();

        if (!isUniqueResult.Result)
        {
            return CommandResponse<AddEntityResult>.Error(
                StandardErrorMessages.NameIsNotUnique("model"),
                nameof(command.Name));
        }

        return null;
    }
}
