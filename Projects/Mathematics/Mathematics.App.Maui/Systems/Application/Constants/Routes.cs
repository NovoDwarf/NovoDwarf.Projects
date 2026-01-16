namespace Mathematics.App.Maui.Systems.Application.Constants;

public static class Routes
{
	// Common
	public const string MainRoute = "/Main";
	public const string SettingsRoute = "/Settings";
	public const string AboutRoute = "/About";
	public const string DebugRoute = "/Debug";
	public const string ModulesRoute = "/Algorithms";

	// Common -> Algorithms
	public const string CompressionsRoute = ModulesRoute + "/Compressions";
	public const string CryptographyRoute = ModulesRoute + "/Cryptography";
	public const string DistributionsRoute = ModulesRoute + "/Distributions";
	public const string FunctionsRoute = ModulesRoute + "/Functions";
	public const string GraphicsRoute = ModulesRoute + "/Graphics";
	public const string RandomsRoute = ModulesRoute + "/Randoms";
	public const string SortingRoute = ModulesRoute + "/Sorting";
	public const string UtilityRoute = ModulesRoute + "/Utility";
	
	// Common -> Algorithms -> Utility
	public const string ConverterRoute = UtilityRoute + "/Converter";
}