using VehicleTaxonomy.Azure.Infrastructure.Db;

namespace VehicleTaxonomy.Azure.Domain.Variants;

public class ListVariantsQueryHandler
{
    private readonly IVehicleTaxonomyRepository _vehicleTaxonomyRepository;

    public ListVariantsQueryHandler(
        IVehicleTaxonomyRepository vehicleTaxonomyRepository
        )
    {
        _vehicleTaxonomyRepository = vehicleTaxonomyRepository;
    }

    public async Task<QueryResponse<IReadOnlyCollection<Variant>>> ExecuteAsync(ListVariantsQuery query, CancellationToken cancellationToken = default)
    {
        var dbResults = await _vehicleTaxonomyRepository.ListAsync(
            VehicleTaxonomyEntity.Variant,
            query.MakeId,
            query.ModelId,
            cancellationToken: cancellationToken
            );

        var results = dbResults
            .Select(r => new Variant()
            {
                VariantId = r.PublicId,
                Name = r.Name,
                EngineSizeInCC = r.VariantData?.EngineSizeInCC,
                FuelCategory = ParseFuelCategory(r.VariantData?.FuelCategory)
            })
            .ToArray();

        return QueryResponse<IReadOnlyCollection<Variant>>.Success(results);
    }

    private static FuelCategory? ParseFuelCategory(string? dbFuelCategory)
    {
        if (string.IsNullOrEmpty(dbFuelCategory))
        {
            return null;
        }

        return EnumParser.ParseOrDefault<FuelCategory>(dbFuelCategory, FuelCategory.Other);
    }
}
