using VehicleTaxonomy.Azure.Infrastructure.Db;

namespace VehicleTaxonomy.Azure.Domain.Variants;

public class AddVariantCommandHandler
{
    private readonly IVehicleTaxonomyRepository _vehicleTaxonomyRepository;
    private readonly IsVariantUniqueQueryHandler _isVariantUniqueQueryHandler;
    private readonly TimeProvider _timeProvider;

    public AddVariantCommandHandler(
        IVehicleTaxonomyRepository vehicleTaxonomyRepository,
        IsVariantUniqueQueryHandler isVariantUniqueQueryHandler,
        TimeProvider timeProvider
        )
    {
        _vehicleTaxonomyRepository = vehicleTaxonomyRepository;
        _isVariantUniqueQueryHandler = isVariantUniqueQueryHandler;
        _timeProvider = timeProvider;
    }

    public async Task<CommandResponse<AddEntityResult>> ExecuteAsync(AddVariantCommand command, CancellationToken cancellationToken = default)
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
            EntityType = VehicleTaxonomyEntity.Variant,
            PublicId = publicId,
            ParentPath = VehicleTaxonomyPath.FormatParentPath(
                VehicleTaxonomyEntity.Variant,
                command.MakeId,
                command.ModelId
                ),
            Name = command.Name.Trim(),
            VariantData = new()
            {
                EngineSizeInCC = command.EngineSizeInCC > 0 ? command.EngineSizeInCC : null,
                FuelCategory = command.FuelCategory?.ToString()
            }
        }, cancellationToken);

        return CommandResponse<AddEntityResult>.Success(new()
        {
            Id = publicId
        });
    }

    private async Task<CommandResponse<AddEntityResult>?> ValidateAsync(
        AddVariantCommand command,
        string id,
        CancellationToken cancellationToken
        )
    {
        // Basic Validation
        var validator = new AddVariantCommandValidator();
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

        // Parent model exists
        var model = await _vehicleTaxonomyRepository.GetByIdAsync(
            VehicleTaxonomyEntity.Model,
            command.ModelId,
            command.MakeId,
            null,
            cancellationToken
            );
        if (model == null)
        {
            return CommandResponse<AddEntityResult>.Error(
                "Model does not exist.",
                nameof(command.ModelId)
                );
        }

        // Uniqueness Validation
        var isUniqueResult = await _isVariantUniqueQueryHandler.ExecuteAsync(new()
        {
            MakeId = command.MakeId,
            ModelId = command.ModelId,
            Name = command.Name
        }, cancellationToken);

        isUniqueResult.ThrowIfInvalid();

        if (!isUniqueResult.Result)
        {
            return CommandResponse<AddEntityResult>.Error(
                StandardErrorMessages.NameIsNotUnique("variant"),
                nameof(command.Name));
        }

        return null;
    }
}
