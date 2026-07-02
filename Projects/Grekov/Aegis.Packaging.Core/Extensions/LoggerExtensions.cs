using Aegis.Packaging.Core.Services;
using Microsoft.Extensions.Logging;

namespace Aegis.Packaging.Core.Extensions;

public static partial class LoggerExtensions
{
	[LoggerMessage(LogLevel.Warning, "[Conflict] Key [{Key}] from [{PackageId}] overrides [{Existing}]")]
	public static partial void PackageConflictOverride(this ILogger<PackageConflictRegistry> logger, string key, string packageId, string existing);
}