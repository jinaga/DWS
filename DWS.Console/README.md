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
<!--- This does not work in a DataGrid column -->
<ComboBox
    ItemsSource="{Binding DataContext.ToolCatalog, RelativeSource={RelativeSource AncestorType={x:Type Window}}}" />
```

However, the combo box is not in the visual tree of the `Window`, so this approach will not work. Instead, we bind to a collection view source. The options for the combo box are defined in the `ToolCatalog` resource.

```xaml
<CollectionViewSource x:Key="ToolCatalog" Source="{Binding ToolCatalog}" />
```

This allows us to properly bind the combo box items to the `ToolCatalog` resource.

```xaml
<ComboBox
    ItemsSource="{Binding Source={StaticResource ToolCatalog}}" />
```

### Selected Tool

When the user selects a tool from the combo box list, we want to capture that selection. There are two ways to bind the selection from a combo box. The first is to bind the `SelectedItem` property of the combo box to a property on the view model.

```xaml
<!--- This is not the option we used -->
<ComboBox
    SelectedItem="{Binding Tool, Mode=TwoWay}" />
```

The second option is to bind the `SelectedValue` property of the combo box to a property on the view model.
```xaml
<ComboBox
    SelectedValue="{Binding Tool, Mode=TwoWay}" />
```

We went with the second option because it allows us to use one data type for the items in the list, and another data type for the selection. When the user selects a tool, we just want to capture the `Tool` object, not the entire `ToolViewModel`.

In order to use the selected value binding, we need to set the `SelectedValuePath` property of the combo box to the property of the items in the list that we want to capture. In this case, we want to capture the `Tool` property of the `ToolViewModel`.

```xaml
<ComboBox
    SelectedValuePath="Tool" />
```

### Edit Tool Name

The combo box column is configured to display the `Name` property of the items in the combo box. This is done using the `DisplayMemberPath` property.

```xaml
<ComboBox
    DisplayMemberPath="Name" />
```

The user may select a tool from the list, or they may enter their own value. The combo box is configured to allow the user to enter their own value not appearing in the list. Both the `IsReadOnly` and `IsEditable` [properties](https://learn.microsoft.com/en-us/dotnet/api/system.windows.controls.combobox.isreadonly?view=windowsdesktop-8.0) control this behavior.

```xaml
<ComboBox
    IsReadOnly="False"
    IsEditable="True" />
```

The selected or entered tool name is stored in the `Tool` property of the `TaskToolViewModel`.

```xaml
<ComboBox
    Text="{Binding Name, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}" />
```

A side-effect of this binding is that when the user selects a tool from the list, the `Name` property is updated with the `Name` of the selected tool. As it turns out, this is helpful because the `Name` property is used to display the tool name in the data grid.