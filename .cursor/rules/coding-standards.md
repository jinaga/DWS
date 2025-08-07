---
alwaysApply: true
---
# DWS Coding Standards

## C# Conventions

### Naming Conventions
- Use PascalCase for public members, classes, and methods
- Use camelCase for private fields and local variables
- Use descriptive names that clearly indicate purpose
- Avoid abbreviations unless they are widely understood

### File Organization
- One class per file
- Use partial classes for generated code (like ObservableObject)
- Group related functionality in the same namespace
- Use meaningful folder structure to organize code

### Code Style
- Use expression-bodied members where appropriate
- Prefer `var` for local variable declarations when type is obvious
- Use `string.Empty` instead of `""` for empty strings
- Use collection initializers `[]` for empty collections
- Use null-conditional operators (`?.`) and null-coalescing operators (`??`) appropriately

### Async/Await Patterns
- Always use `async`/`await` for asynchronous operations
- Return `Task` or `Task<T>` from async methods
- Use `Task.WhenAll()` for parallel operations
- Avoid blocking calls in async methods

### LINQ Usage
- Use method syntax for complex queries
- Use query syntax for simple projections
- Prefer `SingleOrDefault()` over `FirstOrDefault()` for unique constraints
- Use meaningful variable names in LINQ queries

## Jinaga Event Sourcing Patterns

### Fact Types
- Use `[FactType("DWS.EntityName")]` attributes
- Follow the pattern: `EntityName(parameters...)`
- Use `prior` arrays for versioned facts
- Include `IsDeleted` conditions for soft deletes

### Relations
- Use `Relation.Define()` for computed properties
- Filter out deleted items with `!fact.IsDeleted` conditions
- Use meaningful relation names that describe the relationship

### Conditions
- Use `Condition.Define()` for boolean computed properties
- Check for delete/restore patterns consistently
- Use descriptive condition names

## MVVM Patterns

### ViewModels
- Inherit from `ObservableObject` for property change notifications
- Use `[ObservableProperty]` attributes for automatic property generation
- Implement `INotifyPropertyChanged` through the base class
- Use `ObservableCollection<T>` for collections that need UI updates

### Property Patterns
- Use backing fields for complex properties
- Implement `OnPropertyChanged()` in partial methods for custom logic
- Use `ObservableProperty` attribute for simple properties

### Command Patterns
- Use `RelayCommand` or `AsyncRelayCommand` for commands
- Implement `CanExecute` logic for command availability
- Use async commands for operations that return `Task`

## Testing Patterns

### Test Structure
- Use Gherkin-style naming: `Given_When_Then`
- Inherit from `ViewModelTestBase` for common setup
- Use descriptive test method names
- Group related tests in the same test class

### Test Data Setup
- Use factory methods for creating test data
- Use meaningful test data names
- Create helper methods for common test scenarios
- Use `await` for async test setup

### Assertions
- Use FluentAssertions for readable assertions
- Use specific assertion methods (`Should().Be()`, `Should().Contain()`, etc.)
- Write descriptive assertion messages when needed

## Error Prevention Guidelines

### Jinaga Query Patterns
- **Always check fact type definitions** before assuming direct properties
- **Use relationship fact types** for many-to-many relationships
- **Include all required parameters** when creating facts
- **Check IsDeleted conditions** for all relevant fact types

### Common Compilation Errors
- `CS1061: 'Entity' does not contain a definition for 'property'` → Check if relationship is through fact type
- `CS7036: There is no argument given that corresponds to the required parameter` → Check fact constructor parameters
- `CS0246: The type or namespace name 'Type' could not be found` → Check if fact type is defined and uncommented

### Before Compiling
1. Verify all referenced fact types exist in the model
2. Check that all required constructor parameters are provided
3. Ensure relationship queries use the correct fact type chain
4. Validate that observable projections match the query structure
