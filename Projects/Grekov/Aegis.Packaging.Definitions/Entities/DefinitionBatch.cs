namespace Aegis.Packaging.Definitions.Entities;

internal sealed record DefinitionBatch(IReadOnlyList<DefinitionEntry> Entries, IReadOnlyList<PendingReference> PendingReferences);
