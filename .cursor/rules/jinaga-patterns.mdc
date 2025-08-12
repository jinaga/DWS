---
description: Use this rule when working with domain models, creating new fact types, writing Jinaga queries, or when you need to understand event sourcing patterns. This rule covers fact type definitions, relations, conditions, and event sourcing conventions.
---
# Jinaga Event Sourcing Patterns

## Fact Type Definitions

### Core Entities
All domain entities are defined as fact types in [Dispatcher.cs](mdc:DWS.Model/Dispatcher.cs):

```csharp
[FactType("DWS.EntityName")]
public record EntityName(parameters...)
{
    // Relations and conditions
}
```

### Fact Type Naming
- Use `[FactType("DWS.EntityName")]` attribute
- Follow PascalCase naming for fact types
- Use descriptive names that reflect the domain concept
- Group related facts in the same file

### Common Patterns

#### Base Entities
```csharp
[FactType("DWS.Supplier")]
public record Supplier(User creator, Guid supplierGuid)
{
    public Relation<Client> Clients => Relation.Define(facts =>
        from client in facts.OfType<Client>()
        where client.supplier == this && !client.IsDeleted
        select client
    );
}
```

#### Versioned Facts
```csharp
[FactType("DWS.EntityName")]
public record EntityName(Entity entity, string value, EntityName[] prior);
```

#### Soft Delete Pattern
```csharp
public Condition IsDeleted => Condition.Define(facts =>
    facts.Any<EntityDelete>(delete => delete.entity == this &&
        !facts.Any<EntityRestore>(restore => restore.entityDelete == delete)
    )
);
```

## Relations

### Relation Definition
- Use `Relation.Define()` for computed properties
- Always filter out deleted items with `!fact.IsDeleted`
- Use meaningful relation names
- Return the most recent version of versioned facts

### Common Relation Patterns
```csharp
public Relation<EntityName> Names => Relation.Define(facts =>
    from name in facts.OfType<EntityName>()
    where name.entity == this &&
        !facts.OfType<EntityName>().Any(next => next.prior.Contains(name))
    select name
);
```

## Conditions

### Boolean Computed Properties
- Use `Condition.Define()` for boolean properties
- Check for delete/restore patterns consistently
- Use descriptive condition names

### Common Condition Patterns
```csharp
public Condition IsRevoked => Condition.Define(facts =>
    facts.Any<EntityRevoke>(revoke => revoke.entity == this &&
        !facts.Any<EntityRestore>(restore => restore.entityRevoke == revoke)
    )
);
```

## CRM Integration

### CRM Fact Types
CRM integration facts are defined in [CRM.cs](mdc:DWS.Model/CRM.cs):

```csharp
[FactType("DWS.CRMEntity")]
public record CRMEntity(parameters...);
```

### Linking Patterns
- Use linking facts to connect domain entities to CRM entities
- Include creation dates for audit trails
- Use prior arrays for versioned CRM data

## Query Patterns

### Jinaga Queries
- Use `Given<Entity>.Match()` for complex queries
- Use LINQ query syntax for readability
- Filter by supplier context consistently
- Check for deleted items in all queries

### Example Query Pattern
```csharp
var entities = Given<Supplier>.Match((supplier, facts) =>
    from entity in facts.OfType<Entity>()
    where entity.supplier == supplier && !entity.IsDeleted
    select entity
);
```

## Fact Creation

### Creating Facts
- Use `JinagaClient.Fact()` for creating new facts
- Pass required parameters in constructor
- Use `Guid.NewGuid()` for unique identifiers
- Include all required relationships

### Example Fact Creation
```csharp
var entity = await JinagaClient.Fact(new Entity(supplier, Guid.NewGuid()));
var name = await JinagaClient.Fact(new EntityName(entity, "Name", []));
```

## Relationship Query Patterns

### Many-to-Many Relationships
When entities are related through intermediate fact types (like `YardClients`), use this pattern:

```csharp
var query = Given<Supplier>.Match((supplier, facts) =>
    from yard in facts.OfType<Yard>()
    where yard.supplier == supplier && !yard.IsDeleted
    from yardClients in facts.OfType<YardClients>()
    where yardClients.yard == yard && !yardClients.IsDeleted
    from client in facts.OfType<Client>()
    where client == yardClients.client && !client.IsDeleted
    select new { yard, client }
);
```

### Relationship Creation Pattern
When creating new entities that need relationships, create both the entity and the relationship fact:

```csharp
// Create the entities
var yard = await jinagaClient.Fact(new Yard(supplier, Guid.NewGuid()));
var client = await jinagaClient.Fact(new Client(supplier, Guid.NewGuid()));

// Create the relationship
await jinagaClient.Fact(new YardClients(yard, client, DateTime.UtcNow));
```

### Common Mistakes to Avoid
- ❌ Don't assume direct properties like `yard.client` - relationships are fact types
- ❌ Don't forget to check `IsDeleted` conditions on relationship facts
- ✅ Always use the intermediate fact type to traverse relationships
- ✅ Include relationship facts when creating new entities

## Debugging and Validation

### Before Making Changes
1. **Understand the Relationship Model**: Check how entities are related before modifying queries
2. **Review Fact Type Definitions**: Look at the actual fact type structure in `Dispatcher.cs`
3. **Trace Relationship Paths**: Follow the relationship chain through intermediate fact types
4. **Validate Query Logic**: Ensure all `from` clauses properly connect the relationship chain

### Common Query Validation Checklist
- [ ] All entities in the query chain are properly filtered by supplier context
- [ ] All `IsDeleted` conditions are checked for relevant fact types
- [ ] Relationship facts are included in the query chain
- [ ] Observable projections match the entities being projected
- [ ] Observer callbacks reference the correct projection properties
