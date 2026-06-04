using Moq;
using MyTelegram.Services.TLObjectConverters;

namespace MyTelegram.Services.Tests.TLObjectConverters;

public class LayeredServiceTests : TestsFor<LayeredService<ILayeredConverter>>
{
    private List<ILayeredConverter> _converters = [];

    private void SetupConverters(List<int> layers)
    {
        _converters =
        [
            .. layers.Select(layer =>
            {
                var mockConverter = new Mock<ILayeredConverter>();
                mockConverter.SetupGet(c => c.Layer).Returns(layer);
                return mockConverter.Object;
            })
        ];
    }

    protected override LayeredService<ILayeredConverter> CreateSut()
    {
        return new LayeredService<ILayeredConverter>(_converters);
    }

    [Theory]
    [InlineData(new int[] { 180, 184, 195, 198 }, 0, 198)]
    [InlineData(new int[] { 180, 184, 195, 198 }, 184, 184)]
    [InlineData(new int[] { 180, 184, 195, 198 }, 100, 180)]
    [InlineData(new int[] { 180, 184, 195, 198 }, 250, 198)]
    [InlineData(new int[] { 180, 184, 195, 198 }, 190, 195)]
    [InlineData(new int[] { 180, 191, 195, 198 }, 187, 191)]
    [InlineData(new int[] { 184, 195, 198 }, 187, 195)]

    public void GetConverter_ShouldReturnExpectedLayer(int[] layers, int requestLayer, int expectedLayer)
    {
        SetupConverters([.. layers]);
        var sut = CreateSut();

        var result = sut.GetConverter(requestLayer);

        result.Layer.ShouldBe(expectedLayer);
    }

    //public LayeredServiceTests()
    //{
    //    var mockConverter1 = new Mock<ILayeredConverter>();
    //    var mockConverter2 = new Mock<ILayeredConverter>();
    //    var mockConverter3 = new Mock<ILayeredConverter>();
    //    var mockConverter4 = new Mock<ILayeredConverter>();

    //    mockConverter1.SetupGet(c => c.Layer).Returns(180);
    //    mockConverter2.SetupGet(c => c.Layer).Returns(184);
    //    mockConverter3.SetupGet(c => c.Layer).Returns(195);
    //    mockConverter4.SetupGet(c => c.Layer).Returns(198);

    //    Inject<IEnumerable<ILayeredConverter>>(new List<ILayeredConverter>([mockConverter1.Object, mockConverter2.Object, mockConverter3.Object,
    //        mockConverter4.Object]));
    //}

    //[Fact]
    //public void Constructor_ShouldInitializeCorrectly()
    //{
    //    Sut.Converter.Layer.ShouldBe(198);
    //}

    //[Fact]
    //public void GetConverter_ShouldReturnLatestLayerConverter_WhenLayerIsZero()
    //{
    //    var result = Sut.GetConverter(0);
    //    result.Layer.ShouldBe(198);
    //}

    //[Fact]
    //public void GetConverter_ShouldReturnExactMatch_WhenLayerExists()
    //{
    //    var result = Sut.GetConverter(184);
    //    result.Layer.ShouldBe(184);
    //}

    //[Fact]
    //public void GetConverter_ShouldReturnMinLayerConverter_WhenLayerBelowMin()
    //{
    //    var result = Sut.GetConverter(100);
    //    result.Layer.ShouldBe(180);
    //}

    //[Fact]
    //[Theory]
    //public void GetConverter_ShouldReturnMaxLayerConverter_WhenLayerAboveMax()
    //{
    //    var result = Sut.GetConverter(250);
    //    result.Layer.ShouldBe(198);
    //}

    //[Fact]
    //public void GetConverter_ShouldReturnClosestLowerLayer_WhenLayerNotExists()
    //{
    //    var result = Sut.GetConverter(190);
    //    result.Layer.ShouldBe(184);
    //}
}