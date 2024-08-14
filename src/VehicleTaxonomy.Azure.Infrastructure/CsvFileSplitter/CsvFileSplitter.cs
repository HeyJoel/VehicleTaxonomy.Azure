using System.Text;

namespace VehicleTaxonomy.Azure.Infrastructure;

public static class CsvFileSplitter
{
    public static async Task SplitAsync(
        Stream inputCsv,
        int batchSize,
        Func<CsvFileSplitterResult, Task> fileWriter,
        CancellationToken cancellationToken = default
        )
    {
        ArgumentNullException.ThrowIfNull(inputCsv);
        ArgumentNullException.ThrowIfNull(fileWriter);
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(batchSize, 0);

        using var reader = new StreamReader(inputCsv);
        var headerRow = reader.ReadLine();

        var batchNumber = 0;

        while (!reader.EndOfStream)
        {
            batchNumber++;

            var numRows = 0;
            var csv = new StringBuilder();
            csv.AppendLine(headerRow);

            for (var i = 0; i < batchSize && !reader.EndOfStream; i++)
            {
                var row = await reader.ReadLineAsync(cancellationToken);
                csv.AppendLine(row);
                numRows++;
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            using var outputStream = new MemoryStream(bytes);
            outputStream.Position = 0;

            var result = new CsvFileSplitterResult()
            {
                BatchNumber = batchNumber,
                NumRows = numRows,
                File = outputStream
            };

            await fileWriter(result);
        }
    }
}
