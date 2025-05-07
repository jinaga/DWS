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
        // Get all clients for this supplier
        var clientsQuery = Given<Supplier>.Match((supplier, facts) =>
            from client in facts.OfType<Client>()
            where client.supplier == supplier
            select client
        );
        
        var clients = await JinagaClient.Query(clientsQuery, supplier);
        
        // For each client, get yards
        bool foundYardWithName = false;
        foreach (var client in clients)
        {
            // Get yards for this client
            var yardsQuery = Given<Client>.Match((client, facts) =>
                from yard in facts.OfType<Yard>()
                where yard.client == client
                select yard
            );
            
            var yards = await JinagaClient.Query(yardsQuery, client);
            
            // For each yard, check if it has the name we're looking for
            foreach (var yard in yards)
            {
                // Get names for this yard
                var namesQuery = Given<Yard>.Match((yard, facts) =>
                    from name in facts.OfType<YardName>()
                    where name.yard == yard
                    select name
                );
                
                var names = await JinagaClient.Query(namesQuery, yard);
                
                // Check if any name matches what we're looking for
                if (names.Any(n => n.value == "Test Yard"))
                {
                    foundYardWithName = true;
                    break;
                }
            }
            
            if (foundYardWithName) break;
        }
        
        foundYardWithName.Should().BeTrue("A yard with name 'Test Yard' should have been created");
    }
}