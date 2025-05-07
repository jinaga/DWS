using Autofac;
using DWS.Console.ViewModels.TaskOverview.NewTask;
using DWS.Model;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DWS.Console.Tests.Areas.Tasks;

public class NewTaskViewModelErrorTests : AutofacTestBase
{
    [Fact]
    public async Task WhenSavingWithoutYard_CreatesNewYard()
    {
        // Arrange
        var supplier = await CreateSupplier();
        
        // Create a scope to resolve the view model
        using var scope = Container.BeginLifetimeScope(builder =>
        {
            // Register the supplier instance for this test
            builder.RegisterInstance(supplier).AsSelf();
        });
        
        // Resolve the view model from the container
        var viewModel = scope.Resolve<NewTaskViewModel>();
        viewModel.Load();
        await viewModel.Ready();
        
        // Set properties but don't select a yard
        viewModel.ClientName = "Test Client";
        viewModel.YardName = "Test Yard";
        
        // Act
        await viewModel.Save();
        
        // Assert - a new yard should have been created
        // We can verify this by checking if a new yard with the specified name exists
        var yardsQuery = Given<Supplier>.Match((supplier, facts) =>
            from client in facts.OfType<Client>()
            where client.supplier == supplier
            from yard in facts.OfType<Yard>()
            where yard.client == client
            from yardName in facts.OfType<YardName>()
            where yardName.yard == yard && yardName.value == "Test Yard"
            select yard
        );
        
        var yards = await JinagaClient.Query(yardsQuery, supplier);
        yards.Should().HaveCount(1);
    }
}