namespace Grekov.Definitions.Entities;

public sealed record DefPendingReference(
	Type ExpectedType,
	string Id,
	string ResourcePath,
	Action<object?> Apply);
