## Tools Data Grid

The `ToolsDataGrid` control in the `ToolsView` displays a list of tools in a data grid. The data grid is bound to a collection of tools and allows users to select tools from a predefined catalog. These are the design choices that went into configuring the data grid.

### Custom Columns

The view model that supports the data grid is `TaskToolViewModel`. It has a `Tool` property to collect the tool that the user selects, and a `Name` property for the user to enter if they want to add one on the fly. These should not appear as separate columns.

```xaml
<DataGrid AutoGenerateColumns="False">
    <DataGrid.Columns>
        <!--- Define columns explicitly-->
    </DataGrid.Columns>
</DataGrid>
```

The custom column configuration for the data grid includes a single combo box column that allows users to select tools from a predefined catalog. The combo box is configured to allow the user to enter their own value not appearing in the list.

### Tool Catalog

The items in a combo box list are defined using the `ItemsSource` property. The items source in this case is on the `NewTaskViewModel`, not the `TaskToolViewModel`. Ordinarily, this would use relative source binding to locate the parent `Window` in the visual tree, and then pull the `ToolCatalog` from the `DataContext` of the `Window`.

```xaml
ItemsSource="{Binding DataContext.ToolCatalog, RelativeSource={RelativeSource AncestorType={x:Type Window}}}"
```

However, the combo box is not in the visual tree of the `Window`, so this approach will not work. Instead, we bind to a collection view source. The options for the combo box are defined in the `ToolCatalog` resource.

```xaml
<CollectionViewSource x:Key="ToolCatalog" Source="{Binding ToolCatalog}" />
```

This allows us to properly bind the combo box items to the `ToolCatalog` resource.

```xaml
ItemsSource="{Binding Source={StaticResource ToolCatalog}}"
```

### Edit Tool Name

The combo box column is configured to display the `Name` property of the items in the combo box. This is done using the `DisplayMemberPath` property.

```xaml
DisplayMemberPath="Name"
```

The user may select a tool from the list, or they may enter their own value. The combo box is configured to allow the user to enter their own value not appearing in the list. Both the `IsReadOnly` and `IsEditable` [properties](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.combobox.isreadonly?view=windowsdesktop-8.0) control this behavior.

```xaml
IsReadOnly="False" IsEditable="True"
```

The selected or entered tool name is stored in the `Tool` property of the `TaskToolViewModel`.

```xaml
<ComboBox Text="{Binding Name, Mode=TwoWay}" />
```
