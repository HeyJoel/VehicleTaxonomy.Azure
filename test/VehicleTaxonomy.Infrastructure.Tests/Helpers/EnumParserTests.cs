// Copied from Cofoundry under MIT Licence
// https://github.com/cofoundry-cms/cofoundry/blob/855a525/test/Cofoundry.Core.Tests/Core/Parsers/EnumParserTests.cs

namespace VehicleTaxonomy.Infrastructure.Tests.Helpers;

public class EnumParserTests
{
    public enum TestEnum
    {
        Default = 0,
        Value1 = 1,
        Value5 = 5,
        Value10 = 10
    }

    [Fact]
    public void ParseOrNull_WhenValidString_Parses()
    {
        var result = EnumParser.ParseOrNull<TestEnum>("Value5");

        Assert.Equal(TestEnum.Value5, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Value")]
    [InlineData(null)]
    public void ParseOrNull_WhenInvalidString_ReturnsNull(string? value)
    {
        var result = EnumParser.ParseOrNull<TestEnum>(value);

        Assert.Null(result);
    }

    [Fact]
    public void ParseOrNull_WhenValidInt_Parses()
    {
        var result = EnumParser.ParseOrNull<TestEnum>(5);

        Assert.Equal(TestEnum.Value5, result);
    }

    [Fact]
    public void ParseOrNull_WhenInvalidInt_ReturnsNull()
    {
        var result = EnumParser.ParseOrNull<TestEnum>(13);

        Assert.Null(result);
    }

    [Fact]
    public void ParseOrDefault_WhenInvalidWithNoDefault_ReturnsDefault()
    {
        var result = EnumParser.ParseOrDefault<TestEnum>("Inconceivable");

        Assert.Equal(TestEnum.Default, result);
    }

    [Theory]
    [InlineData("", TestEnum.Value5)]
    [InlineData("Value", TestEnum.Value1)]
    [InlineData(null, TestEnum.Value5)]
    public void ParseOrDefault_WhenInvalidWithDefault_ReturnsSpecifiedDefault(string? value, TestEnum? defaultResult)
    {
        var result = EnumParser.ParseOrDefault<TestEnum>(value, defaultResult);

        Assert.Equal(defaultResult, result);
    }

    [Fact]
    public void ParseOrThrow_WhenValid_Parses()
    {
        var result = EnumParser.ParseOrThrow<TestEnum>(5);

        Assert.Equal(TestEnum.Value5, result);
    }

    [Fact]
    public void ParseOrThrow_WhenInvalid_Throws()
    {
        Action sut = () => EnumParser.ParseOrThrow<TestEnum>(4);

        var ex = Assert.Throws<ArgumentException>(() => EnumParser.ParseOrThrow<TestEnum>(4));
        Assert.Equal("4 is not a valid TestEnum value.", ex.Message);
    }
}
