using VehicleTaxonomy.Azure.Infrastructure.Db;

namespace VehicleTaxonomy.Azure.Domain.Models;

public class ListModelsQueryHandler
{
    private readonly IVehicleTaxonomyRepository _vehicleTaxonomyRepository;

    public ListModelsQueryHandler(
        IVehicleTaxonomyRepository vehicleTaxonomyRepository
        )
    {
        _vehicleTaxonomyRepository = vehicleTaxonomyRepository;
    }

    public async Task<QueryResponse<IReadOnlyCollection<Model>>> ExecuteAsync(ListModelsQuery query, CancellationToken cancellationToken = default)
    {
        IEnumerable<Infrastructure.Db.VehicleTaxonomyDocument> dbResults = await _vehicleTaxonomyRepository.ListAsync(
            VehicleTaxonomyEntity.Model,
            query.MakeId,
            null,
            cancellationToken: cancellationToken
            );

        var results = dbResults
            .Select(r => new Model()
            {
                ModelId = r.PublicId,
                Name = r.Name
            })
            .ToArray();

        return QueryResponse<IReadOnlyCollection<Model>>.Success(results);
    }
}
