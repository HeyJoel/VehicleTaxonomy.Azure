namespace VehicleTaxonomy.Azure.Infrastructure.Tests.CsvFileSplitter;

using FluentAssertions.Execution;
using VehicleTaxonomy.Azure.Infrastructure;

public class CsvFileSplitterTests
{
    [Fact]
    public async Task WhenHeaderOnly_NoResults()
    {
        using var file = GetTestFileSourceAsync(nameof(WhenHeaderOnly_NoResults));

        List<CsvFileSplitterResult> results = [];
        await CsvFileSplitter.SplitAsync(file, 1, r =>
        {
            results.Add(r);

            return Task.CompletedTask;
        });

        results.Should().HaveCount(0);
    }

    [Fact]
    public async Task CanSplit()
    {
        using var file = GetTestFileSourceAsync(nameof(CanSplit));

        List<Tuple<CsvFileSplitterResult, string>> results = [];
        await CsvFileSplitter.SplitAsync(file, 3, r =>
        {
            using var reader = new StreamReader(r.File);
            results.Add(new(r, reader.ReadToEnd()));

            return Task.CompletedTask;
        });

        using (new AssertionScope())
        {
            results.Should().HaveCount(3);

            var result1 = results[0];
            result1.Item1.BatchNumber.Should().Be(1);
            result1.Item1.NumRows.Should().Be(3);
            InlineSnapshot.Validate(result1.Item2, """
                col1,col2
                1,One
                2,Two
                3,Three

                """);

            var result2 = results[1];
            result2.Item1.BatchNumber.Should().Be(2);
            result2.Item1.NumRows.Should().Be(3);
            InlineSnapshot.Validate(result2.Item2, """
                col1,col2
                4,Four
                5,Five
                6,Six

                """);

            var result3 = results[2];
            result3.Item1.BatchNumber.Should().Be(3);
            result3.Item1.NumRows.Should().Be(2);
            InlineSnapshot.Validate(result3.Item2, """
                col1,col2
                7,Seven
                8,Eight

                """);
        }
    }

    private Stream GetTestFileSourceAsync(string testName)
    {
        const string directory = "VehicleTaxonomy.Azure.Infrastructure.Tests.CsvFileSplitter.TestResources";

        var fullPath = $"{directory}.{nameof(CsvFileSplitterTests)}_{testName}.csv";
        var stream = GetType().Assembly.GetManifestResourceStream(fullPath);

        if (stream == null)
        {
            throw new FileNotFoundException($"Embedded resource could not be found at path {fullPath}", fullPath);
        }

        return stream;
    }
}
