using System.Collections.ObjectModel;
using LocalizationResourceManager.Maui;
using NovoDwarf.Mathematics.App.Core.ViewModels;
using NovoDwarf.Mathematics.App.Resources.Localizations;
using NovoDwarf.Mathematics.App.Systems.Application.Services;
using NovoDwarf.Mathematics.App.ViewModels.Common;
using NovoDwarf.Mathematics.App.Views.Modules.Utilities.Organizers;
using NovoDwarf.Mathematics.App.Views.Modules.Utilities.Sensors;
using NovoDwarf.Mathematics.App.Views.Modules.Utilities.Tools;
using Utilities.Extensions;

namespace NovoDwarf.Mathematics.App.ViewModels.Utilities;

public partial class UtilityViewModel : BaseViewModel
{
    private readonly ILocalizationResourceManager _manager;
    private readonly IThemeService _themeService;
    private readonly Dictionary<PreviewViewModel, string> _iconMappings = [];

    public UtilityViewModel(ILocalizationResourceManager manager, IThemeService themeService)
    {
        _manager = manager;
        _themeService = themeService;

        Initialize();

        _themeService.ThemeChanged += OnThemeChanged;
    }

    public ObservableCollection<PreviewViewModel> Sensors { get; private set; } = [];

    public ObservableCollection<PreviewViewModel> Organizers { get; private set; } = [];

    public ObservableCollection<PreviewViewModel> Tools { get; private set; } = [];

    private void Initialize()
    {
        var isDark = _themeService.IsDarkMode;

        var sensorData = new[]
        {
            ("Title_Accelerometer", typeof(AccelerometerPage), "accelerometer"),
            ("Title_Barometer", typeof(BarometerPage), "barometer"),
            ("Title_Soundmeter", typeof(SoundmeterPage), "soundmeter"),
            ("Title_Speedometer", typeof(SpeedometerPage), "speedometer"),
            ("Title_Compass", typeof(CompassPage), "compass"),
            ("Title_Flashlight", typeof(FlashlightPage), "flashlight"),
            ("Title_Magnetometer", typeof(MagnetometerPage), "magnetometer"),
            ("Title_Gyroscope", typeof(GyroscopePage), "gyroscope"),
            ("Title_Orientation", typeof(OrientationPage), "orientation"),
            ("Title_DeviceInfo", typeof(DeviceInfoPage), "device_info"),
        };

        Sensors = CreateCollection(sensorData, isDark);

        var organizerData = new[]
        {
            ("Title_Stopwatch", typeof(StopwatchPage), "stopwatch"),
            ("Title_WorldClock", typeof(WorldClockPage), "world_clock"),
            ("Title_Calendar", typeof(CalendarPage), "calendar"),
            ("Title_Converter", typeof(ConverterPage), "converter"),
            ("Title_Timer", typeof(TimerPage), "timer"),
            ("Title_Todo", typeof(NotesPage), "todo"),
        };

        Organizers = CreateCollection(organizerData, isDark);

        var toolData = new[]
        {
            ("Title_Calculator", typeof(CalculatorPage), "calculator"),
            ("Title_CashCalculator", typeof(CashCalculatorPage), "cash_calculator"),
            ("Title_ColorPicker", typeof(ColorPickerPage), "color_picker"),
            ("Title_RegexTester", typeof(RegexTesterPage), "regex_tester"),
            ("Title_HttpTester", typeof(HttpTesterPage), "http_tester"),
            ("Title_CodeGenerator", typeof(CodeGeneratorPage), "code_generator"),
            ("Title_CodeScaner", typeof(CodeScannerPage), "code_scanner"),
            ("Title_Mirror", typeof(MirrorPage), "mirror"),
            ("Title_Ruler", typeof(RulerPage), "ruler"),
            ("Title_Level", typeof(LevelPage), "level"),
            ("Title_SoundGenerator", typeof(SoundGeneratorPage), "soundgenerator"),
            ("Title_Synth", typeof(SynthPage), "synth"),
        };

        Tools = CreateCollection(toolData, isDark);
    }

    private ObservableCollection<PreviewViewModel> CreateCollection((string titleKey, Type viewType, string iconBaseName)[] data, bool isDark)
    {
        var collection = new ObservableCollection<PreviewViewModel>();

        foreach (var (titleKey, viewType, iconBaseName) in data)
        {
            var vm = new PreviewViewModel
            {
                Title = Components_Resources.ResourceManager.GetString(titleKey) ?? titleKey,
                View = viewType,
                Icon = GetIcon(iconBaseName, isDark)
            };

            _iconMappings[vm] = iconBaseName;

            collection.Add(vm);
        }

        return collection.Sort(i => i.Title);
    }

    private string GetIcon(string baseName, bool isDark)
    {
        return isDark ? $"utilities_{baseName}_dark.png" : $"utilities_{baseName}_light.png";
    }

    private void OnThemeChanged(object? sender, AppTheme newTheme)
    {
        var isDark = newTheme == AppTheme.Dark;

        foreach (var (vm, icon) in _iconMappings)
        {
            vm.Icon = GetIcon(icon, isDark);
        }
    }

    public override void Cleanup()
    {
        _iconMappings?.Clear();
        _themeService.ThemeChanged -= OnThemeChanged;
    }
}