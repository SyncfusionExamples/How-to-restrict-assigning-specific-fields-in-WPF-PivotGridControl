# How to restrict assigning specific fields in WPF PivotGridControl?

In [WPF PivotGridControl](https://www.syncfusion.com/wpf-controls/pivot-grid), you can restrict specific fields by handling the **CollectionChanged** events of **PivotRows**, **PivotColumns**, and **PivotCalculations**. You can maintain a list of field names that should be restricted and enforce the restriction within these event handlers.

In this demonstration, we use a **RestrictedFields** list that contains the names of expensive fields such as "Country". And, we have implemented a method called **HandleRestrictedFieldAddition**, which monitors all three collection change events.

When items are added to these collections, the method checks whether the item is a **PivotItem** or **PivotComputationInfo**, and compares the **FieldMappingName** or **FieldName** against the **RestrictedFields** list.

If a restricted field is detected, **Dispatcher.BeginInvoke** is used to defer the removal operation until after the event is complete, thereby avoiding reentrancy issues. The field is then removed from the appropriate collection (**PivotRows, PivotColumns, or PivotCalculations**), and a **MessageBox.Show()** call displays a warning message to the user.

This approach prevents expensive calculations from executing while also providing immediate feedback to the user about why the field cannot be added.

**Code snippet for restrict the specific fields:**
```csharp
 // List of fields that are expensive to calculate
 private readonly List<string> RestrictedFields = new List<string>() { "Country" };

 pivotGrid.PivotColumns.CollectionChanged += PivotColumns_CollectionChanged;
 pivotGrid.PivotRows.CollectionChanged += PivotRows_CollectionChanged;
 pivotGrid.PivotCalculations.CollectionChanged += PivotCalculations_CollectionChanged;

 private void PivotCalculations_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
 {
     HandleRestrictedFieldAddition(e, "PivotCalculations");
 }

 private void PivotRows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
 {
     HandleRestrictedFieldAddition(e, "PivotRows");
 }

 private void PivotColumns_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
 {
     HandleRestrictedFieldAddition(e, "PivotColumns");
 }

 private void HandleRestrictedFieldAddition(System.Collections.Specialized.NotifyCollectionChangedEventArgs e, string areaName)
 {
     // Check if items were added to the collection
     if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && e.NewItems != null)
     {
         foreach (var item in e.NewItems)
         {
             if (item is PivotItem pivotItem)
             {
                 // Check if the added field is in the restricted list
                 if (RestrictedFields.Contains(pivotItem.FieldMappingName, StringComparer.OrdinalIgnoreCase))
                 {
                     Dispatcher.BeginInvoke(new Action(() =>
                     {
                         // Remove the item from the collection
                         if (areaName == "PivotRows")
                         {
                             pivotGrid.PivotRows.Remove(pivotItem);
                         }
                         else if (areaName == "PivotColumns")
                         {
                             pivotGrid.PivotColumns.Remove(pivotItem);
                         }

                         // Show alert to user
                         MessageBox.Show(
                             $"The '{pivotItem.FieldHeader}' field is too expensive to calculate and cannot be added to {areaName}.\n\nPlease use a different field.",
                             "Field Restricted",
                             MessageBoxButton.OK,
                             MessageBoxImage.Warning);
                     }));
                 }
             }
             else if (item is PivotComputationInfo computationInfo && areaName == "PivotCalculations")
             {
                 // Check if the added field is in the restricted list
                 if (RestrictedFields.Contains(computationInfo.FieldName, StringComparer.OrdinalIgnoreCase))
                 {
                     Dispatcher.BeginInvoke(new Action(() =>
                     {
                         // Remove the item from the collection
                         pivotGrid.PivotCalculations.Remove(computationInfo);

                         // Show alert to user
                         MessageBox.Show(
                             $"The '{computationInfo.FieldName}' field is too expensive to calculate and cannot be added to PivotCalculations.\n\nPlease use a different field.",
                             "Field Restricted",
                             MessageBoxButton.OK,
                             MessageBoxImage.Warning);
                     }));
                 }
             }
         }
     }
 }
```

![Restrict Specific Field](Restrict%20Specific%20Field.gif)

Take a moment to peruse the [WPF PivotGridControl](https://help.syncfusion.com/wpf/pivot-grid/pivotgrid-getting-started) documentation, to learn more about pivotgrid with example.