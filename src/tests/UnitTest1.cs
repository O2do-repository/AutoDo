namespace tests;

using Xunit;

namespace Tests
{
    public class SampleTests
    {
        [Fact]
        public void Addition_SimpleValues_ReturnsCorrectSum()
        {
            int a = 2;
            int b = 3;
            int result = a + b;

            Assert.Equal(5, result);
        }
    }
}
