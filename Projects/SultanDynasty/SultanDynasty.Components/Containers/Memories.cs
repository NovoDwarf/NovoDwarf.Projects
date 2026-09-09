using SultanDynasty.Definitons;
using SultanDynasty.Instances.Characters;

namespace SultanDynasty.Instances;

public class Memories
{
	private readonly List<Memory> _items = [];

	public IReadOnlyList<Memory> All => _items;

	public void Remember(Memory memory)
	{
		_items.Add(memory);
	}

	public IEnumerable<Memory> Find(MemoryDef def)
	{
		return _items.Where(x => x.Def == def);
	}

	public IEnumerable<Memory> About(Guid characterId)
	{
		return _items.Where(x =>
			x.SubjectId == characterId ||
			x.TargetId == characterId);
	}

	public void Forget(Memory memory)
	{
		_items.Remove(memory);
	}
}