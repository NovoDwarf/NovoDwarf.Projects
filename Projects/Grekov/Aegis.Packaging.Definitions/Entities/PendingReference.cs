namespace Aegis.Packaging.Definitions.Entities;

internal sealed record PendingReference(Type ExpectedType, string ReferenceId, string OwnerPath, Action<Def> Assign);
