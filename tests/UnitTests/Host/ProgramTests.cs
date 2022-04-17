using FluentAssertions;
using PurchaseOrderProcessor.Api;
using Xunit;

namespace UnitTests.Host
{
    public class ProgramTests
    {
        [Theory]
        [InlineData("Development", "skipHost")]
        [InlineData("Staging", "skipHost")]
        [InlineData("Production", "skipHost")]
        [InlineData("Other", "skipHost")]
        [InlineData("skipHost")]
        [InlineData("", "skipHost")]
        public void Test1(params string[] args)
        {
            //act
            var act = () => Program.Main(args);

            //assert
            act.Should().NotThrow();
        }
    }
}
