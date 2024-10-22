using DWS.Console.Areas.Tasks;
using Xunit;

namespace DWS.Console.Tests.Areas.Tasks
{
    public class YardViewModelTests
    {
        [Fact]
        public void YardViewModel_ShouldSetClientName()
        {
            // Arrange
            var yard = new YardViewModel(new Yard(new Client(new Supplier(new User(), Guid.NewGuid()), Guid.NewGuid()), Guid.NewGuid()));

            // Act
            yard.ClientName = "Test Client";

            // Assert
            Assert.Equal("Test Client", yard.ClientName);
        }

        [Fact]
        public void YardViewModel_ShouldSetYardName()
        {
            // Arrange
            var yard = new YardViewModel(new Yard(new Client(new Supplier(new User(), Guid.NewGuid()), Guid.NewGuid()), Guid.NewGuid()));

            // Act
            yard.YardName = "Test Yard";

            // Assert
            Assert.Equal("Test Yard", yard.YardName);
        }

        [Fact]
        public void YardViewModel_ShouldSetStreet()
        {
            // Arrange
            var yard = new YardViewModel(new Yard(new Client(new Supplier(new User(), Guid.NewGuid()), Guid.NewGuid()), Guid.NewGuid()));

            // Act
            yard.Street = "Test Street";

            // Assert
            Assert.Equal("Test Street", yard.Street);
        }

        [Fact]
        public void YardViewModel_ShouldSetNumber()
        {
            // Arrange
            var yard = new YardViewModel(new Yard(new Client(new Supplier(new User(), Guid.NewGuid()), Guid.NewGuid()), Guid.NewGuid()));

            // Act
            yard.Number = "123";

            // Assert
            Assert.Equal("123", yard.Number);
        }

        [Fact]
        public void YardViewModel_ShouldSetPostalCode()
        {
            // Arrange
            var yard = new YardViewModel(new Yard(new Client(new Supplier(new User(), Guid.NewGuid()), Guid.NewGuid()), Guid.NewGuid()));

            // Act
            yard.PostalCode = "12345";

            // Assert
            Assert.Equal("12345", yard.PostalCode);
        }

        [Fact]
        public void YardViewModel_ShouldSetCity()
        {
            // Arrange
            var yard = new YardViewModel(new Yard(new Client(new Supplier(new User(), Guid.NewGuid()), Guid.NewGuid()), Guid.NewGuid()));

            // Act
            yard.City = "Test City";

            // Assert
            Assert.Equal("Test City", yard.City);
        }

        [Fact]
        public void YardViewModel_ShouldSetCountry()
        {
            // Arrange
            var yard = new YardViewModel(new Yard(new Client(new Supplier(new User(), Guid.NewGuid()), Guid.NewGuid()), Guid.NewGuid()));

            // Act
            yard.Country = "Test Country";

            // Assert
            Assert.Equal("Test Country", yard.Country);
        }
    }
}
