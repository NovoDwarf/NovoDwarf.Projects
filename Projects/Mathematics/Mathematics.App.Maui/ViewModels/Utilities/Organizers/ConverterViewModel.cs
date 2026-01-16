using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalizationResourceManager.Maui;
using Mathematics.App.Maui.Models.Entities;
using Mathematics.App.Maui.Resources.Localizations;
using Mathematics.App.Maui.Systems.Measurements.Entities;
using Mathematics.App.Maui.Systems.Measurements.Services;
using Mathematics.App.Maui.UI.Base;

namespace Mathematics.App.Maui.UI.ViewModels.Utilities;

public partial class ConverterViewModel : BaseViewModel
{
    private readonly InMemoryMeasurementRegistry _registry;
    private readonly ILocalizationResourceManager _manager;

    public ConverterViewModel(InMemoryMeasurementRegistry registry, ILocalizationResourceManager manager)
    {
        _registry = registry;
        _manager = manager;

        ConverterGroups = [ "Величины", "Валюты" ];

        SelectedGroup = (ConverterGroups is [var first, ..] ? first : null) ?? string.Empty;
    }

    public ObservableCollection<string> ConverterGroups { get; }
    [ObservableProperty] public partial string SelectedGroup { get; set; }
    [ObservableProperty] public partial CategoryItem? SelectedCategory { get; set; }
    [ObservableProperty] public partial ObservableCollection<CategoryItem> Categories { get; set; } = [];

    
    [ObservableProperty] public partial UnitItem? SelectedFromUnit { get; set; }
    [ObservableProperty] public partial UnitItem? SelectedToUnit { get; set; }
    [ObservableProperty] public partial ObservableCollection<UnitItem> FromUnits { get; set; } = [];
    [ObservableProperty] public partial ObservableCollection<UnitItem> ToUnits { get; set; } = [];
    
    [ObservableProperty] public partial string InputValue { get; set; } = string.Empty;
    [ObservableProperty] public partial string ResultText { get; set; } = string.Empty;
    [ObservableProperty] public partial bool HasResult { get; set; }
    [ObservableProperty] public partial bool IsBusy { get; set; }

    partial void OnSelectedGroupChanged(string value) => _ = LoadCategoriesAsync();

    partial void OnSelectedCategoryChanged(CategoryItem? value) => _ = LoadUnitsAsync();

    public override IDictionary<string, object?> Save()
    {
        return new Dictionary<string, object?>
        {
            { "Group", SelectedGroup },
            { "Category", SelectedCategory?.Id },
            { "FromUnit", SelectedFromUnit?.Id },
            { "ToUnit", SelectedToUnit?.Id },
            { "Input", InputValue }
        };
    }
    
    public override void Load(IDictionary<string, object?> parameters)
    {
        if (parameters.TryGetValue("Group", out var group))
            SelectedGroup = group as string ?? string.Empty;

        if (parameters.TryGetValue("Category", out var category))
            SelectedCategory = Categories.FirstOrDefault(cat => cat.Id == (category as string));
        
        if (parameters.TryGetValue("FromUnit", out var fromUnit))
            SelectedFromUnit = FromUnits.FirstOrDefault(cat => cat.Id == (fromUnit as string));
        
        if (parameters.TryGetValue("ToUnit", out var toUnit))
            SelectedFromUnit = ToUnits.FirstOrDefault(cat => cat.Id == (toUnit as string));
        
        if (parameters.TryGetValue("Input", out var input))
            SelectedFromUnit = input as UnitItem;
    }
    
    private async Task LoadCategoriesAsync()
    {
        Categories.Clear();
        FromUnits.Clear();
        ToUnits.Clear();

        if (SelectedGroup == "Величины")
        {
            foreach (var q in _registry.Quantities.OrderBy(q => q.DisplayName))
            {
                Categories.Add(new CategoryItem(q.Id, Measurement_Resources.ResourceManager.GetString(q.DisplayName) ?? q.DisplayName));
            }
        }
        else if (SelectedGroup == _manager["Валюты"])
        {
            Categories.Add(new CategoryItem("ALL", _manager["AllCurrencies"]));
        }

        SelectedCategory = Categories is [var first, ..] ? first : null;
        await LoadUnitsAsync();
    }

    private async Task LoadUnitsAsync()
    {
        FromUnits.Clear();
        ToUnits.Clear();

        if (SelectedCategory == null)
            return;

        if (SelectedGroup == "Величины")
        {
            foreach (var u in _registry
                         .GetUnitsByQuantity(SelectedCategory.Id)
                         .OrderBy(u => u.DisplayName)
                         .ThenBy(u => u.Prefix?.Power ?? 0))
            {
                var name = u.DisplayName;
                var parts = name.Split('-');

                var finalName = new StringBuilder();

                foreach (var part in parts)
                {
                    var localizedPart = Measurement_Resources.ResourceManager.GetString(part) ?? string.Empty;
                    finalName.Append(localizedPart);
                }

                var item = new UnitItem(u.Id, finalName.ToString());

                FromUnits.Add(item);
                ToUnits.Add(item);
            }

            SelectedFromUnit = FromUnits.FirstOrDefault();
            SelectedToUnit = ToUnits.FirstOrDefault();
        }

        SelectedFromUnit = FromUnits.FirstOrDefault();
        SelectedToUnit = ToUnits.FirstOrDefault();
    }

    [RelayCommand]
    private async Task Convert()
    {
        HasResult = false;

        if (SelectedFromUnit == null || SelectedToUnit == null)
            return;

        if (!double.TryParse(InputValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
            return;

        var result = _registry.Convert(
            value,
            SelectedFromUnit.Id,
            SelectedToUnit.Id);

        var left = FormatByMagnitude(value);
        var right = FormatByMagnitude(result);

        ResultText = $"{left} {SelectedFromUnit.DisplayName} = " + $"{right} {SelectedToUnit.DisplayName}";
        HasResult = true;
    }

    private static string FormatByMagnitude(double value, int significantDigits = 6)
    {
        if (value == 0)
            return "0";

        var abs = Math.Abs(value);
        var order = Math.Floor(Math.Log10(abs));
        var decimals = Math.Max(0, significantDigits - (int)order - 1);

        return Math.Round(value, decimals)
            .ToString($"F{decimals}", CultureInfo.InvariantCulture);
    }
}