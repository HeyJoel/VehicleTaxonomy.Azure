namespace VehicleTaxonomy.Azure.Infrastructure;

public class CsvFileSplitterResult
{
    /// <summary>
    /// 1-based incrementing counter representing this CSV
    /// file within the batch.
    /// </summary>
    public int BatchNumber { get; set; }

    /// <summary>
    /// 1-based number of data rows in the CSV file. Does not include
    /// the header row.
    /// </summary>
    public int NumRows { get; set; }

    /// <summary>
    /// A stream representing the CSV file. The <see cref="CsvFileSplitter"/>
    /// will handle disposing of the stream so you don't have to.
    /// </summary>
    public Stream File { get; set; } = Stream.Null;
}
