using Microsoft.Extensions.Logging;

namespace Aegis.Packaging.Localizations.Extensions;

public static partial class LoggerExtensions
{
	[LoggerMessage(LogLevel.Debug, "Failed to read localization XML file [{Path}] for package [{PackageId}].")]
	public static partial void LocalizationXmlReadFailed(this ILogger<LocalizationService> logger, Exception exception, string path, string packageId);

	[LoggerMessage(LogLevel.Debug, "Failed to parse localization XML file [{Path}] for package [{PackageId}].")]
	public static partial void LocalizationXmlParseFailed(this ILogger<LocalizationService> logger, Exception exception, string path, string packageId);

	[LoggerMessage(LogLevel.Debug, "Localization XML file [{Path}] for package [{PackageId}] has no locale attribute and no locale could be derived from filename.")]
	public static partial void LocalizationXmlHasNoLocale(this ILogger<LocalizationService> logger, string path, string packageId);

	[LoggerMessage(LogLevel.Debug, "Failed to read translation audit setting.")]
	public static partial void TranslationAuditSettingReadFailed(this ILogger<LocalizationService> logger, Exception exception);

	[LoggerMessage(LogLevel.Debug, "Localization audit: {Reason}. Node=[{NodePath}], Slot=[{Slot}], Value=[{Value}].")]
	public static partial void LocaleAudit(this ILogger logger, string reason, string nodePath, string slot, string value);
}
