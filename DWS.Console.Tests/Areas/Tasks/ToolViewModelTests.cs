using DWS.Console.Areas.Tasks;
using Xunit;

namespace DWS.Console.Tests.Areas.Tasks
{
    public class ToolViewModelTests
    {
        [Fact]
        public void ToolViewModel_ShouldSetName()
        {
            // Arrange
            var tool = new ToolViewModel(new Tool(new Supplier(new User(), Guid.NewGuid()), Guid.NewGuid()));

            // Act
            tool.Name = "Test Tool";

            // Assert
            Assert.Equal("Test Tool", tool.Name);
        }
    }
}
