---
alwaysApply: true
---
# DWS Project Structure

This is a C# WPF application using the Jinaga event sourcing framework. The project follows a layered architecture with clear separation of concerns.

## Project Organization

- **DWS.Model**: Contains domain models and fact types using Jinaga event sourcing
  - [Dispatcher.cs](mdc:DWS.Model/Dispatcher.cs) - Core domain entities (Supplier, Client, Yard, Tool, Worker, etc.)
  - [CRM.cs](mdc:DWS.Model/CRM.cs) - CRM integration models

- **DWS.Console**: WPF application layer
  - Main application entry point and UI components
  - Forms and dialogs for user interaction
  - Asynchronous command processing

- **DWS.Console.ViewModels**: MVVM view models
  - ViewModels for different UI areas (TaskOverview, NewTask, etc.)
  - Dependency injection configuration with Autofac
  - [ConsoleViewModelsModule.cs](mdc:DWS.Console.ViewModels/ConsoleViewModelsModule.cs) - DI registration

- **DWS.Console.Tests**: Unit tests
  - Test infrastructure and base classes
  - Area-specific test organization
  - [ViewModelTestBase.cs](mdc:DWS.Console.Tests/TestInfrastructure/ViewModelTestBase.cs) - Common test utilities

## Key Architectural Patterns

1. **Event Sourcing**: Uses Jinaga for event sourcing with fact types and relations
2. **MVVM**: Clear separation between ViewModels and Views
3. **Dependency Injection**: Autofac container with factory pattern support
4. **Test-Driven Development**: Comprehensive test coverage with Gherkin-style test methods
5. **Async/Await**: Heavy use of asynchronous programming patterns

## Solution Structure
The main solution file [DWS.sln](mdc:DWS.sln) contains four projects:
- DWS.Model (domain layer)
- DWS.Console (presentation layer)
- DWS.Console.ViewModels (view model layer)
- DWS.Console.Tests (test layer)
