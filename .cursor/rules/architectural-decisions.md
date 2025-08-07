---
description: Use this rule when making architectural decisions, understanding design rationale, or when you need to know why certain patterns were chosen. This rule provides context for the reasoning behind key design choices and helps maintain architectural consistency.
---
# Architectural Decisions

## Event Sourcing with Jinaga

### Why Event Sourcing?
- **Audit Trail**: Complete history of all changes
- **Temporal Queries**: Ability to query data at any point in time
- **Data Integrity**: Immutable facts ensure data consistency
- **Scalability**: Events can be replayed and projected

### Jinaga Implementation
- Uses fact types as immutable records
- Relations provide computed properties
- Conditions handle soft deletes and state
- Observer pattern for reactive updates

## MVVM Architecture

### Why MVVM?
- **Separation of Concerns**: Clear separation between UI and business logic
- **Testability**: ViewModels can be unit tested independently
- **Reusability**: ViewModels can be reused across different views
- **Maintainability**: Changes to UI don't affect business logic

### Implementation Patterns
- `ObservableObject` base class for property notifications
- `[ObservableProperty]` for automatic property generation
- `ObservableCollection<T>` for reactive collections
- Partial methods for custom property change logic

## Dependency Injection with Autofac

### Why Autofac?
- **Factory Pattern Support**: Custom extensions for parameter forwarding
- **Lifetime Management**: Proper disposal of resources
- **Testability**: Easy to mock dependencies in tests
- **Modularity**: Module-based registration

### Factory Pattern
- `TransitiveFactory()` extension for parameter forwarding
- Allows ViewModels to receive context-specific parameters
- Maintains dependency injection benefits with factory flexibility

## Async/Await Patterns

### Why Async Everywhere?
- **Responsiveness**: UI remains responsive during operations
- **Scalability**: Better resource utilization
- **Error Handling**: Structured exception handling
- **Composability**: Easy to combine async operations

### Implementation
- All I/O operations are async
- Use `Task.WhenAll()` for parallel operations
- Proper error handling with try-catch blocks
- Async test methods for testing async code

## Testing Strategy

### Why Comprehensive Testing?
- **Confidence**: Changes can be made with confidence
- **Documentation**: Tests serve as living documentation
- **Regression Prevention**: Catch bugs before they reach production
- **Design Feedback**: Tests influence better design

### Test Patterns
- Gherkin-style naming for readability
- Base classes for common setup
- Factory methods for test data creation
- FluentAssertions for readable assertions

## Project Structure

### Layered Architecture
- **DWS.Model**: Domain layer with fact types
- **DWS.Console.ViewModels**: Presentation logic
- **DWS.Console**: UI layer
- **DWS.Console.Tests**: Test layer

### Separation of Concerns
- Each layer has a specific responsibility
- Dependencies flow in one direction
- Clear interfaces between layers
- Testable components at each layer

## Key Design Principles

### Immutability
- Fact types are immutable records
- State changes create new facts
- No mutable state in domain models

### Reactive Programming
- Observers react to fact changes
- UI updates automatically
- Event-driven architecture

### Composition over Inheritance
- Use composition for ViewModels
- Factory pattern for object creation
- Dependency injection for wiring

### Single Responsibility
- Each class has one reason to change
- Methods do one thing well
- Clear separation of concerns

## Performance Considerations

### Lazy Loading
- Load data only when needed
- Use observers for reactive updates
- Implement proper cleanup in `Unload()`

### Memory Management
- Dispose observers properly
- Clear collections when unloading
- Use weak references where appropriate

### Async Operations
- Don't block the UI thread
- Use `ConfigureAwait(false)` for library code
- Handle cancellation tokens properly

## Decision Making Process

### Before Making Changes
1. **Understand the Current Model**: Review existing fact types and relationships
2. **Identify the Relationship Pattern**: Determine if it's direct, one-to-many, or many-to-many
3. **Check Existing Queries**: Look at how similar relationships are currently queried
4. **Validate Against Business Logic**: Ensure the relationship model matches business requirements

### When Modifying Queries
1. **Preserve Existing Patterns**: Follow the established query patterns in the codebase
2. **Maintain Consistency**: Use the same filtering and projection patterns
3. **Test Incrementally**: Make small changes and verify compilation before proceeding
4. **Document Changes**: Update comments to reflect the relationship model

## Learning from Common Mistakes

### Relationship Modeling Mistakes
- **Mistake**: Assuming direct object properties instead of fact-based relationships
- **Solution**: Always check the fact type definitions and use intermediate fact types for relationships
- **Prevention**: Review existing query patterns before making changes

### Query Structure Mistakes
- **Mistake**: Removing necessary relationship chains from queries
- **Solution**: Maintain the complete relationship path through all intermediate fact types
- **Prevention**: Understand the full relationship model before modifying queries

### Fact Creation Mistakes
- **Mistake**: Creating entities without establishing relationships
- **Solution**: Create both the entity and the relationship fact
- **Prevention**: Follow the established patterns in the codebase
