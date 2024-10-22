## Tools Data Grid

The `ToolsDataGrid` control in the `ToolsView` displays a list of tools in a data grid. The data grid is bound to a collection of tools and allows users to select tools from a predefined catalog. These are the design choices that went into configuring the data grid.

### View Models

The primary view model of the dialog is the `NewTaskViewModel`. This view model has a `Tools` observable collection that is data bound to the data grid. The `Tools` collection is a collection of `TaskToolViewModel` objects that represent the tools needed for that task.

There is a third view model. The `ToolViewModel` represents a tool in the catalog. The `NewTaskViewModel` has a `ToolCatalog` observable collection of `ToolViewModel` objects. It populates these objects from a specification.

```csharp
var toolsInSupplier = Given<Supplier>.Match((supplier, facts) =>
    from tool in facts.OfType<Tool>()
    where tool.supplier == supplier && !tool.IsDeleted
    select new
    {
        tool = tool,
        toolNames = facts.Observable(tool.Names.Select(name => name.value))
    }
);
```

### Custom Columns

The `TaskToolViewModel`, which represents a row in the data grid, has a `Tool` property to collect the tool that the user selects. It also has a `Name` property for the user to enter if they want to add a tool on the fly. These should not appear as separate columns, and so we do not auto-generate the columns of the data grid.

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

However, the combo box is not in the visual tree of the `Window`, so this approach will not work in a data grid. Instead, we bind to a collection view source. The options for the combo box are defined in the `ToolCatalog` resource.

```xaml
<CollectionViewSource x:Key="ToolCatalog" Source="{Binding ToolCatalog}" />
```

This allows us to bind the combo box items to the `ToolCatalog` resource.

```xaml
<ComboBox
    ItemsSource="{Binding Source={StaticResource ToolCatalog}}" />
```

The combo box column is configured to display the `Name` property of the items in the combo box. This is done using the `DisplayMemberPath` property.

```xaml
<ComboBox
    DisplayMemberPath="Name" />
```

### Selected Tool

When the user selects a tool from the combo box list, we want to capture that selection. There are two ways to bind the selection from a combo box. The first is to bind the `SelectedItem` property of the combo box to a property on the view model.

```xaml
<!--- This is not the option we used -->
<ComboBox
    SelectedItem="{Binding Tool, Mode=TwoWay}" />
```

This would result in the `ToolViewModel` being stored in the `Tool` property of the `TaskToolViewModel`.

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

A side-effect of this binding is that when the user selects a tool from the list, the `Name` property is updated with the `Name` of the selected tool. As it turns out, this is helpful because the `Name` property is also used to display the tool name in the data grid.

### Searching the Tool Catalog

The combo box is configured to allow the user to search the list of tools. This is done by setting the `IsTextSearchEnabled` and `IsTextSearchCaseSensitive` properties.

```xaml
<ComboBox
    IsTextSearchEnabled="True"
    IsTextSearchCaseSensitive="False" />
```

### Displaying the Tool Name

The most straightforward way to implement a combo box column would be to use the `DataGridComboBoxColumn` class. However, this class does not display the entered text of an editable combo box. Instead, it appears blank while the user is not editing the cell. There are several [online discussions](https://stackoverflow.com/questions/6899697/how-to-implement-editable-datagridcomboboxcolumn-in-wpf-datagrid) of this issue and various workarounds.

Instead of using the `DataGridComboBoxColumn` element, we use a `DataGridTemplateColumn` element. This allows us to define a `ComboBox` element in the editing cell template and a `TextBlock` element in the non-editing cell template.

```xaml
<DataGridTemplateColumn>
    <DataGridTemplateColumn.CellTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Name}" />
        </DataTemplate>
    </DataGridTemplateColumn.CellTemplate>
    <DataGridTemplateColumn.CellEditingTemplate>
        <DataTemplate>
            <!-- Other properties elided for clarity -->
            <ComboBox SelectedValue="{Binding Tool, Mode=TwoWay}" />
        </DataTemplate>
    </DataGridTemplateColumn.CellEditingTemplate>
</DataGridTemplateColumn>
```

This configuration solves the problem of displaying the entered text. It also works with an item selected from the list, since the combo box also sets the `Name` property of the `TaskToolViewModel`. However, it brings with it a usability problem. The combo box does not immediately receive focus when the user begins editing the cell.

### Focus on Edit

To solve this problem, the combo box is wrapped in a container that applies the `FocusManager.IsFocusScope` attached property. This property allows the container to manage focus within its scope. The `FocusManager.FocusedElement` attached property is also applied. This property sets the initial focus to the combo box when the cell enters edit mode.

```xaml
<DataGridTemplateColumn.CellEditingTemplate>
    <DataTemplate>
        <Grid
            FocusManager.IsFocusScope="True"
            FocusManager.FocusedElement="{Binding ElementName=ToolComboBox}">
            <ComboBox
                x:Name="ToolComboBox" />
        </Grid>
    </DataTemplate>
</DataGridTemplateColumn.CellEditingTemplate>
```

### Sorting the Tool Catalog

The `ToolCatalog` observable collection in the `NewTaskViewModel` is sorted alphabetically by tool name. To accomplish this, the `NewTaskViewModel` moves each `ToolViewModel` to the correct place when the name changes. It uses a binary search to find the new insertion point.