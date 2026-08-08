using Grekov.Core;

namespace Grekov;

public abstract class DefContainer<TDef, TInstance>
	where TDef : Def
{
	protected readonly Dictionary<TDef, TInstance> Items = [];

	public IReadOnlyCollection<TInstance> All => Items.Values;

	public bool Contains(TDef def)
		=> Items.ContainsKey(def);

	public TInstance? Find(TDef def)
		=> Items.GetValueOrDefault(def);
}