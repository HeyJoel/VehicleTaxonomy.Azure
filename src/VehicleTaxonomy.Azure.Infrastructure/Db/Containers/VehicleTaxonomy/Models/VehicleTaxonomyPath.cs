namespace VehicleTaxonomy.Azure.Infrastructure.Db;

public static class VehicleTaxonomyPath
{
    private const string RootPath = "/";

    public static string FormatParentPath(
        VehicleTaxonomyEntity entityType,
        string? makePublicId = null,
        string? modelPublicId = null
        )
    {
        switch (entityType)
        {
            case VehicleTaxonomyEntity.Make:
                return RootPath;
            case VehicleTaxonomyEntity.Model:
                ArgumentException.ThrowIfNullOrEmpty(makePublicId);
                return $"/{makePublicId}";
            case VehicleTaxonomyEntity.Variant:
                ArgumentException.ThrowIfNullOrEmpty(makePublicId);
                ArgumentException.ThrowIfNullOrEmpty(modelPublicId);
                return $"/{makePublicId}/{modelPublicId}";
            default:
                throw new NotImplementedException($"Unknown {nameof(VehicleTaxonomyEntity)} '{entityType}'");
        }
    }
}
