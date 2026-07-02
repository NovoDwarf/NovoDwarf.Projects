using Aegis.Packaging.Definitions.Services;
using Microsoft.Extensions.Logging;

namespace Aegis.Packaging.Definitions.Extensions;

public static partial class LoggerExtensions
{
	[LoggerMessage(LogLevel.Information, "Registered def [{Type}] '{Key}' from [{PackageId}]")]
	public static partial void DefinitionRegistered(this ILogger<DefinitionService> logger, string type, string key, string packageId);

	[LoggerMessage(LogLevel.Debug, "Resolved def reference. OwnerPath={OwnerPath}, ExpectedType={ExpectedType}, ReferenceId={ReferenceId}, ResolvedPackageId={ResolvedPackageId}")]
	public static partial void DefinitionReferenceResolved(this ILogger<DefinitionService> logger, string ownerPath, string expectedType, string referenceId, string resolvedPackageId);

	[LoggerMessage(LogLevel.Error, "Failed to resolve def reference. OwnerPath={OwnerPath}, ExpectedType={ExpectedType}, ReferenceId={ReferenceId}")]
	public static partial void DefinitionReferenceMissing(this ILogger<DefinitionService> logger, string ownerPath, string expectedType, string referenceId);
}
