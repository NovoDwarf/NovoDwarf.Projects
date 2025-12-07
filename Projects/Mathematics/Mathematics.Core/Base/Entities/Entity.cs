namespace Mathematics.Core.Base.Entities;

/// <summary>
/// Represents a mathematical entity.
/// </summary>
public abstract class Entity
{
	public Guid Id { get; } = Guid.NewGuid();
	
	public virtual string Name => "Name";
	public virtual string Description => "Desc";
	
	public abstract string[] Path { get; }
	
	public virtual void Set(params object[] parameters) { }
}
