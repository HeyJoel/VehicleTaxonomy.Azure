using VehicleTaxonomy.Azure.Domain.Models;
using VehicleTaxonomy.Azure.Domain.Tests.Makes;
using VehicleTaxonomy.Azure.Infrastructure.Db;

namespace VehicleTaxonomy.Azure.Domain.Tests.Models;

public class ModelTestHelper
{
    private readonly IVehicleTaxonomyRepository _vehicleTaxonomyRepository;
    private readonly AddModelCommandHandler _addModelCommandHandler;
    private readonly MakeTestHelper _makeTestHelper;

    public ModelTestHelper(
        IVehicleTaxonomyRepository vehicleTaxonomyRepository,
        AddModelCommandHandler addModelCommandHandler,
        MakeTestHelper makeTestHelper
        )
    {
        _vehicleTaxonomyRepository = vehicleTaxonomyRepository;
        _addModelCommandHandler = addModelCommandHandler;
        _makeTestHelper = makeTestHelper;
    }

    /// <summary>
    /// Returns the raw CosmosDb record data for a vehicle model.
    /// </summary>
    public async Task<VehicleTaxonomyDocument?> GetRawDocumentAsync(string makeId, string id)
    {
        var result = await _vehicleTaxonomyRepository.GetByIdAsync(VehicleTaxonomyEntity.Model, id, makeId, null);

        return result;
    }

    public async Task<string> AddMakeAsync(string name)
    {
        return await _makeTestHelper.AddMakeAsync(name);
    }

    public async Task<string> AddModelAsync(string makeId, string name)
    {
        var result = await _addModelCommandHandler.ExecuteAsync(new()
        {
            MakeId = makeId,
            Name = name
        });

        result.ThrowIfInvalid();

        return result.Result!.Id;
    }
}
