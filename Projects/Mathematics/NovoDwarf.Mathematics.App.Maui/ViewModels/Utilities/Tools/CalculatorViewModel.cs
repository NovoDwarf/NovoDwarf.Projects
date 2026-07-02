using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NovoDwarf.Mathematics.App.Core.ViewModels;
using NovoDwarf.Mathematics.App.Systems.Algorithms.Services;

namespace NovoDwarf.Mathematics.App.ViewModels.Utilities.Tools;

public enum RoundingMode
{
    None,
    Up,
    Down,
    Nearest
}

public sealed partial class CalculatorViewModel : BaseViewModel
{
    private readonly CalculatorService _calculator;
    private readonly RoundingService _rounding;

    public CalculatorViewModel(CalculatorService calculator, RoundingService rounding)
    {
        _calculator = calculator;
        _rounding = rounding;
    }

    [ObservableProperty] public partial string Expression { get; set; } = "";
    [ObservableProperty] public partial string Result { get; set; } = "0";
    [ObservableProperty] public partial RoundingMode Rounding { get; set; }
    [ObservableProperty] public partial ObservableCollection<string> History { get; set; } = [];

    [RelayCommand]
    private async Task Append(string value)
    {
        Expression += value;
        await PreviewAsync();
    }

    [RelayCommand]
    private void Clear()
    {
        Expression = "";
        Result = "0";
    }

    [RelayCommand]
    private async Task DeleteLast()
    {
        if (!string.IsNullOrEmpty(Expression))
            Expression = Expression[..^1];

        await PreviewAsync();
    }

    [RelayCommand]
    private async Task Calculate()
    {
        if (string.IsNullOrWhiteSpace(Expression))
            return;

        try
        {
            var val = await _calculator.EvaluateAsync(Expression);
            val = _rounding.Apply(val, Rounding);

            Result = val.ToString(CultureInfo.InvariantCulture);
            History.Insert(0, $"{Expression} = {Result}");
            Expression = Result;
        }
        catch
        {
            Result = "";
        }
    }

    private async Task PreviewAsync()
    {
        if (string.IsNullOrWhiteSpace(Expression))
        {
            Result = "0";
            return;
        }

        try
        {
            var val = await _calculator.EvaluateAsync(Expression);
            val = _rounding.Apply(val, Rounding);
            Result = val.ToString(CultureInfo.InvariantCulture);
        }
        catch
        {
            Result = "";
        }
    }
}