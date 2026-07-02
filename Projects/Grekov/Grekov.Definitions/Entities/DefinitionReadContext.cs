namespace Grekov.Definitions.Entities;

public sealed record DefinitionReadContext(
	string PackageId,
	string ResourcePath,
	string FallbackId,
	List<DefPendingReference> PendingReferences);
