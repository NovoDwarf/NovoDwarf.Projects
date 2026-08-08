namespace Grekov.Definitions.Entities;

public sealed record DefReadContext(
	string PackageId,
	string ResourcePath,
	string FallbackId,
	List<DefPendingReference> PendingReferences);
