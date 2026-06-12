using Syncfusion.PivotAnalysis.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PivotGridDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // List of fields that are expensive to calculate
        private readonly List<string> RestrictedFields = new List<string>() { "Country" };

        public MainWindow()
        {
            InitializeComponent();
            pivotGrid.PivotColumns.CollectionChanged += PivotColumns_CollectionChanged;
            pivotGrid.PivotRows.CollectionChanged += PivotRows_CollectionChanged;
            pivotGrid.PivotCalculations.CollectionChanged += PivotCalculations_CollectionChanged;
        }
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
    }
}
