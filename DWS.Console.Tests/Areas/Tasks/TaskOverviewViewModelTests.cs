using Autofac;
using DWS.Console.ViewModels.TaskOverview;
using DWS.Model;
using FluentAssertions;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DWS.Console.Tests.Areas.Tasks;

public class TaskOverviewViewModelTests : AutofacTestBase
{
    [Fact]
    public async Task WhenNoTasks_TaskListIsEmpty()
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
        var viewModel = scope.Resolve<TaskOverviewViewModel>();
        
        // Act
        viewModel.Load();
        
        // Assert
        viewModel.Tasks.Should().BeEmpty();
    }
    
    [Fact]
    public async Task WhenTasksExist_TaskListIsPopulated()
    {
        // Arrange
        var supplier = await CreateSupplier();
        var client = await CreateClient(supplier);
        var yard = await CreateYard(client);
        var task = await JinagaClient.Fact(new DWSTask(yard, Guid.NewGuid()));
        
        // Create a scope to resolve the view model
        using var scope = Container.BeginLifetimeScope(builder =>
        {
            // Register the supplier instance for this test
            builder.RegisterInstance(supplier).AsSelf();
        });
        
        // Resolve the view model from the container
        var viewModel = scope.Resolve<TaskOverviewViewModel>();
        
        // Act
        viewModel.Load();
        
        // Wait for the tasks to load
        await Task.Delay(100);
        
        // Assert
        viewModel.Tasks.Should().HaveCount(1);
        viewModel.Tasks[0].Task.taskGuid.Should().Be(task.taskGuid);
    }
}