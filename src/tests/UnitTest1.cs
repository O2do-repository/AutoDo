namespace tests;

using Xunit;


public class UnitTest1
{
    [Fact]
    public void Addition_SimpleValues_ReturnsCorrectSum()
    {
        int a = 2;
        int b = 4;
        int result = a + b;

        Assert.Equal(5, result);
    }
}

