using DWS.Console.Areas.Tasks;
using DWS.Model;
using Jinaga;
using Moq;
using Xunit;

namespace DWS.Console.Tests.Areas.Tasks
{
    public class NewTaskViewModelTests
    {
        private readonly Mock<JinagaClient> mockJinagaClient;
        private readonly Mock<Supplier> mockSupplier;
        private readonly NewTaskViewModel viewModel;

        public NewTaskViewModelTests()
        {
            mockJinagaClient = new Mock<JinagaClient>();
            mockSupplier = new Mock<Supplier>();
            viewModel = new NewTaskViewModel(mockJinagaClient.Object, mockSupplier.Object);
        }

        [Fact]
        public void LoadTools_ShouldLoadToolsFromSupplier()
        {
            // Arrange
            var tools = new List<ToolViewModel>
            {
                new ToolViewModel(new Tool(mockSupplier.Object, Guid.NewGuid())) { Name = "Tool1" },
                new ToolViewModel(new Tool(mockSupplier.Object, Guid.NewGuid())) { Name = "Tool2" }
            };

            mockJinagaClient.Setup(client => client.Watch(It.IsAny<Specification<Supplier, ToolViewModel>>(), mockSupplier.Object, It.IsAny<Action<ToolViewModel>>()))
                .Callback<Specification<Supplier, ToolViewModel>, Supplier, Action<ToolViewModel>>((spec, supplier, callback) =>
                {
                    foreach (var tool in tools)
                    {
                        callback(tool);
                    }
                });

            // Act
            viewModel.LoadTools();

            // Assert
            Assert.Equal(2, viewModel.ToolCatalog.Count);
            Assert.Contains(viewModel.ToolCatalog, t => t.Name == "Tool1");
            Assert.Contains(viewModel.ToolCatalog, t => t.Name == "Tool2");
        }

        [Fact]
        public void LoadYards_ShouldLoadYardsFromSupplier()
        {
            // Arrange
            var yards = new List<YardViewModel>
            {
                new YardViewModel(new Yard(new Client(mockSupplier.Object, Guid.NewGuid()), Guid.NewGuid())) { YardName = "Yard1" },
                new YardViewModel(new Yard(new Client(mockSupplier.Object, Guid.NewGuid()), Guid.NewGuid())) { YardName = "Yard2" }
            };

            mockJinagaClient.Setup(client => client.Watch(It.IsAny<Specification<Supplier, YardViewModel>>(), mockSupplier.Object, It.IsAny<Action<YardViewModel>>()))
                .Callback<Specification<Supplier, YardViewModel>, Supplier, Action<YardViewModel>>((spec, supplier, callback) =>
                {
                    foreach (var yard in yards)
                    {
                        callback(yard);
                    }
                });

            // Act
            viewModel.LoadYards();

            // Assert
            Assert.Equal(2, viewModel.Yards.Count);
            Assert.Contains(viewModel.Yards, y => y.YardName == "Yard1");
            Assert.Contains(viewModel.Yards, y => y.YardName == "Yard2");
        }

        [Fact]
        public void UnloadTools_ShouldClearTools()
        {
            // Arrange
            viewModel.ToolCatalog.Add(new ToolViewModel(new Tool(mockSupplier.Object, Guid.NewGuid())) { Name = "Tool1" });
            viewModel.ToolCatalog.Add(new ToolViewModel(new Tool(mockSupplier.Object, Guid.NewGuid())) { Name = "Tool2" });

            // Act
            viewModel.UnloadTools();

            // Assert
            Assert.Empty(viewModel.ToolCatalog);
        }

        [Fact]
        public void UnloadYards_ShouldClearYards()
        {
            // Arrange
            viewModel.Yards.Add(new YardViewModel(new Yard(new Client(mockSupplier.Object, Guid.NewGuid()), Guid.NewGuid())) { YardName = "Yard1" });
            viewModel.Yards.Add(new YardViewModel(new Yard(new Client(mockSupplier.Object, Guid.NewGuid()), Guid.NewGuid())) { YardName = "Yard2" });

            // Act
            viewModel.UnloadYards();

            // Assert
            Assert.Empty(viewModel.Yards);
        }
    }
}
