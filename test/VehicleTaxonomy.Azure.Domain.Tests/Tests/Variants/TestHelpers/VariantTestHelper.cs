using VehicleTaxonomy.Azure.Domain.Tests.Models;
using VehicleTaxonomy.Azure.Domain.Variants;
using VehicleTaxonomy.Azure.Infrastructure.Db;

namespace VehicleTaxonomy.Azure.Domain.Tests.Variants;

public class VariantTestHelper
{
    private readonly IVehicleTaxonomyRepository _vehicleTaxonomyRepository;
    private readonly AddVariantCommandHandler _addVariantCommandHandler;
    private readonly ModelTestHelper _modelTestHelper;

    public VariantTestHelper(
        IVehicleTaxonomyRepository vehicleTaxonomyRepository,
        AddVariantCommandHandler addVariantCommandHandler,
        ModelTestHelper modelTestHelper
        )
    {
        _vehicleTaxonomyRepository = vehicleTaxonomyRepository;
        _addVariantCommandHandler = addVariantCommandHandler;
        _modelTestHelper = modelTestHelper;
    }

    /// <summary>
    /// Returns the raw CosmosDb record data for a vehicle variant.
    /// </summary>
    public async Task<VehicleTaxonomyDocument?> GetRawDocumentAsync(string makeId, string modelId, string id)
    {
        var result = await _vehicleTaxonomyRepository.GetByIdAsync(VehicleTaxonomyEntity.Variant, id, makeId, modelId);

        return result;
    }

    public async Task<(string, string)> AddModelWithMakeAsync(string name)
    {
        var makeId = await _modelTestHelper.AddMakeAsync(name + "mk");
        var modelId = await _modelTestHelper.AddModelAsync(makeId, name + "md");

        return (makeId, modelId);
    }

    public async Task<string> AddVariantAsync(string makeId, string modelId, string name, Action<AddVariantCommand>? configureCommand = null)
    {
        var command = new AddVariantCommand()
        {
            MakeId = makeId,
            ModelId = modelId,
            Name = name
        };

        configureCommand?.Invoke(command);
        var result = await _addVariantCommandHandler.ExecuteAsync(command);
        result.ThrowIfInvalid();

        return result.Result!.Id;
    }
}
