using VehicleTaxonomy.Azure.Domain.Makes;
using VehicleTaxonomy.Azure.Infrastructure.Db;

namespace VehicleTaxonomy.Azure.Domain.Tests.Makes;

public class MakeTestHelper
{
    private readonly IVehicleTaxonomyRepository _vehicleTaxonomyRepository;
    private readonly AddMakeCommandHandler _addMakeCommandHandler;

    public MakeTestHelper(
        IVehicleTaxonomyRepository vehicleTaxonomyRepository,
        AddMakeCommandHandler addMakeCommandHandler
        )
    {
        _vehicleTaxonomyRepository = vehicleTaxonomyRepository;
        _addMakeCommandHandler = addMakeCommandHandler;
    }

    /// <summary>
    /// Returns the raw CosmosDb record data for a Make.
    /// </summary>
    public async Task<VehicleTaxonomyDocument?> GetRawDocumentAsync(string id)
    {
        var result = await _vehicleTaxonomyRepository.GetByIdAsync(VehicleTaxonomyEntity.Make, id, null, null);

        return result;
    }

    public async Task<string> AddMakeAsync(string name)
    {
        var result = await _addMakeCommandHandler.ExecuteAsync(new()
        {
            Name = name
        });

        result.ThrowIfInvalid();

        return result.Result!.Id;
    }
}
